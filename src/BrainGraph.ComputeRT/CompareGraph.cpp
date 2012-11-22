#include "pch.h"
#include "CompareGraph.h"
#include <exception>

namespace BrainGraph { namespace Compute { namespace Graph
{
	CompareGraph::CompareGraph(int nVerts, shared_ptr<GraphLookup> lu) : 
		_adjMtx(nVerts), 
		_lu(lu)
	{
		_nVerts = nVerts;
	}

	CompareGraph::~CompareGraph(void)
	{}

	void CompareGraph::AddEdge(int i, int j, shared_ptr<CompareEdge> val)
	{
		if(i >= _nVerts)
			throw std::exception("Exceeded matrix bounds.  i >= number of vertices");
		if(j >= _nVerts)
			throw std::exception("Exceeded matrix bounds.  i >= _m");

		Edges.push_back(val);
		boost::add_edge(i, j, _adjMtx);
	}

	shared_ptr<CompareEdge> CompareGraph::GetEdge(int i, int j)
	{
		int idx = _lu->GetEdge(i, j);
		return Edges[idx];
	}

	void CompareGraph::ComputeComponents()
	{
		// Place to hold boost dfs result for cmps
		vector<int> cmpRawListingByVertex(_nVerts);

		// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
		int componentCount = boost::connected_components(_adjMtx, &cmpRawListingByVertex[0]);
		
		// Make vector big enough to hold the cmps
		Components.resize(componentCount);

		// Load up all the cmps into our cmp map
		for(int i=0; i<cmpRawListingByVertex.size(); i++)
		{
			Components[cmpRawListingByVertex[i]] = shared_ptr<Component>(new Component());
			Components[cmpRawListingByVertex[i]]->Identifier = cmpRawListingByVertex[i];
			Components[cmpRawListingByVertex[i]]->RightTailExtent = 0;
			Components[cmpRawListingByVertex[i]]->Vertices.push_back(i);
		}
		
		for(auto edge : Edges)
		{
			shared_ptr<ComponentEdge> cmpEdge = make_shared<ComponentEdge>();

			cmpEdge->EdgeIndex = edge->Idx;
			// Lookup the edge
			cmpEdge->Edge = edge->Vertices;
			// Look up the first edge vertex
			cmpEdge->ComponentIndex = cmpRawListingByVertex[cmpEdge->Edge.first];
			// Pull out the edge value
			cmpEdge->EdgeValue = edge;

			// Store the ident for this cmp
			Components[cmpEdge->ComponentIndex]->Identifier = cmpEdge->ComponentIndex;
			// Add edge to component edge map
			Components[cmpEdge->ComponentIndex]->Edges.push_back(cmpEdge);
		}
	}

	shared_ptr<Component> CompareGraph::GetLargestComponent()
	{
		//// Find the biggest component
		//int maxEdgeCount = 0, maxId = 0;
		//for(auto &cmp : _grpComponent)
		//{
		//	int cmpEdgeCount = cmp.Edges.size();
		//	if(cmpEdgeCount > maxEdgeCount)
		//	{
		//		maxEdgeCount = cmpEdgeCount;
		//		maxId = cmp.Identifier;
		//	}
		//}
	}

	/*void CompareGraph::GetComponents(std::vector<Component> &components)
	{
		for(auto &cmp : _grpComponent)
		{
			for(auto &edge : cmp.Edges)
			{
				edge.EdgeValue = _grpStats[edge.EdgeIndex];
			}
		}

		components = _grpComponent;
	}*/

}}}
