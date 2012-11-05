using BrainGraph.Compute.Subjects;
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

		public SubjectViewModel()
		{
			Subjects = new BindableCollection<Subject>();
		}

		protected async override void OnActivate()
		{
			base.OnActivate();

			var subjectService = IoC.Get<ISubjectService>();

			//FileOpenPicker openPicker = new FileOpenPicker();
			//openPicker.ViewMode = PickerViewMode.List;
			////openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			//openPicker.FileTypeFilter.Add(".txt");

			//StorageFile file = await openPicker.PickSingleFileAsync();

			var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

			// acquire file
			var file = await folder.GetFileAsync(@"Assets\Subjects\VA2929.txt");

			if (file != null)
			{
				List<Subject> subs = await subjectService.LoadSubjectFile(file);
				foreach (var sub in subs)
				{
					Subjects.Add(sub);
				}
			}

			FolderPicker adjPicker = new FolderPicker();
			adjPicker.ViewMode = PickerViewMode.List;
			adjPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

			adjPicker.FileTypeFilter.Add(".txt");

			StorageFolder adjFolder = await adjPicker.PickSingleFolderAsync();
			if (adjFolder != null)
			{
				await subjectService.LoadSubjectData(adjFolder, 90);
			}

		}

		public BindableCollection<Subject> Subjects { get { return _inlSubjects; } set { _inlSubjects = value; NotifyOfPropertyChange(() => Subjects); } } private BindableCollection<Subject> _inlSubjects;

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
