using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace BrainGraph.WinStore.Common.Util
{
	public class GridViewReadySelector : DataTemplateSelector
	{
		public DataTemplate ReadyTemplate { get; set; }
		public DataTemplate NotReadyTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			IReady ready = item as IReady;

			if (ready != null)
			{
				if (ready.IsReady)
					return ReadyTemplate;
				else
					return NotReadyTemplate;
			}
			else
				return base.SelectTemplateCore(item, container);
		}
	}

	public class GridViewReadyStyleSelector : StyleSelector
	{
		public Style ReadyStyle { get; set; }
		public Style NotReadyStyle { get; set; }

		protected override Style SelectStyleCore(object item, DependencyObject container)
		{
		
			IReady ready = item as IReady;

			if (ready != null)
			{
				if (ready.IsReady)
					return ReadyStyle;
				else
					return NotReadyStyle;
			}
			else
				return base.SelectStyleCore(item, container);
		} 
	}

	public interface IReady
	{
		bool IsReady { get; set; }
	}
}
