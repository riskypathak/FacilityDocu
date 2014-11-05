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
        public string XmlPath { get; set; }
        public IList<int> ProjectIDs { get; set; }

        public SyncManager(IList<int> projectIDs)
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

            this.XmlPath = Path.GetFullPath("Data\\ProjectXml");
        }

        public void UpdateProjectXml()
        {
            IFacilityDocuService service = new FacilityDocuServiceClient();
            ProjectDTO[] projects = service.GetProjectDetails(ProjectIDs.ToArray());

            projects.ToList().ForEach(p => ProjectXmlWriter.Write(p, XmlPath));
        }

        public void UpdateDatabase()
        {
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(string.Empty);

            IFacilityDocuService service = new FacilityDocuServiceClient();
            service.UpdateProject(project);
        }
    }
}
