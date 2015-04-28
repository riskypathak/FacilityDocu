using FacilityDocu.Data;
using FacilityDocu.DTO;
using FacilityDocu.Services.EntityDTOConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FacilityDocu.Services
{
    public class FacilityDocuService : IFacilityDocuService
    {
        public Dictionary<int, string> IsSync(List<int> inputProjects, bool fromTablet)
        {
            Dictionary<int, string> projectStatusData = new Dictionary<int, string>();

            using (var context = new TabletApp_DatabaseEntities())
            {
                foreach (var project in context.Projects)
                {
                    if (fromTablet && project.Template)
                    {
                        continue;
                    }

                    var inputProject = inputProjects.Where(i => i == project.ProjectID);

                    if (inputProject.Count() <= 0)
                    {
                        projectStatusData.Add(project.ProjectID, "new");
                    }
                    else if (project.Close.HasValue && project.Close.Value)
                    {
                        projectStatusData.Add(project.ProjectID, "closed");
                    }
                    else 
                    {
                        if (!project.Template)//only needs to be updated if not template....  else template will be only added at once
                        {
                            projectStatusData.Add(project.ProjectID, "updated");
                        }
                    }
                }
            }

            return projectStatusData;
        }

        public ProjectDTO GetProjectDetails(int projectID)
        {
            ProjectDTO projectDTO = new ProjectDTO();

            using (var _db = new TabletApp_DatabaseEntities())
            {
                EntityConverter.AllResources = EntityConverter.ToResourceDTO(_db.Resources);

                var _currentProject = _db.Projects
                    .FirstOrDefault(x => x.ProjectID == projectID);
                if (_currentProject != null)
                {
                    projectDTO = EntityConverter.ToProjectDTO(_currentProject);
                }
            }

            return projectDTO;
        }

        public bool Login(string userName, string password)
        {
            bool isAuthenticated = false;

            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                isAuthenticated = context.Users.Where(u => u.UserName.Equals(userName) && u.Password.Equals(password)).Count() > 0 ? true : false;
            }

            return isAuthenticated;
        }

        public ProjectDTO UpdateProject(ProjectDTO projectDTO, string userName)
        {
            ProjectDTO updatedProjectDTO = null;

            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                if (Helper.IsNew(projectDTO.ProjectID))
                {
                    Project project = DTOConverter.ToProject(projectDTO, userName);
                    project.User = context.Users.Single(u => u.UserName.Equals(projectDTO.CreatedBy.UserName));

                    context.Projects.Add(project);
                    context.SaveChanges();

                    updatedProjectDTO = GetProjectDetails(project.ProjectID);

                }
                else
                {
                    Project project = DTOConverter.ToProject(projectDTO, userName);

                    int projectID = Convert.ToInt32(projectDTO.ProjectID);

                    Project existingProject = context.Projects.SingleOrDefault(p => p.ProjectID.Equals(projectID));

                    existingProject.Close = project.Close;

                    List<ActionDTO> clientActionsDTO = projectDTO.RigTypes.SelectMany(r => r.Modules.SelectMany(m => m.Steps.SelectMany(s => s.Actions.Select(a => a)))).ToList();

                    bool conflictResult = UpdateProjectDetail(project, existingProject, clientActionsDTO);

                    if(!conflictResult)
                    {
                        return null;
                    }

                    context.SaveChanges();

                    updatedProjectDTO = GetProjectDetails(project.ProjectID);
                }
            }

            return updatedProjectDTO;
        }

        private bool UpdateProjectDetail(Project clientProject, Project databaseProject, List<ActionDTO> clientActionsDTO)
        {
            foreach (ProjectDetail dbAction in databaseProject.ProjectDetails.ToList())
            {
                ProjectDetail clientAction = clientProject.ProjectDetails.FirstOrDefault(p => p.ProjectDetailID == dbAction.ProjectDetailID);

                if (clientAction != null)
                {
                    ProjectDetail databaseAction = databaseProject.ProjectDetails.Single(a => a.ProjectDetailID == clientAction.ProjectDetailID); //took a new one as we cannot change forach action

                    ActionDTO clientActionDTO = clientActionsDTO.Single(c => c.ActionID == clientAction.ProjectDetailID.ToString());

                    if (databaseAction.PublishedDate == clientActionDTO.PublishedAt) // no conflict
                    {
                        if(clientActionDTO.PublishedAt == clientActionDTO.LastUpdatedAt) //no change do nothing
                        {

                        }
                        else if (clientActionDTO.PublishedAt < clientActionDTO.LastUpdatedAt) //update
                        {
                            databaseAction.PublishedDate = DateTime.Now.ToUniversalTime();
                            databaseAction.PublishedBy = clientAction.PublishedBy;

                            databaseAction.Dimensions = clientAction.Dimensions;
                            databaseAction.LiftingGears = clientAction.LiftingGears;
                            databaseAction.Risks = clientAction.Risks;

                            databaseAction.ActionName = clientAction.ActionName;
                            databaseAction.Description = clientAction.Description;

                            databaseAction.ActionDescriptionWarning = clientAction.ActionDescriptionWarning;
                            databaseAction.ActionNameWarning = clientAction.ActionNameWarning;
                            databaseAction.ImportantActionDescription = clientAction.ImportantActionDescription;
                            databaseAction.ImportantActionname = clientAction.ImportantActionname;

                            databaseAction.IsAnalysis = clientAction.IsAnalysis;

                            UpdateActionTool(clientAction, dbAction);
                            UpdateActionResource(clientAction, dbAction);
                            UpdateActionRiskAnalysis(clientAction, dbAction);
                            UpdateActionImages(clientAction, dbAction);
                            UpdateActionAttachments(clientAction, dbAction);
                        }
                        else
                        {
                            //nothing
                        }
                    }
                    else //Conflict
                    {
                        //it means that someone already published new version. So it will ask user to sync first.
                        return false;
                    }
                }
                else
                {
                    databaseProject.ProjectDetails.Remove(databaseProject.ProjectDetails.Single(p => p.ProjectDetailID == dbAction.ProjectDetailID));
                }
            }

            List<ProjectDetail> newProjectDetails = clientProject.ProjectDetails.Where(p => Helper.IsNew(p.ProjectDetailID.ToString())).ToList();
            newProjectDetails.ForEach(np => databaseProject.ProjectDetails.Add(np));

            return true;
        }

        private void UpdateActionImages(ProjectDetail updateProject, ProjectDetail existingPD)
        {
            foreach (ProjectActionImage existingPAI in existingPD.ProjectActionImages.ToList())
            {
                ProjectActionImage updatePAI = updateProject.ProjectActionImages.FirstOrDefault(p => p.ImageID == existingPAI.ImageID);

                if (updatePAI != null)
                {
                    ProjectActionImage modifyPAI = existingPD.ProjectActionImages.Single(a => a.ImageID == updatePAI.ImageID);

                    modifyPAI.Image.CreationDate = updatePAI.Image.CreationDate;
                    modifyPAI.Image.Description = updatePAI.Image.Description;
                    modifyPAI.Image.ImageComments = updatePAI.Image.ImageComments;
                    modifyPAI.Image.ImagePath = updatePAI.Image.ImagePath;
                    modifyPAI.Image.Tags = updatePAI.Image.Tags;
                }
                else
                {
                    existingPD.ProjectActionImages.Remove(existingPD.ProjectActionImages.Single(p => p.ImageID == existingPAI.ImageID));
                }
            }

            List<ProjectActionImage> newRA = updateProject.ProjectActionImages.Where(p => Helper.IsNew(p.ImageID.ToString())).ToList();
            newRA.ForEach(np => existingPD.ProjectActionImages.Add(np));
        }

        private void UpdateActionAttachments(ProjectDetail updateProject, ProjectDetail existingPD)
        {
            foreach (ProjectActionAttachment existingPAI in existingPD.ProjectActionAttachments.ToList())
            {
                ProjectActionAttachment updatePAI = updateProject.ProjectActionAttachments.FirstOrDefault(p => p.AttachmentID == existingPAI.AttachmentID);

                if (updatePAI != null)
                {
                    ProjectActionAttachment modifyPAI = existingPD.ProjectActionAttachments.Single(a => a.AttachmentID == updatePAI.AttachmentID);

                    modifyPAI.Attachment.Name = updatePAI.Attachment.Name;
                    modifyPAI.Attachment.Path = updatePAI.Attachment.Path;
                }
                else
                {
                    existingPD.ProjectActionAttachments.Remove(existingPD.ProjectActionAttachments.Single(p => p.AttachmentID == existingPAI.AttachmentID));
                }
            }

            List<ProjectActionAttachment> newRA = updateProject.ProjectActionAttachments.Where(p => Helper.IsNew(p.AttachmentID.ToString())).ToList();
            newRA.ForEach(np => existingPD.ProjectActionAttachments.Add(np));
        }

        private void UpdateActionRiskAnalysis(ProjectDetail updateProject, ProjectDetail existingPD)
        {
            foreach (RiskAnalysi existingRA in existingPD.RiskAnalysis.ToList())
            {
                RiskAnalysi updateRA = updateProject.RiskAnalysis.FirstOrDefault(p => p.RiskAnalysisID == existingRA.RiskAnalysisID);

                if (updateRA != null)
                {
                    RiskAnalysi modifyRA = existingPD.RiskAnalysis.Single(a => a.RiskAnalysisID == updateRA.RiskAnalysisID);

                    modifyRA.Activity = updateRA.Activity;
                    modifyRA.B = updateRA.B;
                    modifyRA.B_ = updateRA.B_;
                    modifyRA.Controls = updateRA.Controls;
                    modifyRA.Danger = updateRA.Danger;
                    modifyRA.E = updateRA.E;
                    modifyRA.E_ = updateRA.E_;
                    modifyRA.K = updateRA.K;
                    modifyRA.K_ = updateRA.K_;
                    modifyRA.Risk = updateRA.Risk;
                    modifyRA.Risk_ = updateRA.Risk_;

                }
                else
                {
                    existingPD.RiskAnalysis.Remove(existingPD.RiskAnalysis.Single(p => p.RiskAnalysisID == existingRA.RiskAnalysisID));
                }
            }

            List<RiskAnalysi> newRA = updateProject.RiskAnalysis.Where(p => Helper.IsNew(p.RiskAnalysisID.ToString())).ToList();
            newRA.ForEach(np => existingPD.RiskAnalysis.Add(np));
        }

        private void UpdateActionTool(ProjectDetail updateProject, ProjectDetail existingPD)
        {
            foreach (ProjectActionTool existingActionTool in existingPD.ProjectActionTools.ToList())
            {
                ProjectActionTool updateActionTool = updateProject.ProjectActionTools.FirstOrDefault(t => t.ToolID == existingActionTool.ToolID);

                if (updateActionTool != null)
                {
                }
                else
                {
                    existingPD.ProjectActionTools.Remove(existingPD.ProjectActionTools.Single(p => p.ProjectActionToolID == existingActionTool.ProjectActionToolID));
                }
            }

            foreach (ProjectActionTool updateActionTool in updateProject.ProjectActionTools.ToList())
            {
                ProjectActionTool newActionTool = existingPD.ProjectActionTools.FirstOrDefault(t => t != null && t.ToolID == updateActionTool.ToolID);

                if (newActionTool != null)
                {
                }
                else
                {
                    existingPD.ProjectActionTools.Add(updateActionTool);
                }
            }
        }

        private void UpdateActionResource(ProjectDetail updateProject, ProjectDetail existingPD)
        {
            foreach (ProjectActionResource existingActionResource in existingPD.ProjectActionResources.ToList())
            {
                ProjectActionResource updateActionResource = updateProject.ProjectActionResources.FirstOrDefault(t => t.ResourceID == existingActionResource.ResourceID);

                if (updateActionResource != null)
                {
                    ProjectActionResource modifyActionResource = existingPD.ProjectActionResources.Single(r => r.ResourceID == updateActionResource.ResourceID);

                    modifyActionResource.ResourceCount = updateActionResource.ResourceCount;

                }
                else
                {
                    existingPD.ProjectActionResources.Remove(existingPD.ProjectActionResources.Single(p => p.ProjectActionResourceID == existingActionResource.ProjectActionResourceID));
                }
            }

            foreach (ProjectActionResource updateActionResource in updateProject.ProjectActionResources.ToList())
            {
                ProjectActionResource newActionResource = existingPD.ProjectActionResources.FirstOrDefault(t => t != null && t.ResourceID == updateActionResource.ResourceID);

                if (newActionResource != null)
                {
                }
                else
                {
                    existingPD.ProjectActionResources.Add(updateActionResource);
                }
            }
        }

        public Dictionary<string, int> UpdateActionImages(ActionDTO action)
        {
            int actionID = Convert.ToInt32(action.ActionID);

            Dictionary<string, int> dic = new Dictionary<string, int>();


            int tempId = 0;
            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                foreach (ImageDTO imageDTO in action.Images)
                {
                    //insert
                    if (Helper.IsNew(imageDTO.ImageID))
                    {

                        ProjectActionImage projectImage = new ProjectActionImage();
                        projectImage.ProjectDetailID = actionID;

                        projectImage.Image = new Image()
                        {
                            ImageID = Helper.GetUniqueID(),
                            Description = imageDTO.Description,
                            ImagePath = imageDTO.Path,
                            CreationDate = imageDTO.CreationDate,
                            Tags = string.Join(";", imageDTO.Tags.ToArray()),
                        };

                        foreach (CommentDTO commentDTO in imageDTO.Comments)
                        {
                            ImageComment imgComment = new ImageComment()
                            {
                                ImageCommentID = Helper.GetUniqueID(),
                                CreationDate = commentDTO.CommentedAt,
                                ImageID = projectImage.Image.ImageID,
                                Text = commentDTO.Text,
                            };

                            projectImage.Image.ImageComments.Add(imgComment);
                        }

                        context.ProjectActionImages.Add(projectImage);

                        context.SaveChanges();
                        projectImage.Image.ImagePath = SaveImageToFile(projectImage.Image.ImageID, imageDTO);
                        context.SaveChanges();

                        dic.Add(imageDTO.ImageID, projectImage.Image.ImageID);

                    }
                    else
                    {
                        int imageID = Convert.ToInt32(imageDTO.ImageID);

                        //update
                        Image img = context.Images.First(x => x.ImageID == imageID);



                        img.Description = imageDTO.Description;
                        img.Tags = String.Join(",", imageDTO.Tags);

                        context.ImageComments.RemoveRange(context.ImageComments.Where(x => x.ImageID.Value == imageID));

                        foreach (CommentDTO commnet in imageDTO.Comments)
                        {
                            ImageComment imgComment = new ImageComment()
                            {
                                CreationDate = commnet.CommentedAt,
                                ImageID = img.ImageID,
                                Text = commnet.Text,
                                ImageCommentID = tempId--
                            };
                            img.ImageComments.Add(imgComment);
                        }

                        img.ImagePath = SaveImageToFile(img.ImageID, imageDTO);

                        context.SaveChanges();

                        dic.Add(imageDTO.ImageID, imageID);
                    }
                }
            }
            return dic;
        }

        private string GetImageActualPath(string imagePath)
        {
            int lastIndex = System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().LastIndexOf('/');
            return string.Format("{0}/Data/Images/{1}", System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().Substring(0, lastIndex), imagePath);
        }

        private string SaveImageToFile(int imageId, ImageDTO item)
        {
            //image extension as for now using jpg
            string extension = "jpg";
            string dbImagePath = GetImageActualPath(string.Format("{0}.{1}", imageId, extension));

            string filePath = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/Data/Images/{0}.{1}", imageId, extension));

            using (Stream file = File.Create(filePath))
            {
                file.Write(item.FileByteStream, 0, item.FileByteStream.Length);
            }

            return dbImagePath;
        }

        public void UpdateActionAttachments(ActionDTO action)
        {
            int actionID = Convert.ToInt32(action.ActionID);

            int tempId = 0;
            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                foreach (AttachmentDTO attachmentDTO in action.Attachments)
                {
                    //insert
                    if (Helper.IsNew(attachmentDTO.AttachmentID))
                    {
                        ProjectActionAttachment projectAttachment = new ProjectActionAttachment();
                        projectAttachment.ProjectDetailID = actionID;

                        projectAttachment.Attachment = new Attachment()
                        {
                            AttachmentID = Helper.GetUniqueID(),
                            Name = attachmentDTO.Name,
                        };

                        context.ProjectActionAttachments.Add(projectAttachment);

                        context.SaveChanges();
                        projectAttachment.Attachment.Path = SaveAttachmentToFile(projectAttachment.Attachment.AttachmentID, attachmentDTO);
                        context.SaveChanges();
                    }
                    else
                    {
                        int AttachmentID = Convert.ToInt32(attachmentDTO.AttachmentID);

                        //update
                        Attachment attachment = context.Attachments.First(x => x.AttachmentID == AttachmentID);

                        attachment.Name = attachmentDTO.Name;

                        attachment.Path = SaveAttachmentToFile(attachment.AttachmentID, attachmentDTO);

                        context.SaveChanges();
                    }
                }
            }
        }

        private string GetAttachmentActualPath(string AttachmentPath)
        {
            int lastIndex = System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().LastIndexOf('/');
            return string.Format("{0}/Data/Attachments/{1}", System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().Substring(0, lastIndex), AttachmentPath);
        }

        private string SaveAttachmentToFile(int AttachmentId, AttachmentDTO item)
        {
            //Attachment extension as for now using jpg
            string extension = "pdf";
            string dbAttachmentPath = GetAttachmentActualPath(string.Format("{0}.{1}", AttachmentId, extension));

            string filePath = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/Data/Attachments/{0}.{1}", AttachmentId, extension));

            using (FileStream fileStream = File.Create(filePath))
            {
                fileStream.Write(item.FileByteStream, 0, item.FileByteStream.Length);
            }

            return dbAttachmentPath;
        }

        public IList<ToolDTO> GetTools()
        {
            IList<ToolDTO> tools = new List<ToolDTO>();

            using (var context = new TabletApp_DatabaseEntities())
            {
                context.Tools.ToList().ForEach(t => tools.Add(new ToolDTO() { ToolID = t.ToolID.ToString(), Name = t.ToolName }));
            }

            return tools;
        }

        public void CreateTemplate(ProjectDTO projectDTO)
        {
            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                Project project = DTOConverter.ToProject(projectDTO, string.Empty, false);
                project.User = context.Users.Single(u => u.UserName.Equals(projectDTO.CreatedBy.UserName));

                context.Projects.Add(project);

                foreach (RigTypeDTO rigDTO in projectDTO.RigTypes)
                {
                    foreach (ModuleDTO moduleDTO in rigDTO.Modules)
                    {
                        Module module = new Module();
                        module.ModuleID = Helper.GetUniqueID();
                        module.ModuleName = moduleDTO.Name;
                        module.RigTypeID = Convert.ToInt32(rigDTO.RigTypeID);
                        //context.Modules.Add(module);

                        foreach (StepDTO stepDTO in moduleDTO.Steps)
                        {
                            Step step = new Step();
                            step.StepID = Helper.GetUniqueID();
                            step.StepName = stepDTO.Name;
                            //step.ModuleID = module.ModuleID;
                            step.Module = module;

                            //context.Steps.Add(step);

                            foreach (ActionDTO actionDTO in stepDTO.Actions)
                            {
                                ProjectDetail projectDetail = new ProjectDetail();
                                projectDetail.ActionName = actionDTO.Name;
                                projectDetail.Description = actionDTO.Description;
                                projectDetail.ProjectID = project.ProjectID;
                                //projectDetail.StepID = step.StepID;
                                projectDetail.Step = step;

                                context.ProjectDetails.Add(projectDetail);
                            }
                        }
                    }
                }

                context.SaveChanges();
            }
        }

        public Dictionary<int, Dictionary<int, string>> SyncRequiredForUpdatedProjects(Dictionary<int, List<ActionDTO>> projectActionDTOs)
        {
            Dictionary<int, Dictionary<int, string>> listActions = new Dictionary<int, Dictionary<int, string>>();

            using (var context = new TabletApp_DatabaseEntities())
            {
                foreach (int projectId in projectActionDTOs.Keys)
                {
                    List<string> serverActionIds = context.ProjectDetails.Where(p => p.ProjectID == projectId).Select(p => p.ProjectDetailID).ToList().Select(i=>i.ToString()).ToList();
                    List<string> clientActionIds = projectActionDTOs[projectId].Select(p => p.ActionID).ToList();

                    Dictionary<int, string> actionDictionary = new Dictionary<int, string>();

                    //It means action is on server but not on client so we need to add it on client
                    serverActionIds.Except<string>(clientActionIds).ToList().ForEach(a => actionDictionary.Add(int.Parse(a), "new"));

                    //It means action is on client but not on server so we need to delete it from client
                    clientActionIds.Except<string>(serverActionIds).ToList().ForEach(a => actionDictionary.Add(int.Parse(a), "delete"));

                    List<int> commonActionIds = serverActionIds.Intersect<string>(clientActionIds).Select(a => int.Parse(a)).ToList();

                    foreach (int actionId in commonActionIds)
                    {
                        ProjectDetail serverAction = context.ProjectDetails.Single(p => p.ProjectID == projectId && p.ProjectDetailID == actionId);
                        ActionDTO clientAction = projectActionDTOs[projectId].Single(a => a.ActionID == actionId.ToString());

                        if (serverAction.PublishedDate == clientAction.PublishedAt) //thern no change
                        {
                            actionDictionary.Add(serverAction.ProjectDetailID, "nochange");
                        }
                        else if (serverAction.PublishedDate > clientAction.PublishedAt) //change
                        {
                            if (clientAction.LastUpdatedAt == clientAction.PublishedAt)// no change at client
                            {
                                actionDictionary.Add(serverAction.ProjectDetailID, "update");
                            }
                            else
                            {
                                actionDictionary.Add(serverAction.ProjectDetailID, "conflict");
                            }
                        }
                        else
                        {

                        }

                    }

                    listActions.Add(projectId, actionDictionary);
                }
            }

            return listActions;
        }

        public List<ActionDTO> GetProjectActions(int projectID, List<int> actionIds)
        {
            using (var _db = new TabletApp_DatabaseEntities())
            {
                EntityConverter.AllResources = EntityConverter.ToResourceDTO(_db.Resources);

                IList<ActionDTO> actionDTOs = EntityConverter.ToActionDTO(_db.ProjectDetails.Where(d => d.ProjectID == projectID));

                return actionDTOs.ToList();
            }

            return null;
        }

    }
}
