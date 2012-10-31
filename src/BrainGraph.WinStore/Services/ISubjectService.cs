﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using BrainGraph.Compute.Subjects;
using BrainGraph.WinStore.Services;
using BraingGraph.Services.Loaders;
using Windows.Storage;
using BrainLab.Services.Loaders;

namespace BrainGraph.WinStore.Services
{
	public interface ISubjectService
	{
		void LoadSubjectFile(StorageFile file);
		void LoadSubjectData(StorageFolder folder, int vertexLimit);

		List<Subject> GetSubjects();

		List<string> GetGroups();
		List<string> GetDataTypes();

		//int GetFilesLoadedCount();
	}

	public class SubjectService : ISubjectService
	{
		//private readonly IEventAggregator _eventAggregator;

		private List<Subject> _subjects;
		private Dictionary<string, List<Subject>> _subjectsByGroup;
		private Dictionary<string, Subject> _subjectsByEventId;

		private List<string> _dataTypes;
		//private int _filesLoadedCount;

		public SubjectService()
		{
			_subjects = new List<Subject>();
			_subjectsByGroup = new Dictionary<string, List<Subject>>();
			_subjectsByEventId = new Dictionary<string, Subject>();
			_dataTypes = new List<string>();
		}

		public async void LoadSubjectFile(StorageFile file)
		{
			_subjects = await SubjectCSVLoader.LoadSubjectFile(file);

			foreach (var sub in _subjects)
			{
				if (!_subjectsByGroup.ContainsKey(sub.GroupId))
					_subjectsByGroup[sub.GroupId] = new List<Subject>();
					
				_subjectsByGroup[sub.GroupId].Add(sub);

				foreach (var eventId in sub.EventIds)
					_subjectsByEventId[eventId] = sub;
			}

			//_eventAggregator.Publish(new SubjectsLoadedEvent());
		}

		public void LoadSubjectData(StorageFolder folder, int vertexLimit)
		{
			AdjCSVLoader.Load(folder, _subjectsByEventId, vertexLimit);

			//_dataTypes.Clear();
			//foreach (var subject in _subjects)
			//{
			//	foreach (var graph in subject.Graphs)
			//	{
			//		if (!_dataTypes.Contains(graph.Value.DataSource))
			//			_dataTypes.Add(graph.Value.DataSource);
			//	}
			//}

			//_filesLoadedCount = adjLoader.FilesLoaded;

			//_eventAggregator.Publish(new DataLoadedEvent());
		}

		public List<Subject> GetSubjects()
		{
			return _subjects;
		}

		public List<string> GetGroups()
		{
			return _subjectsByGroup.Keys.ToList();
		}

		public List<string> GetDataTypes()
		{
			return _dataTypes;
		}

		//public int GetFilesLoadedCount()
		//{
		//	return _filesLoadedCount;
		//}
	}
}
