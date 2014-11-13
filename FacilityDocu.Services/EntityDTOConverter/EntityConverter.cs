using FacilityDocu.DTO;
using FacilityDocu.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacilityDocu.Services.EntityDTOConverter
{
    public static class EntityConverter
    {

        public static List<ResourceDTO> AllResources;

        public static ProjectDTO ToProjectDTO(Project project)
        {
            ProjectDTO projectDTO = new ProjectDTO();

            projectDTO.Template = project.Template;
            projectDTO.Closed = project.Close.Value;
            projectDTO.Description = project.Description;
            projectDTO.ProjectID = project.ProjectID.ToString();
            projectDTO.CreationDate = project.CreationDate.Value;
            projectDTO.LastUpdatedAt = project.LastUpdatedAt.Value;

            projectDTO.CreatedBy = ToUserDTO(project.User);

            if (project.User1 == null)
            {
                projectDTO.LastUpdatedBy = new UserDTO() { UserName = project.UpdatedBy };
            }
            else
            {
                projectDTO.LastUpdatedBy = ToUserDTO(project.User1);
            }

            projectDTO.RigTypes = ToRigTypesDTO(project.ProjectDetails);
            return projectDTO;
        }

        private static IList<RigTypeDTO> ToRigTypesDTO(ICollection<ProjectDetail> projectDetails)
        {
            IList<RigTypeDTO> rigTypesDTO = new List<RigTypeDTO>();

            foreach (ProjectDetail projectDetail in projectDetails.GroupBy(x => x.Step.Module.RigType.RigTypeID).Select(y => y.First()))
            {

                RigTypeDTO rigTypeDTO = new RigTypeDTO();
                rigTypeDTO.Name = projectDetail.Step.Module.RigType.Name;
                rigTypeDTO.RigTypeID = Convert.ToString(projectDetail.Step.Module.RigType.RigTypeID);
                rigTypeDTO.Modules = ToModulesDTO(projectDetails.Where(p => p.Step.Module.RigType.RigTypeID.Equals(projectDetail.Step.Module.RigTypeID.Value)));

                rigTypesDTO.Add(rigTypeDTO);

            }

            return rigTypesDTO;
        }

        private static IList<ModuleDTO> ToModulesDTO(IEnumerable<ProjectDetail> projectDetails)
        {
            IList<ModuleDTO> modulesDTO = new List<ModuleDTO>();

            foreach (ProjectDetail projectDetail in projectDetails.GroupBy(x => x.Step.Module.ModuleID).Select(y => y.First()))
            {
                ModuleDTO moduleDTO = new ModuleDTO();
                moduleDTO.Name = projectDetail.Step.Module.ModuleName;
                moduleDTO.ModuleID = projectDetail.Step.Module.ModuleID.ToString();

                moduleDTO.Steps = ToStepsDTO(projectDetails.Where(p => p.Step.Module.ModuleID.Equals(projectDetail.Step.Module.ModuleID)));
                modulesDTO.Add(moduleDTO);


            }

            return modulesDTO;
        }

        private static IList<StepDTO> ToStepsDTO(IEnumerable<ProjectDetail> projectDetails)
        {
            IList<StepDTO> stepsDTO = new List<StepDTO>();

            foreach (var projectDetail in projectDetails.GroupBy(x => x.Step.StepID).Select(y => y.First()))
            {
                StepDTO stepDTO = new StepDTO();
                stepDTO.Name = projectDetail.Step.StepName;
                stepDTO.StepID = projectDetail.Step.StepID;

                stepDTO.Actions = ToActionDTO(projectDetails.Where(p => p.Step.StepID.Equals(projectDetail.Step.StepID)));
                stepsDTO.Add(stepDTO);

            }
            return stepsDTO;
        }

        private static IList<ActionDTO> ToActionDTO(IEnumerable<ProjectDetail> projectDetails)
        {
            IList<ActionDTO> actionsDTO = new List<ActionDTO>();

            foreach (var projectDetail in projectDetails)
            {
                ActionDTO actionDTO = new ActionDTO();
                actionDTO.ActionID = Convert.ToString(projectDetail.ProjectDetailID);
                actionDTO.Description = projectDetail.Description;
                actionDTO.Name = projectDetail.ActionName;
                actionDTO.Risks = projectDetail.Risks;
                actionDTO.LiftingGears = projectDetail.LiftingGears;
                actionDTO.Dimensions = projectDetail.Dimensions;
                actionDTO.IsNameWarning = projectDetail.ActionNameWarning.Value;
                actionDTO.IsDescriptionwarning = projectDetail.ActionDescriptionWarning.Value;
                actionDTO.ImportantName = projectDetail.ImportantActionname;
                actionDTO.ImportantDescription = projectDetail.ImportantActionDescription;

                actionDTO.Images = TOImagesDTO(projectDetail);
                actionDTO.Attachments = ToAttachmentsDTO(projectDetail);
                actionDTO.Resources = ToResourcesDTO(projectDetail);
                actionDTO.Tools = ToToolsDTO(projectDetail);
                actionDTO.RiskAnalysis = ToRiskAnalysisDTO(projectDetail);

                actionsDTO.Add(actionDTO);
            }

            return actionsDTO;


        }

        private static IList<AttachmentDTO> ToAttachmentsDTO(ProjectDetail projectDetail)
        {
            IList<AttachmentDTO> AttachmentDTOs = new List<AttachmentDTO>();

            foreach (var projectAttachment in projectDetail.ProjectActionAttachments)
            {
                AttachmentDTO attachmentDTO = new AttachmentDTO();
                attachmentDTO.AttachmentID = projectAttachment.AttachmentID.GetValueOrDefault().ToString();
                attachmentDTO.Name = projectAttachment.Attachment.Name;
                attachmentDTO.Path = projectAttachment.Attachment.Path;

                AttachmentDTOs.Add(attachmentDTO);

            }
            return AttachmentDTOs;
        }

        private static IList<RiskAnalysisDTO> ToRiskAnalysisDTO(ProjectDetail projectDetail)
        {
            IList<RiskAnalysisDTO> analysissDTO = new List<RiskAnalysisDTO>();

            foreach (var analysis in projectDetail.RiskAnalysis)
            {
                RiskAnalysisDTO analysisDTO = new RiskAnalysisDTO();
                analysisDTO.Activity = analysis.Activity;
                analysisDTO.B = Convert.ToDouble(analysis.B.Value);
                analysisDTO.B_ = Convert.ToDouble(analysis.B_);
                analysisDTO.Controls = analysis.Controls;
                analysisDTO.Danger = analysis.Danger;
                analysisDTO.E = Convert.ToDouble(analysis.E.Value);
                analysisDTO.E_ = Convert.ToDouble(analysis.E_);
                analysisDTO.K = Convert.ToDouble(analysis.K.Value);
                analysisDTO.K_ = Convert.ToDouble(analysis.K_);
                analysisDTO.Risk = Convert.ToDouble(analysis.Risk.Value);
                analysisDTO.Risk_ = Convert.ToDouble(analysis.Risk_);
                analysisDTO.RiskAnalysisID = Convert.ToString(analysis.RiskAnalysisID);

                analysissDTO.Add(analysisDTO);
            }

            return analysissDTO;
        }

        private static IList<ResourceDTO> ToResourcesDTO(ProjectDetail projectDetail)
        {
            IList<ResourceDTO> resourcesDTO = new List<ResourceDTO>();

            foreach (var resource in projectDetail.ProjectActionResources)
            {
                ResourceDTO resourceDTO = new ResourceDTO();
                resourceDTO.Name = resource.Resource.ResourceName;
                resourceDTO.ResourceID = Convert.ToString(resource.ResourceID);
                resourceDTO.ResourceCount = Convert.ToString(resource.ResourceCount);
                resourceDTO.Type = Convert.ToString(resource.Resource.Type).ToLower();

                resourcesDTO.Add(resourceDTO);
            }

            IList<string> existingIDs = resourcesDTO.Select(r=>r.ResourceID).ToList();
            AllResources.ForEach(r =>
                {
                    if (!existingIDs.Contains(r.ResourceID))
                    {
                        resourcesDTO.Add(r);
                    }
                }
                    );

            return resourcesDTO;
        }

        private static IList<ToolDTO> ToToolsDTO(ProjectDetail projectDetail)
        {
            IList<ToolDTO> toolsDTO = new List<ToolDTO>();

            foreach (var tool in projectDetail.ProjectActionTools)
            {
                ToolDTO toolDTO = new ToolDTO();
                toolDTO.Name = tool.Tool.ToolName;
                toolDTO.ToolID = Convert.ToString(tool.ToolID);

                toolsDTO.Add(toolDTO);
            }

            return toolsDTO;
        }

        private static IList<ImageDTO> TOImagesDTO(ProjectDetail projectDetail)
        {
            IList<ImageDTO> ImageDTOs = new List<ImageDTO>();

            foreach (var projctImage in projectDetail.ProjectActionImages)
            {
                ImageDTO imageDTO = new ImageDTO();
                imageDTO.ImageID = projctImage.ImageID.GetValueOrDefault().ToString();
                imageDTO.Description = projctImage.Image.Description;
                imageDTO.Path = projctImage.Image.ImagePath;
                imageDTO.CreationDate = projctImage.Image.CreationDate.GetValueOrDefault();
                imageDTO.Tags = projctImage.Image.Tags.Split(';');

                imageDTO.Comments = TOCommentDTO(projctImage.Image.ImageComments);

                ImageDTOs.Add(imageDTO);

            }
            return ImageDTOs;
        }

        private static IList<CommentDTO> TOCommentDTO(ICollection<ImageComment> ImageComments)
        {
            List<CommentDTO> imageComments = new List<CommentDTO>();
            foreach (var imgComments in ImageComments)
            {
                CommentDTO commnet = new CommentDTO();

                commnet.CommentedAt = imgComments.CreationDate.GetValueOrDefault();
                commnet.Text = imgComments.Text;

                imageComments.Add(commnet);

            }

            return imageComments;


        }

        private static UserDTO ToUserDTO(User user)
        {
            UserDTO userDTO = new UserDTO();

            if (user != null)
            {
                userDTO.Name = user.Name;
                userDTO.UserName = user.UserName;
            }

            return userDTO;

        }

        internal static StepDTO ToStepDTO(Step step)
        {
            StepDTO stepDTO = new StepDTO();
            stepDTO.Name = step.StepName;
            stepDTO.StepID = step.StepID;

            return stepDTO;
        }

        internal static List<ResourceDTO> ToResourceDTO(System.Data.Entity.DbSet<Resource> dbSet)
        {
            List<ResourceDTO> resourcesDTO = new List<ResourceDTO>();

            foreach (Resource resource in dbSet)
            {
                ResourceDTO resourceDTO = new ResourceDTO()
                {
                    Name = resource.ResourceName,
                    ResourceCount = "0",
                    ResourceID = resource.ResourceID.ToString(),
                    Type = resource.Type.ToString().ToLower()
                };

                resourcesDTO.Add(resourceDTO);
            }

            return resourcesDTO;
        }
    }
}