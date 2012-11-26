#include "pch.h"
#include <algorithm>
#include "Subject.h"
#include "SubjectGraphEdge.h"
#include "SingleDatatypeCompare.h"
#include "MultiDatatypeSupport.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace concurrency;
	using namespace BrainGraph::Compute::Subjects;

	SingleDatatypeCompare::SingleDatatypeCompare(int subjectCount, int nodes, int edges, Threshold ^threshold) : 
		_edges(edges),
		_nodes(nodes),
		_globals(subjectCount)
	{
		_lu = make_shared<GraphLookup>(nodes);
		_cmpGraph = make_shared<CompareGraph>(nodes, _lu, (float)threshold->Value);

		_threshold = threshold;
		_subjectCount = subjectCount;
		_nodeCount = nodes;
		_edgeCount = edges;

		//// Preallocate our edge mtx
		//for(auto i=0; i<edges; ++i)
		//	_edges[i].resize(subjectCount);
		//// Preallocate our node mtx
		//for(auto i=0; i<nodes; ++i)
		//	_nodes[i].resize(subjectCount);
		//// Preallocate our global mtx
		//_globals.resize(subjectCount);
	}

	SingleDatatypeCompare::~SingleDatatypeCompare(void)
	{}

	void SingleDatatypeCompare::AddSubject(Subject^ subject)
	{
		// TODO: What to do if the graph does not exist?
		// Pull out the graph for this datatype
		auto graph = subject->Graphs->Lookup(_threshold->DataType);

		// Store our edges
		for(auto edgeIdx=0; edgeIdx<_edgeCount; ++edgeIdx)
			_edges[edgeIdx].push_back(graph->Edges[edgeIdx]);

		// Store our nodes
		for(auto nodeIdx=0; nodeIdx<_nodeCount; ++nodeIdx)
			_nodes[nodeIdx].push_back(graph->Nodes[nodeIdx]);

		// TODO: Store our globals
	}

	// Calculate a edge stats for the two groups based on the indexes passed in
	vector<shared_ptr<CompareEdge>> SingleDatatypeCompare::CalcEdgeComparison(vector<int> &idxs, int szGrp1)
	{
		auto &edges = _edges;

		// TODO: Probably need to make this thread safe
		vector<shared_ptr<CompareEdge>> edgeStats(_edgeCount);

		//for(int edgeIndex=0; edgeIndex<_edgeCount; edgeIndex++)
		parallel_for(0, _edgeCount, [=, &idxs, &edgeStats, &edges] (int edgeIndex)
		{
			// Pull out a view of the subject values for a single edge
			auto &edgeValues = edges[edgeIndex];
			
			TStatCalc calcEdgeValue;
			
			// Loop through the vals we were passed
			for (int idx = 0; idx < _subjectCount; ++idx)
			{
				float edgeVal = edgeValues[idxs[idx]].Value;

				if (idx < szGrp1)
					calcEdgeValue.IncludeValue(0, edgeVal);
				else
					calcEdgeValue.IncludeValue(1, edgeVal);
			}

			auto edge = make_shared<CompareEdge>();
			
			edge->Index = edgeIndex;
			edge->Nodes = _lu->GetEdge(edgeIndex);
			edge->Stats = calcEdgeValue.Calculate();

			edgeStats[edgeIndex] = edge;
		});

		return edgeStats;
	}

	vector<shared_ptr<CompareNode>> SingleDatatypeCompare::CalcNodeComparison(vector<int> &idxs, int szGrp1)
	{
		// TODO: Probably need to make this thread safe
		vector<shared_ptr<CompareNode>> nodeStats(_nodeCount);

		//for(int nodeIndex=0; nodeIndex<_nodeCount; nodeIndex++)
		parallel_for(0, _nodeCount, [=, &idxs, &nodeStats] (int nodeIndex)
		{
			// Pull out a view of the subject values for a single edge
			auto nodeValues = _nodes[nodeIndex];

			TStatCalc calcDegree;
			TStatCalc calcStrength;
		
			// Loop through the vals we were passed
			for (int idx = 0; idx < _subjectCount; ++idx)
			{
				auto nodeVal = nodeValues[idxs[idx]];

				if (idx < szGrp1)
				{
					calcDegree.IncludeValue(0, nodeVal.Degree);
					calcStrength.IncludeValue(0, nodeVal.Strength);
				}
				else
				{
					calcDegree.IncludeValue(1, nodeVal.Degree);
					calcStrength.IncludeValue(1, nodeVal.Strength);
				}
			}

			auto node = make_shared<CompareNode>();
			
			node->Index = nodeIndex;
			node->Degree = calcDegree.Calculate();
			node->Strength = calcStrength.Calculate();

			nodeStats[nodeIndex] = node;
		});

		return nodeStats;
	}

	shared_ptr<CompareGraph> SingleDatatypeCompare::Compare(vector<int> &idxs, int szGrp1)
	{
		// Calculate edge group comparison
		_cmpGraph->AddEdges( CalcEdgeComparison(idxs, szGrp1) );

		// Calculate node group comparison
		_cmpGraph->AddNodes( CalcNodeComparison(idxs, szGrp1) );

		// Calculate NBS components
		_cmpGraph->ComputeComponents();

		//// COMPUTE GLOBAL ANALYSES
		// TODO: Global strength
		// TODO: Global diversity

		// Return the largest component
		return _cmpGraph;
	}

	shared_ptr<Component> SingleDatatypeCompare::Permute(vector<int> &idxs, int szGrp1)
	{
		// Create a comparison graph to hold our group comparison results
		unique_ptr<CompareGraph> randomGraph(new CompareGraph(_nodeCount, _lu, (float)_threshold->Value));

		// Calculate edge group comparison
		randomGraph->AddEdges( CalcEdgeComparison(idxs, szGrp1) );

		// Calculate node group comparison
		randomGraph->AddNodes( CalcNodeComparison(idxs, szGrp1) );

		// Calculate NBS components
		randomGraph->ComputeComponents();

		// Update our edge stats with our random values
		_cmpGraph->UpdateEdgeStats( randomGraph->Edges );

		//_cmpGraph->UpdateComponentStats( randomGraph->GetLargestComponent() );
		
		//_cmpGraph->UpdateGlobalStats( ??? );

		// TODO: Make these methods more efficient by not searching for largest multiple times
		// Get the largest components
		auto lrgstRndmCmp = randomGraph->GetLargestComponent();
		auto lrgstRealCmp = _cmpGraph->GetLargestComponent();

		if(lrgstRndmCmp != nullptr)
		{
			if(lrgstRndmCmp->Edges.size() > lrgstRealCmp->Edges.size())
				lrgstRealCmp->RightTailExtent++;
		}

		return lrgstRndmCmp;
	}
}}}
