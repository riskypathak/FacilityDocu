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
            Dictionary<int, Dictionary<int, string>> syncProjectActions = IsSyncRequired();
            UpdateProjectXml(syncProjectActions);
        }

        public void UpdateProjectXml(Dictionary<int, Dictionary<int, string>> syncProjectActions)
        {
            Data.SYNC_DOWNLOAD = true;

            if (syncProjectActions == null)
            {
                foreach (int projectID in this.ProjectIDs)
                {
                    ProjectDTO project = service.GetProjectDetails(Convert.ToInt32(projectID));

                    ProjectXmlWriter.Write(project);
                }
            }
            else
            {
                foreach (int projectID in syncProjectActions.Keys)
                {
                    if (syncProjectActions[projectID] == null)
                    {
                        ProjectDTO project = service.GetProjectDetails(Convert.ToInt32(projectID));

                        ProjectXmlWriter.Write(project);
                    }
                    else
                    {
                        
                    }
                }
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

            UpdateProjectXml(null);

            return ProjectXmlReader.ReadProjectXml(Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", updatedProject.ProjectID)), false);
        }

        public Dictionary<int, Dictionary<int, string>> IsSyncRequired()
        {
            DirectoryInfo rootFolder = new DirectoryInfo(this.ProjectXmlFolderPath);
            FileInfo[] Files = rootFolder.GetFiles("*.xml");

            IList<int> projectIDs = new List<int>();

            foreach (FileInfo file in Files)
            {
                string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}", file.Name));
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

                if (!Helper.IsNew(project.ProjectID.ToString()))
                {
                    projectIDs.Add(Convert.ToInt32(project.ProjectID));
                }
            }

            Dictionary<int, string> result = service.IsSync(projectIDs.ToArray(), false);

            DeleteCloseProjects(result.Where(r => r.Value.Equals("closed")).Select(r => r.Key).ToList());

            List<int> newProjects = result.Where(r => r.Value.Equals("new")).Select(r => r.Key).ToList(); //only for new



            List<int> updatedProjectIds = result.Where(r => r.Value.Equals("updated")).Select(r => r.Key).ToList();
            Dictionary<int, Dictionary<int, string>> syncProjectActions = GetSyncRequiredForProjectActions(updatedProjectIds);

            newProjects.ForEach(n => syncProjectActions.Add(n, null));//add new projects as null so we will get all actions for them...

            return syncProjectActions;
        }

        private Dictionary<int, Dictionary<int, string>> GetSyncRequiredForProjectActions(List<int> updatedProjectIds)
        {
            DirectoryInfo rootFolder = new DirectoryInfo(this.ProjectXmlFolderPath);
            FileInfo[] Files = rootFolder.GetFiles("*.xml");

            Dictionary<int, ActionDTO[]> updateProjects = new Dictionary<int, ActionDTO[]>();

            foreach (int projectId in updatedProjectIds)
            {
                string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectId));
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, true);

                List<ActionDTO> actionDTOs = new List<ActionDTO>();

                foreach (RigTypeDTO rigType in project.RigTypes)
                {
                    foreach (ModuleDTO module in rigType.Modules)
                    {
                        foreach (StepDTO step in module.Steps)
                        {
                            foreach (ActionDTO action in step.Actions)
                            {
                                if (!Helper.IsNew(action.ActionID)) //Need to sync only those which are on server
                                {
                                    actionDTOs.Add(new ActionDTO()
                                    {
                                        ActionID = action.ActionID,
                                        LastUpdatedAt = action.LastUpdatedAt,
                                        PublishedAt = action.PublishedAt
                                    });
                                }
                            }
                        }
                    }
                }

                updateProjects.Add(projectId, actionDTOs.ToArray());
            }

            Dictionary<int, Dictionary<int, string>> resultAction = service.SyncRequiredForUpdatedProjects(updateProjects);

            //TODO: foreach()Delete Action

            return resultAction;
        }

        private void DeleteCloseProjects(List<int> projectClosed)
        {
            foreach (int projectID in projectClosed)
            {
                string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));

                if (File.Exists(projectPath))
                {
                    File.Delete(projectPath);
                }
            }
        }

        public void UploadImages(string projectID)
        {
            string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            List<Services.ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

            actions.ForEach(a =>
                {
                    if (a.Images.Count() > 0)
                    {
                        a.Images.ToList().ForEach(i => i.FileByteStream = ReadImage(i.Path));
                        service.UpdateActionImages(a);
                    }
                });
        }

        private byte[] ReadImage(string imagePath)
        {
            return System.IO.File.ReadAllBytes(imagePath);
        }

        public void UploadAttachments(string projectID)
        {
            string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            List<Services.ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

            actions.ForEach(a =>
            {
                if (a.Attachments.Count() > 0)
                {
                    a.Attachments.ToList().ForEach(i => i.FileByteStream = ReadAttachment(i.Path));
                    service.UpdateActionAttachments(a);
                }
            });
        }

        private byte[] ReadAttachment(string attachmentPath)
        {
            return System.IO.File.ReadAllBytes(attachmentPath);
        }
    }
}
