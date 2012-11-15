using BrainGraph.Compute.Subjects;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Screens.Selection;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class SubjectViewModel : ViewModelBase, IMenuItem, IHandle<SubjectsLoadedEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private ISubjectService _subjectService;
		#endregion

		private SubjectGroupViewModel _sgGroup1;
		private SubjectGroupViewModel _sgGroup2;
		private SubjectGroupViewModel _sgRemaining;

		public SubjectViewModel()
		{
			_sgGroup1 = new SubjectGroupViewModel { Title = "Group 1" };
			_sgGroup2 = new SubjectGroupViewModel { Title = "Group 2" };
			_sgRemaining = new SubjectGroupViewModel { Title = "Unselected" };
			SubjectGroups = new BindableCollection<SubjectGroupViewModel>() { _sgGroup1, _sgGroup2, _sgRemaining };

			Title = "Subjects";
			PrimaryValue = "";
			Subtitle = "List of people who have participated in this study.";

			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				#region Sample Data
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1000", Age = "20", GroupId = "p", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1001", Age = "21", GroupId = "p", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1002", Age = "22", GroupId = "p", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1003", Age = "23", GroupId = "p", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1004", Age = "24", GroupId = "p", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1005", Age = "25", GroupId = "p", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1006", Age = "26", GroupId = "p", Sex = "m" });

				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1007", Age = "20", GroupId = "c", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1008", Age = "21", GroupId = "c", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1009", Age = "22", GroupId = "c", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1010", Age = "23", GroupId = "c", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1011", Age = "24", GroupId = "c", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1012", Age = "25", GroupId = "c", Sex = "m" });
				_sgRemaining.Subjects.Add(new Subject { SubjectId = "1013", Age = "26", GroupId = "c", Sex = "m" });
				#endregion
			}
			else
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_subjectService = IoC.Get<ISubjectService>();
				_eventAggregator.Subscribe(this);

				// TODO: Generate these from the subject loading and user prefs

				DataTypes = new BindableCollection<DataTypeViewModel>();
				DataTypes.Add(new DataTypeViewModel (this) { Title = "DTI", Threshold = "2.15" });
				DataTypes.Add(new DataTypeViewModel (this) { Title = "fMRI", Threshold = "5.225" });

				Groups = new BindableCollection<GroupViewModel>();
				Groups.Add(new GroupViewModel (this) { StudyGroup = "p" });
				Groups.Add(new GroupViewModel (this) { StudyGroup = "c" });
			}
		}

		public async void SetSource(RoutedEventArgs eventArg)
		{
			FileOpenPicker adjPicker = new FileOpenPicker();
			adjPicker.ViewMode = PickerViewMode.List;
			adjPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			adjPicker.FileTypeFilter.Add(".txt");

			StorageFile adjFile = await adjPicker.PickSingleFileAsync();
			if (adjFile != null)
			{
				// TODO: _subjectService.Clear();
				await _subjectService.LoadSubjects(adjFile);  // TODO: Pull the ROI count from somewhere else
			}
		}

		public void SetGroups(RoutedEventArgs eventArgs)
		{
			var sender = (UIElement)eventArgs.OriginalSource;

			var popup = new GroupsPopup();
			popup.DataContext = this;

			// Flyout is a ContentControl so set your content within it.
			Flyout f = new Flyout();
			f.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
			f.Content = popup;
			f.Placement = PlacementMode.Top;
			f.PlacementTarget = sender;

			f.IsOpen = true;
		}

		public void SetData(RoutedEventArgs eventArgs)
		{
			var sender = (UIElement)eventArgs.OriginalSource;

			var popup = new DataTypesPopup();
			popup.DataContext = this;

			// Flyout is a ContentControl so set your content within it.
			Flyout f = new Flyout();
			f.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
			f.Content = popup;
			f.Placement = PlacementMode.Top;
			f.PlacementTarget = sender;

			f.IsOpen = true;
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Subtitle { get { return _inlSubtitle; } set { _inlSubtitle = value; NotifyOfPropertyChange(() => Subtitle); } } private string _inlSubtitle;
		public string Description { get { return _inlDescription; } set { _inlDescription = value; NotifyOfPropertyChange(() => Description); } } private string _inlDescription;
		public string PrimaryValue { get { return _inlPrimaryValue; } set { _inlPrimaryValue = value; NotifyOfPropertyChange(() => PrimaryValue); } } private string _inlPrimaryValue;

		public Type ViewModelType { get { return typeof(SubjectViewModel); } }
		public Type PopupType { get { return null; } }

		protected override void OnActivate()
		{
			base.OnActivate();

			UpdateSubjectGroups();

			#region Picker Stuff
			//var subjectService = IoC.Get<ISubjectService>();

			//FileOpenPicker openPicker = new FileOpenPicker();
			//openPicker.ViewMode = PickerViewMode.List;
			////openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			//openPicker.FileTypeFilter.Add(".txt");

			//StorageFile file = await openPicker.PickSingleFileAsync();

			//var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

			//// acquire file
			//var file = await folder.GetFileAsync(@"Assets\Subjects\VA2929.txt");

			//if (file != null)
			//{
			//	List<Subject> subs = await subjectService.LoadSubjectFile(file);
			//	foreach (var sub in subs)
			//	{
			//		Subjects.Add(sub);
			//	}
			//}

			//FolderPicker adjPicker = new FolderPicker();
			//adjPicker.ViewMode = PickerViewMode.List;
			//adjPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

			//adjPicker.FileTypeFilter.Add(".txt");

			//StorageFolder adjFolder = await adjPicker.PickSingleFolderAsync();

			//StorageFolder adjFolder = await KnownFolders.DocumentsLibrary.GetFolderAsync(@"Studies\SZ_169\NBSm\adj");

			//if (adjFolder != null)
			//{
			//	await subjectService.LoadSubjectData(adjFolder, 90);
			//}
			#endregion
		}

		public void Handle(SubjectsLoadedEvent message)
		{
			var itms = _subjectService.GetSubjects().Count;
			this.PrimaryValue = itms.ToString();
		}

		private Dictionary<string, string> _groupLookup = new Dictionary<string, string>();
		public void AssignGroup(string subjectGroup, string experimentGroup)
		{
			_groupLookup[subjectGroup] = experimentGroup;

			// TODO: Save settings
		}

		public void UpdateSubjectGroups()
		{
			_sgGroup1.Subjects.Clear();
			_sgGroup2.Subjects.Clear();
			_sgRemaining.Subjects.Clear();

			var subjects = _subjectService.GetSubjects();

			foreach (var sub in subjects)
			{
				// Filter by active data types
				bool hasData = true;
				foreach (var dt in DataTypes)
				{
					if (dt.IsIncluded && !sub.Graphs.ContainsKey(dt.Title))
						hasData = false;
				}

				if (hasData)
				{
					// Bucket by groupid
					var groupId = sub.GroupId.ToLower();

					if (_groupLookup.ContainsKey(groupId) && _groupLookup[groupId] == "Group 1")
						_sgGroup1.Subjects.Add(sub);
					else if (_groupLookup.ContainsKey(groupId) && _groupLookup[groupId] == "Group 2")
						_sgGroup2.Subjects.Add(sub);
					else
						_sgRemaining.Subjects.Add(sub);
				}
				else
					_sgRemaining.Subjects.Add(sub);
			}
		}

		public BindableCollection<GroupViewModel> Groups { get { return _inlGroups; } set { _inlGroups = value; NotifyOfPropertyChange(() => Groups); } } private BindableCollection<GroupViewModel> _inlGroups;
		public BindableCollection<DataTypeViewModel> DataTypes { get { return _inlDataTypes; } set { _inlDataTypes = value; NotifyOfPropertyChange(() => DataTypes); } } private BindableCollection<DataTypeViewModel> _inlDataTypes;

		public BindableCollection<SubjectGroupViewModel> SubjectGroups { get { return _inlSubjects; } set { _inlSubjects = value; NotifyOfPropertyChange(() => SubjectGroups); } } private BindableCollection<SubjectGroupViewModel> _inlSubjects;
	}

	public class SubjectGroupViewModel : Screen
	{
		public SubjectGroupViewModel()
		{
			Subjects = new BindableCollection<Subject>();
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public BindableCollection<Subject> Subjects { get { return _inlSubjects; } set { _inlSubjects = value; NotifyOfPropertyChange(() => Subjects); } } private BindableCollection<Subject> _inlSubjects;
	}

	public class DataTypeViewModel : Screen
	{
		private SubjectViewModel _svm;

		public DataTypeViewModel(SubjectViewModel svm)
		{
			_svm = svm;
			_inlIsIncluded = false;
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Threshold { get { return _inlThreshold; } set { _inlThreshold = value; NotifyOfPropertyChange(() => Threshold); } } private string _inlThreshold;

		public bool IsIncluded { 
			get { return _inlIsIncluded; } 
			set { 
				_inlIsIncluded = value; 
				
				_svm.UpdateSubjectGroups();

				NotifyOfPropertyChange(() => IsIncluded);
			} 
		} private bool _inlIsIncluded;
	}

	public class GroupViewModel : Screen
	{
		private SubjectViewModel _svm;

		public GroupViewModel(SubjectViewModel svm)
		{
			_svm = svm;
			_inlExperimentGroup = "None";
		}

		public string StudyGroup { get { return _inlStudyGroup; } set { _inlStudyGroup = value; NotifyOfPropertyChange(() => StudyGroup); } } private string _inlStudyGroup;

		public string[] ExperimentGroups { get { return new string[] { "None", "Group 1", "Group 2" }; } }
		
		public string ExperimentGroup { 
			get { return _inlExperimentGroup; } 
			set { 
				_inlExperimentGroup = value; 
				
				// TODO: Async these
				_svm.AssignGroup(StudyGroup, _inlExperimentGroup);
				_svm.UpdateSubjectGroups();

				NotifyOfPropertyChange(() => ExperimentGroup);
			} 
		} private string _inlExperimentGroup;
	}
}
