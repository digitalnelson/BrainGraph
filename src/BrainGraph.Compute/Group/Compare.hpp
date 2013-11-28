#pragma once
#include <memory>
#include <ppl.h>
#include "../GraphLookup.hpp"
#include "../Subjects/Subject.hpp"
#include "Threshold.hpp"
#include "TStat.hpp"
#include "TStatCalc.hpp"
#include "Edge.hpp"
#include "Node.hpp"
#include "Global.hpp"
#include "Graph.hpp"

namespace BrainGraph { namespace Compute { namespace Group
{
	namespace con = concurrency;
	namespace BCS = BrainGraph::Compute::Subjects;

	class Compare
	{
	public:
		Compare(int subjectCount, int nodes, int edges, std::shared_ptr<Threshold> threshold) :
			_edges(edges),
			_nodes(nodes)
		{
			_lu = std::make_shared<GraphLookup>(nodes);
			_cmpGraph = std::make_shared<Graph>(nodes, _lu, (double)threshold->Value);

			_threshold = threshold;
			_subjectCount = subjectCount;
			_nodeCount = nodes;
			_edgeCount = edges;
		}

		void AddSubject(std::shared_ptr<BCS::Subject> subject)
		{
			// Pull out the graph for this datatype
			auto graph = subject->Graphs[_threshold->DataType];

			// TODO: What to do if the graph does not exist?

			// Store our edges
			for (size_t edgeIdx = 0; edgeIdx<_edgeCount; ++edgeIdx)
				_edges[edgeIdx].push_back(graph->Edges[edgeIdx]);

			// Store our nodes
			for (size_t nodeIdx = 0; nodeIdx<_nodeCount; ++nodeIdx)
				_nodes[nodeIdx].push_back(graph->Nodes[nodeIdx]);

			// Store our globals
			std::shared_ptr<BCS::Global> sgg = std::make_shared<BCS::Global>();
			sgg->Strength = graph->GlobalStrength();
			_globals.push_back(sgg);

			//if (_attrs.size() < subject->Attributes->Size)
			//	_attrs.resize(subject->Attributes->Size);

			//// TODO: Ensure this is the same order
			//// Store our attrs
			//int i = 0;
			//for (auto itm : subject->Attributes)
			//{
			//	_attrs[i].push_back(itm->Value);
			//	++i;
			//}
		}

		// Calculate a edge stats for the two groups based on the indexes passed in
		std::vector<std::shared_ptr<Edge>> CalcEdgeComparison(std::vector<int> &idxs, size_t szGrp1)
		{
			auto &edges = _edges;

			// TODO: Probably need to make this thread safe
			std::vector<std::shared_ptr<Edge>> edgeStats(_edgeCount);

			//for(int edgeIndex=0; edgeIndex<_edgeCount; edgeIndex++)
			con::parallel_for((size_t)0, _edgeCount, [=, &idxs, &edgeStats, &edges](int edgeIndex)
			{
				// Pull out a view of the subject values for a single edge
				auto &edgeValues = edges[edgeIndex];

				TStatCalc calcEdgeValue;

				// Loop through the vals we were passed
				for (size_t idx = 0; idx < _subjectCount; ++idx)
				{
					double edgeVal = edgeValues[idxs[idx]]->Value;

					if (idx < szGrp1)
						calcEdgeValue.IncludeValue(0, edgeVal);
					else
						calcEdgeValue.IncludeValue(1, edgeVal);
				}

				auto edge = std::make_shared<Edge>();

				edge->Index = edgeIndex;
				edge->Nodes = _lu->GetEdge(edgeIndex);
				edge->Stats = calcEdgeValue.Calculate();

				edgeStats[edgeIndex] = edge;
			});

			return edgeStats;
		}

		std::vector<std::shared_ptr<Node>> CalcNodeComparison(std::vector<int> &idxs, int szGrp1)
		{
			// TODO: Probably need to make this thread safe
			std::vector<std::shared_ptr<Node>> nodeStats(_nodeCount);

			//for(int nodeIndex=0; nodeIndex<_nodeCount; nodeIndex++)
			con::parallel_for((size_t)0, _nodeCount, [=, &idxs, &nodeStats](int nodeIndex)
			{
				// Pull out a view of the subject values for a single edge
				auto nodeValues = _nodes[nodeIndex];

				TStatCalc calcDegree;
				TStatCalc calcStrength;

				// Loop through the vals we were passed
				for (size_t idx = 0; idx < _subjectCount; ++idx)
				{
					auto nodeVal = nodeValues[idxs[idx]];

					double avgStrength = nodeVal->TotalStrength / _nodeCount;

					int grpId = 1;
					if (idx < szGrp1)
						grpId = 0;

					calcDegree.IncludeValue(grpId, nodeVal->Degree);
					calcStrength.IncludeValue(grpId, avgStrength);
				}

				auto node = std::make_shared<Node>();

				node->Index = nodeIndex;
				node->Degree = calcDegree.Calculate();
				node->Strength = calcStrength.Calculate();

				nodeStats[nodeIndex] = node;
			});

			return nodeStats;
		}

		std::shared_ptr<Global> CalcGlobalComparison(std::vector<int> &idxs, int szGrp1)
		{
			// TODO: Probably need to make this thread safe
			std::shared_ptr<Global> globalStats = std::make_shared<Global>();

			TStatCalc strength;

			// Loop through the vals we were passed
			for (size_t idx = 0; idx < _subjectCount; ++idx)
			{
				auto subjectGlobals = _globals[idxs[idx]];

				int grpId = 1;
				if (idx < szGrp1)
					grpId = 0;

				strength.IncludeValue(grpId, subjectGlobals->Strength);

				// TODO: Loop here and include the associated global values
				int attrIdx = 0;
				for (auto attr : _attrs)
				{
					if (idx < attr.size())
						globalStats->Strength.IncludePearsonValue(attrIdx, grpId, subjectGlobals->Strength, attr[idx]);

					++attrIdx;
				}
			}

			globalStats->Strength.Stats = strength.Calculate();

			return globalStats;
		}

		std::shared_ptr<Graph> CompareGroups(std::vector<int> &idxs, int szGrp1)
		{
			// Calculate edge group comparison
			_cmpGraph->AddEdges(CalcEdgeComparison(idxs, szGrp1));

			// Calculate node group comparison
			_cmpGraph->AddNodes(CalcNodeComparison(idxs, szGrp1));

			// Calculate NBS components
			_cmpGraph->ComputeComponents();

			// Calculate global group comparison
			_cmpGraph->SetGlobal(CalcGlobalComparison(idxs, szGrp1));

			// Return our real comparison graph
			return _cmpGraph;
		}

		std::shared_ptr<Component> Permute(std::vector<int> &idxs, int szGrp1)
		{
			// Create a comparison graph to hold our group comparison results
			std::unique_ptr<Graph> randomGraph(new Graph(_nodeCount, _lu, (double)_threshold->Value));

			// Calculate edge group comparison
			randomGraph->AddEdges(CalcEdgeComparison(idxs, szGrp1));

			// Calculate node group comparison
			randomGraph->AddNodes(CalcNodeComparison(idxs, szGrp1));

			// Calculate NBS components
			randomGraph->ComputeComponents();

			// Calculate global group comparison
			randomGraph->SetGlobal(CalcGlobalComparison(idxs, szGrp1));

			// Update our edge stats with our random values
			_cmpGraph->UpdateEdgeStats(randomGraph->Edges);

			// Update our edge stats with our random values
			_cmpGraph->UpdateNodeStats(randomGraph->Nodes);

			//_cmpGraph->UpdateComponentStats( randomGraph->GetLargestComponent() );

			_cmpGraph->UpdateGlobalStats(randomGraph->Global);

			// Get the largest components
			auto lrgstRndmCmp = randomGraph->GetLargestComponent();
			auto lrgstRealCmp = _cmpGraph->GetLargestComponent();

			if (lrgstRealCmp != nullptr)
			{
				if (lrgstRndmCmp != nullptr)
				{
					lrgstRealCmp->AddRandomExtentValue(lrgstRndmCmp->Edges.size());
				}
				else
					lrgstRealCmp->AddRandomExtentValue(0);
			}

			return lrgstRndmCmp;
		}

	private:
		size_t _subjectCount;
		size_t _nodeCount;
		size_t _edgeCount;
		size_t _permutations;

		std::shared_ptr<Threshold> _threshold;
		std::shared_ptr<GraphLookup> _lu;
		std::shared_ptr<Graph> _cmpGraph;

		std::vector<std::vector<std::shared_ptr<BCS::Edge>>> _edges;  // Mtx of edge vs subject  e.g. 4005x58
		std::vector<std::vector<std::shared_ptr<BCS::Node>>> _nodes;	// Mtx of node vs subject  e.g. 90x58
		std::vector<std::shared_ptr<BCS::Global>> _globals;			// Arr of subject values  e.g. 1x58
		
		std::vector<std::vector<double>> _attrs;  // Vector of attr names and subject values NAttrx58
	};

}}}

