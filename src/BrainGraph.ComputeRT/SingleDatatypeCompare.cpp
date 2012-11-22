#include "pch.h"
#include "SingleDatatypeCompare.h"
#include "MultiDatatypeSupport.h"
#include <algorithm>

using namespace std;
using namespace concurrency;

namespace BrainGraph { namespace Compute { namespace Graph
{
	SingleDatatypeCompare::SingleDatatypeCompare(int subjectCount, int verts, int edges, Threshold ^threshold) 
		: _subjectEdges(boost::extents[edges][subjectCount]), 
		_lu(new GraphLookup(verts)), 
		_grpStats(edges), 
		_graph(verts), 
		_cmpGraph(new CompareGraph(_vertCount, _lu))
	{
		_threshold = threshold;
		_subjectCount = subjectCount;
		_vertCount = verts;
		_edgeCount = edges;
		_currentSubjectIdx = 0;
		_permutations = 0;
	}

	SingleDatatypeCompare::~SingleDatatypeCompare(void)
	{}

	void SingleDatatypeCompare::AddSubject(Subject^ subject)
	{
		// TODO: What to do if the graph does not exist?
		// Pull out the graph for this datatype
		auto graph = subject->Graphs->Lookup(_threshold->DataType);

		// Add our graph to the list
		int edge = 0;
		for(auto &iter : graph->Edges)
		{
			_subjectEdges[edge][_currentSubjectIdx] = iter.Value;			
			++edge;
		}
		++_currentSubjectIdx;
	}

	// Calculate a T stat for the two groups based on the indexes passed in
	void SingleDatatypeCompare::CalcEdgeTStats(const vector<int> &idxs, int szGrp1, vector<shared_ptr<CompareEdge>> &edgeStats)
	{
		// TODO: Probably need to make this thread safe
		//for(int i=0; i<_edgeCount; i++)
		parallel_for(0, _edgeCount, [=, &idxs, &edgeStats] (int i)
		{
			// Pull out a view of the subject values for a single edge
			SingleEdgeBySubject edgeValues = _subjectEdges[ boost::indices[i][range(0, _subjectCount)] ];
			
			int n1 = 0, n2 = 0;
			float m1 = 0, m2 = 0;
			float dv1 = 0, dv2 = 0;

			// Loop through the vals we were passed
			for (int idx = 0; idx < _subjectCount; ++idx)
			{
				float edgeVal = edgeValues[idxs[idx]];

				if (idx < szGrp1)
				{
					n1++;

					float delta = edgeVal - m1;
					m1 += delta / n1;

					if(n1 > 1)
						dv1 = dv1 + delta * (edgeVal - m1);
				}
				else
				{
					n2++;

					float delta = edgeVal - m2;
					m2 += delta / n2;

					if(n2 > 1)
						dv2 = dv2 + delta * (edgeVal - m2);
				}
			}

			float v1 = abs(dv1) / ( n1 - 1 );
			float v2 = abs(dv2) / ( n2 - 1 );
			
			float tstat = 0;
			if(v1 < 0.00000001f && v2 < 0.00000001f)
				tstat = 0;
			else
				tstat = (m1 - m2) / sqrt( ( v1 / (float)n1 ) + ( v2 / (float)n2 ) );

			auto edge = make_shared<CompareEdge>();
			edge->V1 = v1;
			edge->V2 = v2;
			edge->M1 = m1;
			edge->M2 = m2;
			edge->TStat = tstat;
			edgeStats[i] = edge;
		});
	}

	Component SingleDatatypeCompare::CompareGroups(vector<int> &idxs, int szGrp1)
	{
		// Calculate t stats for this subject labeling
		CalcEdgeTStats(idxs, szGrp1, _grpStats);

		// Create a comparison graph to hold our group comparison results
		shared_ptr<CompareGraph> cmpGraph(new CompareGraph(_vertCount, _lu));

		// Load graph with thresholded t stats
		for(vector<CompareEdge>::size_type idx=0; idx<_grpStats.size(); ++idx)
		{
			// If our edge tstat is larger than our threshold keep it for NBS
			if(abs(_grpStats[idx]->TStat) > _threshold->Value)
			{
				// Lookup our edge
				pair<int, int> edge = _lu->GetEdge(idx);

				// Add our edge to the group graph
				cmpGraph->AddEdge(edge.first, edge.second, _grpStats[idx]);
			}
		}

		// Calculate our largest component
		cmpGraph->ComputeComponents();

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

		//return _grpComponent[maxId];
	}

	Component SingleDatatypeCompare::Permute(const vector<int> &idxs, int szGrp1)
	{
		// Create somewhere temp to store our permuted t stats
		vector<CompareEdge> edgeStats(_edgeCount);

		// Calculate t stats for this random subject labeling
		CalcEdgeTStats(idxs, szGrp1, edgeStats);

		// Create a comparison graph to hold our group comparison results
		unique_ptr<CompareGraph> cmpGraph(new CompareGraph(_vertCount, _lu));

		// Loop through edge stats and calc	our measures
		for(vector<CompareEdge>::size_type idx=0; idx<edgeStats.size(); ++idx)
		{
			// Edge level testing - If this tstat is bigger than our grp tstat, increment the count
			if(abs(edgeStats[idx].TStat) >= abs(_grpStats[idx].TStat))
				_grpStats[idx].RightTailCount++;

			// TODO: Eventually store this as a graph tag so we can have multiple pieces of info stored with our edges
			// NBS level testing - Add this to the NBS graph if above our threshold, we will calc the size of the cmp soon
			if(abs(edgeStats[idx].TStat) > _threshold->Value)
			{
				// Lookup our edge
				pair<int, int> edge = _lu->GetEdge(idx);
				
				// Add our edge to the group graph
				cmpGraph->AddEdge(edge.first, edge.second, edgeStats[idx]);
			}
		}

		// NBS calcs
		vector<Component> cmps;

		//// Calculate our largest component
		//ComputeComponents(g, supraThreshEdgeIdxs, cmps);

		// Find the biggest component
		int maxEdgeCount = 0, maxId = 0;
		//for(auto &cmp : cmps)
		//{
		//	int cmpEdgeCount = cmp.Edges.size();
		//	if(cmpEdgeCount > maxEdgeCount)
		//	{
		//		maxEdgeCount = cmpEdgeCount;
		//		maxId = cmp.Identifier;
		//	}
		//}

		//// Increment rt tail if this is larger than the actual component
		//for(auto &cmp : _grpComponent)
		//{
		//	if(maxEdgeCount >= cmp.Edges.size())
		//		++cmp.RightTailExtent;
		//}

		//// Keep track of permutations
		//++_permutations;

		return cmps[maxId];
	}
}}}
