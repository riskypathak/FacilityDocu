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
            ObservableCollection<int> projectIDs = new ObservableCollection<int>();
            foreach (StorageFile file in xmlFiles)
            {
                string projectPath = Path.Combine(Data.ProjectXmlPath, file.Name.ToString());
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, true);
                projectIDs.Add(Convert.ToInt32(project.ProjectID));
            }
            var result = await service.IsSyncAsync(projectIDs, true);

            DeleteCloseProjects(result.Where(r => r.Value.Equals("closed")).Select(r => r.Key).ToList());

            return result.Where(r => r.Value.Equals("new") || r.Value.Equals("updated")).Select(r => r.Key).ToList();
        }

        private async void DeleteCloseProjects(List<int> projectClosed)
        {
            foreach (int projectID in projectClosed)
            {
                string projectPath = Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", projectID));

                StorageFolder projectFolder = await StorageFolder.GetFolderFromPathAsync(Data.ProjectXmlPath);

                var item = await projectFolder.TryGetItemAsync(projectPath);

                if (item != null)
                {
                    await item.DeleteAsync();
                }
            }
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
            //binding.CloseTimeout = TimeSpan.FromMinutes(5);
            //binding.OpenTimeout = TimeSpan.FromMinutes(5);
            //binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
            //binding.SendTimeout = TimeSpan.FromMinutes(5);
            service = new FacilityDocuServiceClient(binding, new EndpointAddress(strUri));

            this.ProjectIDs = new ObservableCollection<int>();
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
            Data.SYNC_PROCESS = true;
            foreach (var projectID in ProjectIDs)
            {
                ProjectXmlWriter.Write(service.GetProjectDetailsAsync(projectID).Result);
            }
            Data.SYNC_PROCESS = false;
        }

        public async Task<ProjectDTO> Publish(string projectID)
        {
            Data.PUBLISH_PROCESS = true;

            string projectPath = Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);
            var actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions);
            foreach (ActionDTO action in actions)
            {
                if (action.Images.Count > 0)
                {
                    foreach (ImageDTO image in action.Images.Where(i => !string.IsNullOrEmpty(i.Path)))
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(image.Path);
                        var stream = await file.OpenReadAsync();
                        using (var dataReader = new DataReader(stream))
                        {
                            var bytes = new byte[stream.Size];
                            await dataReader.LoadAsync((uint)stream.Size);
                            dataReader.ReadBytes(bytes);
                            image.FileByteStream = bytes;
                        }
                    }
                    try
                    {
                        Dictionary<string, int> dic = await service.UpdateActionImagesAsync(action);

                        foreach (KeyValuePair<string, int> kv in dic)
                        {
                            ImageDTO img = action.Images.SingleOrDefault(i => i.ImageID == kv.Key);

                            if (img != null)
                            {
                                StorageFile file = await StorageFile.GetFileFromPathAsync(img.Path);

                                img.ImageID = kv.Value.ToString();

                                //Task taskDelete;
                                bool isExists = false;
                                try
                                {
                                    var newFilePath = Path.Combine(Path.GetDirectoryName(img.Path), img.ImageID + ".jpg");
                                    var item = await StorageFile.GetFileFromPathAsync(newFilePath);
                                    if (item != null)
                                    {
                                        isExists = true;
                                        //await item.RenameAsync(img.ImageID + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Lets assume no file so no need to delete it and lets eat exception
                                }

                                if (!isExists)
                                {
                                    await file.RenameAsync(img.ImageID + ".jpg");
                                }
                            }
                        }
                    }
                    catch (FaultException exception)
                    {
                        throw exception;
                    }
                }
            }

            await ProjectXmlWriter.Write(project);

            ProjectDTO projectDTO = ProjectXmlReader.ReadProjectXml(Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", projectID)), false);

            Data.PUBLISH_PROCESS = false;
            return projectDTO;
        }
    }
}