using BrainGraph.ComputeRT.Group;
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
using OxyPlot.Axes;
using OxyPlot.Series;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace BrainGraph.WinStore.Screens.Component
{
	public class IntraSummaryViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private INavigationService _navService;
		private IRegionService _regionService;
		private IComputeService _computeService;
		#endregion

		public IntraSummaryViewModel()
		{
			Title = "Intramodal";
			PrimaryValue = "0";
			Subtitle = "Components by data type.";

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_navService = IoC.Get<INavigationService>();
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

				//DataItems.Add(new DataItemViewModel { DataType = "DTI", SigNodeCount = 90 });
				//DataItems.Add(new DataItemViewModel { DataType = "fMRI", SigNodeCount = 90 });
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

			int i = 0;
			Debug.WriteLine("InterModal");
			foreach (var val in results.RandomDistribution)
			{
				Debug.WriteLine(i + " " + val);
				++i;
			}

			foreach (var graph in results.Graphs)
			{
				ComponentViewModel lrgCmp = null;
				foreach (var cmp in graph.Components)
				{
					if (lrgCmp == null || cmp.NodeCount > lrgCmp.NodeCount)
						lrgCmp = cmp;
				}

				if (lrgCmp != null)
				{
					Debug.WriteLine(graph.Name);

					var itm = new DataItemViewModel();
					itm.DataType = graph.Name;
					itm.SigNodeCount = lrgCmp.NodeCount;

					i = 0;
					foreach (var val in lrgCmp.RandomDistribution)
					{
						Debug.WriteLine(i + " " + val);

						++i;
					}

					itm.Nodes = new List<NodeSummaryViewModel>();
					itm.SigNodes = new List<NodeSummaryViewModel>();
					itm.SigEdges = new List<EdgeSummaryViewModel>();

                    itm.PValue = (double)lrgCmp.RandomTailCount / permutations;

					foreach (var region in regions)
						itm.Nodes.Add(new NodeSummaryViewModel { Region = region });

					foreach (var edge in lrgCmp.Edges)
					{
						itm.Nodes[edge.NodeOneIndex].Significant = true;
						itm.Nodes[edge.NodeTwoIndex].Significant = true;

						itm.SigEdges.Add(new EdgeSummaryViewModel());
					}

					foreach (var node in itm.Nodes)
					{
						if (node.Significant)
							itm.SigNodes.Add(node);
					}

					itm.AXPlotModel = LoadPlotModel(itm.Nodes, lrgCmp, r => r.X, r => r.Y);
					itm.SGPlotModel = LoadPlotModel(itm.Nodes, lrgCmp, r => (100 - r.Y), r => r.Z);
					itm.CRPlotModel = LoadPlotModel(itm.Nodes, lrgCmp, r => r.X, r => r.Z);

					DataItems.Add(itm);

					if (String.IsNullOrWhiteSpace(PrimaryValue))
						PrimaryValue += graph.Name + ": " + itm.SigNodeCount.ToString();
					else
						PrimaryValue += "\n" + graph.Name + ": " + itm.SigNodeCount.ToString();
				}
			}
		}

		protected PlotModel LoadPlotModel(List<NodeSummaryViewModel> nodes, ComponentViewModel cmp, Func<ROI, double> horizSelector, Func<ROI, double> vertSelector)
		{
			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);
			model.PlotAreaBorderColor = OxyColors.White;
			model.PlotType = PlotType.Cartesian;

			//var ba = new LinearAxis(AxisPosition.Bottom) { IsAxisVisible = false };
			//var la = new LinearAxis(AxisPosition.Left) { IsAxisVisible = false };

			var ba = new LinearAxis(AxisPosition.Bottom) { AxislineColor = OxyColors.White, TextColor = OxyColors.White, MajorGridlineColor = OxyColors.White, TicklineColor = OxyColors.White };
			var la = new LinearAxis(AxisPosition.Left) { AxislineColor = OxyColors.White, TextColor = OxyColors.White, TicklineColor = OxyColors.White };

			ba.MinimumPadding = 0.1;
			ba.MaximumPadding = 0.1;
			la.MinimumPadding = 0.1;
			la.MaximumPadding = 0.1;

			model.Axes.Add(ba);
			model.Axes.Add(la);

			var sigNodes = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
			};

			var nonSigNodes = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				ColorAxis = null,
				MarkerFill = OxyColors.Gray,
			};

			var sigEdges = new LineSeries
			{
				Color = OxyColor.FromAColor(125, OxyColors.Green),
			};

			var edges = cmp.Edges;
			if (edges != null && edges.Count > 0)
			{
				foreach (var edge in edges)
				{
					var v1 = nodes[edge.NodeOneIndex];
					var v2 = nodes[edge.NodeTwoIndex];

					sigEdges.Points.Add(new DataPoint(Double.NaN, Double.NaN));
					sigEdges.Points.Add(new DataPoint(horizSelector(v1.Region), vertSelector(v1.Region)));
					sigEdges.Points.Add(new DataPoint(horizSelector(v2.Region), vertSelector(v2.Region)));
					sigEdges.Points.Add(new DataPoint(Double.NaN, Double.NaN));
				}
			}

			foreach (var node in nodes)
			{
				if (node.Significant)
					sigNodes.Points.Add(new BrainDataPoint(horizSelector(node.Region), vertSelector(node.Region), node.Region));
				else
					nonSigNodes.Points.Add(new BrainDataPoint(horizSelector(node.Region), vertSelector(node.Region), node.Region));
			}

			model.Series.Add(nonSigNodes);

			if (sigEdges.Points.Count > 0)
				model.Series.Add(sigEdges);

			model.Series.Add(sigNodes);

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

		//private async void SaveChart(StorageFolder outputFolder, string prefix, string dataType, PlotModel plot)
		//{
		//	string fileName = "NodalStrengthByDataType_" + "_" + dataType + "_" + prefix + ".svg";
		//	var outputFile = await outputFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

		//	foreach (var axis in plot.Axes)
		//		axis.TextColor = OxyColors.Black;

		//	var contents = plot.ToSvg(300, 300);

		//	foreach (var axis in plot.Axes)
		//		axis.TextColor = OxyColors.White;

		//	await FileIO.WriteTextAsync(outputFile, contents);
		//}

		//public async void SaveCharts(RoutedEventArgs args)
		//{
		//	FolderPicker wkPicker = new FolderPicker();
		//	wkPicker.ViewMode = PickerViewMode.List;
		//	wkPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		//	wkPicker.FileTypeFilter.Add(".svg");
		//	wkPicker.FileTypeFilter.Add(".html");

		//	var outputFolder = await wkPicker.PickSingleFolderAsync();
		//	if (outputFolder != null)
		//	{
		//		foreach (var dataItem in DataItems)
		//		{
		//			SaveChart(outputFolder, "SG_Neg", dataItem.DataType, dataItem.SGPlotModelNeg);
		//			SaveChart(outputFolder, "AX_Neg", dataItem.DataType, dataItem.AXPlotModelNeg);
		//			SaveChart(outputFolder, "CR_Neg", dataItem.DataType, dataItem.CRPlotModelNeg);

		//			SaveChart(outputFolder, "SG_Pos", dataItem.DataType, dataItem.SGPlotModelPos);
		//			SaveChart(outputFolder, "AX_Pos", dataItem.DataType, dataItem.AXPlotModelPos);
		//			SaveChart(outputFolder, "CR_Pos", dataItem.DataType, dataItem.CRPlotModelPos);
		//		}
		//	}
		//}

		//public void DataTypeSelected(ItemClickEventArgs eventArgs)
		//{
		//	_navService.NavigateToViewModel(typeof(NodalStrengthViewModel), ((DataItemViewModel)eventArgs.ClickedItem).DataType);
		//}

		public override Type ViewModelType { get { return typeof(IntraSummaryViewModel); } }

		public BindableCollection<DataItemViewModel> DataItems { get { return _inlDataItems; } set { _inlDataItems = value; NotifyOfPropertyChange(() => DataItems); } } private BindableCollection<DataItemViewModel> _inlDataItems;
	}

	public class DataItemViewModel : Screen
	{
		public string DataType { get; set; }
		public int SigNodeCount { get; set; }

        public double PValue { get { return _inlpValue; } set { _inlpValue = value; NotifyOfPropertyChange(() => PValue); } } private double _inlpValue;

		public List<NodeSummaryViewModel> Nodes { get; set; }

		public List<NodeSummaryViewModel> SigNodes { get; set; }
		public List<EdgeSummaryViewModel> SigEdges { get; set; }

		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;
	}

	public class NodeSummaryViewModel : Screen
	{
		public string RegionTitle { get; set; }
		public ROI Region { get; set; }

		public NodeViewModel RawNode { get; set; }

		public int Tail { get; set; }
		public double PValue { get; set; }
		public double QValue { get; set; }
		public double Difference { get { return RawNode.Strength.M1 - RawNode.Strength.M2; } }
		public bool Significant { get; set; }
	}

	public class EdgeSummaryViewModel : Screen
	{
		public string RegionTitle { get; set; }
		public ROI ROI { get; set; }

		public EdgeViewModel RawEdge { get; set; }

		public int Tail { get; set; }
		public double PValue { get; set; }
		public double QValue { get; set; }
		public bool Significant { get; set; }
	}
}
