using BrainGraph.Compute.Subjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BrainLab.Services.Loaders
{
	class AdjCSVLoader
	{
		public AdjCSVLoader(string fullPath, int vertexCount)
		{
			_fullPath = fullPath;
			_vertexCount = vertexCount;
			//_sgf = new SubjectGraphFactory(_vertexCount);
		}

		public async void Load(StorageFolder folder, Dictionary<string, Subject> subjectsByEventId)
		{
			var files = await folder.GetFilesAsync();

			int filesLoadedCount = 0;

			//foreach (var adjFile in adjFiles)
			Parallel.ForEach(files, async file => 
			{
				var fileName = file.Name;
				var fileParts = fileName.Split(new char[]{'-'});

				var idents = fileParts[0].Split(new char[] { '_' });
				var eventId = idents[0];

				StringBuilder sbAdjType = new StringBuilder();
				for (int i = 1; i < idents.Length; i++)
				{
					if (i > 1)
						sbAdjType.Append("-");

					sbAdjType.Append(idents[i]);
				}

				var adjType = sbAdjType.ToString();
				//var desc = fileParts[2];

				Subject subject = null;
				if (!subjectsByEventId.ContainsKey(eventId))
					return; //return; //continue;
				else
					subject = subjectsByEventId[eventId];

				//SubjectGraphItem itm = _sgf.CreateSubject();
				//itm.DataSource = adjType;

				// Read in all the lines
				var lines = await Windows.Storage.FileIO.ReadLinesAsync(file);

				for (int lineIdx = 0; lineIdx < lines.Count; lineIdx++)
				{
					var line = lines[lineIdx];
					var columns = line.TrimEnd().Split('\t');

					for (int colIdx = 0; colIdx < columns.Length; colIdx++)
					{
						if (lineIdx < _vertexCount && colIdx < _vertexCount)
						{
							// Only load the upper triangle
							if (colIdx > lineIdx)
								itm.AddEdge(lineIdx, colIdx, Math.Abs(Double.Parse(columns[colIdx])));
						}
					}
				}

				lock (subject.Graphs)
				{
					subject.Graphs[adjType] = itm;
					filesLoadedCount++;
				}
			});

			FilesLoaded = filesLoadedCount;
		}

		public int FilesLoaded { get; set; }

		public Dictionary<string, string> DataTypes = new Dictionary<string, string>();

		private string _fullPath;
		private int _vertexCount;
		private SubjectGraphFactory _sgf;
	}
}
