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
			var smaller = 0;
			var larger = 0;
			if (v1 >= v2)
			{
				smaller = v2;
				larger = v1;
			}
			else
			{
				smaller = v1;
				larger = v2;
			}

			if ((smaller == 1) && (larger == 76))
					return true;
			if ((smaller == 1) && (larger == 77))
					return true;
			if ((smaller == 9) && (larger == 26))
					return true;
			if ((smaller == 10) && (larger == 76))
					return true;
			if ((smaller == 11) && (larger == 76))
					return true;
			if ((smaller == 16) && (larger == 70))
					return true;
			if ((smaller == 16) && (larger == 77))
					return true;
			if ((smaller == 16) && (larger == 76))
					return true;
			if ((smaller == 16) && (larger == 71))
					return true;
			if ((smaller == 17) && (larger == 77))
					return true;
			if ((smaller == 17) && (larger == 76))
					return true;
			if ((smaller == 26) && (larger == 74))
					return true;
			if ((smaller == 28) && (larger == 70))
					return true;
			if ((smaller == 29) && (larger == 70))
					return true;
			if ((smaller == 29) && (larger == 77))
					return true;
			if ((smaller == 29) && (larger == 76))
					return true;
			if ((smaller == 30) && (larger == 70))
					return true;
			if ((smaller == 30) && (larger == 71))
					return true;
			if ((smaller == 31) && (larger == 70))
					return true;
			if ((smaller == 31) && (larger == 71))
					return true;
			if ((smaller == 46) && (larger == 76))
					return true;
			if ((smaller == 46) && (larger == 77))
					return true;
			if ((smaller == 46) && (larger == 70))
					return true;
			if ((smaller == 47) && (larger == 77))
					return true;
			if ((smaller == 47) && (larger == 76))
					return true;
			if ((smaller == 47) && (larger == 70))
					return true;
			if ((smaller == 54) && (larger == 77))
					return true;
			if ((smaller == 55) && (larger == 77))
					return true;
			if ((smaller == 55) && (larger == 76))
					return true;
			if ((smaller == 56) && (larger == 76))
					return true;
			if ((smaller == 56) && (larger == 77))
					return true;
			if ((smaller == 57) && (larger == 77))
					return true;
			if ((smaller == 57) && (larger == 76))
					return true;
			if ((smaller == 70) && (larger == 78))
					return true;
			if ((smaller == 70) && (larger == 80))
					return true;
			if ((smaller == 70) && (larger == 75))
					return true;
			if ((smaller == 70) && (larger == 73))
					return true;
			if ((smaller == 70) && (larger == 79))
					return true;
			if ((smaller == 71) && (larger == 78))
					return true;
			if ((smaller == 76) && (larger == 78))
					return true;
			if ((smaller == 76) && (larger == 80))
					return true;
			if ((smaller == 76) && (larger == 79))
					return true;
			if ((smaller == 76) && (larger == 85))
					return true;
			if ((smaller == 76) && (larger == 81))
					return true;
			if ((smaller == 76) && (larger == 84))
					return true;
			if ((smaller == 77) && (larger == 85))
					return true;
			if ((smaller == 77) && (larger == 78))
					return true;
			if ((smaller == 77) && (larger == 81))
					return true;
			if ((smaller == 77) && (larger == 80))
					return true;
			if ((smaller == 77) && (larger == 79))
					return true;
			if ((smaller == 77) && (larger == 84))
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
									var str = string.Format("{0}, {1:00}{2:00}, {3}", subject.SubjectId, lineIdx, colIdx, val);

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
