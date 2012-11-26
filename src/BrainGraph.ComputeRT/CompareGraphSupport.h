#pragma once

namespace BrainGraph { namespace Compute { namespace Graph
{
	struct TStat
	{
		TStat()
		{
			M1 = 0;
			M2 = 0;
			V1 = 0;
			V2 = 0;
			Value = 0;
			TwoTailCount = 0;
		}

		float M1;
		float M2;

		float V1;
		float V2;
	
		float Value;
		int TwoTailCount;
	};

	struct CompareEdge
	{
		CompareEdge()
		{
			Index = 0;
			AboveNBSThreshold = false;
		}

		int Index;
		std::pair<int, int> Nodes;
		bool AboveNBSThreshold;

		TStat Stats;
	};

	struct CompareNode
	{
		CompareNode()
		{
			Index = 0;
		}

		int Index;

		TStat Degree;
		TStat Strength;
		TStat Diversity;
	};

	class Component
	{
	public:
		Component() : Identifier(0)
		{}

		int Identifier;
		std::vector<std::shared_ptr<CompareEdge>> Edges;
		//std::vector<std::shared_ptr<CompareNode>> Nodes;

		std::vector<int> Vertices;  // TODO: Deprec

		int RightTailExtent;

		double GetAverageEdgeDifference()
		{
			double dVal = 0;
			for(auto edge : Edges)
				dVal += ( edge->Stats.M1 - edge->Stats.M2 );

			return dVal / Edges.size();
		}
	};

}}}
