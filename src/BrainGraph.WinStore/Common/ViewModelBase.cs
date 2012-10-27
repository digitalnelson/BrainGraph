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
		private readonly INavigationService _navigationService = IoC.Get<INavigationService>();

		protected ViewModelBase()
		{}

		public void GoBack()
		{
			_navigationService.GoBack();
		}

		public bool CanGoBack
		{
			get
			{
				return _navigationService.CanGoBack;
			}
		}
	}
}
