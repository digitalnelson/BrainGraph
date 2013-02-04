using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Config
{
	class NBSmConfigViewModel : ViewModelBase, IMenuItem, IHandle<SubjectsLoadedEvent>
	{
		private IEventAggregator _eventAggregator;

		private const string SETTING_PERMUTATIONS = "Permutations";

		public NBSmConfigViewModel()
		{
			Title = "NBSm Config";

			//Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
			//if (roamingSettings.Values.ContainsKey(SETTING_PERMUTATIONS))
			//{
			//	Permutations = roamingSettings.Values[SETTING_PERMUTATIONS] as string;
			//}
			//else
			//	Permutations = "0";

			DataTypes = new BindableCollection<NBSmConfigByDataTypeViewModel>();
			DataTypes.Add(new NBSmConfigByDataTypeViewModel { Title = "DTI", Threshold = "2.15" });
			DataTypes.Add(new NBSmConfigByDataTypeViewModel { Title = "fMRI", Threshold = "5.225" });

			_eventAggregator = IoC.Get<IEventAggregator>();
			_eventAggregator.Subscribe(this);
		}

		//public string Permutations { 
		//	get { return _inlPermutations; }
		//	set 
		//	{ 
		//		_inlPermutations = value;

		//		Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
		//		roamingSettings.Values[SETTING_PERMUTATIONS] = _inlPermutations;

		//		PrimaryValue = value; 
		//		NotifyOfPropertyChange(() => Permutations); 
		//	} 
		//} 
		//private string _inlPermutations;


		public void Handle(SubjectsLoadedEvent message)
		{
			IsReady = true;
		}

		public override Type PopupType
		{
			get
			{
				return typeof(NBSmConfigPopup);
			}
		}
		public BindableCollection<NBSmConfigByDataTypeViewModel> DataTypes { get { return _inlDataTypes; } set { _inlDataTypes = value; NotifyOfPropertyChange(() => DataTypes); } } private BindableCollection<NBSmConfigByDataTypeViewModel> _inlDataTypes;
	}

	public class NBSmConfigByDataTypeViewModel : Screen
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Threshold { get { return _inlThreshold; } set { _inlThreshold = value; NotifyOfPropertyChange(() => Threshold); } } private string _inlThreshold;

		public bool IsIncluded
		{
			get { return _inlIsIncluded; }
			set
			{
				_inlIsIncluded = value;

				// TODO: Async these

				_subjectFilterService.AssignDataType(Title, _inlIsIncluded);
				_subjectFilterService.FilterSubjects();

				NotifyOfPropertyChange(() => IsIncluded);
			}
		} private bool _inlIsIncluded;
	}
}