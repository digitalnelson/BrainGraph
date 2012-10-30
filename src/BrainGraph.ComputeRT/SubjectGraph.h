#pragma once
#include "GraphEdge.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	class SubjectGraph
	{
	public:
			
		SubjectGraph(int nVerts);
		~SubjectGraph(void);

		void AddEdge(int m, int n, double val);
		GraphEdge GetEdge(int m, int n);

		float GlobalStrength();
		void GetMeanVtxStrength(vector<float> &meanVtxStr);

		vector<GraphEdge> Edges;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		int _nVerts;
	};

}}}

