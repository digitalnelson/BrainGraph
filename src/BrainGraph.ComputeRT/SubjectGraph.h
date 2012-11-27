#pragma once
#include "SubjectGraphEdge.h"
#include <collection.h>

namespace BrainGraph { namespace Compute { namespace Subjects
{
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

		std::vector<SubjectGraphEdge> Edges;
		std::vector<SubjectGraphNode> Nodes;

		float GlobalStrength();
		/*std::vector<float> NodalStrength();*/

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		int _nVerts;
	};

}}}

