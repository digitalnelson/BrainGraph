#pragma once
#include <memory>
#include "Subjects/Node.hpp"
#include "EdgeViewModel.hpp"

namespace BrainGraph { namespace ComputeRT { namespace Subjects {

	namespace BCS = BrainGraph::Compute::Subjects;

	public ref class NodeViewModel sealed
	{
	public:
		property int Index { int get() { return _node->Index; } }
		property int Degree { int get() { return (int)_node->Degree; } }
		property double TotalStrength { double get() { return _node->TotalStrength; } }

	internal:
		NodeViewModel(std::shared_ptr<BCS::Node> node)
		{
			_node = node;
		}

	private:
		std::shared_ptr<BCS::Node> _node;
	};

			
} } }

