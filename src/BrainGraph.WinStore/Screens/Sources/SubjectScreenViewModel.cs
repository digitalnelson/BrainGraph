using BrainGraph.ComputeRT.Subjects;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class SubjectScreenViewModel : Screen
	{
		IRegionService _regionService = IoC.Get<IRegionService>();
		ISubjectDataService _subjectService = IoC.Get<ISubjectDataService>();

        public SubjectViewModel Subject { get { return _inlSubject; } set { _inlSubject = value; NotifyOfPropertyChange(() => Subject); } } private SubjectViewModel _inlSubject;
		
		public string SubjectId { get { return Subject.SubjectId; } /*set { Subject.SubjectId = value; NotifyOfPropertyChange(() => SubjectId); }*/ }
		public string GroupId { get { return Subject.GroupId; } set { Subject.GroupId = value; NotifyOfPropertyChange(() => GroupId); } }
		public string Age { get { return Subject.Age; } set { Subject.Age = value; NotifyOfPropertyChange(() => Age); } } 
		public string Sex { get { return Subject.Sex; } set { Subject.Sex = value; NotifyOfPropertyChange(() => Sex); } }

		public string GraphCount { get {

			if (Subject.Graphs != null)
				return Subject.Graphs.Count.ToString();
			else
				return "";
		
		} }

		public void Refresh()
		{
			NotifyOfPropertyChange(() => GraphCount);
		}
	}
}
