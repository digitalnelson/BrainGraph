using BrainGraph.ComputeRT.Group;
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
					pVals.Add((double)edge.Weight.TwoTailCount / (double)permutations);
				}

				var qThresh = -1.0;
				pVals.Sort();
				for (var i = 0; i < pVals.Count; i++)
				{
					double dIdx = (double)i;
					double dReg = (double)pVals.Count;
					double qval = ((dIdx + 1) / dReg) * 0.05;

					if (pVals[i] <= qval)
					{
						qThresh = qval;
						continue;
					}
					else
						break;
				}

				foreach (var edge in graph.Edges)
				{
					var pval = (double)edge.Weight.TwoTailCount / (double)permutations;

                    if ((pval <= 0.000012))
                    {
                        var nvm = new EdgeSigViewModel { RawEdge = edge };
                        nvm.NodeOneTitle = regions[edge.NodeOneIndex].Name.Replace("_", " ");
                        nvm.NodeTwoTitle = regions[edge.NodeTwoIndex].Name.Replace("_", " ");
                        nvm.PValue = pval;

                        edges.Add(nvm);
                    }
                    //else
                    //    break;
				}

				EdgeSigGroupViewModel gvm = new EdgeSigGroupViewModel();
				gvm.GroupName = graph.Name;
				gvm.Edges.AddRange(edges);

				EdgeGroups.Add(gvm);

				total += gvm.Edges.Count;

				if (String.IsNullOrWhiteSpace(PrimaryValue))
					PrimaryValue += graph.Name + ": " + gvm.Edges.Count.ToString();
				else
					PrimaryValue += "\n" + graph.Name + ": " + gvm.Edges.Count.ToString();
			}

			//PrimaryValue = total.ToString();
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
