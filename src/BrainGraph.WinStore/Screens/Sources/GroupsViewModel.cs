using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using OxyPlot;
using System;
using System.Collections.Generic;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class GroupsViewModel : ViewModelBase, IMenuItem, IHandle<SubjectsFilteredEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private IRegionService _regionService;
		private ISubjectDataService _subjectService;
		private ISubjectFilterService _subjectFilterService;
		#endregion

		private SubjectGroupViewModel _sgGroup1;
		private SubjectGroupViewModel _sgGroup2;
		private SubjectGroupViewModel _sgRemaining;

		public GroupsViewModel()
		{
			Title = "Group Details";
			PrimaryValue = "0";
			Subtitle = "Background group metrics.";
			
            DataItems = new BindableCollection<DataItemViewModel>();

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_regionService = IoC.Get<IRegionService>();
				_subjectService = IoC.Get<ISubjectDataService>();
				_subjectFilterService = IoC.Get<ISubjectFilterService>();

				_eventAggregator.Subscribe(this);
			}

			#region Sample Data
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1000", Age = "20", GroupId = "p", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1001", Age = "21", GroupId = "p", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1002", Age = "22", GroupId = "p", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1003", Age = "23", GroupId = "p", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1004", Age = "24", GroupId = "p", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1005", Age = "25", GroupId = "p", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1006", Age = "26", GroupId = "p", Sex = "m" });

				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1007", Age = "20", GroupId = "c", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1008", Age = "21", GroupId = "c", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1009", Age = "22", GroupId = "c", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1010", Age = "23", GroupId = "c", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1011", Age = "24", GroupId = "c", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1012", Age = "25", GroupId = "c", Sex = "m" });
				//_sgRemaining.Subjects.Add(new Subject { SubjectId = "1013", Age = "26", GroupId = "c", Sex = "m" });

			}
			#endregion
		}

        public DataItemViewModel ProcessGroup(Dictionary<string,bool> dts, string groupName, List<BrainGraph.ComputeRT.Subjects.SubjectViewModel> subjects)
        {
            var divm = new DataItemViewModel();
            divm.GroupName = groupName;
			divm.TypeAverages = new Dictionary<string, double[,]>();
                        
            foreach (var dt in dts)
            {
				double[,] groupAverage = new double[90, 90];

                foreach(var sub in subjects)
                {
                    if (dt.Value)
                    {
                        var graph = sub.Graphs[dt.Key];

                        foreach (var edge in graph.Edges)
                        {
							if (edge.NodeOneIndex <= edge.NodeTwoIndex)
							{
								groupAverage[89 - edge.NodeOneIndex, edge.NodeTwoIndex] += edge.Value;
								groupAverage[89 - edge.NodeTwoIndex, edge.NodeOneIndex] += edge.Value;
							}
                        }
                    }
                }

				for (var i = 0; i < 90; i++ )
				{
					for(var j = 0; j < 90; j++)
					{
						groupAverage[i, j] = groupAverage[i, j] / subjects.Count;
					}
				}

				divm.TypeAverages[dt.Key] = groupAverage;
            }

			divm.DTIPlotModel.Axes.Add(new OxyPlot.Axes.ColorAxis { Position = OxyPlot.Axes.AxisPosition.Right, Palette = OxyPalettes.Jet(200), HighColor = OxyColors.Gray, LowColor = OxyColors.Black });
			divm.DTIPlotModel.Series.Add(new OxyPlot.Series.HeatMapSeries { X0 = 0, X1 = 90, Y0 = 0, Y1 = 90, Data = divm.TypeAverages["DTI-adj"], Interpolate = false });

			divm.FMRIPlotModel.Axes.Add(new OxyPlot.Axes.ColorAxis { Position = OxyPlot.Axes.AxisPosition.Right, Palette = OxyPalettes.Jet(100), HighColor = OxyColors.Gray, LowColor = OxyColors.Black });
			divm.FMRIPlotModel.Series.Add(new OxyPlot.Series.HeatMapSeries { X0 = 0, X1 = 90, Y0 = 0, Y1 = 90, Data = divm.TypeAverages["fMRI-mo"], Interpolate = false });

			return divm;
        }

		public void SetupCharts()
		{

		}

		public void Handle(SubjectsFilteredEvent message)
		{
			// TODO: Make this update async after 300msec

            var dts = _subjectFilterService.GetDataTypeSettings();

            DataItems.Add(ProcessGroup(dts, "Group 1", _subjectFilterService.GetGroup1()));
            DataItems.Add(ProcessGroup(dts, "Group 2", _subjectFilterService.GetGroup2()));

            PrimaryValue = DataItems.Count.ToString();
		}

		public override Type ViewModelType { get { return typeof(GroupsViewModel); } }
        public BindableCollection<DataItemViewModel> DataItems { get { return _inlDataItems; } set { _inlDataItems = value; NotifyOfPropertyChange(() => DataItems); } } private BindableCollection<DataItemViewModel> _inlDataItems;

        public class DataItemViewModel : Screen
        {
			public DataItemViewModel()
			{
				DTIPlotModel = new PlotModel();
				FMRIPlotModel = new PlotModel();
			}

            public string GroupName { get; set; }
            public Dictionary<string, double[,]> TypeAverages { get; set; }

			public PlotModel DTIPlotModel { get { return _inlDTIPlotModel; } set { _inlDTIPlotModel = value; NotifyOfPropertyChange(() => DTIPlotModel); } } private PlotModel _inlDTIPlotModel;
			public PlotModel FMRIPlotModel { get { return _inlFMRIPlotModel; } set { _inlFMRIPlotModel = value; NotifyOfPropertyChange(() => FMRIPlotModel); } } private PlotModel _inlFMRIPlotModel;

            public PlotModel AvgEdgePlotModel { get { return _inlAvgEdgePlotModel; } set { _inlAvgEdgePlotModel = value; NotifyOfPropertyChange(() => AvgEdgePlotModel); } } private PlotModel _inlAvgEdgePlotModel;
        }
	}
}

