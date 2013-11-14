#pragma once
#include <memory>
#include <string>
#include <vector>
#include <map>
#include "Graph.hpp"

namespace BrainGraph { namespace Compute { namespace Subjects {

	class Subject
	{
	public:
		Subject(std::wstring subjectId) : SubjectId(subjectId)
		{}

		std::wstring SubjectId;
		std::wstring GroupId;
		std::wstring Sex;
		std::wstring Age;

		std::vector<std::wstring> EventIds;
		std::map<std::wstring, std::shared_ptr<Graph>> Graphs;
	};

}}}