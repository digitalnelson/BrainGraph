﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainGraph.Compute.Subjects;
using Windows.Storage;

namespace BraingGraph.Services.Loaders
{
	class SubjectCSVLoader
	{
		public static async Task<List<Subject>> LoadSubjectFile(StorageFile file)
		{
			// Read in all the lines
			var lines = await Windows.Storage.FileIO.ReadLinesAsync(file);

			if (lines.Count == 0)
				throw new Exception("Subject file empty");

			// Pull out the headers
			char[] splitChars = new char[] { '\t' };
			var headers = lines[0].Split(splitChars);
		
			// TODO: Make error msg for missing cols
			// Find our subject id col
			var subIdIdx = -1;
			for (var i = 0; i < headers.Length; i++)
			{
				if (headers[i] == "subjectId")
					subIdIdx = i;
			}

			// This will be our lookup by subject id
			var subjects = new Dictionary<string, Subject>();

			// Loop through lines of subject file
			foreach (var line in lines.Skip(1))
			{
				// Load up the properties
				var fields = line.Split(splitChars);

				// Pull out the subject id
				var subId = fields[subIdIdx];

				// Create subject if they do not exist
				if (!subjects.ContainsKey(subId))
				{
					var sub = new Subject();
					subjects[subId] = sub;
					subjects[subId].SubjectId = subId;
				}

				// Get the subject
				var subject = subjects[subId];

				// Loop through fields for this subject
				for (int i = 0; i < fields.Length; i++)
				{
					var propName = headers[i];
					var propVal = fields[i];

					switch (propName)
					{
						case "groupId":
							subject.GroupId = propVal.ToUpper();
							break;
						case "sex":
							subject.Sex = propVal;
							break;
						case "age":
							subject.Age = propVal;
							break;
						case "eventId":
							subject.AddEventId(propVal);
							break;
						default:
							{
								double value;								
								bool isNumber = Double.TryParse(fields[i], out value);
								
								if(isNumber)
									subject.AddAttribute(headers[i], value);
							}
							break;
					}	
				}
			}

			return subjects.Values.ToList();
		}
	}
}
