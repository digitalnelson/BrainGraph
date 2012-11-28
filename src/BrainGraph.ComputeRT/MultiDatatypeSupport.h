#pragma once
#include "collection.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
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

	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	struct TStat;
	struct CompareNode;
	struct CompareEdge;
	
	public ref class MultiTStat sealed
	{
	public:
		property float M1 { float get() { return _stat.M1; } }
		property float M2 { float get() { return _stat.M2; } }
		property float V1 { float get() { return _stat.V1; } }
		property float V2 { float get() { return _stat.V2; } }
		property float Value { float get() { return _stat.Value; } }
		property int TwoTailCount { int get() { return _stat.TwoTailCount; } }

	internal:
		MultiTStat(TStat tstat)
		{
			_stat = tstat;
		}

	private:
		TStat _stat;
	};

	public ref class MultiNode sealed
	{
	public:
		property int Index { int get() { return _node->Index; } }
		property MultiTStat^ Strength { MultiTStat^ get() { return ref new MultiTStat(_node->Strength); } }

	internal:
		MultiNode(shared_ptr<CompareNode> node)
		{
			_node = node;
		}

	private:
		shared_ptr<CompareNode> _node;
	};

	public ref class MultiEdge sealed
	{
	internal:
		MultiEdge(shared_ptr<CompareEdge> edge)
		{
			_edge = edge;
		}

	private:
		shared_ptr<CompareEdge> _edge;
	};

	public ref class MultiGlobal sealed
	{
	};

	public ref class MultiGraph sealed
	{
	public:
		MultiGraph()
		{
			_nodes = ref new PC::Vector<MultiNode^>();
			_edges = ref new PC::Vector<MultiEdge^>();
		}

		property Platform::String^ Name;
		property WFC::IVectorView<MultiNode^>^ Nodes { WFC::IVectorView<MultiNode^>^ get() { return _nodes->GetView(); } }
		property WFC::IVectorView<MultiEdge^>^ Edges { WFC::IVectorView<MultiEdge^>^ get() { return _edges->GetView(); } } 

	internal:
		void AddNode(shared_ptr<CompareNode> node)
		{
			_nodes->Append(ref new MultiNode(node));
		}

		void AddEdge(shared_ptr<CompareEdge> edge)
		{
			_edges->Append(ref new MultiEdge(edge));
		}

	private:
		PC::Vector<MultiNode^>^ _nodes;
		PC::Vector<MultiEdge^>^ _edges;
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