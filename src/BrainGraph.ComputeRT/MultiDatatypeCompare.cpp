#include "pch.h"
#include "collection.h"
#include "Subject.h"
//#include "SubjectGraphEdge.h"
#include "SingleDatatypeCompare.h"
#include "MultiDatatypeCompare.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace concurrency;
	using namespace Platform;
	using namespace Windows::Foundation;
	using namespace Windows::Foundation::Collections;
	using namespace BrainGraph::Compute::Subjects;

	MultiDatatypeCompare::MultiDatatypeCompare(int verts, int edges, IVector<Threshold^>^ thresholds) : 
		_multiGraph(new MultiGraph())
	{
		_vertices = verts;
		_edges = edges;

		for(auto thresh : thresholds)
			_dataThresholds.push_back(thresh);
		
		_subCounter = 0;
		_permutations = 0;

		srand((unsigned int)time(0));
	}

	void MultiDatatypeCompare::LoadSubjects(IVector<Subject^>^ group1, IVector<Subject^>^ group2)
	{
		_subjectCount = group1->Size + group2->Size;

		for(auto dataThreshold : _dataThresholds)
			_groupCompareByType[dataThreshold->DataType] = shared_ptr<SingleDatatypeCompare>(new SingleDatatypeCompare(_subjectCount, _vertices, _edges, dataThreshold));

		for(auto subject : group1)
			AddSubject("group1", subject);

		for(auto subject : group2)
			AddSubject("group2", subject);
	}

	void MultiDatatypeCompare::AddSubject(String^ groupId, Subject^ subject)
	{
		// Loop through the graphs for this subject
		for(auto thresh : _dataThresholds)
			_groupCompareByType[thresh->DataType]->AddSubject( subject );

		// Add our subject idx to the proper group vector
		_subIdxsByGroup[groupId].push_back(_subCounter);

		// Make sure to increment the counter
		++_subCounter;
	}
	
	void MultiDatatypeCompare::Compare()
	{
		// Put together a list of idxs representing our two groups
		vector<int> idxs;
		for(auto itm : _subIdxsByGroup["group1"])
			idxs.push_back(itm);
		for(auto itm : _subIdxsByGroup["group2"])
			idxs.push_back(itm);

		// Keep track of our group 1 size for the permutation step
		_group1Count = _subIdxsByGroup["group1"].size();

		// Temporary map to hold our node counts
		vector<int> nodeCounts(_vertices);

		// Loop through our comparisons and call compare group passing our actual subject labels
		for(auto &groupCompareItem : _groupCompareByType)
		{
			// Compare the two groups for this data type
			auto compareGraph = groupCompareItem.second->Compare(idxs, _group1Count);

			// Pull out the largest component
			auto largestComponent = compareGraph->GetLargestComponent();

			if(largestComponent != nullptr)
			{
				// Pull out the vertices and store then in our counting map
				for(auto vert : largestComponent->Vertices)
					++nodeCounts[vert];
			}

			// Store our compare graph as one of our results
			_compareGraphs[groupCompareItem.first] = compareGraph;
		}

		// Load up our multi graph
		int maxOverlap = _groupCompareByType.size();
		for(auto nc=0; nc<nodeCounts.size();++nc)
		{
			auto multiNode = make_shared<MultiNode>(nc);

			if(nodeCounts[nc] == maxOverlap)
				multiNode->IsFullOverlap = true;

			_multiGraph->AddNode(multiNode);
		}
	}

	IAsyncActionWithProgress<int>^ MultiDatatypeCompare::PermuteAsyncWithProgress(int permutations)
	{
		IAsyncActionWithProgress<int>^ action_with_progress = create_async( [=] ( progress_reporter<int> reporter ) 
		{
			// Put together a list of idxs representing our two groups
			vector<int> idxs;
			for(auto itm : _subIdxsByGroup["group1"])
				idxs.push_back(itm);
			for(auto itm : _subIdxsByGroup["group2"])
				idxs.push_back(itm);

			// Keep track of our group 1 size for the permutation step
			auto group1Count = _subIdxsByGroup["group1"].size();
			int &totalPerms = _permutations;

			//for(int permutation=0; permutation<permutations; permutation++)
			parallel_for(0, permutations, [=, &idxs, &totalPerms] ( int permutation )
			{	
				// Shuffle subjects randomly
				random_shuffle(idxs.begin(), idxs.end());

				// Vector for counting node overlap
				std::vector<int> nodeCounts(_vertices);

				// Loop through our comparisons and call permute on them passing our new random subject assortement
				for(auto &groupCompareItem : _groupCompareByType)
				{
					// Compare the two groups for this data type
					auto cmp = groupCompareItem.second->Permute(idxs, group1Count);

					// Pull out the vertices and store then in our counting map
					if(cmp != nullptr)
					{
						for(auto vert : cmp->Vertices)
							++nodeCounts[vert];
					}
				}

				MultiGraph randomGraph;

				// Calculate how many nodes overlap between all of the nodes
				auto maxOverlap = _groupCompareByType.size();
				for(auto idx=0; idx<nodeCounts.size(); ++idx)
				{
					auto multiNode = make_shared<MultiNode>(idx);

					if(nodeCounts[idx] == maxOverlap)
					{
						multiNode->IsFullOverlap = true;
						_multiGraph->IncrementNodalRandomOverlapCount(idx);
					}

					randomGraph.AddNode(multiNode);					
				}

				if(randomGraph.GetTotalNodalOverlapCount() >= _multiGraph->GetTotalNodalOverlapCount())
					_multiGraph->IncrementGraphRandomOverlapCount();

				++totalPerms;

				int tmpPerms = totalPerms;
				if(tmpPerms % 100 == 0)
					reporter.report(tmpPerms);
			});
		});

		return action_with_progress;
	}

	MultiGraphViewModel^ MultiDatatypeCompare::GetResult()
	{
		auto multiGraphVM = ref new MultiGraphViewModel(_multiGraph);

		for(auto compareGraphItm : _compareGraphs)
		{
			auto compareGraphVM = ref new CompareGraphViewModel(compareGraphItm.second);
			compareGraphVM->Name = compareGraphItm.first;

			multiGraphVM->AddCompareGraph(compareGraphVM);
		}

		return multiGraphVM;
	}

	int MultiDatatypeCompare::GetPermutations()
	{
		return _permutations;
	}
}}}