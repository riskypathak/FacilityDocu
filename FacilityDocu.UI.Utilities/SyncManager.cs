
using FacilityDocu.DTO.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using FacilityDocu.DTO;
using FacilityDocu.UI.Utilities.Services;

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
        public void DownloadMasterData()
        {
            string masterDataPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("master.json"));

            if (Helper.isInternetAvailable())
            {
                var client = new RestClient(Data.SYNC_URL_HOST);
                var request = new RestRequest("/Master/All", Method.GET);

                IRestResponse response = client.Execute(request);

                File.WriteAllText(masterDataPath, response.Content);

                Data.MASTER_DATA = JsonConvert.DeserializeObject<IList<MasterDTO>>(response.Content);
            }
            else
            {
                if(File.Exists(masterDataPath))
                {
                    Data.MASTER_DATA = JsonConvert.DeserializeObject<IList<MasterDTO>>(File.ReadAllText(masterDataPath));
                }
            }
        }


        public void Sync()
        {
            Dictionary<int, Dictionary<int, string>> syncProjectActions = IsSyncRequired();
            UpdateProjectXml(syncProjectActions);
        }

        public string UpdateProjectXml(Dictionary<int, Dictionary<int, string>> syncProjectActions)
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
                        //As of now conflict will have server changes preference
                        if (syncProjectActions[projectID].Any(d => d.Value == "conflict"))
                        {
                            //return "Conflict";
                        }

                        ProjectDTO projectDTO = ProjectXmlReader.ReadProjectXml(Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", projectID)), false);

                        IList<int> deleteActionIds = syncProjectActions[projectID].Where(d => d.Value == "delete").Select(d => d.Key).ToList();

                        IList<int> actionsIDsFromServer = syncProjectActions[projectID].Where(d => d.Value == "new" || d.Value == "update" || d.Value == "conflict").Select(d => d.Key).ToArray();

                        IList<ActionDTO> actions = null;

                        if (actionsIDsFromServer.Count > 0)
                        {
                            actions = service.GetProjectActions(projectID, actionsIDsFromServer.ToArray());
                        }

                        Process(actions, deleteActionIds, ref projectDTO);

                        ProjectXmlWriter.Write(projectDTO);
                    }
                }
            }

            Data.SYNC_DOWNLOAD = false;

            return string.Empty;//success
        }

        public static void Process(IList<ActionDTO> actions, IList<int> deleteActionIDs, ref ProjectDTO project)
        {
            foreach (RigTypeDTO rigType in project.RigTypes)
            {
                foreach (ModuleDTO module in rigType.Modules)
                {
                    foreach (StepDTO step in module.Steps)
                    {
                        List<ActionDTO> newActions = new List<ActionDTO>();

                        foreach (ActionDTO action in step.Actions)
                        {
                            ActionDTO newAction = null;

                            if (actions != null && actions.Count > 0)
                            {
                                ActionDTO serverAction = actions.SingleOrDefault(a => a.ActionID == action.ActionID);

                                if (serverAction != null) //update action
                                {
                                    newAction = serverAction;
                                }
                                else
                                {
                                    //no changes
                                    newAction = action;
                                }
                            }
                            else
                            {
                                if (deleteActionIDs.Where(d => d.ToString() == action.ActionID).Count() > 0)
                                {
                                    //delete so dont add
                                }
                                else
                                {
                                    //no changes
                                    newAction = action;
                                }
                            }

                            newActions.Add(newAction);
                        }

                        if (actions != null) //new action
                        {
                            List<ActionDTO> newlyAddedActions = actions.Where(a => a.StepID == step.StepID && !newActions.Contains(a)).ToList();
                            newActions.AddRange(newlyAddedActions);
                        }

                        step.Actions = newActions.ToArray();
                    }
                }
            }
        }

        public ProjectDTO UpdateDatabase(string projectID, bool isInsert)
        {
            string projectPath = Path.Combine(this.ProjectXmlFolderPath, string.Format("{0}.xml", projectID));
            ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

            ProjectDTO updatedProject = service.UpdateProject(project, Data.CURRENT_USER);

            if (updatedProject != null)
            {
                Data.SYNC_DOWNLOAD_UPDATE = true;
                ProjectXmlWriter.Write(updatedProject);
                Data.SYNC_DOWNLOAD_UPDATE = false;

                UploadImages(updatedProject.ProjectID);
                UploadAttachments(updatedProject.ProjectID);

                this.ProjectIDs = new List<int>() { Convert.ToInt32(updatedProject.ProjectID) };

                UpdateProjectXml(null);

                return ProjectXmlReader.ReadProjectXml(Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", updatedProject.ProjectID)), false);
            }
            else
            {
                return null; //need to sync first
            }
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

            Dictionary<int, Dictionary<int, string>> syncProjectActions = new Dictionary<int, Dictionary<int, string>>();
            if (updatedProjectIds.Count > 0)
            {
                syncProjectActions = GetSyncRequiredForProjectActions(updatedProjectIds);
            }

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
                ProjectDTO project = ProjectXmlReader.ReadProjectXml(projectPath, false);

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

            List<ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

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

            List<ActionDTO> actions = project.RigTypes.SelectMany(r => r.Modules).SelectMany(m => m.Steps).SelectMany(s => s.Actions).ToList();

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
