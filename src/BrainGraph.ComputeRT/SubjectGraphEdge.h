#pragma once

namespace BrainGraph { namespace Compute { namespace Subjects
{
	
	struct SubjectGraphEdge
	{
		int Index;
		std::pair<int, int> Vertices;

		float Value;

		SubjectGraphEdge()
		{
			Index = 0;
			Value = 0;
		}
	};

	struct SubjectGraphNode
	{
		int Index;

		float Degree;
		float Strength;
		float Diversity;

		SubjectGraphNode()
		{
			Index = 0;
			Degree = 0;
			Strength = 0;
			Diversity = 0;
		}
	};

	struct SubjectGraphGlobal
	{
	};

}}}
