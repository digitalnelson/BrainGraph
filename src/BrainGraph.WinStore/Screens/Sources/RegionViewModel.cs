using BrainGraph.WinStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class RegionViewModel
	{
		public ROI Region { get; set; }

		public string Title { get { return Region.Name; } }
		public string Index { get { return Region.Index.ToString(); } }
	}
}
