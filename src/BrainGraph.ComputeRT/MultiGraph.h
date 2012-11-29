#pragma once
#include "collection.h"
#include "MultiTStat.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	struct CompareNode;
	struct CompareEdge;
	
	public ref class MultiNode sealed
	{
	public:
		property int Index { int get() { return _node->Index; } }
		property MultiTStat^ Strength { MultiTStat^ get() { return ref new MultiTStat(_node->Strength); } }

	internal:
		MultiNode(std::shared_ptr<CompareNode> node)
		{
			_node = node;
		}

	private:
		std::shared_ptr<CompareNode> _node;
	};

	public ref class MultiEdge sealed
	{
	public:
		property int NodeOneIndex { int get() { return _edge->Nodes.first; } }
		property int NodeTwoIndex { int get() { return _edge->Nodes.second; } }
		property MultiTStat^ Weight { MultiTStat^ get() { return ref new MultiTStat(_edge->Stats); } }

	internal:
		MultiEdge(std::shared_ptr<CompareEdge> edge)
		{
			_edge = edge;
		}

	private:
		std::shared_ptr<CompareEdge> _edge;
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
		void AddNode(std::shared_ptr<CompareNode> node)
		{
			_nodes->Append(ref new MultiNode(node));
		}

		void AddEdge(std::shared_ptr<CompareEdge> edge)
		{
			_edges->Append(ref new MultiEdge(edge));
		}

	private:
		PC::Vector<MultiNode^>^ _nodes;
		PC::Vector<MultiEdge^>^ _edges;
	};
	
}}}