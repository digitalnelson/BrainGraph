#pragma once
#include "SubjectGraphEdge.h"
#include <collection.h>

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace std;
	using namespace Platform;
	namespace WFC = Windows::Foundation::Collections;

	ref class Subject;

	public ref class SubjectGraph sealed
	{
	public:
		SubjectGraph(int nVerts);
		void AddEdge(int i, int j, double val);

		property Subject^ Subject;
		property String^ DataType;

	internal:
		//SubjectGraphEdge GetEdge(int m, int n);
		float GlobalStrength();
		void GetMeanVtxStrength(vector<float> &meanVtxStr);

		std::vector<SubjectGraphEdge> Edges;
		std::vector<SubjectGraphNode> Nodes;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		int _nVerts;
	};

}}}

