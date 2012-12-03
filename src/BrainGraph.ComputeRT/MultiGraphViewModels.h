#pragma once
#include "collection.h"
#include "CompareGraphViewModels.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	
	public ref class MultiGlobal sealed
	{
	};

	public ref class MultiGraphViewModel sealed
	{
	public:
		property WFC::IVectorView<CompareGraphViewModel^>^ Graphs { WFC::IVectorView<CompareGraphViewModel^>^ get()  {  return _graphs->GetView(); } } 

	internal:
		MultiGraphViewModel(shared_ptr<MultiGraph> multiGraph)
		{
			_multiGraph = multiGraph;
			_graphs = ref new PC::Vector<CompareGraphViewModel^>();
		}

		void AddCompareGraph(CompareGraphViewModel^ graph)
		{
			_graphs->Append(graph);
		}

	private:
		std::shared_ptr<MultiGraph> _multiGraph;
		PC::Vector<CompareGraphViewModel^>^ _graphs;
	};
	
}}}