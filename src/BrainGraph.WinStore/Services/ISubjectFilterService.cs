using BrainGraph.ComputeRT.Subjects;
using BrainGraph.WinStore.Events;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;

namespace BrainGraph.WinStore.Services
{
	interface ISubjectFilterService
	{
        void AddSubject(SubjectViewModel subject);
		List<string> GetGroups();
		Dictionary<string, ComputeGroup> GetGroupSettings();
		List<string> GetDataTypes();
		Dictionary<string, bool> GetDataTypeSettings();

		void AssignGroup(string subjectGroup, ComputeGroup computeGroup);
		void AssignDataType(string dataType, bool enabled);

		void FilterSubjects();

        List<SubjectViewModel> GetGroup1();
        List<SubjectViewModel> GetGroup2();
        List<SubjectViewModel> GetRemaining();

		void Clear();
	}

	public class SubjectFilterService : ISubjectFilterService
	{
		private IEventAggregator _eventAggregator = IoC.Get<IEventAggregator>();

		private Dictionary<string, ComputeGroup> _groupLookup = new Dictionary<string, ComputeGroup>();
		private Dictionary<string, bool> _dataTypeLookup = new Dictionary<string, bool>();

        private List<SubjectViewModel> _all = new List<SubjectViewModel>();
        private List<SubjectViewModel> _group1 = new List<SubjectViewModel>();
        private List<SubjectViewModel> _group2 = new List<SubjectViewModel>();
        private List<SubjectViewModel> _remaining = new List<SubjectViewModel>();

        public void AddSubject(SubjectViewModel subject)
		{
			if(!_groupLookup.ContainsKey(subject.GroupId.ToLower()))
				_groupLookup[subject.GroupId.ToLower()] = ComputeGroup.GroupNone;

			foreach (var graph in subject.Graphs)
			{
				if (!_dataTypeLookup.ContainsKey(graph.Key))
                    _dataTypeLookup[graph.Key] = false;
			}

			_all.Add(subject);
		}

		public List<string> GetGroups()
		{
			return _groupLookup.Keys.ToList();
		}

		public Dictionary<string, ComputeGroup> GetGroupSettings()
		{
			return _groupLookup;
		}

		public List<string> GetDataTypes()
		{
			return _dataTypeLookup.Keys.ToList();
		}

		public Dictionary<string, bool> GetDataTypeSettings()
		{
			return _dataTypeLookup;
		}

		public void AssignGroup(string subjectGroup, ComputeGroup computeGroup)
		{
			_groupLookup[subjectGroup.ToLower()] = computeGroup;
		}

		public void AssignDataType(string dataType, bool enabled)
		{
			_dataTypeLookup[dataType] = enabled;
		}

		public void FilterSubjects()
		{
			_group1.Clear();
			_group2.Clear();
			_remaining.Clear();

			foreach (var subject in _all)
			{
				// Filter by active data types
				bool hasData = true;

				foreach (var dt in _dataTypeLookup)
				{
					if (dt.Value && !subject.Graphs.ContainsKey(dt.Key))
						hasData = false;
				}

				if (hasData)
				{
					// Bucket by groupid
					var groupId = subject.GroupId.ToLower();

					if (_groupLookup.ContainsKey(groupId) && _groupLookup[groupId] == ComputeGroup.GroupOne)
						_group1.Add(subject);
					else if (_groupLookup.ContainsKey(groupId) && _groupLookup[groupId] == ComputeGroup.GroupTwo)
						_group2.Add(subject);
					else
						_remaining.Add(subject);
				}
				else
					_remaining.Add(subject);
			}

			_eventAggregator.Publish(new SubjectsFilteredEvent());
		}

        public List<SubjectViewModel> GetGroup1()
		{
			return _group1;
		}

        public List<SubjectViewModel> GetGroup2()
		{
			return _group2;
		}

        public List<SubjectViewModel> GetRemaining()
		{
			return _remaining;
		}

		public void Clear()
		{
			_groupLookup.Clear();
			_dataTypeLookup.Clear();

			_all.Clear();
			_group1.Clear();
			_group2.Clear();
			_remaining.Clear();
		}
	}
}
