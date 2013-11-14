using BrainGraph.ComputeRT.Group;
using BrainGraph.ComputeRT.Subjects;
using BrainGraph.ComputeRT.Multi;
using System.Collections.Generic;
using Windows.Foundation;

namespace BrainGraph.WinStore.Services
{
	public enum ComputeGroup
	{
		GroupNone,
		GroupOne,
		GroupTwo
	}

	public interface IComputeService
	{
		void SetThreshold(string dataType, string threshold);
        void LoadSubjects(int nodes, int edges, List<string> dataTypes, List<SubjectViewModel> group1, List<SubjectViewModel> group2);
		void CompareGroups();
		IAsyncActionWithProgress<int> PermuteGroupsAsync(int permutations);
		MultiGraphViewModel GetResults();
		int GetPermutations();
	}

	public class ComputeService : IComputeService
	{
		private MultiDatatypeCompare _compare;
		private Dictionary<string, string> _thresholds = new Dictionary<string,string>();

		public ComputeService()
		{}

		public void SetThreshold(string dataType, string threshold)
		{
			_thresholds[dataType] = threshold;
		}

        public void LoadSubjects(int nodes, int edges, List<string> dataTypes, List<SubjectViewModel> group1, List<SubjectViewModel> group2)
		{
			List<ThresholdViewModel> thresholds = new List<ThresholdViewModel>();

			foreach (var dataType in dataTypes)
			{
				thresholds.Add(new ThresholdViewModel { DataType = dataType, Value = double.Parse(_thresholds[dataType]) });
			}

			_compare = new MultiDatatypeCompare(nodes, edges, thresholds);
			_compare.LoadSubjects(group1, group2);
		}

		public void CompareGroups()
		{
			_compare.Compare();
		}

		public IAsyncActionWithProgress<int> PermuteGroupsAsync(int permutations)
		{
			return _compare.PermuteAsyncWithProgress(permutations);
		}

		public MultiGraphViewModel GetResults()
		{
			return _compare.GetResult();
		}

		public int GetPermutations()
		{
			return _compare.GetPermutations();
		}
	}
}
