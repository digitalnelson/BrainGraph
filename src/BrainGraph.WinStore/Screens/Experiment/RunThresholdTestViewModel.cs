using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Experiment
{
	public class RunThresholdTestViewModel : ViewModelBase, IMenuItem, IHandle<SubjectsLoadedEvent>
	{
		private IEventAggregator _eventAggregator = IoC.Get<IEventAggregator>();

		public RunThresholdTestViewModel()
		{
			Title = "Find Thresholds";
			PrimaryValue = "\xE0F2";

			_eventAggregator.Subscribe(this);
		}

		public void Handle(SubjectsLoadedEvent message)
		{
			IsReady = true;
		}
	}
}
