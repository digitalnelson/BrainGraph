using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Selection
{
	public class PermutationViewModel : ViewModelBase, IMenuItem, IHandle<SubjectsLoadedEvent>
	{
		private IEventAggregator _eventAggregator;

		private const string SETTING_PERMUTATIONS = "Permutations";

		public PermutationViewModel()
		{
			Title = "Permutations";

			Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
			if (roamingSettings.Values.ContainsKey(SETTING_PERMUTATIONS))
			{
				Permutations = roamingSettings.Values[SETTING_PERMUTATIONS] as string;
			}
			else
				Permutations = "0";

			_eventAggregator = IoC.Get<IEventAggregator>();
			_eventAggregator.Subscribe(this);
		}

		public string Permutations { 
			get { return _inlPermutations; }
			set 
			{ 
				_inlPermutations = value;

				Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
				roamingSettings.Values[SETTING_PERMUTATIONS] = _inlPermutations;

				PrimaryValue = value; 
				NotifyOfPropertyChange(() => Permutations); 
			} 
		} 
		private string _inlPermutations;


		public void Handle(SubjectsLoadedEvent message)
		{
			IsReady = true;
		}

		public override Type PopupType
		{
			get
			{
				return typeof(PermutationPopup);
			}
		}
	}
}
