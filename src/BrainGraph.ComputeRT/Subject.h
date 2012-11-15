#pragma once
#include "SubjectGraph.h"
#include <collection.h>

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Platform;
	using namespace Platform::Collections;
	using namespace Windows::Foundation::Collections;

	[Windows::UI::Xaml::Data::Bindable]
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

		property IVectorView<String^>^ EventIds
		{
			IVectorView<String^>^ get() { return _eventIds->GetView(); }
		}

		property IMapView<String^, String^>^ Attributes
		{
			IMapView<String^, String^>^ get() { return _attributes->GetView(); }
		}

		property IMap<String^, SubjectGraph^>^ Graphs
		{
			IMap<String^, SubjectGraph^>^ get() { return _graphs; }
		}
	
	private:
		Vector<String^>^ _eventIds;
		Map<String^, String^>^ _attributes;
		Map<String^, SubjectGraph^>^ _graphs;
	};

} } }

