#pragma once
#include <atomic>
#include "math.h"

namespace BrainGraph { namespace Compute { namespace Group
{
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

	

}}}
