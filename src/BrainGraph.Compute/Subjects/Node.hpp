#pragma once

namespace BrainGraph { namespace Compute { namespace Subjects
{
	struct Node
	{
		int Index;

		double Degree;
		double TotalStrength;

		Node()
		{
			Index = 0;
			Degree = 0;
			TotalStrength = 0;
		}
	};

}}}
