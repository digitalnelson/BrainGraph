using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using BrainGraph.Compute.Graph;
using BrainGraph.Compute.Subjects;
using BrainGraph.WinStore.Services;

namespace BrainGraph.WinStore.Services
{
	public enum ComputeGroup
	{
		GroupNone,
		GroupOne,
		GroupTwo
	}

	public class PermutationProgress
	{
		public int Complete { get; set; }
	}

	public interface IComputeService
	{
		void LoadSubjects(int nodes, int edges, List<Threshold> dataTypes, List<Subject> group1, List<Subject> group2);
		void CompareGroups();
		void PermuteGroups(int permutations, IProgress<PermutationProgress> progress);
		Overlap GetResults();
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
			if (_compare != null)
				_compare.Compare();
		}

		public void PermuteGroups(int permutations, IProgress<PermutationProgress> progress)
		{
			//if (_compare != null)
			//{
			//	int batchSize = 5000;
			//	int numComplete = 0;

			//	//progress.Report(new PermutationProgress { Complete = 0 });

			//	while(numComplete < permutations)
			//	{
			//		int leftOver = permutations - numComplete;
			//		int nextBatch = batchSize < leftOver ? batchSize : leftOver;

			//		// Generate a bunch of random subject idxs
			//		List<int> subjects = new List<int>();
			//		for (int i = 0; i < _filteredSubjectData.Count; i++)
			//			subjects.Add(i);

			//		Random rnd = new Random();
			//		var subjPermutations = new List<List<int>>();
			//		for (int p = 0; p < nextBatch; p++)
			//		{
			//			var randomizedList = from item in subjects
			//								 orderby rnd.Next()
			//								 select item;

			//			subjPermutations.Add(randomizedList.ToList());
			//		}

			//		// Run our permutation
			//		//_compare.Permute(nextBatch, subjPermutations, _dataTypes);

			//		numComplete += nextBatch;
			//		progress.Report(new PermutationProgress { Complete = numComplete });
			//	}

			//	//_eventAggregator.Publish(new NBSResultsAvailable());
			//}
		}

		public Overlap GetResults()
		{
			return null; //_compare.GetResult();
		}
	}
}
