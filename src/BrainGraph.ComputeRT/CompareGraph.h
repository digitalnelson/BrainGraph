#pragma once
#include "CompareGraphSupport.h"
#include "GraphLookup.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	class CompareGraph
	{
	public:
		CompareGraph(int nVerts, shared_ptr<GraphLookup> lu, double nbsThreshold);
		~CompareGraph(void);

		void AddEdges(std::vector<std::shared_ptr<CompareEdge>> &edges);
		void AddNodes(std::vector<std::shared_ptr<CompareNode>> &nodes);
		void SetGlobal(std::shared_ptr<CompareGlobal> global);

		void UpdateEdgeStats(std::vector<std::shared_ptr<CompareEdge>> &edges);
		void UpdateNodeStats(std::vector<std::shared_ptr<CompareNode>> &nodes);
		void UpdateGlobalStats(std::shared_ptr<CompareGlobal> global);
		
		void ComputeComponents();
		shared_ptr<Component> GetLargestComponent();

		vector<shared_ptr<CompareEdge>> Edges;
		vector<shared_ptr<CompareNode>> Nodes;
		vector<shared_ptr<Component>> Components;
		shared_ptr<CompareGlobal> Global;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		shared_ptr<GraphLookup> _lu;
		
		int _nVerts;
		double _nbsThreshold;

		int _nLargestComponentId;
	};

}}}

