using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;

namespace BrainGraph.WinStore.Screens.Subjects
{
	public class SubjectViewModel : ViewModelBase
	{

		protected async override void OnActivate()
		{
			base.OnActivate();

			FileOpenPicker openPicker = new FileOpenPicker();
			openPicker.ViewMode = PickerViewMode.List;
			//openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			openPicker.FileTypeFilter.Add(".txt");

			StorageFile file = await openPicker.PickSingleFileAsync();
			if (file != null)
			{
				// Application now has read/write access to the picked file
				var text = "Picked photo: " + file.Name;

				var subjectService = IoC.Get<ISubjectService>();
				subjectService.LoadSubjectFile(file);
			}
		}

		//internal bool EnsureUnsnapped()
		//{
		//	// FilePicker APIs will not work if the application is in a snapped state.
		//	// If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
		//	bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
		//	if (!unsnapped)
		//	{
		//		NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
		//	}

		//	return unsnapped;
		//}
	}
}
