#pragma once
#include <memory>
#include <utility>
#include <vector>
#include "boost/graph/adjacency_matrix.hpp"
#include "Edge.hpp"
#include "Node.hpp"
#include "Global.hpp"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	class Graph
	{
	public:
		Graph(int nVerts) : _adjMtx(nVerts), Nodes(nVerts)
		{
			_nVerts = nVerts;
		}

		void AddEdge(int i, int j, double val)
		{
			boost::add_edge(i, j, _adjMtx);

			std::shared_ptr<Edge> edge = std::make_shared<Edge>();
			edge->Vertices = std::pair<int, int>(i, j);
			edge->Value = val;

			Edges.push_back(edge);

			if (Nodes[i] == nullptr)
				Nodes[i] = std::make_shared<Node>();
			if (Nodes[j] == nullptr)
				Nodes[j] = std::make_shared<Node>();

			Nodes[i]->Index = i;
			Nodes[j]->Index = j;

			if (val > 0)
			{
				// Update the degree - note: this will only be valid in some data types
				Nodes[i]->Degree += 1;
				Nodes[j]->Degree += 1;

				// Update the total strength
				Nodes[i]->TotalStrength += val;
				Nodes[j]->TotalStrength += val;
			}
		}

		double GlobalStrength()
		{
			if (_nVerts != 0)
			{
				double total = 0;

				for (auto node : Nodes)
					total += (node->TotalStrength / (_nVerts - 1));

				return total / _nVerts;
			}
			else
				return 0;
		}

		std::vector<std::shared_ptr<Edge>> Edges;
		std::vector<std::shared_ptr<Node>> Nodes;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		int _nVerts;
	};

}}}
