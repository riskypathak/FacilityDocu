using FacilityDocu.UI.Utilities.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.UI.Utilities
{
    public class SyncManager
    {
        public string ProjectXmlFolderPath { get; set; }
        public IList<int> ProjectIDs { get; set; }

        public SyncManager(IList<int> projectIDs): this()
        {
            this.ProjectIDs = projectIDs;

            if (!Directory.Exists(Path.GetFullPath("Data")))
            {
                Directory.CreateDirectory("Data");
            }
            if (!Directory.Exists(Path.GetFullPath("Data\\ProjectXml")))
            {
                Directory.CreateDirectory("Data\\ProjectXml");
            }
        }

        public SyncManager()
        {
            this.ProjectXmlFolderPath = Path.GetFullPath("Data\\ProjectXml");
        }

        public void UpdateProjectXml()
        {
            IFacilityDocuService service = new FacilityDocuServiceClient();
            ProjectDTO[] projects = service.GetProjectDetails(ProjectIDs.ToArray());

            projects.ToList().ForEach(p => ProjectXmlWriter.Write(p, ProjectXmlFolderPath));
        }

        public void UpdateDatabase(int projectID, bool isInsert)
        {
            string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            if (isInsert)
            {
                project.ProjectID = "0";
            }

            IFacilityDocuService service = new FacilityDocuServiceClient();
            service.UpdateProject(project);
        }

        public IList<int> IsSyncRequired()
        {
            DirectoryInfo rootFolder = new DirectoryInfo(this.ProjectXmlFolderPath);
            FileInfo[] Files = rootFolder.GetFiles("*.xml");

            Dictionary<int, DateTime> projectIDs = new Dictionary<int,DateTime>();

            foreach (FileInfo file in Files)
            {
                string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}", file.Name));
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

                projectIDs.Add(Convert.ToInt32(project.ProjectID), project.LastUpdatedAt);
            }

            IFacilityDocuService service = new FacilityDocuServiceClient();
            Dictionary<int, bool> result = service.IsSync(projectIDs);

            return result.Where(r => r.Value).Select(r => r.Key).ToList();
        }
    }
}
