#pragma once
#include <memory>
#include "collection.h"
#include "Subjects/Subject.hpp"
#include "Subjects/Graph.hpp"
#include "GraphViewModel.hpp"

namespace BrainGraph { namespace ComputeRT { namespace Subjects
{
	namespace P = Platform;
	namespace PC = Platform::Collections;
	namespace WFC = Windows::Foundation::Collections;
	namespace BCS = BrainGraph::Compute::Subjects;

	public ref class SubjectViewModel sealed
	{
	public:

		SubjectViewModel(P::String^ subjectId)
		{
			_subject = std::make_shared<BCS::Subject>(subjectId->Data());

			_eventIds = ref new PC::Vector<P::String^>();
			_attributes = ref new PC::Map<P::String^, double>();
			_graphs = ref new PC::Map<P::String^, GraphViewModel^>();
		}

		void AddEventId(P::String^ eventId)
		{
			_eventIds->Append(eventId);
			_subject->EventIds.push_back(std::wstring(eventId->Data()));
		}

		void AddAttribute(P::String^ name, double value)
		{
			_attributes->Insert(name, value); // TODO: Dedupe
		}

		void AddGraph(P::String^ dataType, GraphViewModel^ graph)
		{
			_subject->Graphs[dataType->Data()] = graph->Graph;  // TODO: Dedupe
		}

		property P::String^ SubjectId { P::String^ get() { return ref new P::String(_subject->SubjectId.c_str()); }}
		property P::String^ GroupId { P::String^ get() { return ref new P::String(_subject->GroupId.c_str()); } void set(P::String^ val){ _subject->GroupId = val->Data(); } };
		property P::String^ Age { P::String^ get() { return ref new P::String(_subject->Age.c_str()); } void set(P::String^ val){ _subject->Age = val->Data(); } };
		property P::String^ Sex { P::String^ get() { return ref new P::String(_subject->Sex.c_str()); } void set(P::String^ val){ _subject->Sex = val->Data(); } };
		
		property int GraphCount
		{
			int get() { return _graphs->Size; }
		}

		property WFC::IVectorView<P::String^>^ EventIds
		{
			WFC::IVectorView<P::String^>^ get() { return _eventIds->GetView(); }
		}

		property WFC::IMapView<P::String^, double>^ Attributes
		{
			WFC::IMapView<P::String^, double>^ get() { return _attributes->GetView(); }
		}

		property WFC::IMapView<P::String^, GraphViewModel^>^ Graphs
		{
			WFC::IMapView<P::String^, GraphViewModel^>^ get()
			{
				PC::Map<P::String^, GraphViewModel^>^ items = ref new PC::Map<P::String^, GraphViewModel^>();

				for (auto itm : _subject->Graphs)
					items->Insert(ref new P::String(itm.first.c_str()), ref new GraphViewModel(itm.second));

				return items->GetView();
			}
		}
	
	internal:
		std::shared_ptr<BCS::Subject> _subject;

	private:
		PC::Vector<P::String^>^ _eventIds;
		PC::Map<P::String^, double>^ _attributes;
		PC::Map<P::String^, GraphViewModel^>^ _graphs;
	};

} } }


