#pragma once
#include "collection.h"
#include "MultiGraph.h"

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

	struct Vertex
	{
		int Id;
		bool IsFullOverlap;
		int RandomOverlapCount;

		Vertex()
		{
			Id = 0;
			IsFullOverlap = false;
			RandomOverlapCount = 0;
		}
	};

	class Component;

	public ref class Overlap sealed
	{
		
	private:
		std::vector<std::shared_ptr<Vertex>> Vertices;
		
		int RightTailOverlapCount;
		
		std::map<Platform::String^, std::vector<Component>> Components;
		std::map<int, int> Distribution;
	};

	public ref class MultiGlobal sealed
	{
	};

	public ref class MultiResult sealed
	{
	public:
		MultiResult()
		{
			_graphs = ref new PC::Vector<MultiGraph^>();
		}

		property WFC::IVectorView<MultiGraph^>^ Graphs { WFC::IVectorView<MultiGraph^>^ get()  {  return _graphs->GetView(); } } 

	internal:
		void AddGraph(MultiGraph^ graph)
		{
			_graphs->Append(graph);
		}

	private:
		PC::Vector<MultiGraph^>^ _graphs;
	};

}}}