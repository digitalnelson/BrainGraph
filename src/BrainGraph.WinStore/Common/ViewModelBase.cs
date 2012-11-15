using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Common
{
	/// <summary>
	/// Base view model for all our main screens, the method GoBack will be bound via convention
	/// to the back button and only display when it can go back due to the template of the back 
	/// button (Collapsed when Disabled)
	/// </summary>
	public abstract class ViewModelBase : Screen
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
	}
}
