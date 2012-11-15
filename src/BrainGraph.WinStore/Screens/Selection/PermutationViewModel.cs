using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Selection
{
	public class PermutationViewModel : Screen, IMenuItem
	{
		public PermutationViewModel()
		{
			Title = "Permutations";
			Permutations = "10,000";
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Subtitle { get { return _inlSubtitle; } set { _inlSubtitle = value; NotifyOfPropertyChange(() => Subtitle); } } private string _inlSubtitle;
		public string Description { get { return _inlDescription; } set { _inlDescription = value; NotifyOfPropertyChange(() => Description); } } private string _inlDescription;
		public string PrimaryValue { get { return _inlPermutations; } set {} }

		public string Permutations { 
			get { return _inlPermutations; } 
			set { _inlPermutations = value; NotifyOfPropertyChange(() => Permutations); NotifyOfPropertyChange(() => PrimaryValue); } 
		} private string _inlPermutations;

		public Type ViewModelType { get { return typeof(PermutationViewModel); } }
		public Type PopupType { get { return typeof(PermutationPopup); } }
	}
}
