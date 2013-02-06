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
	class NBSmConfigViewModel : ViewModelBase, IMenuItem, IHandle<SubjectsFilteredEvent>
	{
		private Windows.Storage.ApplicationDataContainer _roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
		private IEventAggregator _eventAggregator;
		private ISubjectFilterService _subjectFilterService = IoC.Get<ISubjectFilterService>();

		public const string SETTING_DT_PREFIX = "DataType_";

		public NBSmConfigViewModel()
		{
			Title = "NBSm Thresholds";
			
			DataTypes = new BindableCollection<NBSmConfigByDataTypeViewModel>();

			_eventAggregator = IoC.Get<IEventAggregator>();
			_eventAggregator.Subscribe(this);
		}
		
		public void Handle(SubjectsFilteredEvent message)
		{
			IsReady = true;

			var dataTypeSettings = _subjectFilterService.GetDataTypeSettings();
			StringBuilder sbItems = new StringBuilder();

			DataTypes.Clear();
			foreach (var type in dataTypeSettings)
			{
				if (type.Value)
				{
					var dataTypeKey = SETTING_DT_PREFIX + type.Key.ToLower();
					var threshold = "2.0";

					if (_roamingSettings.Values.ContainsKey(dataTypeKey))
						threshold = _roamingSettings.Values[dataTypeKey] as string;

					DataTypes.Add(new NBSmConfigByDataTypeViewModel { Title = type.Key, Threshold = threshold });
					sbItems.AppendLine(string.Format("{0}: {1}", type.Key, threshold));
				}
			}

			PrimaryValue = sbItems.ToString();
		}

		public override Type PopupType { get { return typeof(NBSmConfigPopup); } }

		public BindableCollection<NBSmConfigByDataTypeViewModel> DataTypes { get { return _inlDataTypes; } set { _inlDataTypes = value; NotifyOfPropertyChange(() => DataTypes); } } private BindableCollection<NBSmConfigByDataTypeViewModel> _inlDataTypes;
	}

	public class NBSmConfigByDataTypeViewModel : Screen
	{
		private IComputeService _computeService = IoC.Get<IComputeService>();

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Threshold 
		{ 
			get { return _inlThreshold; } 
			set 
			{
				if (value != _inlThreshold)
				{
					_inlThreshold = value;
					_computeService.SetThreshold(_inlTitle, _inlThreshold);

					var dataTypeKey = NBSmConfigViewModel.SETTING_DT_PREFIX + _inlTitle.ToLower();

					Windows.Storage.ApplicationData.Current.RoamingSettings.Values[dataTypeKey] = _inlThreshold;
				}
				
				//_subjectFilterService.AssignDataType(Title, _inlIsIncluded);
				//_subjectFilterService.FilterSubjects();

				NotifyOfPropertyChange(() => Threshold); 
			} 

		} private string _inlThreshold;
	}
}