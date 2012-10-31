#pragma once
#include "CompareGraphSupport.h"
#include "GraphLookup.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	class CompareGraph
	{
	public:
		CompareGraph(int nVerts, GraphLookup* lu);
		~CompareGraph(void);

		void AddEdge(int m, int n, CompareEdge val);
		CompareEdge GetEdge(int m, int n);

		void ComputeComponents(vector<int> &edgeIdxs);
		//void GetComponents(vector<Component> &components);

		vector<CompareEdge> Edges;
		vector<Component> Components;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		GraphLookup * _lu;
		int _nVerts;
	};

}}}

