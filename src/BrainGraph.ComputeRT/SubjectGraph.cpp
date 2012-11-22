#include "pch.h"
#include "SubjectGraph.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Platform;
	using namespace Windows::Foundation::Collections;

	SubjectGraph::SubjectGraph(int nVerts) : _adjMtx(nVerts)
	{
		_nVerts = nVerts;
	}

	void SubjectGraph::AddGraphLines(Windows::Foundation::Collections::IVector<String^>^ lines)
	{
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
	}

	//SubjectGraphEdge SubjectGraph::GetEdge(int i, int j)
	//{
	//	// TODO: Figure out how to do a lookup on this
	//	int idx = 0; //_lu->GetEdge(i, j);
	//	return Edges[idx];
	//}

	float SubjectGraph::GlobalStrength()
	{
		/*std::vector<float> roiStrs;

		for(int vert=0; vert<_nVerts; vert++)
		{
			float gsRoi = 0;
			for(int overt=0; overt<_nVerts; overt++)
			{
				if(vert != overt)
				{
					int idx = _lu->GetEdge(vert, overt);
					gsRoi += EdgeValues[idx].Value;
				}
			}

			roiStrs.push_back(gsRoi / (_nVerts - 1));
		}

		float gs = 0;

		for (auto i : roiStrs)
		{
			gs += i;
		}

		return gs / _nVerts;*/

		return 0;
	}

	void SubjectGraph::GetMeanVtxStrength(std::vector<float> &meanVtxStr)
	{
		/*for(int vert=0; vert<_nVerts; vert++)
		{
			float gsRoi = 0;
			for(int overt=0; overt<_nVerts; overt++)
			{
				if(vert != overt)
				{
					int idx = _lu->GetEdge(vert, overt);
					gsRoi += EdgeValues[idx].Value;
				}
			}

			meanVtxStr.push_back(gsRoi / (_nVerts - 1));
		}*/
	}

}}}