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
		public async static Task LoadFile(Subject subject, StorageFile file, int vertexCount)
		{
			DateTime dtStart = DateTime.Now;

			var fileName = file.Name;
			var fileParts = fileName.Split(new char[] { '-' });

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

			SubjectGraph subjectGraph = new SubjectGraph(vertexCount);
			//subjectGraph.DataType = adjType;

			// Read in all the lines
			var lines = await Windows.Storage.FileIO.ReadLinesAsync(file);

			//subjectGraph.AddGraphLines(lines);

			for (int lineIdx = 0; lineIdx < lines.Count; lineIdx++)
			{
				var line = lines[lineIdx];
				var columns = line.TrimEnd().Split('\t');

				for (int colIdx = 0; colIdx < columns.Length; colIdx++)
				{
					if (lineIdx < vertexCount && colIdx < vertexCount)
					{
						// Only load the upper triangle
						if (colIdx > lineIdx)
							subjectGraph.AddEdge(lineIdx, colIdx, Math.Abs(Double.Parse(columns[colIdx])));
					}
				}
			}

			subject.AddGraph(adjType, subjectGraph);

			DateTime dtFinish = DateTime.Now;
			TimeSpan tsDiff = dtFinish - dtStart;
			double count = tsDiff.TotalMilliseconds;
		}

		public async static Task Load(StorageFolder folder, Dictionary<string, Subject> subjectsByEventId, int vertexCount)
		{
			DateTime dtStart = DateTime.Now;
			var files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName);

			var multiFileLoad = files.Select(async file =>
			{
				var fileName = file.Name;
				var fileParts = fileName.Split(new char[] { '-' });

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
					return null; //return; //continue;
				else
					subject = subjectsByEventId[eventId];

				SubjectGraph subjectGraph = new SubjectGraph(vertexCount);
				//subjectGraph.DataType = adjType;

				// Read in all the lines
				var lines = await Windows.Storage.FileIO.ReadLinesAsync(file);

				//subjectGraph.AddGraphLines(lines);

				for (int lineIdx = 0; lineIdx < lines.Count; lineIdx++)
				{
					var line = lines[lineIdx];
					var columns = line.TrimEnd().Split('\t');

					for (int colIdx = 0; colIdx < columns.Length; colIdx++)
					{
						if (lineIdx < vertexCount && colIdx < vertexCount)
						{
							// Only load the upper triangle
							if (colIdx > lineIdx)
								subjectGraph.AddEdge(lineIdx, colIdx, Math.Abs(Double.Parse(columns[colIdx])));
						}
					}
				}

				subject.AddGraph(adjType, subjectGraph);

				return subjectGraph;
			});

			var graphs = await Task.WhenAll(multiFileLoad);

			DateTime dtFinish = DateTime.Now;
			TimeSpan tsDiff = dtFinish - dtStart;
			double count = tsDiff.TotalMilliseconds;
		}
	}
}
