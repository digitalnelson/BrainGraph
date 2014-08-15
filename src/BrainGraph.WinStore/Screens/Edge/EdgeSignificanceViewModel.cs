using BrainGraph.ComputeRT.Group;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BrainGraph.WinStore.Screens.Edge
{
	class EdgeSignificanceViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator = IoC.Get<IEventAggregator>();
		private IRegionService _regionService = IoC.Get<IRegionService>();
		private IComputeService _computeService = IoC.Get<IComputeService>();
		#endregion

		public EdgeSignificanceViewModel()
		{
			Title = "Significance";
			PrimaryValue = "0";

			EdgeGroups = new BindableCollection<EdgeSigGroupViewModel>();
			_eventAggregator.Subscribe(this);
		}

		public void Handle(PermutationCompleteEvent message)
		{
			IsReady = true;

			int total = 0;
			EdgeGroups.Clear();

			var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			PrimaryValue = "";

			foreach (var graph in results.Graphs)
			{
				List<EdgeSigViewModel> edges = new List<EdgeSigViewModel>();

				var pVals = new List<double>();
				foreach (var edge in graph.Edges)
				{
						var pval = (double)edge.Weight.TwoTailCount / (double)permutations;

						var nvm = new EdgeSigViewModel { RawEdge = edge };
						nvm.NodeOneTitle = regions[edge.NodeOneIndex].Name.Replace("_", " ");
						nvm.NodeTwoTitle = regions[edge.NodeTwoIndex].Name.Replace("_", " ");
						nvm.PValue = pval;

						edges.Add(nvm);
				}

				var edgesByPVal = from edge in edges
													orderby edge.PValue
													select edge;

				Debug.WriteLine("Edge Results - {0}", graph.Name);
				Debug.WriteLine("Node1Idx\tNode2Idx\tDiff\tPvalue");

				foreach (var edge in edgesByPVal)
				{
						Debug.WriteLine("{0}\t{1}\t{2}\t{3}", edge.RawEdge.NodeOneIndex, edge.RawEdge.NodeTwoIndex, edge.Difference, edge.PValue);
				}

				EdgeSigGroupViewModel gvm = new EdgeSigGroupViewModel();
				gvm.GroupName = graph.Name;
				gvm.Edges.AddRange(edgesByPVal);

				EdgeGroups.Add(gvm);

				total += gvm.Edges.Count;

				if (String.IsNullOrWhiteSpace(PrimaryValue))
					PrimaryValue += graph.Name + ": " + gvm.Edges.Count.ToString();
				else
					PrimaryValue += "\n" + graph.Name + ": " + gvm.Edges.Count.ToString();
			}
		}

		public override Type ViewModelType { get { return typeof(EdgeSignificanceViewModel); } }

		public BindableCollection<EdgeSigGroupViewModel> EdgeGroups { get; private set; }
	}

	public class EdgeSigGroupViewModel
	{
		public EdgeSigGroupViewModel()
		{
			Edges = new List<EdgeSigViewModel>();
		}

		public string GroupName { get; set; }
		public List<EdgeSigViewModel> Edges { get; set; }
	}

	public class EdgeSigViewModel
	{
		public string NodeOneTitle { get; set; }
		public string NodeTwoTitle { get; set; }
		public EdgeViewModel RawEdge { get; set; }

		public double PValue { get; set; }
		public double Difference { get { return RawEdge.Weight.M1 - RawEdge.Weight.M2; } }
	}
}
