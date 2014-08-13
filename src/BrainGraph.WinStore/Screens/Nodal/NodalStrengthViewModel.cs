using BrainGraph.ComputeRT.Group;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainGraph.WinStore.Screens.Nodal
{
	public class NodalStrengthViewModel : ViewModelBase
	{
		#region Private Service Vars
		private IRegionService _regionService = IoC.Get<IRegionService>();
		private IComputeService _computeService = IoC.Get<IComputeService>();
		#endregion

		public NodalStrengthViewModel()
		{
			Title = "Strength";
			PrimaryValue = "0";
			Subtitle = "Regional average measure of connectivity by data type.";

			Nodes = new BindableCollection<NodalViewModel>();
		}

		protected override void OnActivate()
		{
			LoadData();

			base.OnActivate();
		}

		protected void LoadData()
		{
			Nodes.Clear();

			var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			foreach (var graph in results.Graphs)
			{
				if (graph.Name == Parameter)
				{ 
					List<NodalViewModel> nodes = new List<NodalViewModel>();

					foreach (var node in graph.Nodes)
					{
						var nvm = new NodalViewModel { RawNode = node };
						nvm.RegionTitle = regions[node.Index].Name.Replace("_", " ");
						nvm.Tail = node.Strength.TwoTailCount;
						nvm.PValue = (double)node.Strength.TwoTailCount / (double)permutations;

						nodes.Add(nvm);
					}

					var regionCount = regions.Count;
					var nodesByPVal = from node in nodes
									  orderby node.PValue
									  select node;

                    var sigNodes = new List<NodalViewModel>();
					
                    var index = 1;
					foreach (var node in nodesByPVal)
					{
						node.QValue = ((double)index / (double)regionCount) * 0.05;

                        if (node.PValue <= 0.00055)
                            sigNodes.Add(node);

						++index;
					}

                    //var sigNodes = from node in nodes
                    //               where node.PValue <= node.QValue
                    //               select node;
					
					Nodes.AddRange(nodes.OrderBy(n => n.PValue));
					SigNodeCount = sigNodes.Count();
				}
			}
		}

		public string Parameter { get { return _inlParameter; } set { _inlParameter = value; NotifyOfPropertyChange(() => Parameter); Title = Parameter + " Strength"; } } private string _inlParameter;
		public BindableCollection<NodalViewModel> Nodes { get; private set; }
		public int SigNodeCount { get; set; }
	}
	
	public class NodalViewModel : Screen
	{
		public string RegionTitle { get; set; }
		public NodeViewModel RawNode { get; set; }

		public int Tail { get; set; }
		public double PValue { get; set; }
		public double QValue { get; set; }
		public double Difference { get { return RawNode.Strength.M1 - RawNode.Strength.M2; } }
	}
}
