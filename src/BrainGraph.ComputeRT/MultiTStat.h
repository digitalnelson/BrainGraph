#pragma once
#include "collection.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	struct TStat;
	
	public ref class MultiTStat sealed
	{
	public:
		property float M1 { float get() { return _stat.M1; } }
		property float M2 { float get() { return _stat.M2; } }
		
		property float V1 { float get() { return _stat.V1; } }
		property float V2 { float get() { return _stat.V2; } }
		
		property float Value { float get() { return _stat.Value; } }
		property int TwoTailCount { int get() { return _stat.TwoTailCount; } }

	internal:
		MultiTStat(TStat tstat)
		{
			_stat = tstat;
		}

	private:
		TStat _stat;
	};

}}}