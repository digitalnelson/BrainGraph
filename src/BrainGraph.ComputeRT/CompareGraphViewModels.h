#pragma once
#include "collection.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	struct CompareNode;
	struct CompareEdge;
	struct TStat;
	
	public ref class TStatViewModel sealed
	{
	public:
		property double M1 { double get() { return _stat.M1; } }
		property double M2 { double get() { return _stat.M2; } }
		
		property double V1 { double get() { return _stat.V1; } }
		property double V2 { double get() { return _stat.V2; } }
		
		property double Value { double get() { return _stat.Value; } }
		property int TwoTailCount { int get() { return _stat.TwoTailCount; } }

	internal:
		TStatViewModel(TStat tstat)
		{
			_stat = tstat;
		}

	private:
		TStat _stat;
	};

	public ref class NodeViewModel sealed
	{
	public:
		property int Index { int get() { return _node->Index; } }
		property TStatViewModel^ Strength { TStatViewModel^ get() { return ref new TStatViewModel(_node->Strength); } }

	internal:
		NodeViewModel(std::shared_ptr<CompareNode> node)
		{
			_node = node;
		}

	private:
		std::shared_ptr<CompareNode> _node;
	};

	public ref class EdgeViewModel sealed
	{
	public:
		property int NodeOneIndex { int get() { return _edge->Nodes.first; } }
		property int NodeTwoIndex { int get() { return _edge->Nodes.second; } }
		property TStatViewModel^ Weight { TStatViewModel^ get() { return ref new TStatViewModel(_edge->Stats); } }

	internal:
		EdgeViewModel(std::shared_ptr<CompareEdge> edge)
		{
			_edge = edge;
		}

	private:
		std::shared_ptr<CompareEdge> _edge;
	};

	public ref class CompareGraphViewModel sealed
	{
	public:
		CompareGraphViewModel()
		{
			_nodes = ref new PC::Vector<NodeViewModel^>();
			_edges = ref new PC::Vector<EdgeViewModel^>();
		}

		property Platform::String^ Name;
		property WFC::IVectorView<NodeViewModel^>^ Nodes { WFC::IVectorView<NodeViewModel^>^ get() { return _nodes->GetView(); } }
		property WFC::IVectorView<EdgeViewModel^>^ Edges { WFC::IVectorView<EdgeViewModel^>^ get() { return _edges->GetView(); } } 

	internal:
		void AddNode(std::shared_ptr<CompareNode> node)
		{
			_nodes->Append(ref new NodeViewModel(node));
		}

		void AddEdge(std::shared_ptr<CompareEdge> edge)
		{
			_edges->Append(ref new EdgeViewModel(edge));
		}

	private:
		PC::Vector<NodeViewModel^>^ _nodes;
		PC::Vector<EdgeViewModel^>^ _edges;
	};
	
}}}