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

		double M1;
		double M2;

		double V1;
		double V2;
	
		double Value;
		int TwoTailCount;
	};

	class Pearson
	{
	public:
		Pearson()
		{
			SumX = SumY = SumProdXY = SumXSqu = SumYSqu = Count = 0;
		}

		void Include(double x, double y)
		{
			SumX += x;
			SumY += y;
			SumProdXY += x * y;
			SumXSqu += pow(x, 2);
			SumYSqu += pow(y, 2);

			++Count;
		}

		double Calculate()
		{
			return ( ( Count * SumProdXY ) - ( SumX * SumY ) ) / sqrt ( (Count * SumXSqu - pow(SumX, 2)) * (Count * SumYSqu - pow(SumY, 2)) );
		}

	private:
		double SumX;
		double SumY;
		double SumProdXY;
		double SumXSqu;
		double SumYSqu;
		int Count;
	};

	class PearsonCompare
	{
	public:
		PearsonCompare()
		{}

		void IncludeValue(int groupId, double x, double y)
		{
			All.Include(x, y);
			
			if(groupId == 0)
				Group1.Include(x, y);
			else
				Group2.Include(x, y);
		}
		
		Pearson All, Group1, Group2;
	};

	struct CompareItem
	{
		TStat Stat;
		std::vector<PearsonCompare> Associations;
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

	struct CompareGlobal
	{
		TStat Strength;
		CompareItem StregnthItm;
	};

}}}
