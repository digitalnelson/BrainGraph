using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Config
{
	class NBSmConfigViewModel : ViewModelBase, IMenuItem, IHandle<SubjectsLoadedEvent>, IHandle<SubjectsFilteredEvent>
	{
		private IEventAggregator _eventAggregator;
		private ISubjectFilterService _subjectFilterService = IoC.Get<ISubjectFilterService>();

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

		public void Handle(SubjectsFilteredEvent message)
		{
			var dataTypeSettings = _subjectFilterService.GetDataTypeSettings();

			DataTypes.Clear();
			foreach (var type in dataTypeSettings)
			{
				if(type.Value)
					DataTypes.Add(new NBSmConfigByDataTypeViewModel { Title = type.Key + " altered" });
			}
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
		public string Threshold 
		{ 
			get { return _inlThreshold; } 
			set 
			{ 
				_inlThreshold = value;

				// TODO: Async these

				//_subjectFilterService.AssignDataType(Title, _inlIsIncluded);
				//_subjectFilterService.FilterSubjects();

				NotifyOfPropertyChange(() => Threshold); 
			} 

		} private string _inlThreshold;
	}
}