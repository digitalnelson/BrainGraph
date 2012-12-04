using BrainGraph.Compute.Graph;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainGraph.WinStore.Screens.Nodal
{
	public class NodalStrengthViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator = IoC.Get<IEventAggregator>();
		private IRegionService _regionService = IoC.Get<IRegionService>();
		private IComputeService _computeService = IoC.Get<IComputeService>();
		#endregion

		public NodalStrengthViewModel()
		{
			Title = "Strength";
			PrimaryValue = "0";

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				NodeGroups = new BindableCollection<NodeGroupViewModel>();
				_eventAggregator.Subscribe(this);
			}
		}

		public void Handle(PermutationCompleteEvent message)
		{
			IsReady = true;
			NodeGroups.Clear();

			PrimaryValue = "";

			var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			foreach (var graph in results.Graphs)
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

				var index = 1;
				foreach(var node in nodesByPVal)
				{
					node.QValue = ( (double)index / (double)regionCount ) * 0.05;

					++index;
				}

				var sigNodes = from node in nodes
							   where node.PValue <= node.QValue
							   select node;

				NodeGroupViewModel ngvm = new NodeGroupViewModel();
				ngvm.GroupName = graph.Name;
				ngvm.Nodes.AddRange(nodes.OrderBy(n => n.PValue));
				ngvm.SigNodeCount = sigNodes.Count();
				
				NodeGroups.Add(ngvm);

				if (String.IsNullOrWhiteSpace(PrimaryValue))
					PrimaryValue += graph.Name + ": " + ngvm.SigNodeCount.ToString();
				else
					PrimaryValue += "\n" + graph.Name + ": " + ngvm.SigNodeCount.ToString();
			}
		}

		public override Type ViewModelType { get { return typeof(NodalStrengthViewModel); } }

		public BindableCollection<NodeGroupViewModel> NodeGroups { get; private set; }
	}

	public class NodeGroupViewModel : Screen
	{
		public NodeGroupViewModel()
		{
			Nodes = new BindableCollection<NodalViewModel>();
		}

		public string GroupName { get { return _inlGroupName; } set { _inlGroupName = value; NotifyOfPropertyChange(() => GroupName); } } private string _inlGroupName;
		public BindableCollection<NodalViewModel> Nodes { get { return _inlNodes; } set { _inlNodes = value; NotifyOfPropertyChange(() => Nodes); } } private BindableCollection<NodalViewModel> _inlNodes;
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
