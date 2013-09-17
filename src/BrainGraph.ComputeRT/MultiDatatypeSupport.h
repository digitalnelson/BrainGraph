#pragma once
#include "collection.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;

	public ref class Threshold sealed
	{
	public:
		property Platform::String^ DataType;
		property double Value;
	};

	struct MultiNode
	{
		int Id;
		bool IsFullOverlap;
		int RandomOverlapCount;

		MultiNode(int id)
		{
			Id = id;
			IsFullOverlap = false;
			RandomOverlapCount = 0;
		}
	};

	struct MultiGraph
	{
	public:
		MultiGraph()
		{
			_fullOverlapTotal = 0;
			_randomGraphOverlapCount = 0;
		}

		void AddNode(shared_ptr<MultiNode> node)
		{
			Nodes.push_back(node);
			
			if( node->IsFullOverlap )
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

		void AddRandomOverlapValue(int randomOverlapCount)
		{
			if(randomOverlapCount >= _fullOverlapTotal)
				++_randomGraphOverlapCount;

			if(RandomDistribution.size() <= randomOverlapCount)
			{
				std::lock_guard<std::mutex> mtx(lockRandomDist);

				if(RandomDistribution.size() <= randomOverlapCount)
					RandomDistribution.resize(randomOverlapCount + 1);
			}

			RandomDistribution[randomOverlapCount].local()++;
		}

		std::vector<shared_ptr<MultiNode>> Nodes;
		std::vector<concurrency::combinable<int>> RandomDistribution;

	private:
		int _fullOverlapTotal;

		std::mutex lockRandomDist;
		int _randomGraphOverlapCount;
	};
}}}