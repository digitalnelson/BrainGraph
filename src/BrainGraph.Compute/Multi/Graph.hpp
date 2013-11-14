#pragma once
#include <memory>
#include <atomic>
#include <mutex>
#include <vector>
#include <map>
#include <ppl.h>
#include "Node.hpp"

namespace BrainGraph { namespace Compute { namespace Multi {

	struct Graph
	{
	public:
		Graph()
		{
			_fullOverlapTotal = 0;
			_randomGraphOverlapCount = 0;
		}

		void AddNode(std::shared_ptr<Node> node)
		{
			Nodes.push_back(node);

			if (node->IsFullOverlap)
				++_fullOverlapTotal;
		}

		void IncrementNodalRandomOverlapCount(int index)
		{
			this->Nodes[index]->RandomOverlapCount++;
		}

		int GetTotalNodalOverlapCount()
		{
			return _fullOverlapTotal;
		}

		void AddRandomOverlapValue(size_t randomOverlapCount)
		{
			if (randomOverlapCount >= _fullOverlapTotal)
				++_randomGraphOverlapCount;

			if (RandomDistribution.size() <= randomOverlapCount)
			{
				std::lock_guard<std::mutex> mtx(lockRandomDist);

				if (RandomDistribution.size() <= randomOverlapCount)
					RandomDistribution.resize(randomOverlapCount + 1);
			}

			RandomDistribution[randomOverlapCount].local()++;
		}

		std::vector<std::shared_ptr<Node>> Nodes;
		std::vector<concurrency::combinable<int>> RandomDistribution;

	private:
		size_t _fullOverlapTotal;

		std::mutex lockRandomDist;
		std::atomic<int> _randomGraphOverlapCount;
	};
			

} } }