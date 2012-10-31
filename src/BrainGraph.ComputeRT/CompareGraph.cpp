#include "pch.h"
#include "CompareGraph.h"
#include <exception>

namespace BrainGraph { namespace Compute { namespace Graph
{
	CompareGraph::CompareGraph(int nVerts, GraphLookup* lu) : _adjMtx(nVerts)
	{
		_nVerts = nVerts;
		_lu = lu;
	}

	CompareGraph::~CompareGraph(void)
	{}

	void CompareGraph::AddEdge(int i, int j, CompareEdge val)
	{
		if(i >= _nVerts)
			throw std::exception("Exceeded matrix bounds.  i >= number of verticies");
		if(j >= _nVerts)
			throw std::exception("Exceeded matrix bounds.  i >= _m");

		Edges.push_back(val);

		boost::add_edge(i, j, _adjMtx);
	}

	CompareEdge CompareGraph::GetEdge(int i, int j)
	{
		int idx = _lu->GetEdge(i, j);
		return Edges[idx];
	}

	void CompareGraph::ComputeComponents(vector<int> &edgeIdxs)
	{
		vector<int> cmpRawListingByVertex(_nVerts);

		// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
		int componentCount = boost::connected_components(_adjMtx, &cmpRawListingByVertex[0]);
		
		Components.resize(componentCount);

		for(int i=0; i<cmpRawListingByVertex.size(); i++)
		{
			Components[cmpRawListingByVertex[i]].Identifier = cmpRawListingByVertex[i];
			Components[cmpRawListingByVertex[i]].RightTailExtent = 0;
			Components[cmpRawListingByVertex[i]].Vertices.push_back(i);
		}
		
		for(auto edgeIdx : edgeIdxs)
		{
			ComponentEdge edgeItm;

			edgeItm.EdgeIndex = edgeIdx;
			// Lookup the edge
			edgeItm.Edge = _lu->GetEdge(edgeIdx);
			// Look up the first edge vertex
			edgeItm.ComponentIndex = cmpRawListingByVertex[edgeItm.Edge.first];
			// Pull out the edge value
			//edgeItm.EdgeValue = _grpStats[edgeIdx];

			// Store the ident for this cmp
			Components[edgeItm.ComponentIndex].Identifier = edgeItm.ComponentIndex;
			// Add edge to component edge map
			Components[edgeItm.ComponentIndex].Edges.push_back(edgeItm);
		}
	}

	/*void SingleDatatypeCompare::GetComponents(std::vector<Component> &components)
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
