#pragma once
#include <memory>
#include <atomic>
#include <mutex>
#include <vector>
#include "ppl.h"
#include "Edge.hpp"
#include "Node.hpp"

namespace BrainGraph { namespace Compute { namespace Group
{
	class Component
	{
	public:
		Component() : Identifier(0), RightTailExtent(0)
		{
			//RandomDistribution.resize(4005);
		}

		int Identifier;

		std::vector<std::shared_ptr<Edge>> Edges;
		std::vector<std::shared_ptr<Node>> Nodes;
		std::vector<int> Vertices;  // TODO: Deprec

		std::mutex lockRandomDist;
		std::vector<concurrency::combinable<int>> RandomDistribution;
		std::atomic<int> RightTailExtent;

		void AddRandomExtentValue(size_t randomEdgeCount)
		{
			if(randomEdgeCount > Edges.size())
				RightTailExtent++;

			if(RandomDistribution.size() <= randomEdgeCount)
			{
				std::lock_guard<std::mutex> mtx(lockRandomDist);

				if(RandomDistribution.size() <= randomEdgeCount)
					RandomDistribution.resize(randomEdgeCount + 1);
			}

			RandomDistribution[randomEdgeCount].local()++;
		}

		double GetAverageEdgeDifference()
		{
			double dVal = 0;
			for(auto edge : Edges)
				dVal += ( edge->Stats.M1 - edge->Stats.M2 );

			return dVal / Edges.size();
		}
	};

}}}
