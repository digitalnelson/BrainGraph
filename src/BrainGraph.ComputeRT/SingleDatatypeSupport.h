#pragma once

namespace BrainGraph { namespace Compute { namespace Graph
{
	struct TStat;

	class TStatCalc
	{
	public:
		TStatCalc()
		{
			n1 = 0, n2 = 0;
			m1 = 0, m2 = 0;
			dv1 = 0, dv2 = 0;
		}

		void IncludeValue(int groupId, double value)
		{
			if(groupId == 0)
			{
				n1++;

				double delta = value - m1;
				m1 += delta / n1;

				if(n1 > 1)
					dv1 = dv1 + delta * (value - m1);
			}
			else
			{
				n2++;

				double delta = value - m2;
				m2 += delta / n2;

				if(n2 > 1)
					dv2 = dv2 + delta * (value - m2);
			}
		}

		TStat Calculate()
		{
			double v1 = abs(dv1) / ( n1 - 1 );
			double v2 = abs(dv2) / ( n2 - 1 );
			
			double tstat = 0;
			if(v1 < 0.00000001f && v2 < 0.00000001f)
				tstat = 0;
			else
				tstat = (m1 - m2) / sqrt( ( v1 / (double)n1 ) + ( v2 / (double)n2 ) );

			TStat stat;
			stat.V1 = v1;
			stat.V2 = v2;
			stat.M1 = m1;
			stat.M2 = m2;
			stat.Value = tstat;

			return stat;
		}
		
	private:
		int n1, n2;
		double m1, m2;
		double dv1, dv2;
	};

	class PearsonCalc
	{
	public:
		PearsonCalc()
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
			return ( Count * SumProdXY ) - ( SumX * SumY ) / sqrt ( (Count * SumXSqu - pow(SumX, 2)) * (Count * SumYSqu - pow(SumY, 2)) );
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
		
		PearsonCalc All, Group1, Group2;
	};
}}}