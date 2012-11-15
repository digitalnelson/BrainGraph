#pragma once
#include "SubjectGraphEdge.h"
#include <collection.h>

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace std;
	using namespace Platform;
	using namespace Windows::Foundation::Collections;

	public ref class SubjectGraph sealed
	{
	public:
		SubjectGraph(int nVerts);
		void AddEdge(int i, int j, double val);
		void AddGraphLines(Windows::Foundation::Collections::IVector<String^>^ lines);

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

