using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class DataTypeViewModel : Screen
	{
		private ISubjectFilterService _subjectFilterService = IoC.Get<ISubjectFilterService>();
		private SubjectsViewModel _svm;

		public DataTypeViewModel(SubjectsViewModel svm, string title, bool isIncluded)
		{
			_svm = svm;

			_inlTitle = title;
			_inlIsIncluded = isIncluded;
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Threshold { get { return _inlThreshold; } set { _inlThreshold = value; NotifyOfPropertyChange(() => Threshold); } } private string _inlThreshold;

		public bool IsIncluded
		{
			get { return _inlIsIncluded; }
			set
			{
				_inlIsIncluded = value;

				// TODO: Async these

				_subjectFilterService.AssignDataType(Title, _inlIsIncluded);
				_subjectFilterService.FilterSubjects();

				NotifyOfPropertyChange(() => IsIncluded);
			}
		} private bool _inlIsIncluded;
	}

	public class GroupViewModel : Screen
	{
		private ISubjectFilterService _subjectFilterService = IoC.Get<ISubjectFilterService>();
		private SubjectsViewModel _svm;

		public GroupViewModel(SubjectsViewModel svm, String studyGroup, ComputeGroup computeGroup)
		{
			_svm = svm;

			_inlStudyGroup = studyGroup;

			if (computeGroup == ComputeGroup.GroupOne)
				_inlExperimentGroup = "Group 1";
			else if (computeGroup == ComputeGroup.GroupTwo)
				_inlExperimentGroup = "Group 2";
			else
				_inlExperimentGroup = "None";
		}

		public string StudyGroup { get { return _inlStudyGroup; } set { _inlStudyGroup = value; NotifyOfPropertyChange(() => StudyGroup); } } private string _inlStudyGroup;

		public string[] ExperimentGroups { get { return new string[] { "None", "Group 1", "Group 2" }; } }

		public string ExperimentGroup
		{
			get { return _inlExperimentGroup; }
			set
			{
				_inlExperimentGroup = value;

				// TODO: Async these
				
				if(_inlExperimentGroup == "Group 1")
					_subjectFilterService.AssignGroup(StudyGroup, ComputeGroup.GroupOne);
				else if(_inlExperimentGroup == "Group 2")
					_subjectFilterService.AssignGroup(StudyGroup, ComputeGroup.GroupTwo);
				else
					_subjectFilterService.AssignGroup(StudyGroup, ComputeGroup.GroupNone);

				_subjectFilterService.FilterSubjects();

				NotifyOfPropertyChange(() => ExperimentGroup);
			}
		} private string _inlExperimentGroup;
	}
}
