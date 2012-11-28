using BrainGraph.Compute.Graph;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Models;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace BrainGraph.WinStore.Screens.Nodal
{
	public class NodalStrengthViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		private IEventAggregator _eventAggregator;
		private IRegionService _regionService;
		private IComputeService _computeService;

		public NodalStrengthViewModel()
		{
			Title = "Strength";
			PrimaryValue = "0";

			NodeGroups = new BindableCollection<NodeGroupViewModel>();

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_regionService = IoC.Get<IRegionService>();
				_computeService = IoC.Get<IComputeService>();
				_eventAggregator.Subscribe(this);
			}

			#region Sample Data
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				
				NodeGroups.Add(new NodeGroupViewModel() { GroupName = "Data Type 1", Nodes = new BindableCollection<NodeViewModel>() { new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" }, new NodeViewModel { RegionTitle = "This is ", PValue = "0.654", Difference = "-312.023" }, new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" } } });
				NodeGroups.Add(new NodeGroupViewModel() { GroupName = "Data Type 2", Nodes = new BindableCollection<NodeViewModel>() { new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" }, new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" }, new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" }, new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" }, new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" }, new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" }, new NodeViewModel { DisplayName = "Test", RegionTitle = "Test" } } });
			}
			#endregion
		}

		public void Handle(PermutationCompleteEvent message)
		{
			IsReady = true;

			int total = 0;

			var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			foreach (var graph in results.Graphs)
			{
				NodeGroupViewModel ngvm = new NodeGroupViewModel();
				ngvm.GroupName = graph.Name;
				ngvm.Nodes = new BindableCollection<NodeViewModel>();

				List<MultiNode> nodes = new List<MultiNode>();

				foreach (var node in graph.Nodes)
				{
					var pval = (double)node.Strength.TwoTailCount / (double)permutations;

					//if (pval < 0.05 && Math.Abs(node.Strength.Value) >= 2.0 )
					if (pval < 0.05)
					{
						string strFormat = "####0.0###";

						var nvm = new NodeViewModel();
						nvm.RegionTitle = regions[node.Index].Name.Replace("_", " ");
						nvm.Difference = (node.Strength.M1 - node.Strength.M2).ToString(strFormat);
						nvm.PValue = pval.ToString(strFormat);
						nvm.TStat = node.Strength.Value.ToString(strFormat);

						ngvm.Nodes.Add(nvm);
					}
				}

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
		public string RegionTitle { get { return _inlRegionTitle; } set { _inlRegionTitle = value; NotifyOfPropertyChange(() => RegionTitle); } } private string _inlRegionTitle;
		
		public string Difference { get { return _inlDifference; } set { _inlDifference = value; NotifyOfPropertyChange(() => Difference); } } private string _inlDifference;
		public string TStat { get { return _inlTStat; } set { _inlTStat = value; NotifyOfPropertyChange(() => TStat); } } private string _inlTStat;
		public string PValue { get { return _inlPValue; } set { _inlPValue = value; NotifyOfPropertyChange(() => PValue); } } private string _inlPValue;
	}
}
