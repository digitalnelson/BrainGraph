#pragma once
#include "collection.h"
#include "SubjectGraphViewModels.h"
#include "CompareGraphViewModels.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	
	public ref class MultiNodeViewModel sealed
	{
	public:
		property int Id { int get()  {  return _multiNode->Id; } }
		property bool IsFullOverlap { bool get()  {  return _multiNode->IsFullOverlap; } }

	internal:
		MultiNodeViewModel(shared_ptr<MultiNode> multiNode)
		{
			_multiNode = multiNode;
		}

	private:
		std::shared_ptr<MultiNode> _multiNode;
	};

	public ref class MultiGraphViewModel sealed
	{
	public:
		property WFC::IVectorView<MultiNodeViewModel^>^ MultiNodes { WFC::IVectorView<MultiNodeViewModel^>^ get()  {  return _multiNodes->GetView(); } }
		property WFC::IVectorView<CompareGraphViewModel^>^ Graphs { WFC::IVectorView<CompareGraphViewModel^>^ get()  {  return _graphs->GetView(); } } 

	internal:
		MultiGraphViewModel(shared_ptr<MultiGraph> multiGraph)
		{
			_multiNodes = ref new PC::Vector<MultiNodeViewModel^>();
			_graphs = ref new PC::Vector<CompareGraphViewModel^>();

			_multiGraph = multiGraph;

			for(auto node : _multiGraph->Nodes)
				_multiNodes->Append(ref new MultiNodeViewModel(node));

			// TODO: Spin through the compare graphs and add to the collection and delete the AddCompareGraph method
		}

		void AddCompareGraph(CompareGraphViewModel^ graph)
		{
			_graphs->Append(graph);
		}

	private:
		std::shared_ptr<MultiGraph> _multiGraph;
		
		PC::Vector<MultiNodeViewModel^>^ _multiNodes;
		PC::Vector<CompareGraphViewModel^>^ _graphs;
	};
	
}}}