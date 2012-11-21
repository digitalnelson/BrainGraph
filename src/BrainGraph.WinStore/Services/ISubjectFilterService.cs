using BrainGraph.Compute.Subjects;
using BrainGraph.WinStore.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Services
{
	interface ISubjectFilterService
	{
		void AddSubject(Subject subject);
		List<string> GetGroups();
		List<string> GetDataTypes();
		Dictionary<string, bool> GetDataTypeSettings();

		void AssignGroup(string subjectGroup, ComputeGroup computeGroup);
		void AssignDataType(string dataType, bool enabled);

		void FilterSubjects();

		List<Subject> GetGroup1();
		List<Subject> GetGroup2();
		List<Subject> GetRemaining();

		void Clear();
	}

	public class SubjectFilterService : ISubjectFilterService
	{
		private IEventAggregator _eventAggregator = IoC.Get<IEventAggregator>();

		private Dictionary<string, ComputeGroup> _groupLookup = new Dictionary<string, ComputeGroup>();
		private Dictionary<string, bool> _dataTypeLookup = new Dictionary<string, bool>();

		private List<Subject> _all = new List<Subject>();
		private List<Subject> _group1 = new List<Subject>();
		private List<Subject> _group2 = new List<Subject>();
		private List<Subject> _remaining = new List<Subject>();

		public void AddSubject(Subject subject)
		{
			if(!_groupLookup.ContainsKey(subject.GroupId))
				_groupLookup[subject.GroupId] = ComputeGroup.GroupNone;

			foreach (var graph in subject.Graphs)
			{
				if (!_dataTypeLookup.ContainsKey(graph.Value.DataType))
					_dataTypeLookup[graph.Value.DataType] = false;
			}

			_all.Add(subject);
		}

		public List<string> GetGroups()
		{
			return _groupLookup.Keys.ToList();
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

			// TODO: Save settings
		}

		public void AssignDataType(string dataType, bool enabled)
		{
			_dataTypeLookup[dataType] = enabled;

			// TODO: Save settings
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

		public List<Subject> GetGroup1()
		{
			return _group1;
		}

		public List<Subject> GetGroup2()
		{
			return _group2;
		}

		public List<Subject> GetRemaining()
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
