#pragma once
#include "SubjectGraphSupport.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Platform;
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	ref class SubjectGraph;

	public ref class EdgeViewModel sealed
	{
	public:
		property int NodeOneIndex { int get() { return _edge->Vertices.first; } }
		property int NodeTwoIndex { int get() { return _edge->Vertices.second; } }

	internal:
		EdgeViewModel(std::shared_ptr<SubjectGraphEdge> edge)
		{
			_edge = edge;
		}

	private:
		std::shared_ptr<SubjectGraphEdge> _edge;
	};

	public ref class NodeViewModel sealed
	{
	public:
		property int Index { int get() { return _node->Index; } }
		property int Degree { int get() { return _node->Degree; } }
		property int TotalStrength { int get() { return _node->TotalStrength; } }

	internal:
		NodeViewModel(std::shared_ptr<SubjectGraphNode> node)
		{
			_node = node;
		}

	private:
		std::shared_ptr<SubjectGraphNode> _node;
	};

	public ref class SubjectGraphViewModel sealed
	{
	public:
		property WFC::IVectorView<NodeViewModel^>^ Nodes { WFC::IVectorView<NodeViewModel^>^ get() { return _nodes->GetView(); } }
		property WFC::IVectorView<EdgeViewModel^>^ Edges { WFC::IVectorView<EdgeViewModel^>^ get() { return _edges->GetView(); } } 

	
		SubjectGraphViewModel(SubjectGraph^ graph)
		{
			//_graph = graph;

			//Global = ref new GlobalViewModel(_graph->Global);

			//_components = ref new PC::Vector<ComponentViewModel^>();
			//for(auto compareComponent : graph->Components)
			//	AddComponent(compareComponent);

			_nodes = ref new PC::Vector<NodeViewModel^>();
			for(auto compareNode : graph->Nodes)
				AddNode(compareNode);

			_edges = ref new PC::Vector<EdgeViewModel^>();
			for(auto compareEdge : graph->Edges)
				AddEdge(compareEdge);
		}

	internal:
		void AddNode(std::shared_ptr<SubjectGraphNode> node)
		{
			_nodes->Append(ref new NodeViewModel(node));
		}

		void AddEdge(std::shared_ptr<SubjectGraphEdge> edge)
		{
			_edges->Append(ref new EdgeViewModel(edge));
		}

	private:
		PC::Vector<NodeViewModel^>^ _nodes;
		PC::Vector<EdgeViewModel^>^ _edges;
	};

}}}

