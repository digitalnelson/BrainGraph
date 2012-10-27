using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using BrainGraph.Storage;
using BrainGraph.WinStore.Common;

namespace BrainGraph.WinStore.Screens.Regions
{
	public class RegionViewModel : ViewModelBase
	{
		public ROI Region { get; set; }

		public int Index { get { return Region != null ? Region.Index : 0; } }
		public string Title { get { return Region != null ? Region.Name : ""; } }
	}
}
