using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using BrainGraph.WinStore.Screens;
using BrainGraph.WinStore.Screens.Selection;
using BrainGraph.WinStore.Screens.Sources;
using Windows.Storage.Pickers;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Screens.Experiment;
using BrainGraph.Compute.Graph;

namespace BrainGraph.WinStore
{
	public class MainMenuViewModel : ViewModelBase
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private INavigationService _navService;
		private IRegionService _regionService;
		private ISubjectDataService _subjectService;
		private ISubjectFilterService _subjectFilterService;
		private IComputeService _computeService;
		#endregion

		public MainMenuViewModel()
		{
			Groups = new BindableCollection<MenuGroup>();

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_navService = IoC.Get<INavigationService>();
				_regionService = IoC.Get<IRegionService>();
				_subjectService = IoC.Get<ISubjectDataService>();
				_subjectFilterService = IoC.Get<ISubjectFilterService>();
				_computeService = IoC.Get<IComputeService>();

				Groups.Add(new MenuGroup { Title = "Source", Items = { IoC.Get<RegionsViewModel>(), IoC.Get<SubjectsViewModel>() } });
				Groups.Add(new MenuGroup { Title = "Experiment", Items = { new PermutationViewModel(), new RunExperimentViewModel() } });
				Groups.Add(new MenuGroup { Title = "Measures", Items = { new MenuItem { Title = "Strength" }, new MenuItem { Title = "Diversity" }, new MenuItem { Title = "Clustering" }, new MenuItem { Title = "Modularity" }, } });
				Groups.Add(new MenuGroup { Title = "NBSm", Items = { new MenuItem { Title = "Intermodal" }, new MenuItem { Title = "By Type" }, } });
			}
			//else
			//{
			//	Groups.Add(new MenuGroup { Title = "Source", Items = { new RegionsViewModel(), new SubjectsViewModel(), new PermutationViewModel() } });
			//	Groups.Add(new MenuGroup { Title = "Measures", Items = { new MenuItem { Title = "Strength" }, new MenuItem { Title = "Diversity" }, new MenuItem { Title = "Clustering" }, new MenuItem { Title = "Modularity" }, } });
			//	Groups.Add(new MenuGroup { Title = "NBSm", Items = { new MenuItem { Title = "Intermodal" }, new MenuItem { Title = "By Type" }, } });
			//}
		}

		public async void SetWorkingFolder(RoutedEventArgs eventArgs)
		{
			FolderPicker wkPicker = new FolderPicker();
			wkPicker.ViewMode = PickerViewMode.List;
			wkPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			wkPicker.FileTypeFilter.Add(".txt");

			var folder = await wkPicker.PickSingleFolderAsync();
			if (folder != null)
			{
				Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
				roamingSettings.Values["WorkingFolder"] = folder.Path;

				_eventAggregator.Publish(new SettingWorkingFolderUpdated());
			}
		}

		public void MenuItemSelected(ItemClickEventArgs eventArgs)
		{
			var menuItem = (IMenuItem)eventArgs.ClickedItem;
			var popupType = menuItem.PopupType;

			if (menuItem is RunExperimentViewModel)
			{
				var dtItms = _subjectFilterService.GetDataTypeSettings();

				List<Threshold> dataTypes = new List<Threshold>();
				foreach (var itm in dtItms)
				{
					if (itm.Value)
					{
						Threshold t = new Threshold()
						{
							DataType = itm.Key,
							Value = 2.1
						};

						if (t.DataType == "fMRI-mo")
							t.Value = 5.0;

						dataTypes.Add(t);
					}
				}

				_computeService.LoadSubjects(_regionService.GetNodeCount(), _regionService.GetEdgeCount(), dataTypes, _subjectFilterService.GetGroup1(), _subjectFilterService.GetGroup2());
				_computeService.CompareGroups();
				_computeService.PermuteGroups(5000);
			}
			else if (popupType != null)
			{
				var sender = (GridView)eventArgs.OriginalSource;
				var clickedUI = sender.ItemContainerGenerator.ContainerFromItem(eventArgs.ClickedItem);
				
				var popup = (UserControl)Activator.CreateInstance(popupType);
				popup.DataContext = menuItem;

				// Flyout is a ContentControl so set your content within it.
				Callisto.Controls.Flyout f = new Callisto.Controls.Flyout();
				f.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
				f.Content = popup;
				f.Placement = PlacementMode.Top;

				if (clickedUI is GridViewItem)
					f.PlacementTarget = clickedUI as UIElement;
				else
					f.PlacementTarget = sender as UIElement;

				f.IsOpen = true;
			}
			else
			{
				if (menuItem.ViewModelType != null)
					_navService.NavigateToViewModel(menuItem.ViewModelType);
			}
		}

		public BindableCollection<MenuGroup> Groups { get { return _inlGroups; } set { _inlGroups = value; NotifyOfPropertyChange(() => Groups); } } private BindableCollection<MenuGroup> _inlGroups;
	}

	public class MenuGroup : Screen
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public BindableCollection<IMenuItem> Items { get { return _inlItems; } set { _inlItems = value; NotifyOfPropertyChange(() => Items); } } private BindableCollection<IMenuItem> _inlItems = new BindableCollection<IMenuItem>();
	}
}
