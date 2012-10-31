#include "pch.h"
#include "Subject.h"

namespace BrainGraph { namespace Compute { namespace Subjects
{

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
