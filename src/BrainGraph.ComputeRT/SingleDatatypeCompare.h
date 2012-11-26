#pragma once
#include "GraphLookup.h"
#include "CompareGraph.h"
#include "CompareGraphSupport.h"
#include "SingleDatatypeSupport.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	namespace BCS = BrainGraph::Compute::Subjects;

	ref class BCS::Subject;
	ref class Threshold;
	struct BCS::SubjectGraphEdge;

	class SingleDatatypeCompare
	{
	public:
		SingleDatatypeCompare(int subjectCount, int verts, int edges, Threshold ^dataType);
		~SingleDatatypeCompare(void);

		void AddSubject(BCS::Subject^ subject);
		shared_ptr<Component> Compare(std::vector<int> &idxs, int szGrp1);
		shared_ptr<Component> Permute(std::vector<int> &idxs, int szGrp1);
	
	private:
		vector<shared_ptr<CompareEdge>> CalcEdgeComparison(std::vector<int> &idxs, int szGrp1);
		vector<shared_ptr<CompareNode>> CalcNodeComparison(vector<int> &idxs, int szGrp1);

		int _subjectCount;
		int _nodeCount;
		int _edgeCount;
		int _permutations;

		Threshold ^_threshold;
		shared_ptr<GraphLookup> _lu;
		shared_ptr<CompareGraph> _cmpGraph;

		std::vector<std::vector<BCS::SubjectGraphEdge>> _edges;  // Mtx of edge vs subject  e.g. 4005x58
		std::vector<std::vector<BCS::SubjectGraphNode>> _nodes;	// Mtx of node vs subject  e.g. 90x58
		std::vector<BCS::SubjectGraphGlobal> _globals;			// Arr of subject values  e.g. 1x58
	};

}}}

