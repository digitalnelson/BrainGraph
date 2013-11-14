using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using OxyPlot;
using System;

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

		public void Handle(SubjectsFilteredEvent message)
		{
			// TODO: Make this update async after 300msec

            var dts = _subjectFilterService.GetDataTypeSettings();
            var g1 = _subjectFilterService.GetGroup1();

            var divm = new DataItemViewModel();
            divm.GroupName = "Group 1";

            foreach(var sub in g1)
            {
                foreach (var dt in dts)
                {
                    if (dt.Value)
                    {
                        var graph = sub.Graphs[dt.Key];

                        foreach (var edge in graph.Edges)
                        {
                            
                        }
                    }
                }
            }

            DataItems.Add(divm);

            var g2 = _subjectFilterService.GetGroup1();

            PrimaryValue = DataItems.Count.ToString();
		}

		public override Type ViewModelType { get { return typeof(GroupsViewModel); } }
        public BindableCollection<DataItemViewModel> DataItems { get { return _inlDataItems; } set { _inlDataItems = value; NotifyOfPropertyChange(() => DataItems); } } private BindableCollection<DataItemViewModel> _inlDataItems;

        public class DataItemViewModel : Screen
        {
            public string GroupName { get; set; }

            public PlotModel AvgEdgePlotModel { get { return _inlAvgEdgePlotModel; } set { _inlAvgEdgePlotModel = value; NotifyOfPropertyChange(() => AvgEdgePlotModel); } } private PlotModel _inlAvgEdgePlotModel;
        }
	}
}

