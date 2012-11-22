#pragma once
#include "MultiDatatypeSupport.h"
#include "SingleDatatypeCompare.h"
#include "Subject.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace Windows::Foundation::Collections;
	using namespace BrainGraph::Compute::Subjects;

	public ref class MultiDatatypeCompare sealed
	{
	public:

		MultiDatatypeCompare(int verts, int edges, IVector<Threshold^>^ thresholds);
		void LoadSubjects(IVector<Subject^>^ group1, IVector<Subject^>^ group2);		
		void Compare();
		void Permute(const vector<vector<int>> &permutations);  // TODO: Move the permutation generation to c++ code from c#

		//Overlap^ GetOverlapResult();*/

	private:
		void AddSubject(String^ groupId, Subject^ itm);

		int _subjectCount;
		int _vertices;
		int _edges;
		int _subCounter;

		int _realOverlap;
		int _rightTailOverlapCount;
		int _permutations;

		int _group1Count;

		vector<Threshold^> _dataThresholds;

		map<String^, shared_ptr<SingleDatatypeCompare>> _dataByType;
		
		map<String^, std::vector<int>> _subIdxsByGroup;
		
		map<int, shared_ptr<Vertex>> _verticesById;
		map<int, int> _overlapDistribution;
	};
}}}

