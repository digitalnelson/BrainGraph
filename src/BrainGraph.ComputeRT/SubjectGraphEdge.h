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
		float TotalStrength;

		SubjectGraphNode()
		{
			Index = 0;
			Degree = 0;
			TotalStrength = 0;
		}
	};

	struct SubjectGraphGlobal
	{
		float Strength;

		SubjectGraphGlobal()
		{
			Strength = 0;
		}
	};

}}}
