#pragma once
#include <memory>
#include <utility>
#include <vector>
#include "boost/graph/adjacency_matrix.hpp"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	struct SubjectGraphEdge
	{
		SubjectGraphEdge()
		{
			Index = 0;
			Value = 0;
		}

		int Index;
		std::pair<int, int> Vertices;

		double Value;
	};

	struct SubjectGraphNode
	{
		int Index;

		double Degree;
		double TotalStrength;

		SubjectGraphNode()
		{
			Index = 0;
			Degree = 0;
			TotalStrength = 0;
		}
	};

	struct SubjectGraphGlobal
	{
		double Strength;

		SubjectGraphGlobal()
		{
			Strength = 0;
		}
	};

	struct SubjectGraphAttribute
	{
		double Value;
	};

	class SubjectGraph
	{
	public:
		SubjectGraph::SubjectGraph(int nVerts) : _adjMtx(nVerts), Nodes(nVerts)
		{
			_nVerts = nVerts;
		}

		void SubjectGraph::AddEdge(int i, int j, double val)
		{
			boost::add_edge(i, j, _adjMtx);

			std::shared_ptr<SubjectGraphEdge> edge = std::make_shared<SubjectGraphEdge>();
			edge->Vertices = std::pair<int, int>(i, j);
			edge->Value = val;

			Edges.push_back(edge);

			if (Nodes[i] == nullptr)
				Nodes[i] = std::make_shared<SubjectGraphNode>();
			if (Nodes[j] == nullptr)
				Nodes[j] = std::make_shared<SubjectGraphNode>();

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

		double SubjectGraph::GlobalStrength()
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

		std::vector<std::shared_ptr<SubjectGraphEdge>> Edges;
		std::vector<std::shared_ptr<SubjectGraphNode>> Nodes;

	private:
		boost::adjacency_matrix<boost::undirectedS> _adjMtx;
		int _nVerts;
	};

}}}
