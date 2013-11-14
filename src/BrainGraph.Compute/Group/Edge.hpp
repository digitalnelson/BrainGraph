#pragma once
#include <atomic>
#include "TStat.hpp"

namespace BrainGraph { namespace Compute { namespace Group
{
	struct Edge
	{
		Edge()
		{
			Index = 0;
			AboveNBSThreshold = false;
		}

		int Index;
		std::pair<int, int> Nodes;
		bool AboveNBSThreshold;

		TStat Stats;
	};

}}}
