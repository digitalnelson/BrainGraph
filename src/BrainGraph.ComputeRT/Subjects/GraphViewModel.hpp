#pragma once
#include <memory>
#include "Subjects/Graph.hpp"
#include "EdgeViewModel.hpp"
#include "NodeViewModel.hpp"

namespace BrainGraph {namespace ComputeRT {namespace Subjects
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	namespace BCS = BrainGraph::Compute::Subjects;
	
	public ref class GraphViewModel sealed
	{
	public:		
		GraphViewModel(int verts)
		{
			_verts = verts;
		}

		void AddEdge(int i, int j, double val)
		{
			if (Graph == nullptr)
				Graph = std::make_shared<BCS::Graph>(_verts);

			Graph->AddEdge(i, j, val);
		}

		property WFC::IVectorView<NodeViewModel^>^ Nodes 
		{ 
			WFC::IVectorView<NodeViewModel^>^ get() 
			{ 
				PC::Vector<NodeViewModel^>^ items = ref new PC::Vector<NodeViewModel^>();
				
				for (auto itm : Graph->Nodes)
					items->Append(ref new NodeViewModel(itm));

				return items->GetView(); 
			} 
		}

		property WFC::IVectorView<EdgeViewModel^>^ Edges 
		{ 
			WFC::IVectorView<EdgeViewModel^>^ get() 
			{ 
				PC::Vector<EdgeViewModel^>^ items = ref new PC::Vector<EdgeViewModel^>();

				for (auto itm : Graph->Edges)
					items->Append(ref new EdgeViewModel(itm));

				return items->GetView();
			} 
		}

	internal:
		GraphViewModel(std::shared_ptr<BCS::Graph> graph)
		{
			Graph = graph;
		}

		std::shared_ptr<BCS::Graph> Graph;

	private:
		int _verts;
	};

}}}

