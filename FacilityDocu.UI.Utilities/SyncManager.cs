using FacilityDocu.UI.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        private IFacilityDocuService service;

        public SyncManager(IList<int> projectIDs)
            : this()
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

            if (!Directory.Exists(Path.GetFullPath("Data\\Images")))
            {
                Directory.CreateDirectory("Data\\Images");
            }
        }

        public SyncManager()
        {
            service = new FacilityDocuServiceClient();
            this.ProjectXmlFolderPath = Path.GetFullPath("Data\\ProjectXml");
        }

        public void Sync()
        {
            this.ProjectIDs = IsSyncRequired();
            UpdateProjectXml();
        }

        public void UpdateProjectXml()
        {
            Data.SYNC_DOWNLOAD = true;
            foreach (int projectID in this.ProjectIDs)
            {
                ProjectDTO project = service.GetProjectDetails(Convert.ToInt32(projectID));

                ProjectXmlWriter.Write(project);
            }

            Data.SYNC_DOWNLOAD = false;
        }

        public ProjectDTO UpdateDatabase(string projectID, bool isInsert)
        {
            string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            ProjectDTO updatedProject = service.UpdateProject(project);

            Data.SYNC_DOWNLOAD_UPDATE = true;
            ProjectXmlWriter.Write(updatedProject);
            Data.SYNC_DOWNLOAD_UPDATE = false;

            UploadImages(updatedProject.ProjectID);
            UploadAttachments(updatedProject.ProjectID);

            this.ProjectIDs = new List<int>() { Convert.ToInt32(updatedProject.ProjectID) };

            UpdateProjectXml();

            return ProjectXmlReader.ReadProjectXml(Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml",updatedProject.ProjectID)),false);
        }

        public IList<int> IsSyncRequired()
        {
            DirectoryInfo rootFolder = new DirectoryInfo(this.ProjectXmlFolderPath);
            FileInfo[] Files = rootFolder.GetFiles("*.xml");

            Dictionary<int, DateTime> projectIDs = new Dictionary<int, DateTime>();

            foreach (FileInfo file in Files)
            {
                string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}", file.Name));
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

                if (!Helper.IsNew(project.ProjectID.ToString()))
                {
                    projectIDs.Add(Convert.ToInt32(project.ProjectID), project.LastUpdatedAt);
                }
            }

            Dictionary<int, bool> result = service.IsSync(projectIDs);

            return result.Where(r => r.Value).Select(r => r.Key).ToList();
        }

        public void UploadImages(string projectID)
        {
            string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            List<Services.ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

            actions.ForEach(a =>
                {
                    a.Images.ToList().ForEach(i => i.FileByteStream = ReadImage(i.Path));
                    service.UpdateActionImages(a);
                });
        }

        private byte[] ReadImage(string imagePath)
        {
            byte[] bytes = null;

            Image image = Image.FromFile(imagePath);
            using (MemoryStream stream = new MemoryStream())
            {
                // Save image to stream.
                image.Save(stream, ImageFormat.Jpeg);
                bytes = stream.ToArray();
            }

            return bytes;
        }

        public void UploadAttachments(string projectID)
        {
            string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            List<Services.ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

            actions.ForEach(a =>
            {
                a.Attachments.ToList().ForEach(i => i.FileByteStream = ReadAttachment(i.Path));
               service.UpdateActionAttachments(a);
            });
        }

        private byte[] ReadAttachment(string attachmentPath)
        {
            return System.IO.File.ReadAllBytes(attachmentPath);
        }
    }
}
