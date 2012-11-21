using BrainGraph.WinStore.Common.Util;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens
{
	public interface IMenuItem
	{
		string Title { get; set; }
		string Subtitle { get; set; }
		string Description { get; set; }
		string PrimaryValue { get; set; }
		
		Type ViewModelType { get; }
		Type PopupType { get; }
	}

	public class MenuItem : Screen, IMenuItem, IReady
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Subtitle { get { return _inlSubtitle; } set { _inlSubtitle = value; NotifyOfPropertyChange(() => Subtitle); } } private string _inlSubtitle;
		public string Description { get { return _inlDescription; } set { _inlDescription = value; NotifyOfPropertyChange(() => Description); } } private string _inlDescription;
		public string PrimaryValue { get { return _inlPrimaryValue; } set { _inlPrimaryValue = value; NotifyOfPropertyChange(() => PrimaryValue); } } private string _inlPrimaryValue;

		public bool IsReady { get { return _inlIsReady; } set { _inlIsReady = value; NotifyOfPropertyChange(() => IsReady); } } private bool _inlIsReady = false;

		public Type ViewModelType { get { return typeof(MenuItem); } }
		public Type PopupType { get { return null; } }
	}
}
