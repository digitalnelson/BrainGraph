#pragma once
#include "collection.h"
#include "Multi/Graph.hpp"
#include "../Group/GroupViewModels.hpp"

namespace BrainGraph { namespace ComputeRT { namespace Multi
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	namespace BCM = BrainGraph::Compute::Multi;
	namespace BCRG = BrainGraph::ComputeRT::Group;
	
	public ref class MultiNodeViewModel sealed
	{
	public:
		property int Id { int get()  {  return _multiNode->Id; } }
		property bool IsFullOverlap { bool get()  {  return _multiNode->IsFullOverlap; } }

	internal:
		MultiNodeViewModel(std::shared_ptr<BCM::Node> multiNode)
		{
			_multiNode = multiNode;
		}

	private:
		std::shared_ptr<BCM::Node> _multiNode;
	};

	public ref class MultiGraphViewModel sealed
	{
	public:
		property WFC::IVectorView<MultiNodeViewModel^>^ MultiNodes { WFC::IVectorView<MultiNodeViewModel^>^ get()  {  return _multiNodes->GetView(); } }
		property WFC::IVectorView<BCRG::GraphViewModel^>^ Graphs { WFC::IVectorView<BCRG::GraphViewModel^>^ get()  { return _graphs->GetView(); } }
		property WFC::IVectorView<int>^ RandomDistribution { WFC::IVectorView<int>^ get() { return _randomDistribution->GetView(); } }

	internal:
		MultiGraphViewModel(std::shared_ptr<BCM::Graph> multiGraph)
		{
			_multiNodes = ref new PC::Vector<MultiNodeViewModel^>();
			_graphs = ref new PC::Vector<BCRG::GraphViewModel^>();

			_multiGraph = multiGraph;

			for(auto node : _multiGraph->Nodes)
				_multiNodes->Append(ref new MultiNodeViewModel(node));

			_randomDistribution = ref new PC::Vector<int>();
			for(auto distVal : _multiGraph->RandomDistribution)
			{
				_randomDistribution->Append(distVal.combine(std::plus<int>()));
			}

			// TODO: Spin through the compare graphs and add to the collection and delete the AddCompareGraph method
		}

		void AddCompareGraph(BCRG::GraphViewModel^ graph)
		{
			_graphs->Append(graph);
		}

	private:
		std::shared_ptr<BCM::Graph> _multiGraph;
		
		PC::Vector<MultiNodeViewModel^>^ _multiNodes;
		PC::Vector<BCRG::GraphViewModel^>^ _graphs;
		PC::Vector<int>^ _randomDistribution;
	};
	
}}}