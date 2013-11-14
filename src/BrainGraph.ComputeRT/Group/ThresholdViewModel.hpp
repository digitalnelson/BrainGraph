#pragma once
#include <memory>
#include "collection.h"
#include "Group/Threshold.hpp"

namespace BrainGraph { namespace ComputeRT { namespace Group
{
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	namespace BCG = BrainGraph::Compute::Group;

	public ref class ThresholdViewModel sealed
	{
	public:
		property Platform::String^ DataType;
		property double Value;
	
	internal:
		std::shared_ptr<BCG::Threshold> GetThreshold()
		{
			std::shared_ptr<BCG::Threshold> thresh = std::make_shared<BCG::Threshold>();

			thresh->DataType = std::wstring(this->DataType->Data());
			thresh->Value = this->Value;

			return thresh;
		}
	};

}}}