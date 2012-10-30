#pragma once
#include <collection.h>

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Platform;
	using namespace Platform::Collections;

	public ref class Subject sealed
	{
	public:

		Subject()
		{
			EventIds = ref new Vector<String^>();
			Attributes = ref new Map<String^, String^>();
		}

		//void AddAttribute(String^ name, String^ value);

		property String^ SubjectId;
		property String^ GroupId;
		property String^ Age;
		property String^ Sex;

		property Vector<String^>^ EventIds;
		property Map<String^, String^>^ Attributes;
		//property Dictionary<String^, SubjectGraphItem^>^ Graphs;
	};

} } }


