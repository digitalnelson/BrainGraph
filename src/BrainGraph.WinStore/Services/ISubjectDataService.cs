using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using BrainGraph.ComputeRT.Subjects;
using BrainGraph.Services.Loaders;

namespace BrainGraph.WinStore.Services
{
	public interface ISubjectDataService
	{
        Task<List<SubjectViewModel>> LoadSubjects(StorageFile file);
        List<SubjectViewModel> GetSubjects();

		Task IndexDataFolder(StorageFolder folder, int vertexLimit);
        Task LoadSubjectData(SubjectViewModel subject, int vertexLimit);

		void Clear();
	}

	public class SubjectDataService : ISubjectDataService
	{
		private readonly IEventAggregator _eventAggregator;

        private List<SubjectViewModel> _subjects;
        private Dictionary<string, SubjectViewModel> _subjectsByEventId;

		public SubjectDataService()
		{
			_eventAggregator = IoC.Get<IEventAggregator>();
            _subjects = new List<SubjectViewModel>();
            _subjectsByEventId = new Dictionary<string, SubjectViewModel>();
		}

        public async Task<List<SubjectViewModel>> LoadSubjects(StorageFile file)
		{
			_subjects = await SubjectCSVLoader.LoadSubjectFile(file);

			foreach (var sub in _subjects)
			{
				foreach (var eventId in sub.EventIds)
					_subjectsByEventId[eventId] = sub;
			}

			return _subjects;
		}

		class AdjFile
		{
			public StorageFile File { get; set; }
			public string DataType { get; set; }
			public GraphViewModel Graph { get; set; }
		}

		private Dictionary<string, List<AdjFile>> _adjBySubjectId = new Dictionary<string, List<AdjFile>>();

		public async Task IndexDataFolder(StorageFolder folder, int vertexLimit)
		{
			var files = await folder.GetFilesAsync();

			foreach (var file in files)
			{
				var fileName = file.Name;
				var fileParts = fileName.Split(new char[] { '-' });

				var idents = fileParts[0].Split(new char[] { '_' });
				var eventId = idents[0];

				if (_subjectsByEventId.ContainsKey(eventId))
				{
					var subject = _subjectsByEventId[eventId];
					
					if (!_adjBySubjectId.ContainsKey(subject.SubjectId))
						_adjBySubjectId[subject.SubjectId] = new List<AdjFile>();

					StringBuilder sbAdjType = new StringBuilder();
					for (int i = 1; i < idents.Length; i++)
					{
						if (i > 1)
							sbAdjType.Append("-");

						sbAdjType.Append(idents[i]);
					}

					AdjFile adj = new AdjFile();
					adj.File = file;
                    adj.Graph = new GraphViewModel(vertexLimit);
					adj.DataType = sbAdjType.ToString();

					_adjBySubjectId[subject.SubjectId].Add(adj);
				}
			}
		}

        public async Task LoadSubjectData(SubjectViewModel subject, int vertexLimit)
		{
			if (_adjBySubjectId.ContainsKey(subject.SubjectId))
			{
				var adjs = _adjBySubjectId[subject.SubjectId];

				foreach(var adj in adjs)
				{
					// Read in all the lines
					var lines = await Windows.Storage.FileIO.ReadLinesAsync(adj.File);

					//subjectGraph.AddGraphLines(lines);

					for (int lineIdx = 0; lineIdx < lines.Count; lineIdx++)
					{
						var line = lines[lineIdx];
						var columns = line.TrimEnd().Split('\t');

						for (int colIdx = 0; colIdx < columns.Length; colIdx++)
						{
							if (lineIdx < vertexLimit && colIdx < vertexLimit)
							{
								// Only load the upper triangle
								if (colIdx > lineIdx)
									adj.Graph.AddEdge(lineIdx, colIdx, Math.Abs(Double.Parse(columns[colIdx])));
							}
						}
					}

					subject.AddGraph(adj.DataType, adj.Graph);
				}
			}
		}

        public List<SubjectViewModel> GetSubjects()
		{
			return _subjects;
		}

		public void Clear()
		{
			_subjects.Clear();
			_subjectsByEventId.Clear();
		}
    }
}
