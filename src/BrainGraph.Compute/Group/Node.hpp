#pragma once
#include <atomic>
#include "TStat.hpp"

namespace BrainGraph { namespace Compute { namespace Group
{
	struct Node
	{
		Node()
		{
			Index = 0;
		}

		int Index;

		TStat Degree;
		TStat Strength;
		TStat Diversity;
	};

}}}
