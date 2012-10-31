#pragma once

namespace BrainGraph { namespace Compute { namespace Subjects
{
	
	struct SubjectGraphEdge
	{
		SubjectGraphEdge()
		{
			Index = 0;
			Value = 0;
		}

		int Index;
		std::pair<int, int> Vertices;

		float Value;
	};

}}}
