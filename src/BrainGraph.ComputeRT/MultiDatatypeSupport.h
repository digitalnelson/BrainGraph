#pragma once
#include "CompareGraphSupport.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace Platform;

	struct Threshold
	{
		string DataType;
		double Value;
	};

	struct Vertex
	{
		int Id;

		bool IsFullOverlap;
		int RandomOverlapCount;

		Vertex()
		{
			Id = 0;
			IsFullOverlap = false;
			RandomOverlapCount = 0;
		}
	};

	ref class Overlap
	{
		
	internal:
		vector<shared_ptr<Vertex>> Vertices;
		
		int RightTailOverlapCount;
		
		map<String^, vector<Component>> Components;
		map<int, int> Distribution;
	};
}}}