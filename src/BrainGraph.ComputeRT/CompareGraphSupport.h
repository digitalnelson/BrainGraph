#pragma once

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	struct CompareEdge
	{
		CompareEdge()
		{
			Idx = 0;
			M1 = 0;
			M2 = 0;
			V1 = 0;
			V2 = 0;
			TStat = 0;
			PValue = 0;
			RightTailCount = 0;
		}

		int Idx;
		std::pair<int, int> Vertices;

		float M1;
		float M2;

		float V1;
		float V2;
	
		float TStat;
	
		float PValue;
		int RightTailCount;
	};

	struct ComponentEdge
	{
		int ComponentIndex;

		std::pair<int, int> Edge;
		int EdgeIndex;

		shared_ptr<CompareEdge> EdgeValue;
	};

	struct Component
	{
		int Identifier;
		vector<shared_ptr<ComponentEdge>> Edges;
		vector<int> Vertices;

		int RightTailExtent;
	};

}}}
