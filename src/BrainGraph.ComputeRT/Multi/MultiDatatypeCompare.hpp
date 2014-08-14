#pragma once
#include <memory>
#include <random>
#include <ppltasks.h>
#include "BrainGraph.Compute/Multi/Graph.hpp"
#include "BrainGraph.Compute/Group/Compare.hpp"
#include "../Subjects/SubjectViewModel.hpp"
#include "../Group/ThresholdViewModel.hpp"
#include "MultiViewModels.hpp"

namespace BrainGraph { namespace ComputeRT { namespace Multi
{
	#pragma region Standard Namespace Redefs
	namespace P = Platform;
	namespace PC = Platform::Collections;
	namespace WF = Windows::Foundation;
	namespace WFC = Windows::Foundation::Collections;
	namespace CON = concurrency;
	namespace BCG = BrainGraph::Compute::Group;
	namespace BCM = BrainGraph::Compute::Multi;
	namespace BCRS = BrainGraph::ComputeRT::Subjects;
	namespace BCRC = BrainGraph::ComputeRT::Group;
	namespace BCRM = BrainGraph::ComputeRT::Multi;
	#pragma endregion

	public ref class MultiDatatypeCompare sealed
	{
	public:

		MultiDatatypeCompare(int verts, int edges, WFC::IVector<BCRC::ThresholdViewModel^>^ thresholds) :
			_multiGraph(new BCM::Graph())
		{
			_vertices = verts;
			_edges = edges;

			for (auto thresh : thresholds)
				_dataThresholds.push_back(thresh);

			_subCounter = 0;
			_permutations = 0;

			srand((unsigned int)time(0));
		}

		void LoadSubjects(WFC::IVector<BCRS::SubjectViewModel^>^ group1, WFC::IVector<BCRS::SubjectViewModel^>^ group2)
		{
			_subjectCount = group1->Size + group2->Size;

			for (auto dataThreshold : _dataThresholds)
				_groupCompareByType[dataThreshold->DataType] = std::shared_ptr<BCG::Compare>(new BCG::Compare(_subjectCount, _vertices, _edges, dataThreshold->GetThreshold()));

			for (auto subject : group1)
				AddSubject("group1", subject);

			for (auto subject : group2)
				AddSubject("group2", subject);
		}

		void AddSubject(P::String^ groupId, BCRS::SubjectViewModel^ subject)
		{
			// Loop through the graphs for this subject
			for (auto thresh : _dataThresholds)
				_groupCompareByType[thresh->DataType]->AddSubject(subject->_subject);

			// Add our subject idx to the proper group vector
			_subIdxsByGroup[groupId].push_back(_subCounter);

			// Make sure to increment the counter
			++_subCounter;
		}

		void Compare()
		{
			// Put together a list of idxs representing our two groups
			std::vector<int> idxs;
			for (auto itm : _subIdxsByGroup["group1"])
				idxs.push_back(itm);
			for (auto itm : _subIdxsByGroup["group2"])
				idxs.push_back(itm);

			// Keep track of our group 1 size for the permutation step
			_group1Count = _subIdxsByGroup["group1"].size();

			// Temporary map to hold our node counts
			std::vector<int> nodeCounts(_vertices);

			// Loop through our comparisons and call compare group passing our actual subject labels
			for (auto &groupCompareItem : _groupCompareByType)
			{
				// Compare the two groups for this data type
				auto compareGraph = groupCompareItem.second->CompareGroups(idxs, _group1Count);

				// Pull out the largest component
				auto largestComponent = compareGraph->GetLargestComponent();

				if (largestComponent != nullptr)
				{
					// Pull out the vertices and store then in our counting map
					for (auto vert : largestComponent->Vertices)
						++nodeCounts[vert];
				}

				// Store our compare graph as one of our results
				_compareGraphs[groupCompareItem.first] = compareGraph;
			}

			// Load up our multi graph
			int maxOverlap = _groupCompareByType.size();
			for (auto nc = 0; nc<nodeCounts.size(); ++nc)
			{
				auto multiNode = std::make_shared<BCM::Node>(nc);

				if (nodeCounts[nc] == maxOverlap)
					multiNode->IsFullOverlap = true;

				_multiGraph->AddNode(multiNode);
			}
		}

		WF::IAsyncActionWithProgress<int>^ PermuteAsyncWithProgress(int permutations)
		{
			try
			{
				WF::IAsyncActionWithProgress<int>^ action_with_progress = CON::create_async([=](CON::progress_reporter<int> reporter)
				{
					try
					{
						// Put together a list of idxs representing our two groups
						std::vector<int> idxs;
						for (auto itm : _subIdxsByGroup["group1"])
							idxs.push_back(itm);
						for (auto itm : _subIdxsByGroup["group2"])
							idxs.push_back(itm);

						// Keep track of our group 1 size for the permutation step
						auto group1Count = _subIdxsByGroup["group1"].size();
						//int &totalPerms = _permutations;

						std::atomic<int> permCount(0);

						for (int permutation = 0; permutation<permutations; permutation++)
							//parallel_for(0, permutations, [=, &idxs, &permCount](int permutation)
						{
							//try
							//{
							std::vector<int> idxCpy(idxs);

							std::random_device rndDev;
							std::default_random_engine engRand(rndDev());

							// Shuffle subjects randomly
							std::shuffle(begin(idxCpy), end(idxCpy), engRand);

							// Vector for counting node overlap
							std::vector<int> nodeCounts(_vertices);

							// Loop through our comparisons and call permute on them passing our new random subject assortement
							for (auto &groupCompareItem : _groupCompareByType)
							{
								// Compare the two groups for this data type
								auto cmp = groupCompareItem.second->Permute(idxCpy, group1Count);

								// Pull out the vertices and store then in our counting map
								if (cmp != nullptr)
								{
									for (auto vert : cmp->Vertices)
										++nodeCounts[vert];
								}
							}

							BCM::Graph randomGraph;

							// Calculate how many nodes overlap between all of the nodes
							auto maxOverlap = _groupCompareByType.size();
							for (size_t idx = 0; idx < nodeCounts.size(); ++idx)
							{
								auto multiNode = std::make_shared<BCM::Node>(idx);

								if (nodeCounts[idx] == maxOverlap)
								{
									multiNode->IsFullOverlap = true;
									_multiGraph->IncrementNodalRandomOverlapCount(idx);
								}

								randomGraph.AddNode(multiNode);
							}

							_multiGraph->AddRandomOverlapValue(randomGraph.GetTotalNodalOverlapCount());

							permCount++;

							if (permCount % 100 == 0)
								reporter.report(permCount);

							//}
							//catch (Exception^ e)
							//{
							//	throw ref new Exception(E_FAIL, "Bad multigraph.");
							//}
						}//);

						_permutations = permCount;

					}
					catch (P::Exception^ e)
					{
						throw ref new P::Exception(E_FAIL, "Something bad happened.");
					}
				});

				return action_with_progress;
			}
			catch (P::Exception^ e)
			{
				throw ref new P::Exception(E_FAIL, "Something really bad happened.");
			}
		}

		MultiGraphViewModel^ GetResult()
		{
			auto multiGraphVM = ref new MultiGraphViewModel(_multiGraph);

			for (auto compareGraphItm : _compareGraphs)
			{
				auto compareGraphVM = ref new BCRG::GraphViewModel(compareGraphItm.second);
				compareGraphVM->Name = compareGraphItm.first;

				multiGraphVM->AddCompareGraph(compareGraphVM);
			}

			return multiGraphVM;
		}

		int GetPermutations()
		{
			return _permutations;
		}

	private:
		std::map<Platform::String^, std::vector<int>> _subIdxsByGroup;
		int _subjectCount;
		int _subCounter;
		int _group1Count;
				
		std::vector<BCRG::ThresholdViewModel^> _dataThresholds;
		std::map<Platform::String^, std::shared_ptr<BCG::Compare>> _groupCompareByType;

		std::map<Platform::String^, std::shared_ptr<BCG::Graph>> _compareGraphs;
		
		int _permutations;

		std::shared_ptr<BCM::Graph> _multiGraph;
		int _vertices;
		int _edges;
	};
}}}

