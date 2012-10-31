#pragma once
#include "GraphLookup.h"
#include "SubjectGraph.h"
#include "CompareGraph.h"
#include "CompareGraphSupport.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace BrainGraph::Compute::Subjects;

	class SingleDatatypeCompare
	{
	public:
		SingleDatatypeCompare(int subjectCount, int verts, int edges);
		~SingleDatatypeCompare(void);

		void AddGraph(SubjectGraph^ graph);
		Component CompareGroups(vector<int> &idxs, int szGrp1, double tStatThreshold);
		Component Permute(const vector<int> &idxs, int szGrp1, double tStatThreshold);
		void GetComponents(vector<Component> &components);
	
	private:
		typedef boost::multi_array<float, 2> EdgesBySubject;
		typedef boost::multi_array<float, 2>::array_view<1>::type SingleEdgeBySubject;
		typedef boost::multi_array_types::index_range range;
		typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;

		void CalcEdgeTStats(const vector<int> &idxs, int szGrp1, vector<CompareEdge> &edgeStats);
		void ComputeComponents(UDGraph &graph, vector<int> &edgeIdxs, vector<Component> &components);

		int _subjectCount;
		int _currentSubjectIdx;
		int _vertCount;
		int _edgeCount;
		int _permutations;

		GraphLookup _lu;
		EdgesBySubject _subjectEdges;  // Mtx of edge vs subject  e.g. 4005x58

		vector<float> _allEdges;

		UDGraph _graph;
		
		vector<CompareEdge> _grpStats;
		vector<int> _grpSupraThreshEdgeIdxs;
		vector<pair<int, int>> _grpSupraThreshEdges;
		vector<Component> _grpComponent;
	};

}}}

