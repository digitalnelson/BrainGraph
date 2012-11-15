using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using Callisto.Controls;
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

namespace BrainGraph.WinStore
{
	public class MainMenuViewModel : ViewModelBase
	{
		#region Private Service Vars
		private INavigationService _navService;
		private IRegionService _regionService;
		private ISubjectService _subjectService;
		#endregion

		private bool _needsInit = true;

		public MainMenuViewModel()
		{
			Groups = new BindableCollection<MenuGroup>();

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_navService = IoC.Get<INavigationService>();
				_regionService = IoC.Get<IRegionService>();
				_subjectService = IoC.Get<ISubjectService>();

				Groups.Add(new MenuGroup { Title = "Source", Items = { IoC.Get<RegionsViewModel>(), IoC.Get<SubjectViewModel>(), new PermutationViewModel() } });
				Groups.Add(new MenuGroup { Title = "Measures", Items = { new MenuItem { Title = "Strength" }, new MenuItem { Title = "Diversity" }, new MenuItem { Title = "Clustering" }, new MenuItem { Title = "Modularity" }, } });
				Groups.Add(new MenuGroup { Title = "NBSm", Items = { new MenuItem { Title = "Intermodal" }, new MenuItem { Title = "By Type" }, } });
			}
			else
			{
				Groups.Add(new MenuGroup { Title = "Source", Items = { new RegionsViewModel(), new SubjectViewModel(), new PermutationViewModel() } });
				Groups.Add(new MenuGroup { Title = "Measures", Items = { new MenuItem { Title = "Strength" }, new MenuItem { Title = "Diversity" }, new MenuItem { Title = "Clustering" }, new MenuItem { Title = "Modularity" }, } });
				Groups.Add(new MenuGroup { Title = "NBSm", Items = { new MenuItem { Title = "Intermodal" }, new MenuItem { Title = "By Type" }, } });
			}
		}

		protected override async void OnActivate()
		{
 			 base.OnActivate();

			if (_needsInit)
			{
				StorageFolder studyFolder = await KnownFolders.DocumentsLibrary.GetFolderAsync(@"Studies\SZ_169\NBSm");

				var initRegions = Task.Run(async () =>
				{
					var regionFilePath = @"AAL.txt";
					var regionFile = await studyFolder.GetFileAsync(regionFilePath);

					await _regionService.Load(regionFile);
				});

				var initSubjects = Task.Run(async () =>
				{
					var subjectFilePath = @"VA2929.txt";
					//var subjectDataPath = "";

					var subjectFile = await studyFolder.GetFileAsync(subjectFilePath);

					await _subjectService.LoadSubjects(subjectFile);
				});

				Task.WhenAll(new Task[] { initRegions, initSubjects }).ContinueWith(act =>
				{
					_needsInit = false;
				});
			}
		}

		public void MenuItemSelected(ItemClickEventArgs eventArgs)
		{
			var menuItem = (IMenuItem)eventArgs.ClickedItem;
			var popupType = menuItem.PopupType;

			if (popupType != null)
			{
				var sender = (GridView)eventArgs.OriginalSource;
				var clickedUI = sender.ItemContainerGenerator.ContainerFromItem(eventArgs.ClickedItem);
				
				var popup = (UserControl)Activator.CreateInstance(popupType);
				popup.DataContext = menuItem;

				// Flyout is a ContentControl so set your content within it.
				Flyout f = new Flyout();
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

	public class MenuItem : Screen, IMenuItem
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Subtitle { get { return _inlSubtitle; } set { _inlSubtitle = value; NotifyOfPropertyChange(() => Subtitle); } } private string _inlSubtitle;
		public string Description { get { return _inlDescription; } set { _inlDescription = value; NotifyOfPropertyChange(() => Description); } } private string _inlDescription;
		public string PrimaryValue { get { return _inlPrimaryValue; } set { _inlPrimaryValue = value; NotifyOfPropertyChange(() => PrimaryValue); } } private string _inlPrimaryValue;

		public Type ViewModelType { get { return typeof(MenuItem); } }
		public Type PopupType { get { return null; } }
	}

	public class MenuGroup : Screen
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public BindableCollection<IMenuItem> Items { get { return _inlItems; } set { _inlItems = value; NotifyOfPropertyChange(() => Items); } } private BindableCollection<IMenuItem> _inlItems = new BindableCollection<IMenuItem>();
	}
}
