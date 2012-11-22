#pragma once
#include "CompareGraphSupport.h"
#include "GraphLookup.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	class CompareGraph
	{
	public:
		CompareGraph(int nVerts, shared_ptr<GraphLookup> lu);
		~CompareGraph(void);

		void AddEdge(int m, int n, shared_ptr<CompareEdge> val);
		shared_ptr<CompareEdge> GetEdge(int m, int n);

		void ComputeComponents();
		shared_ptr<Component> GetLargestComponent();

		vector<shared_ptr<CompareEdge>> Edges;
		vector<shared_ptr<Component>> Components;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		shared_ptr<GraphLookup> _lu;
		int _nVerts;
	};

}}}

