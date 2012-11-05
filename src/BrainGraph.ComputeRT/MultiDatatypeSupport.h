#pragma once
#include "CompareGraphSupport.h"

namespace BrainGraph { namespace Compute { namespace Graph
{
	using namespace std;
	using namespace Platform;

	public ref class Threshold sealed
	{
	public:
		property String^ DataType;
		property double Value;
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

	public ref class Overlap sealed
	{
		
	private:
		vector<shared_ptr<Vertex>> Vertices;
		
		int RightTailOverlapCount;
		
		map<String^, vector<Component>> Components;
		map<int, int> Distribution;
	};
}}}