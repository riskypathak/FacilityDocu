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
        public static ModuleDTO ToModuleDTO(Module module)
        {
            ModuleDTO moduleDTO = new ModuleDTO();

            moduleDTO.ModuleID = Convert.ToString(module.ModuleID);
            moduleDTO.Name = module.ModuleName;

            return moduleDTO;
        }

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
            projectDTO.LastUpdatedBy = ToUserDTO(project.User1);

            projectDTO.RigTypes = ToRigTypesDTO(project.ProjectDetails);
            return projectDTO;
        }

        private static IList<RigTypeDTO> ToRigTypesDTO(ICollection<ProjectDetail> projectDetails)
        {
            IList<RigTypeDTO> rigTypesDTO = new List<RigTypeDTO>();

            foreach (ProjectDetail projectDetail in projectDetails.GroupBy(p => p.RigTypeID).Select(grp => grp.First()))
            {
                RigTypeDTO rigTypeDTO = new RigTypeDTO();
                rigTypeDTO.Name = projectDetail.RigType.Name;
                rigTypeDTO.RigTypeID = projectDetail.RigType.RigTypeID.ToString();

                rigTypesDTO.Add(rigTypeDTO);

                rigTypeDTO.Modules = ToModulesDTO(projectDetails.Where(p => p.RigTypeID.Value.ToString().Equals(rigTypeDTO.RigTypeID)));
            }

            return rigTypesDTO;
        }

        private static IList<ModuleDTO> ToModulesDTO(IEnumerable<ProjectDetail> projectDetails)
        {
            IList<ModuleDTO> modulesDTO = new List<ModuleDTO>();

            foreach (ProjectDetail projectDetail in projectDetails.GroupBy(p => p.ModuleID).Select(grp => grp.First()))
            {
                ModuleDTO moduleDTO = new ModuleDTO();
                moduleDTO.Name = projectDetail.Module.ModuleName;
                moduleDTO.ModuleID = projectDetail.Module.ModuleID.ToString();

                modulesDTO.Add(moduleDTO);

                moduleDTO.Steps = ToStepsDTO(projectDetails.Where(p => p.ModuleID.Value.ToString().Equals(moduleDTO.ModuleID)));
            }

            return modulesDTO;
        }

        private static IList<StepDTO> ToStepsDTO(IEnumerable<ProjectDetail> projectDetails)
        {
            IList<StepDTO> stepsDTO = new List<StepDTO>();

            foreach (var projectDetail in projectDetails.GroupBy(p => p.StepID).Select(grp => grp.First()))
            {
                StepDTO stepDTO = new StepDTO();
                stepDTO.Name = projectDetail.Step.StepName;
                stepDTO.StepID = projectDetail.Step.StepID;

                stepsDTO.Add(stepDTO);
                stepDTO.Actions = ToActionDTO(projectDetails.Where(x => x.StepID.Value == stepDTO.StepID));


                stepsDTO.Add(stepDTO);
            }
            return stepsDTO;



        }

        private static IList<ActionDTO> ToActionDTO(IEnumerable<ProjectDetail> projectDetails)
        {
            IList<ActionDTO> actionsDTO = new List<ActionDTO>();

            foreach (var projectDetail in projectDetails.GroupBy(p => p.ActionID).Select(grp => grp.First()))
            {
                ActionDTO actionDTO = new ActionDTO();
                actionDTO.ActionID = projectDetail.ActionID.GetValueOrDefault().ToString();
                actionDTO.Description = projectDetail.Action.Description;
                actionDTO.Name = projectDetail.Action.ActionName;
                actionDTO.Risks = projectDetail.Risks;
                actionDTO.LiftingGears = projectDetail.LiftingGears;
                actionDTO.Dimensions = projectDetail.Dimensions;

                actionDTO.Images = TOImagesDTO(projectDetails.Where(x => x.ActionID.Value == projectDetail.ActionID));
                actionDTO.Resources = ToResourcesDTO(projectDetails.Where(x => x.ActionID.Value == projectDetail.ActionID));
                actionDTO.Tools = ToToolsDTO(projectDetails.Where(x => x.ActionID.Value == projectDetail.ActionID));
                actionDTO.RiskAnalysis = ToRiskAnalysisDTO(projectDetails.Where(x => x.ActionID.Value == projectDetail.ActionID));

                actionsDTO.Add(actionDTO);


            }

            return actionsDTO;


        }

        private static IList<RiskAnalysisDTO> ToRiskAnalysisDTO(IEnumerable<ProjectDetail> analysiss)
        {
            IList<RiskAnalysisDTO> analysissDTO = new List<RiskAnalysisDTO>();

            foreach (var analysis in analysiss.Select(x => x.ProjectRiskAnalysis).FirstOrDefault())
            {
                RiskAnalysisDTO analysisDTO = new RiskAnalysisDTO();
                analysisDTO.Activity = analysis.RiskAnalysi.Activity;
                analysisDTO.B = Convert.ToDouble(analysis.RiskAnalysi.B.Value);
                analysisDTO.B_ = Convert.ToDouble(analysis.RiskAnalysi.B_);
                analysisDTO.Controls = analysis.RiskAnalysi.Controls;
                analysisDTO.Danger = analysis.RiskAnalysi.Danger;
                analysisDTO.E = Convert.ToDouble(analysis.RiskAnalysi.E.Value);
                analysisDTO.E_ = Convert.ToDouble(analysis.RiskAnalysi.E_);
                analysisDTO.K = Convert.ToDouble(analysis.RiskAnalysi.K.Value);
                analysisDTO.K_ = Convert.ToDouble(analysis.RiskAnalysi.K_);
                analysisDTO.Risk = Convert.ToDouble(analysis.RiskAnalysi.Risk.Value);
                analysisDTO.Risk_ = Convert.ToDouble(analysis.RiskAnalysi.Risk_);
                analysisDTO.RiskAnalysisID = Convert.ToString(analysis.RiskAnalysi.RiskAnalysisID);
                analysisDTO.RiskAnalysisType = new RiskAnalysisTypeDTO() { RiskTypeID = analysis.RiskAnalysi.RiskType.RiskTypeID.ToString(), Name = analysis.RiskAnalysi.RiskType.Description };

                analysissDTO.Add(analysisDTO);
            }

            return analysissDTO;
        }

        private static IList<ResourceDTO> ToResourcesDTO(IEnumerable<ProjectDetail> resources)
        {
            IList<ResourceDTO> resourcesDTO = new List<ResourceDTO>();

            foreach (var resource in resources.Select(x => x.ProjectActionResources).FirstOrDefault())
            {
                ResourceDTO resourceDTO = new ResourceDTO();
                resourceDTO.Name = resource.Resource.ResourceName;
                resourceDTO.ResourceID = Convert.ToString(resource.ResourceID);
                resourceDTO.ResourceCount = Convert.ToString(resource.ResourceCount);

                resourcesDTO.Add(resourceDTO);
            }

            return resourcesDTO;
        }

        private static IList<ToolDTO> ToToolsDTO(IEnumerable<ProjectDetail> tools)
        {
            IList<ToolDTO> toolsDTO = new List<ToolDTO>();

            foreach (var tool in tools.Select(x => x.ProjectActionTools).FirstOrDefault())
            {
                ToolDTO toolDTO = new ToolDTO();
                toolDTO.Name = tool.Tool.ToolName;
                toolDTO.ToolID = Convert.ToString(tool.ToolID);

                toolsDTO.Add(toolDTO);
            }

            return toolsDTO;
        }

        private static IList<ImageDTO> TOImagesDTO(IEnumerable<ProjectDetail> enumerable)
        {
            IList<ImageDTO> ImageDTOs = new List<ImageDTO>();

            foreach (var projectDetail in enumerable.Select(x => x.ProjectActionImages).FirstOrDefault())
            {
                ImageDTO imageDTO = new ImageDTO();
                imageDTO.ImageID = projectDetail.ImageID.GetValueOrDefault().ToString();
                imageDTO.Description = projectDetail.Image.Description;
                imageDTO.Path = GetImageActualPath(projectDetail.Image.ImagePath);
                imageDTO.CreationDate = projectDetail.Image.CreationDate.GetValueOrDefault();
                imageDTO.Tags = projectDetail.Image.Tags.Split(';');

                imageDTO.Comments = TOCommentDTO(projectDetail.Image.ImageComments);

                ImageDTOs.Add(imageDTO);

            }
            return ImageDTOs;

        }

        private static string GetImageActualPath(string imagePath)
        {
            int lastIndex = System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().LastIndexOf('/');
            return string.Format("{0}/Data/Images/{1}", System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString().Substring(0, lastIndex), imagePath);
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

        internal static ActionDTO ToActionDTO(StepAction action)
        {
            ActionDTO actionDTO = new ActionDTO();
            actionDTO.ActionID = action.ActionID.GetValueOrDefault().ToString();
            actionDTO.Description = action.Action.Description;
            actionDTO.Name = action.Action.ActionName;

            return actionDTO;
        }

    }
}