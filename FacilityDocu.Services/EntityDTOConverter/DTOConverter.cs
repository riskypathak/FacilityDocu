using FacilityDocu.DTO;
using FacilityDocu.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacilityDocu.Services.EntityDTOConverter
{
    public static class DTOConverter
    {
        public static Project ToProject(ProjectDTO projectDTO, bool fullConversion = true)
        {
            Project project = new Project();

            project.Description = projectDTO.Description;
            project.Client = projectDTO.Client;
            project.Persons = projectDTO.Persons;
            project.ProjectNumber = projectDTO.ProjectNumber;
            project.Location = projectDTO.Location;

            if (!Helper.IsNew(projectDTO.ProjectID))
            {
                project.ProjectID = Convert.ToInt32(projectDTO.ProjectID);
            }

            project.CreationDate = projectDTO.CreationDate;

            project.Template = projectDTO.Template;
            project.Close = projectDTO.Closed;

            if (fullConversion)
            {
                project.ProjectDetails = ToProjectDetail(projectDTO);
            }

            return project;
        }

        private static ICollection<ProjectDetail> ToProjectDetail(ProjectDTO projectDTO)
        {
            IList<ProjectDetail> projectDetails = new List<ProjectDetail>();

            foreach (RigTypeDTO rigTypeDTO in projectDTO.RigTypes)
            {
                foreach (ModuleDTO moduleDTO in rigTypeDTO.Modules)
                {
                    foreach (StepDTO stepDTO in moduleDTO.Steps)
                    {
                        foreach (ActionDTO actionDTO in stepDTO.Actions)
                        {
                            ProjectDetail projectDetail = new ProjectDetail();

                            if (Helper.IsNew(actionDTO.ActionID))
                            {
                                projectDetail.ProjectDetailID = Helper.GetUniqueID();
                            }
                            else
                            {
                                projectDetail.ProjectDetailID = Convert.ToInt32(actionDTO.ActionID);
                            }

                            projectDetail.ActionName = actionDTO.Name;
                            projectDetail.Description = actionDTO.Description;

                            if (!Helper.IsNew(stepDTO.StepID))
                            {
                                projectDetail.StepID = Convert.ToInt32(stepDTO.StepID);
                            }
                            else
                            {
                                projectDetail.StepID = Helper.GetUniqueID();
                            }

                            projectDetail.Dimensions = actionDTO.Dimensions;
                            projectDetail.LiftingGears = actionDTO.LiftingGears;
                            projectDetail.Risks = actionDTO.Risks;

                            projectDetail.ActionDescriptionWarning = actionDTO.IsDescriptionwarning;
                            projectDetail.ActionNameWarning = actionDTO.IsNameWarning;
                            projectDetail.ImportantActionDescription = actionDTO.ImportantDescription;
                            actionDTO.ImportantName = actionDTO.ImportantName;

                            projectDetail.ProjectActionTools = ToProjectActionTool(actionDTO.Tools, projectDetail);
                            projectDetail.ProjectActionResources = ToProjectActionResources(actionDTO.Resources, projectDetail);
                            projectDetail.ProjectActionAttachments = ToProjectActionAttachments(actionDTO.Attachments, projectDetail);
                            projectDetail.ProjectActionImages = ToProjectActionImages(actionDTO.Images, projectDetail);
                            projectDetail.RiskAnalysis = ToProjectActionRiskAnalysis(actionDTO.RiskAnalysis, projectDetail);

                            projectDetails.Add(projectDetail);
                        }
                    }
                }
            }

            return projectDetails;
        }

        private static ICollection<RiskAnalysi> ToProjectActionRiskAnalysis(IList<RiskAnalysisDTO> analysissDTO, ProjectDetail projectDetail)
        {
            IList<RiskAnalysi> analysiss = new List<RiskAnalysi>();

            if (analysissDTO != null)
            {
                foreach (RiskAnalysisDTO analysisDTO in analysissDTO)
                {
                    RiskAnalysi analysis = new RiskAnalysi();

                    if (!Helper.IsNew(analysisDTO.RiskAnalysisID))
                    {
                        analysis.RiskAnalysisID = Convert.ToInt32(analysisDTO.RiskAnalysisID);
                    }
                    else
                    {
                        analysis.RiskAnalysisID = Helper.GetUniqueID();
                    }


                    analysis.ProjectDetailID = projectDetail.ProjectDetailID;
                    analysis.Activity = analysisDTO.Activity;
                    analysis.B = Convert.ToDecimal(analysisDTO.B);
                    analysis.B_ = Convert.ToString(analysisDTO.B_);
                    analysis.Controls = analysisDTO.Controls;
                    analysis.Danger = analysisDTO.Danger;
                    analysis.E = Convert.ToDecimal(analysisDTO.E);
                    analysis.E_ = Convert.ToString(analysisDTO.E_);
                    analysis.K = Convert.ToDecimal(analysisDTO.K);
                    analysis.K_ = Convert.ToString(analysisDTO.K_);
                    analysis.Risk = Convert.ToDecimal(analysisDTO.Risk);
                    analysis.Risk_ = Convert.ToString(analysisDTO.Risk_);

                    analysiss.Add(analysis);
                }
            }
            return analysiss;
        }

        private static ICollection<ProjectActionImage> ToProjectActionImages(IList<ImageDTO> images, ProjectDetail projectDetail)
        {
            IList<ProjectActionImage> projectImages = new List<ProjectActionImage>();

            if (images != null)
            {


                foreach (ImageDTO imageDTO in images)
                {
                    ProjectActionImage projectImage = new ProjectActionImage();
                    projectImage.ProjectDetailID = projectDetail.ProjectDetailID;
                    
                    projectImage.Image = new Image()
                    {
                        Description = imageDTO.Description,
                        ImagePath = imageDTO.Path,
                        CreationDate = imageDTO.CreationDate,
                        Tags = string.Join(";", imageDTO.Tags.ToArray()),
                        ImageComments = ToProjectActionImageComments(imageDTO.Comments, imageDTO.ImageID)
                    };

                    if (!Helper.IsNew(imageDTO.ImageID))
                    {
                        projectImage.Image.ImageID = Convert.ToInt32(imageDTO.ImageID);
                    }
                    else
                    {
                        projectImage.Image.ImageID = Helper.GetUniqueID();
                    }

                    projectImage.ImageID = projectImage.Image.ImageID;
                    projectImage.IsUsed = imageDTO.Used;

                    projectImages.Add(projectImage);
                }
            }

            return projectImages;
        }

        private static ICollection<ImageComment> ToProjectActionImageComments(IList<CommentDTO> comments, string imageID)
        {
            IList<ImageComment> projectTools = new List<ImageComment>();

            if (comments != null)
            {
                foreach (CommentDTO commentDTO in comments)
                {
                    ImageComment comment = new ImageComment();
                    comment.CreationDate = Convert.ToDateTime(commentDTO.CommentedAt);

                    if (!Helper.IsNew(commentDTO.CommentID))
                    {
                        comment.ImageCommentID = Convert.ToInt32(commentDTO.CommentID);
                    }
                    else
                    {
                        comment.ImageCommentID = Helper.GetUniqueID();
                    }

                    comment.ImageID = Convert.ToInt32(imageID);
                    comment.Text = commentDTO.Text;

                    projectTools.Add(comment);
                }
            }

            return projectTools;
        }

        private static ICollection<ProjectActionAttachment> ToProjectActionAttachments(IList<AttachmentDTO> attachments, ProjectDetail projectDetail)
        {
            IList<ProjectActionAttachment> projectAttachments = new List<ProjectActionAttachment>();

            if (attachments != null)
            {
                foreach (AttachmentDTO attachmentDTO in attachments)
                {

                    ProjectActionAttachment projectAttachment = new ProjectActionAttachment();
                    projectAttachment.ProjectDetailID = projectDetail.ProjectDetailID;
                    projectAttachment.Attachment = new Attachment() { Name = attachmentDTO.Name, Path = attachmentDTO.Path };

                    if (!Helper.IsNew(attachmentDTO.AttachmentID))
                    {
                        projectAttachment.Attachment.AttachmentID = Convert.ToInt32(attachmentDTO.AttachmentID);
                    }
                    else
                    {
                        projectAttachment.Attachment.AttachmentID = Helper.GetUniqueID();
                    }

                    projectAttachment.AttachmentID = projectAttachment.Attachment.AttachmentID;

                    projectAttachments.Add(projectAttachment);
                }
            }

            return projectAttachments;
        }

        private static ICollection<ProjectActionResource> ToProjectActionResources(IList<ResourceDTO> resources, ProjectDetail projectDetail)
        {
            IList<ProjectActionResource> projectResources = new List<ProjectActionResource>();

            if (resources != null)
            {
                foreach (ResourceDTO resourceDTO in resources)
                {
                    if (Convert.ToInt32(resourceDTO.ResourceCount) > 0)
                    {
                        ProjectActionResource projectResource = new ProjectActionResource();
                        projectResource.ResourceID = Convert.ToInt32(resourceDTO.ResourceID);
                        projectResource.ResourceCount = Convert.ToInt32(resourceDTO.ResourceCount);
                        projectResource.ProjectDetailID = projectDetail.ProjectDetailID;

                        projectResources.Add(projectResource);
                    }
                }
            }

            return projectResources;
        }

        private static ICollection<ProjectActionTool> ToProjectActionTool(IList<ToolDTO> tools, ProjectDetail projectDetail)
        {
            IList<ProjectActionTool> projectTools = new List<ProjectActionTool>();

            if (tools != null)
            {
                foreach (ToolDTO toolDTO in tools)
                {
                    ProjectActionTool projectTool = new ProjectActionTool();
                    projectTool.ToolID = Convert.ToInt32(toolDTO.ToolID);
                    projectTool.ProjectDetailID = projectDetail.ProjectDetailID;

                    projectTools.Add(projectTool);
                }
            }
            return projectTools;
        }
    }
}