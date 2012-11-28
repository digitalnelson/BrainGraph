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
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class SubjectsViewModel : ViewModelBase, IMenuItem, IHandle<RegionsLoadedEvent>, IHandle<SubjectsFilteredEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private IRegionService _regionService;
		private ISubjectDataService _subjectService;
		private ISubjectFilterService _subjectFilterService;
		#endregion

		private const string SETTING_SUBJECT_FILE_TOKEN = "SubjectFileToken";
		private const string SETTING_SUBJECT_DATA_FOLDER_TOKEN = "SubjectDataFolderToken";
		private const string SETTING_SUBJECT_GROUPS = "SubjectGroups";
		private const string SETTING_SELECTED_DATATYPES = "SelectedDataTypes";

		private SubjectGroupViewModel _sgGroup1;
		private SubjectGroupViewModel _sgGroup2;
		private SubjectGroupViewModel _sgRemaining;

		public SubjectsViewModel()
		{
			Title = "Subjects";
			PrimaryValue = "0";
			Subtitle = "List of people who have participated in this study.";
			
			_sgGroup1 = new SubjectGroupViewModel { Title = "Group 1" };
			_sgGroup2 = new SubjectGroupViewModel { Title = "Group 2" };
			_sgRemaining = new SubjectGroupViewModel { Title = "Unselected" };
			SubjectGroups = new BindableCollection<SubjectGroupViewModel>() { _sgGroup1, _sgGroup2, _sgRemaining };

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

		public void Handle(RegionsLoadedEvent message)
		{
			IsReady = true;
			
			Task.Factory.StartNew(async () =>
			{
				await OpenFileFromCache();
				await OpenFolderFromCache();

			}, new System.Threading.CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
		}

		public async Task OpenFileFromCache()
		{
			Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
			if (roamingSettings.Values.ContainsKey(SETTING_SUBJECT_FILE_TOKEN))
			{
				var token = roamingSettings.Values[SETTING_SUBJECT_FILE_TOKEN] as string;
				StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
				
				await LoadSubjectsFile(file);
			}
		}

		public async Task OpenFileFromPicker(RoutedEventArgs args)
		{
			FileOpenPicker picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.List;
			picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			picker.FileTypeFilter.Add(".txt");

			var file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				var token = StorageApplicationPermissions.FutureAccessList.Add(file);

				Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
				roamingSettings.Values[SETTING_SUBJECT_FILE_TOKEN] = token;

				await LoadSubjectsFile(file);

				// Try to pull the data for the subjects based on the cached data folder
				// It's ok if it fails miserably.
				await OpenFolderFromCache();
			}
		}

		public async Task LoadSubjectsFile(StorageFile file)
		{
			Groups.Clear();
			DataTypes.Clear();

			_sgRemaining.Subjects.Clear();
			_sgGroup1.Subjects.Clear();
			_sgGroup2.Subjects.Clear();

			_subjectFilterService.Clear();
			//_subjectService.Clear();

			var subjects = await _subjectService.LoadSubjects(file);

			foreach (var subject in subjects)
			{
				var subViewModel = new SubjectViewModel { Subject = subject };

				_sgRemaining.Subjects.Add(subViewModel);
			}

			this.PrimaryValue = subjects.Count.ToString();
		}

		public async Task OpenFolderFromCache()
		{
			Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
			if (roamingSettings.Values.ContainsKey(SETTING_SUBJECT_DATA_FOLDER_TOKEN))
			{
				var token = roamingSettings.Values[SETTING_SUBJECT_DATA_FOLDER_TOKEN] as string;
				StorageFolder folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
				
				await LoadSubjectsFolder(folder);
			}
		}

		public async Task OpenFolderFromPicker(RoutedEventArgs args)
		{
			FolderPicker picker = new FolderPicker();
			picker.ViewMode = PickerViewMode.List;
			picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			picker.FileTypeFilter.Add(".txt");

			var folder = await picker.PickSingleFolderAsync();
			if (folder != null)
			{
				var token = StorageApplicationPermissions.FutureAccessList.Add(folder);

				Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
				roamingSettings.Values[SETTING_SUBJECT_DATA_FOLDER_TOKEN] = token;

				await LoadSubjectsFolder(folder);
			}
		}

		public async Task LoadSubjectsFolder(StorageFolder folder)
		{
			if (_sgRemaining.Subjects.Count > 0)
			{
				await _subjectService.IndexDataFolder(folder, _regionService.GetNodeCount());

				foreach (var subViewModel in _sgRemaining.Subjects)
				{
					// TODO: What happens if data can't be loaded?

					// Make sure the subjects data is loaded
					await _subjectService.LoadSubjectData(subViewModel.Subject, _regionService.GetNodeCount());

					// Add our subject so it can be included (or excluded) by the filters
					_subjectFilterService.AddSubject(subViewModel.Subject);

					// Allow UI to update
					subViewModel.Refresh();
				}

				Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
				
				// Load groups preferences
				if (roamingSettings.Values.ContainsKey(SETTING_SUBJECT_GROUPS))
				{
					var groups = roamingSettings.Values[SETTING_SUBJECT_GROUPS] as Windows.Storage.ApplicationDataCompositeValue;
					foreach (var group in groups)
						_subjectFilterService.AssignGroup(group.Key, (ComputeGroup)Enum.Parse(typeof(ComputeGroup), group.Value as string));
				}

				var subGroups = _subjectFilterService.GetGroupSettings();
				foreach (var group in subGroups)
					Groups.Add(new GroupViewModel(this, group.Key, group.Value));

				// Load data type preferences
				if (roamingSettings.Values.ContainsKey(SETTING_SELECTED_DATATYPES))
				{
					var dataTypes = roamingSettings.Values[SETTING_SELECTED_DATATYPES] as Windows.Storage.ApplicationDataCompositeValue;
					foreach (var dataType in dataTypes)
						_subjectFilterService.AssignDataType(dataType.Key, (bool)dataType.Value);
				}

				var subDataTypes = _subjectFilterService.GetDataTypeSettings();
				foreach (var dataType in subDataTypes)
					DataTypes.Add(new DataTypeViewModel(this, dataType.Key, dataType.Value));

				_eventAggregator.Publish(new SubjectsLoadedEvent());

				_subjectFilterService.FilterSubjects();
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

		public void Handle(SubjectsFilteredEvent message)
		{
			// TODO: Make this update async after 300msec

			_sgRemaining.Subjects.Clear();
			_sgGroup1.Subjects.Clear();
			_sgGroup2.Subjects.Clear();

			foreach (var subject in _subjectFilterService.GetGroup1())
				_sgGroup1.Subjects.Add(new SubjectViewModel { Subject = subject });

			foreach (var subject in _subjectFilterService.GetGroup2())
				_sgGroup2.Subjects.Add(new SubjectViewModel { Subject = subject });

			foreach (var subject in _subjectFilterService.GetRemaining())
				_sgRemaining.Subjects.Add(new SubjectViewModel { Subject = subject });

			Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

			var grpSettings = _subjectFilterService.GetGroupSettings();
			var dtSettings = _subjectFilterService.GetDataTypeSettings();

			var groups = new Windows.Storage.ApplicationDataCompositeValue();
			
			foreach (var group in grpSettings)
				groups[group.Key] = group.Value.ToString();

			roamingSettings.Values[SETTING_SUBJECT_GROUPS] = groups;

			var dataTypes = new Windows.Storage.ApplicationDataCompositeValue();
			
			foreach (var dataType in dtSettings)
				dataTypes[dataType.Key] = dataType.Value;
			
			roamingSettings.Values[SETTING_SELECTED_DATATYPES] = dataTypes;
		}

		public override Type ViewModelType { get { return typeof(SubjectsViewModel); } }

		public BindableCollection<SubjectGroupViewModel> SubjectGroups { get { return _inlSubjects; } set { _inlSubjects = value; NotifyOfPropertyChange(() => SubjectGroups); } } private BindableCollection<SubjectGroupViewModel> _inlSubjects;

		public BindableCollection<GroupViewModel> Groups { get { return _inlGroups; } set { _inlGroups = value; NotifyOfPropertyChange(() => Groups); } } private BindableCollection<GroupViewModel> _inlGroups = new BindableCollection<GroupViewModel>();
		public BindableCollection<DataTypeViewModel> DataTypes { get { return _inlDataTypes; } set { _inlDataTypes = value; NotifyOfPropertyChange(() => DataTypes); } } private BindableCollection<DataTypeViewModel> _inlDataTypes = new BindableCollection<DataTypeViewModel>();
	}
}
