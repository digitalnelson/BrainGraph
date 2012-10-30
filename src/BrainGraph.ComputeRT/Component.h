#pragma once

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;

	struct EdgeValue
	{
		EdgeValue()
		{
			Idx = 0;
			Value = 0;
			M1 = 0;
			M2 = 0;
			V1 = 0;
			V2 = 0;
			TStat = 0;
			PValue = 0;
			RightTailCount = 0;
		}

		int Idx;

		float Value;

		float M1;
		float M2;

		float V1;
		float V2;
	
		float TStat;
	
		float PValue;
		int RightTailCount;
	};

	struct GroupEdge
	{
		GroupEdge()
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

	typedef std::pair<int, int> Edge;
	//struct Edge
	//{
	//	int EdgeIndex;
	//	pair<int, int> Vertices;

	//	float Value;
	//};

	struct ComponentEdge
	{
		int ComponentIndex;

		Edge Edge;
		int EdgeIndex;

		EdgeValue EdgeValue;
	};

	struct Component
	{
		int Identifier;
		vector<ComponentEdge> Edges;
		vector<int> Vertices;

		int RightTailExtent;
	};

}}}
