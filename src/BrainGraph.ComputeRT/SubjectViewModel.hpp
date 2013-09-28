#pragma once
#include "SubjectGraph.hpp"
#include "SubjectGraphViewModels.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Platform;
	using namespace Platform::Collections;
	using namespace Windows::Foundation::Collections;

	public ref class SubjectViewModel sealed
	{
	public:

		SubjectViewModel()
		{
			_eventIds = ref new Vector<String^>();
			_attributes = ref new Map<String^, double>();
			_graphs = ref new Map<String^, SubjectGraphViewModel^>();
		}

		void AddEventId(String^ eventId)
		{
			_eventIds->Append(eventId);
		}

		void AddAttribute(String^ name, double value)
		{
			_attributes->Insert(name, value); // TODO: Dedupe
		}

		void AddGraph(String^ dataType, SubjectGraphViewModel^ graph)
		{
			_graphs->Insert(dataType, graph);  // TODO: Dedupe
		}

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

		property IMapView<String^, double>^ Attributes
		{
			IMapView<String^, double>^ get() { return _attributes->GetView(); }
		}

		property IMapView<String^, SubjectGraphViewModel^>^ Graphs
		{
			IMapView<String^, SubjectGraphViewModel^>^ get() { return _graphs->GetView(); }
		}
	
	private:
		Vector<String^>^ _eventIds;
		Map<String^, double>^ _attributes;
		Map<String^, SubjectGraphViewModel^>^ _graphs;
	};

} } }


