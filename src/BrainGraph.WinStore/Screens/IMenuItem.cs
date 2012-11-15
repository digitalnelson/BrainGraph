using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens
{
	public interface IMenuItem
	{
		string Title { get; set; }
		string Subtitle { get; set; }
		string Description { get; set; }
		string PrimaryValue { get; set; }
		
		Type ViewModelType { get; }
		Type PopupType { get; }
	}
}
