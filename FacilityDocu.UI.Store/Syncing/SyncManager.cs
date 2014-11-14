using Tablet_App.ServiceReference1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Storage;
using Windows.Storage.Search;
using System.Collections.ObjectModel;

namespace Tablet_App
{
    public class SyncManager
    {

        public ObservableCollection<int> ProjectIDs { get; set; }

        static IReadOnlyList<StorageFile> selectFiles;
        Dictionary<int, bool> result;
        Dictionary<int, DateTime> projectIDs;
        FacilityDocuServiceClient service;

        public async void getXML()
        {
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".xml");

            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(Data.ProjectXmlPath + "\\ProjectXML");

            var folderFile = sf.CreateFileQueryWithOptions(queryOptions);
            selectFiles = await folderFile.GetFilesAsync();
        }
        public IList<int> IsSyncRequired()
        {

            // DirectoryInfo rootFolder = new DirectoryInfo(this.ProjectXmlFolderPath);
            //  FileInfo[] Files = rootFolder.GetFiles("*.xml");

            getXML();

            Dictionary<int, DateTime> projectIDs = new Dictionary<int, DateTime>();

            foreach (StorageFile file in selectFiles)
            {
                string projectPath = Path.Combine(Data.ProjectXmlPath + "\\ProjectXML", file.Name.ToString());
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);
                projectIDs.Add(Convert.ToInt32(project.ProjectID), project.LastUpdatedAt);

            }

            checkProjectIDs();

            return result.Where(r => r.Value).Select(r => r.Key).ToList();
        }
        public async void checkProjectIDs()
        {
            result = await service.IsSyncAsync(projectIDs);
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
            // this.ProjectIDs = projectIDs;
        }
        public async void UpdateProjectXml()
        {

            //get Projects From Database to be update
            ObservableCollection<ProjectDTO> projects = await service.GetProjectDetailsAsync(ProjectIDs);

            // List<ProjectDTO> projects = new List<ProjectDTO>();

            foreach (var selectP in projects)
            {
                ProjectXmlWriter.Write(selectP);
            }

        }
        public async void UpdateDatabase(int projectID, bool isInsert)
        {
            //string projectPath = Path.Combine(OfflineData.xmlp, string.Format("Data\\{0}.xml", projectID));
            //ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            //if (isInsert)
            //{
            //    project.ProjectID = "0";
            //}

            await service.UAsync();
        }


    }
}
