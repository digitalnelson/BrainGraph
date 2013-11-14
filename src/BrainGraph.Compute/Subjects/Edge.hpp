#pragma once
#include <map>

namespace BrainGraph { namespace Compute { namespace Subjects
{
	struct Edge
	{
		Edge()
		{
			Index = 0;
			Value = 0;
		}

		int Index;
		std::pair<int, int> Vertices;

		double Value;
	};

}}}
