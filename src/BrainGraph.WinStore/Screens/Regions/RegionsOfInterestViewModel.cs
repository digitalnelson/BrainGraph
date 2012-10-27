using BrainGraph.Storage;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Common.Viz;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Regions
{
	public class RegionsOfInterestViewModel : ViewModelBase
	{
		private INavigationService _navService = IoC.Get<INavigationService>();
		private IRegionService _regionService = IoC.Get<IRegionService>();

		public RegionsOfInterestViewModel()
		{
			Regions = new BindableCollection<RegionViewModel>();
		}

		protected override async void OnActivate()
		{
			if (RegionFile == null)
			{
				await Task.Run(delegate
				{
					var defaultPath = @"Assets\Atlases\AAL.txt";
					if (RegionFile != defaultPath)
						RegionFile = defaultPath;
				});
			}

			base.OnActivate();
		}

		private List<RegionalViewModel> _rvms;

		public async void LoadRegions()
		{
			Regions.Clear();

			try
			{
				await _regionService.Load(RegionFile);

				_rvms = new List<RegionalViewModel>();

				var regions = _regionService.GetRegionsByIndex();
				foreach (var region in regions)
				{
					var rgn = IoC.Get<RegionViewModel>();
					rgn.Region = region;

					Regions.Add(rgn);

					RegionalViewModel rvm = new RegionalViewModel
					{
						ROI = region,
					};

					_rvms.Add(rvm);
				}

				AXPlotModel = LoadPlotModel(_rvms, r => r.X, r => r.Y);
				SGPlotModel = LoadPlotModel(_rvms, r => (100 - r.Y), r => r.Z);
				CRPlotModel = LoadPlotModel(_rvms, r => r.X, r => r.Z);
			}
			catch (Exception)
			{
			}
		}

		protected class RegionalViewModel
		{
			public ROI ROI { get; set; }
		}

		protected PlotModel LoadPlotModel(List<RegionalViewModel> rsvms, Func<ROI, double> horizSelector, Func<ROI, double> vertSelector)
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

			foreach (var rsvm in rsvms)
			{
				s1.Points.Add(new BrainDataPoint(horizSelector(rsvm.ROI), vertSelector(rsvm.ROI), rsvm.ROI));
			}

			model.Series.Add(s1);

			return model;
		}

		public string RegionFile { get { return _regionFile; } set { _regionFile = value; NotifyOfPropertyChange(() => RegionFile); LoadRegions(); } } private string _regionFile;

		public BindableCollection<RegionViewModel> Regions { get; private set; }

		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;
	}
}
