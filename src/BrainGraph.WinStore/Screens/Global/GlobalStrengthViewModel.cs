using BrainGraph.Compute.Graph;
using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainGraph.WinStore.Screens.Global
{
	public class GlobalStrengthViewModel : ViewModelBase, IHandle<PermutationCompleteEvent>
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private IRegionService _regionService;
		private IComputeService _computeService;
		#endregion

		public GlobalStrengthViewModel()
		{
			Title = "Strength";
			PrimaryValue = "0";

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_regionService = IoC.Get<IRegionService>();
				_computeService = IoC.Get<IComputeService>();

				DataItems = new BindableCollection<DataItemViewModel>();
				_eventAggregator.Subscribe(this);
			}

			#region Design Data
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				DataItems = new BindableCollection<DataItemViewModel>();

				GlobalViewModel gvm = new GlobalViewModel();
				gvm.Strength.M1 = 1;
				gvm.Strength.M2 = 2;
				gvm.Strength.V1 = 3;
				gvm.Strength.V2 = 4;
				gvm.Strength.Value = 5;
				gvm.Strength.TwoTailCount = 6;

				DataItems.Add(new DataItemViewModel { DataType = "DTI", Global = gvm });
				DataItems.Add(new DataItemViewModel { DataType = "fMRI", Global = gvm });
			}
			#endregion
		}

		public void Handle(PermutationCompleteEvent message)
		{
			IsReady = true;
			DataItems.Clear();

			PrimaryValue = "";

			//var regions = _regionService.GetRegionsByIndex();
			var results = _computeService.GetResults();
			int permutations = _computeService.GetPermutations();

			foreach (var graph in results.Graphs)
			{
				var itm = new DataItemViewModel();

				itm.DataType = graph.Name;
				itm.Global = graph.Global;
				itm.PVal = (double)itm.Global.Strength.TwoTailCount / (double)permutations;

				DataItems.Add(itm);

				if (String.IsNullOrWhiteSpace(PrimaryValue))
					PrimaryValue += graph.Name + ": " + itm.PVal.ToString("0.000");
				else
					PrimaryValue += "\n" + graph.Name + ": " + itm.PVal.ToString("0.000");
			}
		}

		public override Type ViewModelType { get { return typeof(GlobalStrengthViewModel); } }

		public BindableCollection<DataItemViewModel> DataItems { get { return _inlDataItems; } set { _inlDataItems = value; NotifyOfPropertyChange(() => DataItems); } } private BindableCollection<DataItemViewModel> _inlDataItems;
	}

	public class DataItemViewModel : Screen
	{
		public string DataType { get; set; }
		public GlobalViewModel Global { get; set; }

		public double PVal { get; set; }
	}
}
