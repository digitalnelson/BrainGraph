#pragma once
#include "collection.h"
#include "Group/TStat.hpp"
#include "Group/Edge.hpp"
#include "Group/Node.hpp"
#include "Group/Component.hpp"
#include "Group/Global.hpp"
#include "Group/Graph.hpp"

namespace BrainGraph { namespace ComputeRT { namespace Group
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	namespace BCG = BrainGraph::Compute::Group;
	
	public ref class TStatViewModel sealed
	{
	public:
		TStatViewModel()
		{}

		TStatViewModel(double m1, double m2, double v1, double v2, double value, int twotailcount)
		{
			_stat.M1 = m1;
			_stat.M2 = m2;
			_stat.V1 = v1;
			_stat.V2 = v2;
			_stat.Value = value;
			_stat.TwoTailCount.exchange(twotailcount);
		}

		property double M1 { double get() { return _stat.M1; } void set(double val) { _stat.M1 = val; } }
		property double M2 { double get() { return _stat.M2; } void set(double val) { _stat.M2 = val; } }
		
		property double V1 { double get() { return _stat.V1; } void set(double val) { _stat.V1 = val; } }
		property double V2 { double get() { return _stat.V2; } void set(double val) { _stat.V2 = val; } }
		
		property double Value { double get() { return _stat.Value; } void set(double val) { _stat.Value = val; } }
		property int TwoTailCount { int get() { return _stat.TwoTailCount; } void set(int val) { _stat.TwoTailCount = val; } }

	internal:
		TStatViewModel(BCG::TStat tstat)
		{
			_stat = tstat;
		}

	private:
		BCG::TStat _stat;
	};

	public ref class EdgeViewModel sealed
	{
	public:
		property int NodeOneIndex { int get() { return _edge->Nodes.first; } }
		property int NodeTwoIndex { int get() { return _edge->Nodes.second; } }
		property TStatViewModel^ Weight { TStatViewModel^ get() { return ref new TStatViewModel(_edge->Stats); } }

	internal:
		EdgeViewModel(std::shared_ptr<BCG::Edge> edge)
		{
			_edge = edge;
		}

	private:
		std::shared_ptr<BCG::Edge> _edge;
	};

	public ref class NodeViewModel sealed
	{
	public:
		property int Index { int get() { return _node->Index; } }
		property TStatViewModel^ Strength { TStatViewModel^ get() { return ref new TStatViewModel(_node->Strength); } }

	internal:
		NodeViewModel(std::shared_ptr<BCG::Node> node)
		{
			_node = node;
		}

	private:
		std::shared_ptr<BCG::Node> _node;
	};

	public ref class ComponentViewModel sealed
	{
	public:
		property WFC::IVectorView<EdgeViewModel^>^ Edges { WFC::IVectorView<EdgeViewModel^>^ get() { return _edges->GetView(); } } 
		property int NodeCount { int get() { return _component->Vertices.size(); } }
		property WFC::IVectorView<int>^ RandomDistribution { WFC::IVectorView<int>^ get() { return _randomDistribution->GetView(); } }
		property int RandomTailCount { int get() { return _component->RightTailExtent; }}

	internal:
		ComponentViewModel(std::shared_ptr<BCG::Component> component)
		{
			_component = component;

			_edges = ref new PC::Vector<EdgeViewModel^>();
			for(auto compareEdge : _component->Edges)
				AddEdge(compareEdge);

			_randomDistribution = ref new PC::Vector<int>();
			for(auto distVal : _component->RandomDistribution)
			{
				_randomDistribution->Append(distVal.combine(std::plus<int>()));
			}
		}

		void AddEdge(std::shared_ptr<BCG::Edge> edge)
		{
			_edges->Append(ref new EdgeViewModel(edge));
		}

	private:
		std::shared_ptr<BCG::Component> _component;
		PC::Vector<EdgeViewModel^>^ _edges;
		PC::Vector<int>^ _randomDistribution;
	};

	public ref class GlobalViewModel sealed
	{
	public:
		GlobalViewModel()
		{
			_global = std::make_shared<BCG::Global>();
		}

		property TStatViewModel^ Strength { TStatViewModel^ get() { return ref new TStatViewModel(_global->Strength.Stats); } }

	internal:
		GlobalViewModel(std::shared_ptr<BCG::Global> global)
		{
			_global = global;
		}

	private:
		std::shared_ptr<BCG::Global> _global;
	};

	public ref class GraphViewModel sealed
	{
	public:
		property Platform::String^ Name;
		property GlobalViewModel^ Global;
		property WFC::IVectorView<ComponentViewModel^>^ Components { WFC::IVectorView<ComponentViewModel^>^ get() { return _components->GetView(); } }
		property WFC::IVectorView<NodeViewModel^>^ Nodes { WFC::IVectorView<NodeViewModel^>^ get() { return _nodes->GetView(); } }
		property WFC::IVectorView<EdgeViewModel^>^ Edges { WFC::IVectorView<EdgeViewModel^>^ get() { return _edges->GetView(); } } 

	internal:
		GraphViewModel(std::shared_ptr<BCG::Graph> graph)
		{
			_graph = graph;

			Global = ref new GlobalViewModel(_graph->Global);

			_components = ref new PC::Vector<ComponentViewModel^>();
			for(auto compareComponent : graph->Components)
				AddComponent(compareComponent);

			_nodes = ref new PC::Vector<NodeViewModel^>();
			for(auto compareNode : graph->Nodes)
				AddNode(compareNode);

			_edges = ref new PC::Vector<EdgeViewModel^>();
			for(auto compareEdge : graph->Edges)
				AddEdge(compareEdge);
		}

		void AddComponent(std::shared_ptr<BCG::Component> component)
		{
			_components->Append(ref new ComponentViewModel(component));
		}

		void AddNode(std::shared_ptr<BCG::Node> node)
		{
			_nodes->Append(ref new NodeViewModel(node));
		}

		void AddEdge(std::shared_ptr<BCG::Edge> edge)
		{
			_edges->Append(ref new EdgeViewModel(edge));
		}

	private:
		std::shared_ptr<BCG::Graph> _graph;
		
		PC::Vector<ComponentViewModel^>^ _components;
		PC::Vector<NodeViewModel^>^ _nodes;
		PC::Vector<EdgeViewModel^>^ _edges;
	};
	
}}}