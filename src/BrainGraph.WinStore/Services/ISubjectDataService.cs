using System.Diagnostics;
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

		private bool DoWeCare(int v1, int v2)
		{
			if((v1 == 84 && v2 == 76) || (v1 == 76 && v2 == 84))
				return true;

			if ((v1 == 16 && v2 == 76) || (v1 == 76 && v2 == 16))
				return true;

			if ((v1 == 80 && v2 == 76) || (v1 == 76 && v2 == 80))
				return true;

			if ((v1 == 78 && v2 == 76) || (v1 == 76 && v2 == 78))
				return true;

			if ((v1 == 46 && v2 == 76) || (v1 == 76 && v2 == 46))
				return true;

			if ((v1 == 0 && v2 == 76) || (v1 == 76 && v2 == 0))
				return true;

			if ((v1 == 56 && v2 == 76) || (v1 == 76 && v2 == 56))
				return true;

			if ((v1 == 54 && v2 == 76) || (v1 == 76 && v2 == 54))
				return true;

			if ((v1 == 30 && v2 == 70) || (v1 == 70 && v2 == 30))
				return true;

			if ((v1 == 28 && v2 == 76) || (v1 == 76 && v2 == 28))
				return true;

			return false;
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
								var val = Double.Parse(columns[colIdx]);

								if (DoWeCare(lineIdx, colIdx) && (adj.DataType == "fMRI-mo") && (colIdx > lineIdx))
								{
									var str = string.Format("{0}, {1}{2}, {3}", subject.SubjectId, lineIdx, colIdx, val);

									Debug.WriteLine(str);
								}

								// Only load the upper triangle
								if (colIdx > lineIdx)
									adj.Graph.AddEdge(lineIdx, colIdx, Math.Abs(val));
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
