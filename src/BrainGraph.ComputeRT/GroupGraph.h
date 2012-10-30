#pragma once
#include "Component.h"
#include "GraphLookup.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	class GroupGraph
	{
	public:
		GroupGraph(int nVerts, GraphLookup* lu);
		~GroupGraph(void);

		void AddEdge(int m, int n, GroupEdge val);
		GroupEdge GetEdge(int m, int n);

		void ComputeComponents(vector<int> &edgeIdxs);
		//void GetComponents(vector<Component> &components);

		vector<GroupEdge> Edges;
		vector<Component> Components;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		GraphLookup * _lu;
		int _nVerts;
	};

}}}

