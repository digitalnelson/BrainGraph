#pragma once
#include "SubjectGraphEdge.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace std;
	using namespace Platform;

	public ref class SubjectGraph sealed
	{
	public:
		SubjectGraph(int nVerts);
		void AddEdge(int m, int n, double val);

		property String^ DataType;

	internal:
		SubjectGraphEdge GetEdge(int m, int n);
		float GlobalStrength();
		void GetMeanVtxStrength(vector<float> &meanVtxStr);

		property vector<SubjectGraphEdge> Edges;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		int _nVerts;
	};

}}}

