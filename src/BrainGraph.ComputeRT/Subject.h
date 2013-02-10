#pragma once
#include "SubjectGraph.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Platform;
	using namespace Platform::Collections;
	using namespace Windows::Foundation::Collections;

	public ref class Subject sealed
	{
	public:

		Subject()
		{
			_eventIds = ref new Vector<String^>();
			_attributes = ref new Map<String^, String^>();
			_graphs = ref new Map<String^, SubjectGraph^>();
		}

		void AddEventId(String^ eventId);
		void AddAttribute(String^ name, String^ value);
		void AddGraph(SubjectGraph^ graph);

		property String^ SubjectId;
		property String^ GroupId;
		property String^ Age;
		property String^ Sex;
		
		property int GraphCount
		{
			int get() { return _graphs->Size; }
		}

		property IVectorView<String^>^ EventIds
		{
			IVectorView<String^>^ get() { return _eventIds->GetView(); }
		}

		property IMapView<String^, String^>^ Attributes
		{
			IMapView<String^, String^>^ get() { return _attributes->GetView(); }
		}

		property IMapView<String^, SubjectGraph^>^ Graphs
		{
			IMapView<String^, SubjectGraph^>^ get() { return _graphs->GetView(); }
		}
	
	private:
		Vector<String^>^ _eventIds;
		Map<String^, String^>^ _attributes;
		Map<String^, SubjectGraph^>^ _graphs;
	};

} } }


