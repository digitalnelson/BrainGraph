using BrainGraph.WinStore.Common.Util;
using BrainGraph.WinStore.Screens;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BrainGraph.WinStore.Common
{
	/// <summary>
	/// Base view model for all our main screens, the method GoBack will be bound via convention
	/// to the back button and only display when it can go back due to the template of the back 
	/// button (Collapsed when Disabled)
	/// </summary>
	public abstract class ViewModelBase : Screen, IMenuItem, IReady
	{
		private readonly INavigationService _navigationService;

		protected ViewModelBase()
		{
			if(!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
				_navigationService = IoC.Get<INavigationService>();
		}

		public void GoBack()
		{
			if(_navigationService != null)
				_navigationService.GoBack();
		}

		public bool CanGoBack
		{
			get
			{
				if (_navigationService != null)
					return _navigationService.CanGoBack;
				else
					return false;
			}
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Subtitle { get { return _inlSubtitle; } set { _inlSubtitle = value; NotifyOfPropertyChange(() => Subtitle); } } private string _inlSubtitle;
		public string Description { get { return _inlDescription; } set { _inlDescription = value; NotifyOfPropertyChange(() => Description); } } private string _inlDescription;
		public string PrimaryValue { get { return _inlPrimaryValue; } set { _inlPrimaryValue = value; NotifyOfPropertyChange(() => PrimaryValue); } } private string _inlPrimaryValue;

		public virtual Type ViewModelType { get { return null; } }
		public virtual Type PopupType { get { return null; } }

		public bool IsReady { get { return _inlIsReady; } set { _inlIsReady = value;  NotifyOfPropertyChange(() => IsReady);  } } private bool _inlIsReady = false;
	}
}
