#pragma once
#include "Component.h"
#include "GraphLookup.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	class Graph
	{
	public:
		
		typedef boost::adjacency_matrix<boost::undirectedS> UDGraph;
		
		Graph(int nVerts, GraphLookup* lu);
		~Graph(void);

		void AddEdge(int m, int n, EdgeValue val);
		EdgeValue GetEdge(int m, int n);

		float GlobalStrength();
		void GetMeanVtxStrength(vector<float> &meanVtxStr);

		void ComputeComponents();
		void GetComponents(vector<Component> &components);
		int GetLargestComponentId();
		int GetComponentExtent(int id);

		vector<Edge> Edges;
		vector<EdgeValue> EdgeValues;
		
		int ComponentCount;
		map<int, vector<int>> ComponentVertices;
		map<int, vector<ComponentEdge>> ComponentEdges;

	private:
		UDGraph _graph;
		GraphLookup * _lu;
		int _nVerts;
	};

}}}