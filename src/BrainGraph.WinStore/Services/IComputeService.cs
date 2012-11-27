using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using BrainGraph.Compute.Graph;
using BrainGraph.Compute.Subjects;
using BrainGraph.WinStore.Services;
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
		void LoadSubjects(int nodes, int edges, List<Threshold> dataTypes, List<Subject> group1, List<Subject> group2);
		void CompareGroups();
		IAsyncActionWithProgress<int> PermuteGroupsAsync(int permutations);
		void GetResults();
	}

	public class ComputeService : IComputeService
	{
		private MultiDatatypeCompare _compare;

		public ComputeService()
		{}

		public void LoadSubjects(int nodes, int edges, List<Threshold> dataTypes, List<Subject> group1, List<Subject> group2)
		{
			_compare = new MultiDatatypeCompare(nodes, edges, dataTypes);
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

		public void GetResults()
		{
			_compare.GetResult();
		}
	}
}
