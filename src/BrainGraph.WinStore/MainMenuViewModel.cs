using BrainGraph.WinStore.Common;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BrainGraph.WinStore
{
	public class MainMenuViewModel : ViewModelBase
	{
		private INavigationService _navService = IoC.Get<INavigationService>();

		public MainMenuViewModel()
		{
			Items = new BindableCollection<MenuItem>();
			Items.Add(new MenuItem { Title = "Regions Of Interest", ViewModelType = typeof(Screens.Regions.RegionsOfInterestViewModel), Subtitle = "Areas of the brain defined in 3D representing areas of interest to be studied", Description = "Areas of the brain defined in 3D representing areas of interest to be studied" });
			Items.Add(new MenuItem { Title = "Subjects", ViewModelType = typeof(Screens.Subjects.SubjectViewModel) });
		}

		public void MenuItemSelected(ItemClickEventArgs eventArgs)
		{
			var menuItem = (MenuItem)eventArgs.ClickedItem;

			_navService.NavigateToViewModel(menuItem.ViewModelType);
		}

		public BindableCollection<MenuItem> Items { get { return _inlItems; } set { _inlItems = value; NotifyOfPropertyChange(() => Items); } } private BindableCollection<MenuItem> _inlItems;
	}

	public class MenuItem : Screen
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Subtitle { get { return _inlSubtitle; } set { _inlSubtitle = value; NotifyOfPropertyChange(() => Subtitle); } } private string _inlSubtitle;
		public string Description { get { return _inlDescription; } set { _inlDescription = value; NotifyOfPropertyChange(() => Description); } } private string _inlDescription;
		public Type ViewModelType { get { return _inlViewModelType; } set { _inlViewModelType = value; NotifyOfPropertyChange(() => ViewModelType); } } private Type _inlViewModelType;
	}
}
