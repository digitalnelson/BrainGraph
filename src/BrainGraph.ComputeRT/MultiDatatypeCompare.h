#pragma once
#include "MultiDatatypeSupport.h"
#include "SingleDatatypeCompare.h"
#include "Subject.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace BrainGraph::Compute::Subjects;

	public ref class MultiDatatypeCompare sealed
	{
	public:
		/*MultiDatatypeCompare(int subjectCount, int verts, int edges, vector<String^> &dataTypes);
		
		void AddSubject(Subject^ itm);
		void Compare(String^ group1, String^ group2, map<String^, Threshold> &threshes);
		void Permute(const vector<vector<int>> &permutations, map<String^, Threshold> &threshes);

		Overlap^ GetOverlapResult();*/

	private:
		int _subjectCount;
		int _vertices;
		int _edges;
		int _subCounter;

		int _realOverlap;
		int _rightTailOverlapCount;
		int _permutations;

		int _group1Count;

		vector<String^> _dataTypes;
		map<String^, shared_ptr<SingleDatatypeCompare>> _dataByType;
		map<String^, std::vector<int>> _subIdxsByGroup;
		map<int, shared_ptr<Vertex>> _verticesById;
		map<int, int> _overlapDistribution;
	};
}}}

