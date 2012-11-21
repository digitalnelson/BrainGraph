using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Selection
{
	class DataTypesViewModel : Screen
	{
		public DataTypesViewModel()
		{
			Title = "Data Types";
			PrimaryValue = "3";
			Subtitle = "Identification and selection of adjacency matricies by data type.";

			DataTypes = new BindableCollection<DataTypeViewModel>();
			DataTypes.Add(new DataTypeViewModel { Title = "DTI", Threshold = "2.15" });
			DataTypes.Add(new DataTypeViewModel { Title = "fMRI", Threshold = "5.225" });
		}

		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Subtitle { get { return _inlSubtitle; } set { _inlSubtitle = value; NotifyOfPropertyChange(() => Subtitle); } } private string _inlSubtitle;
		public string Description { get { return _inlDescription; } set { _inlDescription = value; NotifyOfPropertyChange(() => Description); } } private string _inlDescription;
		public string PrimaryValue { get { return _inlPrimaryValue; } set { _inlPrimaryValue = value; NotifyOfPropertyChange(() => PrimaryValue); } } private string _inlPrimaryValue;

		public Type ViewModelType { get { return typeof(DataTypesViewModel); } }
		public Type PopupType { get { return typeof(DataTypesPopup); } }

		public BindableCollection<DataTypeViewModel> DataTypes { get { return _inlDataTypes; } set { _inlDataTypes = value; NotifyOfPropertyChange(() => DataTypes); } } private BindableCollection<DataTypeViewModel> _inlDataTypes;
	}

	public class DataTypeViewModel : Screen
	{
		public string Title { get { return _inlTitle; } set { _inlTitle = value; NotifyOfPropertyChange(() => Title); } } private string _inlTitle;
		public string Threshold { get { return _inlThreshold; } set { _inlThreshold = value; NotifyOfPropertyChange(() => Threshold); } } private string _inlThreshold;
	}
}
