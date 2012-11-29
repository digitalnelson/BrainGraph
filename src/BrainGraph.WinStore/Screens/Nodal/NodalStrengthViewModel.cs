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

			NodeGroups = new BindableCollection<NodeGroupViewModel>();
			_eventAggregator.Subscribe(this);
		}

		public void Handle(PermutationCompleteEvent message)
		{
			IsReady = true;

			int total = 0;
			NodeGroups.Clear();

			var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			foreach (var graph in results.Graphs)
			{
				List<NodeViewModel> nodes = new List<NodeViewModel>();

				foreach (var node in graph.Nodes)
				{
					var pval = (float)node.Strength.TwoTailCount / (float)permutations;

					if (pval < 0.05 && Math.Abs(node.Strength.Value) >= 2.0 )
					{
						var nvm = new NodeViewModel { RawNode = node };
						nvm.RegionTitle = regions[node.Index].Name.Replace("_", " ");
						nvm.PValue = pval;

						nodes.Add(nvm);
					}
				}

				NodeGroupViewModel ngvm = new NodeGroupViewModel();
				ngvm.GroupName = graph.Name;
				ngvm.Nodes.AddRange(nodes.OrderBy(n => n.RegionTitle));

				NodeGroups.Add(ngvm);

				total += ngvm.Nodes.Count;
			}

			PrimaryValue = total.ToString();
		}

		public override Type ViewModelType { get { return typeof(NodalStrengthViewModel); } }

		public BindableCollection<NodeGroupViewModel> NodeGroups { get; private set; }
	}

	public class NodeGroupViewModel : Screen
	{
		public NodeGroupViewModel()
		{
			Nodes = new BindableCollection<NodeViewModel>();
		}

		public string GroupName { get { return _inlGroupName; } set { _inlGroupName = value; NotifyOfPropertyChange(() => GroupName); } } private string _inlGroupName;
		public BindableCollection<NodeViewModel> Nodes { get { return _inlNodes; } set { _inlNodes = value; NotifyOfPropertyChange(() => Nodes); } } private BindableCollection<NodeViewModel> _inlNodes;
	}

	public class NodeViewModel : Screen
	{
		public string RegionTitle { get; set; }
		public MultiNode RawNode { get; set; }

		public float PValue { get; set; }
		public float Difference { get { return RawNode.Strength.M1 - RawNode.Strength.M2; } }
	}
}
