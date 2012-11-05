using System;
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
		Task<List<Subject>> LoadSubjectFile(StorageFile file);
		Task LoadSubjectData(StorageFolder folder, int vertexLimit);

		List<Subject> GetSubjects();

		List<string> GetGroups();
		List<string> GetDataTypes();

		//void SetDataTypes(Dictionary<string, double> dataTypes);
		//Dictionary<string, double> GetDataTypes();

		//void SetGroups(List<string> group1Idents, List<string> group2Idents);

		//void FilterSubjects();
		//Dictionary<ComputeGroup, List<Subject>> GetFilteredSubjectsByComputeGroup();
		//Dictionary<ComputeGroup, int> GetFilteredSubjectCountsByComputeGroup();

		//int GetFilesLoadedCount();
	}

	public class SubjectService : ISubjectService
	{
		//private readonly IEventAggregator _eventAggregator;

		private List<Subject> _subjects;
		private Dictionary<string, List<Subject>> _subjectsByGroup;
		private Dictionary<string, Subject> _subjectsByEventId;

		//private Dictionary<string, Threshold> _dataTypes;
		private List<string> _group1Idents;
		private List<string> _group2Idents;

		private List<Subject> _filteredSubjectData;
		private Dictionary<ComputeGroup, List<Subject>> _filteredSubjectDataByGroup;

		private List<string> _dataTypes;
		//private int _filesLoadedCount;

		public SubjectService()
		{
			_subjects = new List<Subject>();
			_subjectsByGroup = new Dictionary<string, List<Subject>>();
			_subjectsByEventId = new Dictionary<string, Subject>();
			_dataTypes = new List<string>();
		}

		public async Task<List<Subject>> LoadSubjectFile(StorageFile file)
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

			return _subjects;

			//_eventAggregator.Publish(new SubjectsLoadedEvent());
		}

		public async Task LoadSubjectData(StorageFolder folder, int vertexLimit)
		{
			await AdjCSVLoader.Load(folder, _subjectsByEventId, vertexLimit);

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

		//public void SetGroups(List<string> group1Idents, List<string> group2Idents)
		//{
		//	_group1Idents = group1Idents;
		//	_group2Idents = group2Idents;
		//}

		//public void SetDataTypes(Dictionary<string, double> dataTypes)
		//{
		//	_dataTypes = dataTypes;
		//}

		//public Dictionary<string, double> GetDataTypes()
		//{
		//	return _dataTypes;
		//}

		public void FilterSubjects()
		{
			if (_dataTypes != null && _group1Idents != null && _group2Idents != null)
			{
				// Loop through our subject data and get rid of the ones without complete data based on user selection
				_filteredSubjectData = new List<Subject>();
				_filteredSubjectDataByGroup = new Dictionary<ComputeGroup, List<Subject>>();

				_filteredSubjectDataByGroup[ComputeGroup.GroupOne] = new List<Subject>();
				_filteredSubjectDataByGroup[ComputeGroup.GroupTwo] = new List<Subject>();
								
				foreach (var subject in _subjects)
				{
					bool bHasData = true;

					foreach (var dt in _dataTypes)
					{
						if (!subject.Graphs.ContainsKey(dt))
						{
							bHasData = false;
							break;
						}
					}

					if (bHasData)
					{
						_filteredSubjectData.Add(subject);

						var computeGrp = ComputeGroup.GroupOne;
						if (_group2Idents.Contains(subject.GroupId))
							computeGrp = ComputeGroup.GroupTwo;

						_filteredSubjectDataByGroup[computeGrp].Add(subject);
					}
				}

				//_eventAggregator.Publish(new SubjectsFilteredEvent());
			}
		}

		public Dictionary<ComputeGroup, List<Subject>> GetFilteredSubjectsByComputeGroup()
		{
			return _filteredSubjectDataByGroup;
		}

		public Dictionary<ComputeGroup, int> GetFilteredSubjectCountsByComputeGroup()
		{
			Dictionary<ComputeGroup, int> counts = new Dictionary<ComputeGroup, int>();
			foreach (var itm in _filteredSubjectDataByGroup)
			{
				counts[itm.Key] = itm.Value.Count;
			}

			return counts;
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
