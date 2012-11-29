using BrainGraph.Compute.Graph;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;

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

			EdgeGroups = new BindableCollection<EdgeGroupViewModel>();
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

			foreach (var graph in results.Graphs)
			{
				List<EdgeViewModel> edges = new List<EdgeViewModel>();

				foreach (var edge in graph.Edges)
				{
					var pval = (float)edge.Weight.TwoTailCount / (float)permutations;

					if (pval < 0.05 && Math.Abs(edge.Weight.Value) >= 2.0)
					{
						var nvm = new EdgeViewModel { RawEdge = edge };
						nvm.NodeOneTitle = regions[edge.NodeOneIndex].Name.Replace("_", " ");
						nvm.NodeTwoTitle = regions[edge.NodeTwoIndex].Name.Replace("_", " ");
						nvm.PValue = pval;

						edges.Add(nvm);
					}
				}

				EdgeGroupViewModel gvm = new EdgeGroupViewModel();
				gvm.GroupName = graph.Name;
				gvm.Edges.AddRange(edges);

				EdgeGroups.Add(gvm);

				total += gvm.Edges.Count;
			}

			PrimaryValue = total.ToString();
		}

		public override Type ViewModelType { get { return typeof(EdgeSignificanceViewModel); } }

		public BindableCollection<EdgeGroupViewModel> EdgeGroups { get; private set; }
	}

	public class EdgeGroupViewModel
	{
		public EdgeGroupViewModel()
		{
			Edges = new List<EdgeViewModel>();
		}

		public string GroupName { get; set; }
		public List<EdgeViewModel> Edges { get; set; }
	}

	public class EdgeViewModel
	{
		public string NodeOneTitle { get; set; }
		public string NodeTwoTitle { get; set; }
		public MultiEdge RawEdge { get; set; }

		public float PValue { get; set; }
		public float Difference { get { return RawEdge.Weight.M1 - RawEdge.Weight.M2; } }
	}
}
