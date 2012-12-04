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

		void IncrementGraphRandomOverlapCount()
		{
			++_randomGraphOverlapCount;
		}

		std::vector<shared_ptr<MultiNode>> Nodes;

	private:
		int _fullOverlapTotal;
		int _randomGraphOverlapCount;
	};

	//class Component;

	//public ref class Overlap sealed
	//{
	//	
	//private:
	//	std::vector<std::shared_ptr<MultiNode>> Vertices;
	//	
	//	int RightTailOverlapCount;
	//	
	//	std::map<Platform::String^, std::vector<Component>> Components;
	//	std::map<int, int> Distribution;
	//};
}}}