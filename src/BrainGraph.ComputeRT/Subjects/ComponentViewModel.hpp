#pragma once
#include <memory>
#include "collection.h"

namespace BrainGraph { 	namespace ComputeRT {	namespace Subjects	{

	//namespace PC = Platform::Collections;
	//namespace WFC = Windows::Foundation::Collections;
	//namespace BCS = BrainGraph::Compute::Subjects;

	//public ref class ComponentViewModel sealed
	//{
	//public:
	//	property WFC::IVectorView<EdgeViewModel^>^ Edges { WFC::IVectorView<EdgeViewModel^>^ get() { return _edges->GetView(); } }
	//	property int NodeCount { int get() { return _component->Vertices.size(); } }
	//	property WFC::IVectorView<int>^ RandomDistribution { WFC::IVectorView<int>^ get() { return _randomDistribution->GetView(); } }

	//internal:
	//	ComponentViewModel(std::shared_ptr<BCS::Component> component)
	//	{
	//		_component = component;

	//		_edges = ref new PC::Vector<EdgeViewModel^>();
	//		for (auto compareEdge : _component->Edges)
	//			AddEdge(compareEdge);

	//		_randomDistribution = ref new PC::Vector<int>();
	//		for (auto distVal : _component->RandomDistribution)
	//		{
	//			_randomDistribution->Append(distVal.combine(plus<int>()));
	//		}
	//	}

	//	void AddEdge(std::shared_ptr<BCS::CompareEdge> edge)
	//	{
	//		_edges->Append(ref new EdgeViewModel(edge));
	//	}

	//private:
	//	std::shared_ptr<BCS::Component> _component;
	//	PC::Vector<EdgeViewModel^>^ _edges;
	//	PC::Vector<int>^ _randomDistribution;
	//};

} } }