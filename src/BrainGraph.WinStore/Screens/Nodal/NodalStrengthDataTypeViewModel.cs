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
using OxyPlot.Metro;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace BrainGraph.WinStore.Screens.Nodal
{
	public class NodalStrengthDataTypeViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private INavigationService _navService;
		private IRegionService _regionService;
		private IComputeService _computeService;
		#endregion

		public NodalStrengthDataTypeViewModel()
		{
			Title = "Nodal Strength";
			PrimaryValue = "0";
			Subtitle = "Regional average measure of connectivity by data type.";

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
				List<DtNodalViewModel> nodes = new List<DtNodalViewModel>();

				foreach (var node in graph.Nodes)
				{
					var nvm = new DtNodalViewModel { RawNode = node };
					nvm.RegionTitle = regions[node.Index].Name.Replace("_", " ");
					nvm.ROI = regions[node.Index];
					nvm.Tail = node.Strength.TwoTailCount;
					nvm.PValue = (double)node.Strength.TwoTailCount / (double)permutations;

					nodes.Add(nvm);
				}

				var regionCount = regions.Count;
				var nodesByPVal = from node in nodes
								  orderby node.PValue
								  select node;

        var sigNodes = new List<DtNodalViewModel>();

				Debug.WriteLine("Node Results - {0}", graph.Name);
				Debug.WriteLine("NodeIdx\tDiff\tPvalue");

        var index = 1;
        foreach (var node in nodesByPVal)
        {
            node.QValue = ((double)index / (double)regionCount) * 0.05;
						Debug.WriteLine("{0}\t{1}\t{2}", node.RawNode.Index, node.RawNode.Strength.M1 - node.RawNode.Strength.M2, node.PValue);

            ++index;
        }

				var diff = from node in nodes
						   select node.Difference;

				double max = diff.Max();
				double min = diff.Min();

				if (max <= 0)
					max = 1;
				if (min >= 0)
					min = -1;

				OxyPalette maxPal = OxyPalette.Interpolate(32, OxyColor.FromArgb(64, 64, 64, 64), OxyColors.Red);
				OxyPalette minPal = OxyPalette.Interpolate(32, OxyColors.Blue, OxyColor.FromArgb(64, 64, 64, 64)); 

				var itm = new DataItemViewModel();
				itm.DataType = graph.Name;
				itm.SigNodeCount = sigNodes.Count();

				itm.AXPlotModelPos = LoadPlotModel(nodes, r => r.X, r => r.Y, max, 0, maxPal);
				itm.SGPlotModelPos = LoadPlotModel(nodes, r => (100 - r.Y), r => r.Z, max, 0, maxPal);
				itm.CRPlotModelPos = LoadPlotModel(nodes, r => r.X, r => r.Z, max, 0, maxPal);

				itm.AXPlotModelNeg = LoadPlotModel(nodes, r => r.X, r => r.Y, 0, min, minPal);
				itm.SGPlotModelNeg = LoadPlotModel(nodes, r => (100 - r.Y), r => r.Z, 0, min, minPal);
				itm.CRPlotModelNeg = LoadPlotModel(nodes, r => r.X, r => r.Z, 0, min, minPal);

				DataItems.Add(itm);

				if (String.IsNullOrWhiteSpace(PrimaryValue))
					PrimaryValue += graph.Name + ": " + itm.SigNodeCount.ToString();
				else
					PrimaryValue += "\n" + graph.Name + ": " + itm.SigNodeCount.ToString();
			}
		}

		protected PlotModel LoadPlotModel(List<DtNodalViewModel> nodes, Func<ROI, double> horizSelector, Func<ROI, double> vertSelector, double max, double min, OxyPalette palette)
		{
			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);
			model.PlotAreaBorderColor = OxyColors.Transparent;
			model.PlotType = PlotType.Cartesian;

			var ba = new LinearAxis(AxisPosition.Bottom) { IsAxisVisible = false };
			var la = new LinearAxis(AxisPosition.Left) { IsAxisVisible = false };
			var ca = new ColorAxis { TextColor = OxyColors.White, TicklineColor = OxyColors.LightGray, Position = AxisPosition.Right, Palette = palette, Minimum = -1, Maximum = 1, AbsoluteMaximum = max, AbsoluteMinimum = min };

			ba.MinimumPadding = 0.1;
			ba.MaximumPadding = 0.1;
			la.MinimumPadding = 0.1;
			la.MaximumPadding = 0.1;

			model.Axes.Add(ba);
			model.Axes.Add(la);
			model.Axes.Add(ca);

			var s1 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
			};

			var s2 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				ColorAxis = null,
				MarkerFill = OxyColors.Gray,
			};

			foreach (var node in nodes)
			{
					if (node.Significant)
					{
						if(node.Difference >= min && node.Difference <= max)
							s1.Points.Add(new BrainScatterPoint(horizSelector(node.ROI), vertSelector(node.ROI), node.ROI, node.Difference));
						else
							s1.Points.Add(new BrainScatterPoint(horizSelector(node.ROI), vertSelector(node.ROI), node.ROI, 0));
					}
					else
						s2.Points.Add(new BrainScatterPoint(horizSelector(node.ROI), vertSelector(node.ROI), node.ROI, 0));
			}

			model.Series.Add(s2);
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

		private async void SaveChart(StorageFolder outputFolder, string prefix, string dataType, PlotModel plot)
		{
			string fileName = "NodalStrengthByDataType_" + "_" + dataType + "_" + prefix + ".svg";
			var outputFile = await outputFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

			foreach (var axis in plot.Axes)
				axis.TextColor = OxyColors.Black;

			var contents = plot.ToSvg(300, 300, false, new MetroRenderContext(new Canvas()));

			foreach (var axis in plot.Axes)
				axis.TextColor = OxyColors.White;

			await FileIO.WriteTextAsync(outputFile, contents);
		}

		public async void SaveCharts(RoutedEventArgs args)
		{
			FolderPicker wkPicker = new FolderPicker();
			wkPicker.ViewMode = PickerViewMode.List;
			wkPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			wkPicker.FileTypeFilter.Add(".svg");
			wkPicker.FileTypeFilter.Add(".html");

			var outputFolder = await wkPicker.PickSingleFolderAsync();
			if (outputFolder != null)
			{
				foreach (var dataItem in DataItems)
				{
					SaveChart(outputFolder, "SG_Neg", dataItem.DataType, dataItem.SGPlotModelNeg);
					SaveChart(outputFolder, "AX_Neg", dataItem.DataType, dataItem.AXPlotModelNeg);
					SaveChart(outputFolder, "CR_Neg", dataItem.DataType, dataItem.CRPlotModelNeg);

					SaveChart(outputFolder, "SG_Pos", dataItem.DataType, dataItem.SGPlotModelPos);
					SaveChart(outputFolder, "AX_Pos", dataItem.DataType, dataItem.AXPlotModelPos);
					SaveChart(outputFolder, "CR_Pos", dataItem.DataType, dataItem.CRPlotModelPos);
				}
			}
		}

		public void DataTypeSelected(ItemClickEventArgs eventArgs)
		{
			_navService.NavigateToViewModel(typeof(NodalStrengthViewModel), ((DataItemViewModel)eventArgs.ClickedItem).DataType);
		}

		public override Type ViewModelType { get { return typeof(NodalStrengthDataTypeViewModel); } }

		public BindableCollection<DataItemViewModel> DataItems { get { return _inlDataItems; } set { _inlDataItems = value; NotifyOfPropertyChange(() => DataItems); } } private BindableCollection<DataItemViewModel> _inlDataItems;
	}

	public class DataItemViewModel : Screen
	{
		public string DataType { get; set; }
		public int SigNodeCount { get; set; }

		public PlotModel SGPlotModelPos { get { return _inlSGPlotModelPos; } set { _inlSGPlotModelPos = value; NotifyOfPropertyChange(() => SGPlotModelPos); } } private PlotModel _inlSGPlotModelPos;
		public PlotModel AXPlotModelPos { get { return _inlAXPlotModelPos; } set { _inlAXPlotModelPos = value; NotifyOfPropertyChange(() => AXPlotModelPos); } } private PlotModel _inlAXPlotModelPos;
		public PlotModel CRPlotModelPos { get { return _inlCRPlotModelPos; } set { _inlCRPlotModelPos = value; NotifyOfPropertyChange(() => CRPlotModelPos); } } private PlotModel _inlCRPlotModelPos;

		public PlotModel SGPlotModelNeg { get { return _inlSGPlotModelNeg; } set { _inlSGPlotModelNeg = value; NotifyOfPropertyChange(() => SGPlotModelNeg); } } private PlotModel _inlSGPlotModelNeg;
		public PlotModel AXPlotModelNeg { get { return _inlAXPlotModelNeg; } set { _inlAXPlotModelNeg = value; NotifyOfPropertyChange(() => AXPlotModelNeg); } } private PlotModel _inlAXPlotModelNeg;
		public PlotModel CRPlotModelNeg { get { return _inlCRPlotModelNeg; } set { _inlCRPlotModelNeg = value; NotifyOfPropertyChange(() => CRPlotModelNeg); } } private PlotModel _inlCRPlotModelNeg;
	}

	public class DtNodalViewModel : Screen
	{
		public string RegionTitle { get; set; }
		public ROI ROI { get; set; }

		public NodeViewModel RawNode { get; set; }

		public int Tail { get; set; }
		public double PValue { get; set; }
		public double QValue { get; set; }
		public double Difference { get { return RawNode.Strength.M1 - RawNode.Strength.M2; } }
		public bool Significant { get; set; }
	}
}
