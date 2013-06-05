using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Screens;
using BrainGraph.WinStore.Screens.Component;
using BrainGraph.WinStore.Screens.Config;
using BrainGraph.WinStore.Screens.Edge;
using BrainGraph.WinStore.Screens.Experiment;
using BrainGraph.WinStore.Screens.Global;
using BrainGraph.WinStore.Screens.Nodal;
using BrainGraph.WinStore.Screens.Sources;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

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

		private RunExperimentViewModel _running;
		private RunThresholdTestViewModel _runThresholdTest;
		private PermutationViewModel _permutations;
		private NBSmConfigViewModel _nbsmConfig;

		public MainMenuViewModel()
		{
			Groups = new BindableCollection<MenuGroup>();

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_running = IoC.Get<RunExperimentViewModel>();
				_runThresholdTest = IoC.Get<RunThresholdTestViewModel>();
				_permutations = IoC.Get<PermutationViewModel>();
				_nbsmConfig = IoC.Get<NBSmConfigViewModel>();

				_eventAggregator = IoC.Get<IEventAggregator>();
				_navService = IoC.Get<INavigationService>();
				_regionService = IoC.Get<IRegionService>();
				_subjectService = IoC.Get<ISubjectDataService>();
				_subjectFilterService = IoC.Get<ISubjectFilterService>();
				_computeService = IoC.Get<IComputeService>();

				var regionsVM = IoC.Get<RegionsViewModel>();

				Groups.Add(new MenuGroup { Title = "Source", Items = { regionsVM, IoC.Get<SubjectsViewModel>() } });
				Groups.Add(new MenuGroup { Title = "Config", Items = { _permutations, _nbsmConfig } });
				Groups.Add(new MenuGroup { Title = "Compute", Items = { _runThresholdTest, _running } });
				Groups.Add(new MenuGroup { Title = "Global", Items = { IoC.Get<GlobalStrengthViewModel>() } });
				Groups.Add(new MenuGroup { Title = "Component", Items = { IoC.Get<IntermodalViewModel>(), IoC.Get<IntraSummaryViewModel>()/*, new MenuItem { Title = "Associations" },*/ } });
				Groups.Add(new MenuGroup { Title = "Nodal", Items = { IoC.Get<NodalStrengthDataTypeViewModel>() } });
				Groups.Add(new MenuGroup { Title = "Edge", Items = { IoC.Get<EdgeSignificanceViewModel>() } });
			}
		}

		public async void MenuItemSelected(ItemClickEventArgs eventArgs)
		{
			var menuItem = (IMenuItem)eventArgs.ClickedItem;
			var popupType = menuItem.PopupType;

			if (menuItem is RunThresholdTestViewModel)
			{
				//int permutations = Int32.Parse(_permutations.Permutations);

				//if (permutations > 0)
				//{
				//	for (int thresh = 100; thresh < 800; thresh += 5)
				//	{
				//		var dtItms = _subjectFilterService.GetDataTypeSettings();

				//		double dThresh = ((double)thresh / (double)100);

				//		List<Threshold> dataTypes = new List<Threshold>();
				//		foreach (var itm in dtItms)
				//		{
				//			if (itm.Value)
				//			{
				//				Threshold t = new Threshold()
				//				{
				//					DataType = itm.Key,
				//					Value = dThresh
				//				};

				//				dataTypes.Add(t);
				//			}
				//		}

				//		// Load the subjects into the compute service
				//		_computeService.LoadSubjects(_regionService.GetNodeCount(), _regionService.GetEdgeCount(), dataTypes, _subjectFilterService.GetGroup1(), _subjectFilterService.GetGroup2());

				//		// Compare groups based on real labels
				//		_computeService.CompareGroups();

				//		// Create our async permutation computation to figure out p values
				//		var permutation = _computeService.PermuteGroupsAsync(permutations);

				//		// Handle progress reporting
				//		permutation.Progress += new Windows.Foundation.AsyncActionProgressHandler<int>((_, p) =>
				//		{
				//			_runThresholdTest.PrimaryValue = p.ToString();
				//		});

				//		// Run the thingy and fix the display when it finishes
				//		await permutation.AsTask();

				//		_runThresholdTest.PrimaryValue = thresh.ToString();
				//		//_eventAggregator.Publish(new PermutationCompleteEvent());

				//		var result = _computeService.GetResults();

				//		Debug.WriteLine("Threshold: " + dThresh.ToString("0.00"));
				//		foreach (var graph in result.Graphs)
				//		{
				//			foreach (var component in graph.Components)
				//			{
				//				if(component.Edges.Count > 0)
				//					Debug.WriteLine("Item: " + graph.Name + " Nodes: " + component.NodeCount.ToString() + " Edges: " + component.Edges.Count.ToString());
				//			}
				//		}
				//	}
				//}
			}
			else if (menuItem is RunExperimentViewModel)
			{
				int permutations = Int32.Parse(_permutations.Permutations);

				if (permutations > 0)
				{
					// Get our active data types
					var dataTypes = _subjectFilterService.GetDataTypeSettings().Where(itm => itm.Value == true).Select(itm => itm.Key).ToList();

					// Load the subjects into the compute service
					_computeService.LoadSubjects(_regionService.GetNodeCount(), _regionService.GetEdgeCount(), dataTypes, _subjectFilterService.GetGroup1(), _subjectFilterService.GetGroup2());

					// Compare groups based on real labels
					_computeService.CompareGroups();

					// Create our async permutation computation to figure out p values
					var permutation = _computeService.PermuteGroupsAsync(permutations);

					// Handle progress reporting
					permutation.Progress += new Windows.Foundation.AsyncActionProgressHandler<int>((_, p) =>
					{
						_running.PrimaryValue = p.ToString();
					});

					// Run the thingy and fix the display when it finishes
					await permutation.AsTask();

					_running.PrimaryValue = permutations.ToString();
					_eventAggregator.Publish(new PermutationCompleteEvent());
				}
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

	public class MainMenuDesignData : Screen
	{
		public MainMenuDesignData()
		{
			Groups = new BindableCollection<MenuGroup>();
			Groups.Add(new MenuGroup { Title = "One", Items = { new MenuItem { Title = "First Item" }, new MenuItem { Title = "Second Item" }, new MenuItem { Title = "Third Item" } } });
			Groups.Add(new MenuGroup { Title = "Two", Items = { new MenuItem { Title = "Fourth Item" }, new MenuItem { Title = "Fifth Item" }, new MenuItem { Title = "Sixth Item" } } });
		}

		public BindableCollection<MenuGroup> Groups { get { return _inlGroups; } set { _inlGroups = value; NotifyOfPropertyChange(() => Groups); } } private BindableCollection<MenuGroup> _inlGroups;
	}
}
