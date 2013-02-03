using BrainGraph.Compute.Graph;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Screens;
using BrainGraph.WinStore.Screens.Edge;
using BrainGraph.WinStore.Screens.Experiment;
using BrainGraph.WinStore.Screens.Global;
using BrainGraph.WinStore.Screens.Nodal;
using BrainGraph.WinStore.Screens.Selection;
using BrainGraph.WinStore.Screens.Sources;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		private RunExperimentViewModel _running = IoC.Get<RunExperimentViewModel>();
		private RunThresholdTestViewModel _runThresholdTest = IoC.Get<RunThresholdTestViewModel>();
		private PermutationViewModel _permutations = IoC.Get<PermutationViewModel>();

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
				Groups.Add(new MenuGroup { Title = "Config", Items = { _permutations, _runThresholdTest, _running } });
				Groups.Add(new MenuGroup { Title = "Global", Items = { IoC.Get<GlobalStrengthViewModel>(), new MenuItem { Title = "Associations" }, } });
				Groups.Add(new MenuGroup { Title = "Component", Items = { new MenuItem { Title = "Intermodal" }, new MenuItem { Title = "By Type" }, new MenuItem { Title = "Associations" }, } });
				Groups.Add(new MenuGroup { Title = "Nodal", Items = { IoC.Get<NodalStrengthDataTypeViewModel>(), new MenuItem { Title = "Associations" }, } });
				Groups.Add(new MenuGroup { Title = "Edge", Items = { IoC.Get<EdgeSignificanceViewModel>(), new MenuItem { Title = "Associations" }, } });
			}
		}

		public async void MenuItemSelected(ItemClickEventArgs eventArgs)
		{
			var menuItem = (IMenuItem)eventArgs.ClickedItem;
			var popupType = menuItem.PopupType;

			if (menuItem is RunThresholdTestViewModel)
			{
				int permutations = Int32.Parse(_permutations.Permutations);

				if (permutations > 0)
				{
					for (double thresh = 7; thresh < 8; thresh += 0.05)
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
									Value = thresh
								};

								dataTypes.Add(t);
							}
						}

						// Load the subjects into the compute service
						_computeService.LoadSubjects(_regionService.GetNodeCount(), _regionService.GetEdgeCount(), dataTypes, _subjectFilterService.GetGroup1(), _subjectFilterService.GetGroup2());

						// Compare groups based on real labels
						_computeService.CompareGroups();

						// Create our async permutation computation to figure out p values
						var permutation = _computeService.PermuteGroupsAsync(permutations);

						// Handle progress reporting
						permutation.Progress += new Windows.Foundation.AsyncActionProgressHandler<int>((_, p) =>
						{
							_runThresholdTest.PrimaryValue = p.ToString();
						});

						// Run the thingy and fix the display when it finishes
						await permutation.AsTask();

						_runThresholdTest.PrimaryValue = thresh.ToString();
						//_eventAggregator.Publish(new PermutationCompleteEvent());

						var result = _computeService.GetResults();

						Debug.WriteLine("Threshold: " + thresh.ToString());
						foreach (var graph in result.Graphs)
						{
							foreach (var component in graph.Components)
							{
								if(component.Edges.Count > 0)
									Debug.WriteLine("Item: " + graph.Name + " Nodes: " + component.NodeCount.ToString() + " Edges: " + component.Edges.Count.ToString());
							}
						}
					}
				}
			}
			else if (menuItem is RunExperimentViewModel)
			{
				int permutations = Int32.Parse(_permutations.Permutations);

				if (permutations > 0)
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
}
