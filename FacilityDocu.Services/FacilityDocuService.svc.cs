using FacilityDocu.DTO;
using FacilityDocu.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using FacilityDocu.Services.EntityDTOConverter;
using System.IO;

namespace FacilityDocu.Services
{
    public class FacilityDocuService : IFacilityDocuService
    {
        public Dictionary<int, bool> IsSync(Dictionary<int, DateTime> inputProjects)
        {
            Dictionary<int, bool> _statusData = new Dictionary<int, bool>();

            using (var context = new TabletApp_DatabaseEntities())
            {
                foreach (var project in context.Projects)
                {
                    var inputProject = inputProjects.Where(i => i.Key == project.ProjectID);

                    if (inputProject.Count() <= 0)
                    {
                        _statusData.Add(project.ProjectID, true);
                    }
                    else
                    {
                        DateTime inputDate = inputProject.First().Value;
                        if (inputProject != null && DateTime.Compare(project.LastUpdatedAt.GetValueOrDefault(), inputDate) > 0)
                        {
                            _statusData.Add(project.ProjectID, true);
                        }
                        else
                        {
                            _statusData.Add(project.ProjectID, false);
                        }
                    }
                }
            }

            return _statusData;
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

        public ProjectDTO UpdateProject(ProjectDTO projectDTO)
        {
            ProjectDTO updatedProjectDTO = null;

            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                if (Helper.IsNew(projectDTO.ProjectID))
                {
                    Project project = DTOConverter.ToProject(projectDTO);
                    project.User = context.Users.Single(u => u.UserName.Equals(projectDTO.CreatedBy.UserName));
                    project.User1 = context.Users.Single(u => u.UserName.Equals(projectDTO.LastUpdatedBy.UserName));

                    context.Projects.Add(project);
                    context.SaveChanges();

                    updatedProjectDTO = GetProjectDetails(project.ProjectID);

                }
                else
                {
                    Project project = DTOConverter.ToProject(projectDTO);

                    int projectID = Convert.ToInt32(projectDTO.ProjectID);

                    Project existingProject = context.Projects.SingleOrDefault(p => p.ProjectID.Equals(projectID));

                    existingProject.Close = project.Close;
                    existingProject.LastUpdatedAt = project.LastUpdatedAt;

                    UpdateProjectDetail(project, existingProject);

                    context.SaveChanges();

                    updatedProjectDTO = GetProjectDetails(project.ProjectID);
                }
            }

            return updatedProjectDTO;
        }

        private void UpdateProjectDetail(Project project, Project existingProject)
        {
            foreach (ProjectDetail existingPD in existingProject.ProjectDetails.ToList())
            {
                ProjectDetail updateProject = project.ProjectDetails.FirstOrDefault(p => p.ProjectDetailID == existingPD.ProjectDetailID);

                if (updateProject != null)
                {
                    ProjectDetail modifyAction = existingProject.ProjectDetails.Single(a => a.ProjectDetailID == updateProject.ProjectDetailID);

                    modifyAction.Dimensions = updateProject.Dimensions;
                    modifyAction.LiftingGears = updateProject.LiftingGears;
                    modifyAction.Risks = updateProject.Risks;

                    modifyAction.ActionName = updateProject.ActionName;
                    modifyAction.Description = updateProject.Description;

                    modifyAction.ActionDescriptionWarning = updateProject.ActionDescriptionWarning;
                    modifyAction.ActionNameWarning = updateProject.ActionNameWarning;
                    modifyAction.ImportantActionDescription = updateProject.ImportantActionDescription;
                    modifyAction.ImportantActionname = updateProject.ImportantActionname;

                    UpdateActionTool(updateProject, existingPD);
                    UpdateActionResource(updateProject, existingPD);
                    UpdateActionRiskAnalysis(updateProject, existingPD);
                    UpdateActionImages(updateProject, existingPD);
                }
                else
                {
                    existingProject.ProjectDetails.Remove(existingProject.ProjectDetails.Single(p => p.ProjectDetailID == existingPD.ProjectDetailID));
                }
            }

            List<ProjectDetail> newProjectDetails = project.ProjectDetails.Where(p => Helper.IsNew(p.ProjectDetailID.ToString())).ToList();
            newProjectDetails.ForEach(np => existingProject.ProjectDetails.Add(np));
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

        public void UpdateActionImages(ActionDTO action)
        {
            int actionID = Convert.ToInt32(action.ActionID);

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
                    }
                }
            }
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

            using(FileStream fileStream = File.Create(filePath))
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
    }
}
