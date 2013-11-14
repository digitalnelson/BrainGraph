#pragma once
#include <memory>
#include <vector>
#include <boost/graph/adjacency_matrix.hpp>
#include <boost/graph/graph_utility.hpp>
#include <boost/graph/connected_components.hpp>
#include "Edge.hpp"
#include "Node.hpp"
#include "Component.hpp"
#include "Global.hpp"
#include "../GraphLookup.hpp"

namespace BrainGraph { namespace Compute { namespace Group
{
	class Graph
	{
	public:
		
		Graph(int nVerts, std::shared_ptr<GraphLookup> lu, double nbsThreshold) :
			_adjMtx(nVerts),
			_lu(lu),
			_nLargestComponentId(-1)
		{
			_nVerts = nVerts;
			_nbsThreshold = nbsThreshold;
		}

		~Graph(void)
		{}

		// TODO: Consider applying move semantics here
		void AddEdges(std::vector<std::shared_ptr<Edge>> &edges)
		{
			for (auto edge : edges)
				Edges.push_back(edge);
		}

		void AddNodes(std::vector<std::shared_ptr<Node>> &nodes)
		{
			for (auto node : nodes)
				Nodes.push_back(node);
		}

		void SetGlobal(std::shared_ptr<Global> global)
		{
			Global = global;
		}

		void UpdateEdgeStats(std::vector<std::shared_ptr<Edge>> &edges)
		{
			for (size_t idx = 0; idx<edges.size(); ++idx)
			{
				// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
				if (abs(edges[idx]->Stats.Value) >= abs(this->Edges[idx]->Stats.Value))
					this->Edges[idx]->Stats.TwoTailCount++;
			}
		}

		void UpdateNodeStats(std::vector<std::shared_ptr<Node>> &nodes)
		{
			for (size_t idx = 0; idx<nodes.size(); ++idx)
			{
				// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
				if (abs(nodes[idx]->Strength.Value) >= abs(this->Nodes[idx]->Strength.Value))
					this->Nodes[idx]->Strength.TwoTailCount++;
			}
		}

		void UpdateGlobalStats(std::shared_ptr<Global> global)
		{
			if (abs(global->Strength.Stats.Value) >= abs(Global->Strength.Stats.Value))
				Global->Strength.Stats.TwoTailCount++;
		}

		void ComputeComponents()
		{
			// TODO: Should this happen when we add the edges?
			// Load graph with thresholded t stats
			for (auto edge : Edges)
			{
				// If our edge tstat is larger than our threshold keep it for NBS
				if (abs(edge->Stats.Value) > _nbsThreshold)
				{
					edge->AboveNBSThreshold = true;
					boost::add_edge(edge->Nodes.first, edge->Nodes.second, _adjMtx);
				}
			}

			// Place to hold boost dfs result for cmps
			std::vector<int> cmpRawListingByVertex(_nVerts);

			// Ask boost for a raw list of components (makes a vector with idx of vertex and val of cmp)
			int componentCount = boost::connected_components(_adjMtx, &cmpRawListingByVertex[0]);

			// Make vector big enough to hold the cmps
			Components.clear();
			Components.resize(componentCount);

			// Load up all the cmps into our cmp map
			for (size_t i = 0; i<cmpRawListingByVertex.size(); i++)
			{
				if (Components[cmpRawListingByVertex[i]] == nullptr)
				{
					Components[cmpRawListingByVertex[i]] = std::shared_ptr<Component>(new Component());
					Components[cmpRawListingByVertex[i]]->Identifier = cmpRawListingByVertex[i];
				}

				Components[cmpRawListingByVertex[i]]->Vertices.push_back(i);
			}

			for (auto edge : Edges)
			{
				if (edge->AboveNBSThreshold)
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
			for (auto cmp : Components)
			{
				// Check the size of this guy
				int cmpEdgeCount = cmp->Edges.size();

				if (cmpEdgeCount > maxEdgeCount)
				{
					maxEdgeCount = cmpEdgeCount;
					_nLargestComponentId = cmp->Identifier;
				}
			}
		}

		std::shared_ptr<Component> GetLargestComponent()
		{
			if (_nLargestComponentId >= 0)
				return Components[_nLargestComponentId];
			else
				return nullptr;
		}

		std::vector<std::shared_ptr<Edge>> Edges;
		std::vector<std::shared_ptr<Node>> Nodes;
		std::vector<std::shared_ptr<Component>> Components;
		std::shared_ptr<Global> Global;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		std::shared_ptr<GraphLookup> _lu;
		
		int _nVerts;
		double _nbsThreshold;

		int _nLargestComponentId;
	};

}}}

