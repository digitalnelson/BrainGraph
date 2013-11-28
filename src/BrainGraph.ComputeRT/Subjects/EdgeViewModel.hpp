#pragma once
#include <memory>
#include "BrainGraph.Compute/Subjects/Edge.hpp"

namespace BrainGraph { namespace ComputeRT { namespace Subjects	{

	namespace BCS = BrainGraph::Compute::Subjects;

	public ref class EdgeViewModel sealed
	{
	public:
		property int NodeOneIndex { int get() { return _edge->Vertices.first; } }
		property int NodeTwoIndex { int get() { return _edge->Vertices.second; } }
		property double Value {double get(){ return _edge->Value; } }

	internal:
		EdgeViewModel(std::shared_ptr<BCS::Edge> edge)
		{
			_edge = edge;
		}

	private:
		std::shared_ptr<BCS::Edge> _edge;
	};

} } }