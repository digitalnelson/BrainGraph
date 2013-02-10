#include "pch.h"
#include "CompareGraph.h"
#include <exception>

namespace BrainGraph { namespace Compute { namespace Graph
{
	CompareGraph::CompareGraph(int nVerts, shared_ptr<GraphLookup> lu, double nbsThreshold) : 
		_adjMtx(nVerts), 
		_lu(lu),
		_nLargestComponentId(-1)
	{
		_nVerts = nVerts;
		_nbsThreshold = nbsThreshold;
	}

	CompareGraph::~CompareGraph(void)
	{}

	// TODO: Consider applying move semantics here
	void CompareGraph::AddEdges(std::vector<std::shared_ptr<CompareEdge>> &edges)
	{
		for(auto edge : edges)
			Edges.push_back(edge);
	}

	void CompareGraph::AddNodes(std::vector<std::shared_ptr<CompareNode>> &nodes)
	{
		for(auto node : nodes)
			Nodes.push_back(node);
	}

	void CompareGraph::SetGlobal(std::shared_ptr<CompareGlobal> global)
	{
		Global = global;
	}

	void CompareGraph::UpdateEdgeStats(std::vector<std::shared_ptr<CompareEdge>> &edges)
	{
		for(auto idx=0; idx<edges.size(); ++idx)
		{
			// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
			if(abs(edges[idx]->Stats.Value) >= abs(this->Edges[idx]->Stats.Value))
				this->Edges[idx]->Stats.TwoTailCount++;
		}
	}

	void CompareGraph::UpdateNodeStats(std::vector<std::shared_ptr<CompareNode>> &nodes)
	{
		for(auto idx=0; idx<nodes.size(); ++idx)
		{
			// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
			if(abs(nodes[idx]->Strength.Value) >= abs(this->Nodes[idx]->Strength.Value))
				this->Nodes[idx]->Strength.TwoTailCount++;
		}
	}

	void CompareGraph::UpdateGlobalStats(std::shared_ptr<CompareGlobal> global)
	{
		if(abs(global->Strength.Value) >= abs(Global->Strength.Value))
			Global->Strength.TwoTailCount++;
	}

	void CompareGraph::ComputeComponents()
	{
		// TODO: Should this happen when we add the edges?
		// Load graph with thresholded t stats
		for(auto edge : Edges)
		{
			// If our edge tstat is larger than our threshold keep it for NBS
			if(abs(edge->Stats.Value) > _nbsThreshold)
			{
				edge->AboveNBSThreshold = true;
				boost::add_edge(edge->Nodes.first, edge->Nodes.second, _adjMtx);
			}
		}

		// Place to hold boost dfs result for cmps
		vector<int> cmpRawListingByVertex(_nVerts);

		// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
		int componentCount = boost::connected_components(_adjMtx, &cmpRawListingByVertex[0]);
		
		// Make vector big enough to hold the cmps
		Components.clear();
		Components.resize(componentCount);

		// Load up all the cmps into our cmp map
		for(int i=0; i<cmpRawListingByVertex.size(); i++)
		{
			if(Components[cmpRawListingByVertex[i]] == nullptr)
			{
				Components[cmpRawListingByVertex[i]] = shared_ptr<Component>(new Component());
				Components[cmpRawListingByVertex[i]]->Identifier = cmpRawListingByVertex[i];
				Components[cmpRawListingByVertex[i]]->RightTailExtent = 0;
			}

			Components[cmpRawListingByVertex[i]]->Vertices.push_back(i);
		}
		
		for(auto edge : Edges)
		{
			if(edge->AboveNBSThreshold)
			{
				// Lookup the component index for this edge
				int componentIdx = cmpRawListingByVertex[edge->Nodes.first];

				// Add edge to component edge map
				Components[componentIdx]->Edges.push_back(edge);
			}
		}

		// Some vars for the biggest cmp
		int maxEdgeCount = 0;

		// Loop through cmps and find the biggest
		for(auto cmp : Components)
		{
			// Check the size of this guy
			int cmpEdgeCount = cmp->Edges.size();

			if(cmpEdgeCount > maxEdgeCount)
			{
				maxEdgeCount = cmpEdgeCount;
				_nLargestComponentId = cmp->Identifier;
			}
		}
	}

	shared_ptr<Component> CompareGraph::GetLargestComponent()
	{
		if(_nLargestComponentId >= 0)
			return Components[_nLargestComponentId];
		else
			return nullptr;
	}

}}}
