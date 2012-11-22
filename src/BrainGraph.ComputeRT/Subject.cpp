#include "pch.h"
#include "Subject.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{
	using namespace Windows::ApplicationModel::Core;
	using namespace Windows::Foundation;
	using namespace Windows::UI::Core;
	using namespace Windows::UI::Xaml;

	void Subject::AddEventId(String^ eventId)
	{
		_eventIds->Append(eventId);
	}

	void Subject::AddAttribute(String^ name, String^ value)
	{
		_attributes->Insert(name, value); // TODO: Dedupe
	}

	void Subject::AddGraph(SubjectGraph^ graph)
	{
		_graphs->Insert(graph->DataType, graph);  // TODO: Dedupe
	}

} } }
