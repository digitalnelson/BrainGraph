#pragma once
#include <atomic>

namespace BrainGraph { namespace Compute { namespace Group
{
	struct TStat
	{
		double M1;
		double M2;

		double V1;
		double V2;
	
		double Value;
		std::atomic<int> TwoTailCount;

		TStat()
		{
			M1 = 0;
			M2 = 0;
			V1 = 0;
			V2 = 0;
			Value = 0;
			TwoTailCount = 0;
		}

		TStat(TStat &other) : M1(other.M1), M2(other.M2), V1(other.V1), V2(other.V2), Value(other.Value), TwoTailCount(other.TwoTailCount.load())
		{}

		TStat& operator= (const TStat &other)
		{
			M1 = other.M1;
			M2 = other.M2;
			V1 = other.V1;
			V2 = other.V2;
			Value = other.Value;
			TwoTailCount.exchange(other.TwoTailCount.load());

			return *this;
		}
	};

	

}}}
