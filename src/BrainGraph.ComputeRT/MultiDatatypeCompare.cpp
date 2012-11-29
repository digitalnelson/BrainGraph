#include "pch.h"
#include "collection.h"
#include "Subject.h"
#include "SubjectGraphEdge.h"
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

	MultiDatatypeCompare::MultiDatatypeCompare(int verts, int edges, IVector<Threshold^>^ thresholds)
	{
		_vertices = verts;
		_edges = edges;

		for(auto thresh : thresholds)
			_dataThresholds.push_back(thresh);
		
		_subCounter = 0;

		srand((unsigned int)time(0));

		_permutations = 0;
		_realOverlap = 0;
		_rightTailOverlapCount = 0;
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

			// Pull out the vertices and store then in our counting map
			for(auto vert : largestComponent->Vertices)
				++nodeCounts[vert];

			// Store our compare graph as one of our results
			_compareGraphs[groupCompareItem.first] = compareGraph;
		}

		// Calculate how many nodes overlap between all of the data types
		int maxOverlap = _groupCompareByType.size();
		for(auto nc=0; nc<nodeCounts.size();++nc)
		{
			_verticesById[nc] = shared_ptr<Vertex>(new Vertex());
			_verticesById[nc]->Id = nc;

			if(nodeCounts[nc] == maxOverlap)
			{
				_verticesById[nc]->IsFullOverlap = true;
				++_realOverlap;
			}
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

			//for(int i=0; i<permutations; i++)
			parallel_for(0, permutations, [=, &idxs, &totalPerms] (int permutation)
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

					if(cmp != nullptr)
					{
						// Pull out the vertices and store then in our counting map
						for(auto vert : cmp->Vertices)
							++nodeCounts[vert];
					}
				}

				// Calculate how many nodes overlap between all of the nodes
				int permOverlap = 0, maxOverlap = _groupCompareByType.size();
				for(auto nc=0; nc<nodeCounts.size();++nc)
				{
					if(nodeCounts[nc] == maxOverlap)
					{
						_verticesById[nc]->RandomOverlapCount++;
						++permOverlap;
					}
				}

				if(_overlapDistribution.count(permOverlap) == 0)
					_overlapDistribution[permOverlap] = 0;
			
				_overlapDistribution[permOverlap]++;

				// NBS multimodal compare
				if(permOverlap >= _realOverlap)
					++_rightTailOverlapCount;

				++totalPerms;

				int tmpPerms = totalPerms;
				if(tmpPerms % 100 == 0)
					reporter.report(tmpPerms);
			});
		});

		return action_with_progress;
	}

	MultiResult^ MultiDatatypeCompare::GetResult()
	{
		auto result = ref new MultiResult();

		for(auto compareGraphItm : _compareGraphs)
		{
			auto multiGraph = ref new MultiGraph();
			multiGraph->Name = compareGraphItm.first;
			
			auto compareGraph = compareGraphItm.second;

			for(auto compareNode : compareGraph->Nodes)
			{
				multiGraph->AddNode(compareNode);
			}

			for(auto compareEdge : compareGraph->Edges)
			{
				multiGraph->AddEdge(compareEdge);
			}

			result->AddGraph(multiGraph);
		}

		return result;
	}

	int MultiDatatypeCompare::GetPermutations()
	{
		return _permutations;
	}

	//std::unique_ptr<Overlap> MultiDatatypeCompare::GetOverlapResult()
	//{
	//	unique_ptr<Overlap> overlap(new Overlap());

	//	for(auto &dataItem : _dataByType)
	//		dataItem.second->GetComponents(overlap->Components[dataItem.first]); // Ask the graph for the components

	//	for(auto vtx : _verticesById)
	//	{
	//		if(vtx.second->IsFullOverlap)
	//			overlap->Vertices.push_back(vtx.second);
	//	}

	//	overlap->RightTailOverlapCount = _rightTailOverlapCount;
	//	overlap->Distribution = _overlapDistribution;

	//	return overlap;
	//}
}}}