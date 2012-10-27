using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore
{
	public class MainPageViewModel : Screen
	{
		public MainPageViewModel()
		{
			Message = "It's Working!";
		}

		public string Message { get { return _inlMessage; } set { _inlMessage = value; NotifyOfPropertyChange(() => Message); } } private string _inlMessage;
	}
}
