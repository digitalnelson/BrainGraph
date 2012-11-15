using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainGraph.Storage;
using Caliburn.Micro;
using BrainGraph.WinStore.Services.Loaders;
using Windows.Storage;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Models;

namespace BrainGraph.WinStore.Services
{
	public interface IRegionService
	{
		Task Load(StorageFile roiFile);
		
		List<ROI> GetRegionsByIndex();
		int GetNodeCount();
		int GetEdgeCount();
	}

	public class RegionService : IRegionService
	{
		private readonly IEventAggregator _eventAggregator;

		private List<ROI> _regionsOfInterest;
		private Dictionary<int, ROI> _regionsOfInterestByIndex;

		public RegionService()
		{
			_eventAggregator = IoC.Get<IEventAggregator>();

			_regionsOfInterest = new List<ROI>();
			_regionsOfInterestByIndex = new Dictionary<int, ROI>();
		}

		public async Task Load(StorageFile roiFile)
		{
			var rois = await ROILoader.Load(roiFile);

			foreach (var roi in rois)
			{
				_regionsOfInterest.Add(roi);
				_regionsOfInterestByIndex[roi.Index] = roi;
			}

			_eventAggregator.Publish(new RegionsLoadedEvent());
		}

		public List<ROI> GetRegionsByIndex()
		{
			return _regionsOfInterest.OrderBy(r => r.Index).ToList();
		}

		public int GetNodeCount()
		{
			return _regionsOfInterest.Count;
		}

		public int GetEdgeCount()
		{
			return (_regionsOfInterest.Count * (_regionsOfInterest.Count - 1)) / 2;
		}
	}
}
