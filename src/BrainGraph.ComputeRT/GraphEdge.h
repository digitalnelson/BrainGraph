#pragma once

namespace BrainGraph { namespace Compute { namespace Graph
{
	
	struct GraphEdge
	{
		GraphEdge()
		{
			Index = 0;
			Value = 0;
		}

		int Index;
		std::pair<int, int> Vertices;

		float Value;
	};

}}}
