using FacilityDocu.UI.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.UI.Utilities
{
    public class SyncManager
    {
        public string XmlPath { get; set; }
        public IList<int> projectIDs { get; set; }

        public SyncManager(IList<int> projectIDs)
        {
            this.projectIDs = projectIDs;
        }

        public void UpdateProjectXml()
        {
            IFacilityDocuService service = new FacilityDocuServiceClient();
            ProjectDTO[] projects = service.GetProjectDetails(projectIDs.ToArray());

            projects.ToList().ForEach(p => ProjectXmlWriter.Write(p));
        }

        public void UpdateDatabase()
        {
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(string.Empty);

            IFacilityDocuService service = new FacilityDocuServiceClient();
            service.UpdateProject(project);
        }
    }
}
