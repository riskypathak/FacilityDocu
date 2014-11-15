using Tablet_App.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Storage;
using Windows.Storage.Search;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;

namespace Tablet_App
{
    public class SyncManager
    {

        public ObservableCollection<int> ProjectIDs { get; set; }

        private IReadOnlyList<StorageFile> xmlFiles;

        FacilityDocuServiceClient service;

        private async void GetAllProjects()
        {
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".xml");

            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(Data.ProjectXmlPath);

            var folderFile = sf.CreateFileQueryWithOptions(queryOptions);
            xmlFiles = await folderFile.GetFilesAsync();
        }

        public IList<int> IsSyncRequired()
        {
            GetAllProjects();

            Dictionary<int, DateTime> projectIDs = new Dictionary<int, DateTime>();

            foreach (StorageFile file in xmlFiles)
            {
                string projectPath = Path.Combine(Data.ProjectXmlPath, file.Name.ToString());
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, true);
                projectIDs.Add(Convert.ToInt32(project.ProjectID), project.LastUpdatedAt);
            }

            var result = service.IsSyncAsync(projectIDs);

            return result.Result.Where(r => r.Value).Select(r => r.Key).ToList();
        }

        public SyncManager()
        {
            service = new FacilityDocuServiceClient();
        }

        public SyncManager(IList<int> projectIDs)
            : this()
        {
            foreach (var pID in projectIDs)
            {
                ProjectIDs.Add(pID);
            }
        }

        public async void UpdateProjectXml()
        {

            //get Projects From Database to be update

            ObservableCollection<ProjectDTO> projects = await service.GetProjectDetailsAsync(ProjectIDs);

            foreach (var project in projects)
            {
                ProjectXmlWriter.Write(project);
            }
        }

        public async void UploadImages(string projectID)
        {
            string projectPath = Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", projectID));

            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            List<ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

            foreach (ActionDTO action in actions)
            {
                foreach (ImageDTO image in action.Images)
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(image.Path);
                    using (IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                    {
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                        PixelDataProvider pixelData = await decoder.GetPixelDataAsync();

                        image.FileByteStream = pixelData.DetachPixelData();
                    }
                }
                await service.UpdateActionImagesAsync(action);
            }
        }
    }
}
