using BrainGraph.Compute.Graph;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Common.Viz;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Models;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainGraph.WinStore.Screens.Nodal
{
	public class NodalStrengthDataTypeViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private IRegionService _regionService;
		private IComputeService _computeService;
		#endregion

		public NodalStrengthDataTypeViewModel()
		{
			Title = "Strength";
			PrimaryValue = "0";
			Subtitle = "Regional average measure of connectivity by data type.";

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_regionService = IoC.Get<IRegionService>();
				_computeService = IoC.Get<IComputeService>();

				DataItems = new BindableCollection<DataItemViewModel>();
				_eventAggregator.Subscribe(this);
			}

			#region Design Data
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				DataItems = new BindableCollection<DataItemViewModel>();

				//GlobalViewModel gvm = new GlobalViewModel();
				//gvm.Strength.M1 = 1;
				//gvm.Strength.M2 = 2;
				//gvm.Strength.V1 = 3;
				//gvm.Strength.V2 = 4;
				//gvm.Strength.Value = 5;
				//gvm.Strength.TwoTailCount = 6;

				DataItems.Add(new DataItemViewModel { DataType = "DTI", SigNodeCount = 90 });
				DataItems.Add(new DataItemViewModel { DataType = "fMRI", SigNodeCount = 90 });
			}
			#endregion
		}

		protected void LoadResults()
		{
			IsReady = true;
			DataItems.Clear();

			PrimaryValue = "";

			var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			foreach (var graph in results.Graphs)
			{
				var itm = new DataItemViewModel();

				itm.DataType = graph.Name;

				List<DtNodalViewModel> nodes = new List<DtNodalViewModel>();

				foreach (var node in graph.Nodes)
				{
					var nvm = new DtNodalViewModel { RawNode = node };
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
				foreach (var node in nodesByPVal)
				{
					node.QValue = ((double)index / (double)regionCount) * 0.05;
					++index;
				}

				var sigNodes = from node in nodes
							   where node.PValue <= node.QValue
							   select node;
			
				itm.SigNodeCount = sigNodes.Count();

				DataItems.Add(itm);

				if (String.IsNullOrWhiteSpace(PrimaryValue))
					PrimaryValue += graph.Name + ": " + itm.SigNodeCount.ToString();
				else
					PrimaryValue += "\n" + graph.Name + ": " + itm.SigNodeCount.ToString();
			}
		}

		protected PlotModel LoadPlotModel(Func<ROI, double> horizSelector, Func<ROI, double> vertSelector, List<ROI> regions)
		{
			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);
			model.PlotAreaBorderColor = OxyColors.White;
			model.PlotType = PlotType.Cartesian;

			var ba = new LinearAxis(AxisPosition.Bottom) { AxislineColor = OxyColors.White, TextColor = OxyColors.White, MajorGridlineColor = OxyColors.White, TicklineColor = OxyColors.White };
			var la = new LinearAxis(AxisPosition.Left) { AxislineColor = OxyColors.White, TextColor = OxyColors.White, TicklineColor = OxyColors.White };

			ba.MinimumPadding = 0.1;
			ba.MaximumPadding = 0.1;
			la.MinimumPadding = 0.1;
			la.MaximumPadding = 0.1;

			model.Axes.Add(ba);
			model.Axes.Add(la);

			var s1 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.White),
			};

			foreach (var region in regions)
			{
				s1.Points.Add(new BrainDataPoint(horizSelector(region), vertSelector(region), region));
			}

			model.Series.Add(s1);

			return model;
		}

		protected override void OnActivate()
		{
			base.OnActivate();

			if (DataItems.Count == 0)
				LoadResults();
		}

		public void Handle(PermutationCompleteEvent message)
		{
			LoadResults();
		}

		public override Type ViewModelType { get { return typeof(NodalStrengthDataTypeViewModel); } }
		public BindableCollection<DataItemViewModel> DataItems { get { return _inlDataItems; } set { _inlDataItems = value; NotifyOfPropertyChange(() => DataItems); } } private BindableCollection<DataItemViewModel> _inlDataItems;
	}

	public class DataItemViewModel : Screen
	{
		public string DataType { get; set; }
		public int SigNodeCount { get; set; }

		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;
	}

	public class DtNodalViewModel : Screen
	{
		public string RegionTitle { get; set; }
		public NodeViewModel RawNode { get; set; }

		public int Tail { get; set; }
		public double PValue { get; set; }
		public double QValue { get; set; }
		public double Difference { get { return RawNode.Strength.M1 - RawNode.Strength.M2; } }
	}
}
