#pragma once
#include <atomic>
#include "Pearson.hpp"

namespace BrainGraph { namespace Compute { namespace Group
{

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

}}}
