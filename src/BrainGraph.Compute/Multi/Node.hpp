#pragma once
#include <memory>
#include <atomic>
#include <vector>
#include <map>

namespace BrainGraph { namespace Compute { namespace Multi {

	struct Node
	{
		int Id;
		bool IsFullOverlap;
		std::atomic<int> RandomOverlapCount;

		Node(int id)
		{
			Id = id;
			IsFullOverlap = false;
			RandomOverlapCount = 0;
		}
	};

} } }