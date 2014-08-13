using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainGraph.Storage;
using System.IO;
using Windows.Storage;
using BrainGraph.WinStore.Models;

namespace BrainGraph.WinStore.Services.Loaders
{
	class ROILoader
	{
		public static async Task<List<ROI>> Load(StorageFile file)
		{		
			// read content
			var lines = await Windows.Storage.FileIO.ReadLinesAsync(file);

			// Read in all the lines
			//string[] lines = System.IO.File.ReadAllLines(_fullPath);
			List<ROI> regionsOfInterest = new List<ROI>();

			foreach (var line in lines)
			{
				var fields = line.Split('\t');

				ROI roi = new ROI()
				{
					Index = Int32.Parse(fields[0]),
					Name = fields[1],
					Ident = Int32.Parse(fields[2]),
					X = Double.Parse(fields[6]),
					Y = Double.Parse(fields[7]),
					Z = Double.Parse(fields[8]),
                    TX = Double.Parse(fields[10]),
                    TY = Double.Parse(fields[11]),
                    TZ = Double.Parse(fields[12]),
				};
				
				regionsOfInterest.Add(roi);
			}

			return regionsOfInterest;
		}
	}
}
