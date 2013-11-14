#pragma once
#include <atomic>
#include <vector>
#include "TStat.hpp"
#include "PearsonCompare.hpp"

namespace BrainGraph { namespace Compute { namespace Group
{
	struct Item
	{
		Item()
		{
		}

		void IncludePearsonValue(size_t associationId, int groupId, double x, double y)
		{
			if(Associations.size() <= associationId)
				Associations.push_back(PearsonCompare());

			Associations[associationId].IncludeValue(groupId, x, y);
		}

		TStat Stats;
		std::vector<PearsonCompare> Associations;
	};

}}}
