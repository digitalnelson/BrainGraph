#include "pch.h"
#include "SubjectGraph.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Platform;
	using namespace Windows::Foundation::Collections;

	SubjectGraph::SubjectGraph(int nVerts) : _adjMtx(nVerts), Nodes(nVerts)
	{
		_nVerts = nVerts;
	}

	void SubjectGraph::AddEdge(int i, int j, double val)
	{
		if(i >= _nVerts)
			throw ref new Platform::Exception(E_FAIL, "Exceeded matrix bounds.  i >= number of verticies");
		if(j >= _nVerts)
			throw ref new Platform::Exception(E_FAIL, "Exceeded matrix bounds.  i >= _m");

		boost::add_edge(i, j, _adjMtx);

		SubjectGraphEdge edge;
		edge.Vertices = std::pair<int, int>(i, j);
		edge.Value = val;

		Edges.push_back(edge);

		Nodes[i].Index = i;
		Nodes[j].Index = j;

		if(val > 0)
		{
			// Update the degree - note: this will only be valid in some data types
			Nodes[i].Degree += 1;
			Nodes[j].Degree += 1;

			// Update the total strength
			Nodes[i].TotalStrength += val;
			Nodes[j].TotalStrength += val;
		}
	}

	double SubjectGraph::GlobalStrength()
	{
		if(_nVerts != 0)
		{
			double total = 0;

			for(auto node : Nodes)
				total += ( node.TotalStrength / (_nVerts - 1));
		
			return total / _nVerts;
		}
		else
			return 0;
	}

}}}