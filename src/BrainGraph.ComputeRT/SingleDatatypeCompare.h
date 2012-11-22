#pragma once
#include "GraphLookup.h"
#include "SubjectGraph.h"
#include "CompareGraph.h"
#include "CompareGraphSupport.h"
#include "Subject.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace BrainGraph::Compute::Subjects;

	ref class Threshold;

	class SingleDatatypeCompare
	{
	public:
		SingleDatatypeCompare(int subjectCount, int verts, int edges, Threshold ^dataType);
		~SingleDatatypeCompare(void);

		void AddSubject(Subject^ subject);
		Component CompareGroups(vector<int> &idxs, int szGrp1);
		Component Permute(const vector<int> &idxs, int szGrp1);
		void GetComponents(vector<Component> &components);
	
	private:
		typedef boost::multi_array<float, 2> EdgesBySubject;
		typedef boost::multi_array<float, 2>::array_view<1>::type SingleEdgeBySubject;
		typedef boost::multi_array_types::index_range range;
		typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;

		void CalcEdgeTStats(const vector<int> &idxs, int szGrp1, vector<CompareEdge> &edgeStats);
		void ComputeComponents(UDGraph &graph, vector<int> &edgeIdxs, vector<Component> &components);

		Threshold ^_threshold;
		shared_ptr<CompareGraph> _cmpGraph;

		int _subjectCount;
		int _currentSubjectIdx;
		int _vertCount;
		int _edgeCount;
		int _permutations;

		shared_ptr<GraphLookup> _lu;
		EdgesBySubject _subjectEdges;  // Mtx of edge vs subject  e.g. 4005x58

		vector<float> _allEdges;

		UDGraph _graph;
		
		vector<CompareEdge> _grpStats;
		vector<int> _grpSupraThreshEdgeIdxs;
		vector<pair<int, int>> _grpSupraThreshEdges;
		vector<Component> _grpComponent;
	};

}}}

