#pragma once

namespace BrainGraph { namespace Compute { namespace Graph
{
	public ref class Threshold sealed
	{
	public:
		property Platform::String^ DataType;
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

	struct Component;

	public ref class Overlap sealed
	{
		
	private:
		std::vector<std::shared_ptr<Vertex>> Vertices;
		
		int RightTailOverlapCount;
		
		std::map<Platform::String^, std::vector<Component>> Components;
		std::map<int, int> Distribution;
	};
}}}