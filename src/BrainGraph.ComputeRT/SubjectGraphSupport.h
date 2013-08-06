#pragma once
#include "collection.h"

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

		double Value;
	};

	struct SubjectGraphNode
	{
		int Index;

		double Degree;
		double TotalStrength;

		SubjectGraphNode()
		{
			Index = 0;
			Degree = 0;
			TotalStrength = 0;
		}
	};

	struct SubjectGraphGlobal
	{
		double Strength;

		SubjectGraphGlobal()
		{
			Strength = 0;
		}
	};

	struct SubjectGraphAttribute
	{
		double Value;
	};

}}}
