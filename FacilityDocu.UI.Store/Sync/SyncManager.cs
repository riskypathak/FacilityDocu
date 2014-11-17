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
using System.ServiceModel;

namespace Tablet_App
{
    public class SyncManager
    {

        public ObservableCollection<int> ProjectIDs { get; set; }

        private IReadOnlyList<StorageFile> xmlFiles;

        FacilityDocuServiceClient service;

        public async Task Sync()
        {
            IList<int> projectIDS = await IsSyncRequired();

            this.ProjectIDs = new ObservableCollection<int>();
            foreach (int projectID in projectIDS)
            {
                this.ProjectIDs.Add(projectID);
            }

            await UpdateProjectXml();
        }

        private async Task GetAllProjects()
        {
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".xml");

            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(Data.ProjectXmlPath);

            var folderFile = sf.CreateFileQueryWithOptions(queryOptions);
            xmlFiles = await folderFile.GetFilesAsync();
        }

        public async Task<IList<int>> IsSyncRequired()
        {
            await GetAllProjects();

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
            string strUri = "http://tlof.no-ip.biz:9876/FacilityDocu/FacilityDocuService.svc";
            BasicHttpBinding binding = new BasicHttpBinding();
            
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxBufferPoolSize = 2147483647;
            

            binding.ReaderQuotas.MaxDepth = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;
            binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;

            service = new FacilityDocuServiceClient(binding, new EndpointAddress(strUri));
        }

        public SyncManager(IList<int> projectIDs)
            : this()
        {
            foreach (var pID in projectIDs)
            {
                ProjectIDs.Add(pID);
            }
        }

        public async Task UpdateProjectXml()
        {
            foreach (var projectID in ProjectIDs)
            {
                ProjectXmlWriter.Write(service.GetProjectDetailsAsync(projectID).Result);
            }
        }

        public async Task<ProjectDTO> UploadImages(string projectID)
        {
            string projectPath = Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", projectID));

            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            List<ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

            foreach (ActionDTO action in actions)
            {
                if (action.Images.Count > 0)
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

            ProjectDTO projectDTO = await service.GetProjectDetailsAsync(Convert.ToInt32(project.ProjectID));
            return projectDTO;
        }
    }
}
