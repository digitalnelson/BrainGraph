using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class SubjectGroupViewModel : Screen
	{
		public SubjectGroupViewModel()
		{
			Subjects = new BindableCollection<SubjectViewModel>();
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public BindableCollection<SubjectViewModel> Subjects { get { return _inlSubjects; } set { _inlSubjects = value; NotifyOfPropertyChange(() => Subjects); } } private BindableCollection<SubjectViewModel> _inlSubjects;
	}
}
