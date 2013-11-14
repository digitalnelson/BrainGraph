using BrainGraph.ComputeRT.Subjects;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Common.Viz;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Models;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Diagnostics;

namespace BrainGraph.WinStore.Screens.Component
{
	public class IntermodalViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		#region Private Service Vars
		private Windows.Storage.ApplicationDataContainer _roamingSettings;
		private IEventAggregator _eventAggregator;
		private INavigationService _navService;
		private IRegionService _regionService;
		private IComputeService _computeService;
		private ISubjectFilterService _filterService;
		private ISubjectDataService _dataService;
		#endregion

		private const string SETTING_ROI_FILE_TOKEN = "ROIFileToken";

		public IntermodalViewModel()
		{
			Regions = new BindableCollection<RegionViewModel>();
			OverlapRegions = new BindableCollection<RegionViewModel>();

			Title = "Intermodal";
			PrimaryValue = "0";
			Subtitle = "";
			IsReady = false;

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
				_eventAggregator = IoC.Get<IEventAggregator>();
				_navService = IoC.Get<INavigationService>();
				_regionService = IoC.Get<IRegionService>();
				_computeService = IoC.Get<IComputeService>();
				_filterService = IoC.Get<ISubjectFilterService>();
				_dataService = IoC.Get<ISubjectDataService>();
				_eventAggregator.Subscribe(this);
			}
		}

		public void LoadData()
		{
			IsReady = true;

			var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			Regions.Clear();

			foreach (var region in regions)
				Regions.Add(new RegionViewModel() { Region = region });

			int overlapCount = 0;
			foreach (var multiNode in results.MultiNodes)
			{
				if (multiNode.IsFullOverlap)
				{
					Regions[multiNode.Id].IsOverlap = multiNode.IsFullOverlap;
					OverlapRegions.Add(Regions[multiNode.Id]);
					overlapCount++;
				}
			}

			// Get the data types
			var dataTypes = _filterService.GetDataTypes();
			// Get the subject data
			var grp1 = _filterService.GetGroup1();
			var grp2 = _filterService.GetGroup2();

			// Loop through datatypes and calc avg str for intermodal cpt for each subj
			foreach (var dataType in dataTypes)
			{
				foreach (var sub in grp1)
				{
					var subGraphVM = sub.Graphs[dataType];
					var dTotal = 0.0;
					var dRegCount = 0.0;

					foreach (var reg in OverlapRegions)
					{
						var nodeVM = subGraphVM.Nodes[Int32.Parse(reg.Index)];  // TODO: Why is index a string?

						dTotal += ((double)nodeVM.TotalStrength / (double)nodeVM.Degree);
						dRegCount++;
					}

					Debug.WriteLine(string.Format("{0},{1},{2},{3}", dataType, dTotal / dRegCount, sub.Attributes["CogMem"], sub.GroupId));
				}
			}			

			this.PrimaryValue = overlapCount.ToString();

			AXPlotModel = LoadPlotModel(r => r.X, r => r.Y);
			SGPlotModel = LoadPlotModel(r => (100 - r.Y), r => r.Z);
			CRPlotModel = LoadPlotModel(r => r.X, r => r.Z);
		}

		protected override void OnActivate()
		{
			LoadData();

			base.OnActivate();
		}

		public void Handle(PermutationCompleteEvent message)
		{
			LoadData();
		}

		protected PlotModel LoadPlotModel(Func<ROI, double> horizSelector, Func<ROI, double> vertSelector)
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

			var s2 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.Red),
			};

			foreach (var rvm in Regions)
			{
				if(rvm.IsOverlap)
					s2.Points.Add(new BrainDataPoint(horizSelector(rvm.Region), vertSelector(rvm.Region), rvm.Region));
				else
					s1.Points.Add(new BrainDataPoint(horizSelector(rvm.Region), vertSelector(rvm.Region), rvm.Region));
			}

			model.Series.Add(s1);
			model.Series.Add(s2);

			return model;
		}

		public override Type ViewModelType { get { return typeof(IntermodalViewModel); } }
		
		public BindableCollection<RegionViewModel> Regions { get; private set; }
		public BindableCollection<RegionViewModel> OverlapRegions { get; private set; }

		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;
	}


	public class RegionViewModel
	{
		public ROI Region { get; set; }

		public string Title { get { return Region.Name; } }
		public string Index { get { return Region.Index.ToString(); } }

		public bool IsOverlap { get; set; }
	}
}
