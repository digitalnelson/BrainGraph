#pragma once
#include "MultiDatatypeSupport.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	// Namespace shortcuts
	namespace WF = Windows::Foundation;
	namespace WFC = Windows::Foundation::Collections;
	namespace BCS = BrainGraph::Compute::Subjects;

	// Forward decls
	ref class BCS::Subject;
	class SingleDatatypeCompare;

	public ref class MultiDatatypeCompare sealed
	{
	public:

		MultiDatatypeCompare(int verts, int edges, WFC::IVector<Threshold^>^ thresholds);
		void LoadSubjects(WFC::IVector<BCS::Subject^>^ group1, WFC::IVector<BCS::Subject^>^ group2);		
		void Compare();
		WF::IAsyncActionWithProgress<int>^ PermuteAsyncWithProgress(int permutations);
		void GetResult();

	private:
		void AddSubject(Platform::String^ groupId, BCS::Subject^ itm);

		int _vertices;
		int _edges;

		int _subjectCount;
		int _subCounter;
		int _group1Count;

		std::map<Platform::String^, std::vector<int>> _subIdxsByGroup;
		std::map<Platform::String^, std::shared_ptr<SingleDatatypeCompare>> _groupCompareByType;
		std::vector<Threshold^> _dataThresholds;
		
		int _realOverlap;
		int _rightTailOverlapCount;
		int _permutations;
		std::map<int, std::shared_ptr<Vertex>> _verticesById;
		std::map<int, int> _overlapDistribution;
	};
}}}

