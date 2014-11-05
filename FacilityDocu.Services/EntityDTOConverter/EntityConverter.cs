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

            projectDTO.Description = project.Description;
            projectDTO.ProjectID = project.ProjectID.ToString();

            projectDTO.RigTypes = ToRigTypesDTO(project.ProjectDetails);

            //TODO
            //projectDTO.CreatedBy = project.CreatedBy;
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
                //TODO
                //stepDTO.Number = projectDetail.Step.

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
                //TODO
                //actionDTO.Number =  projectDetail.Action

                actionsDTO.Add(actionDTO);

                actionDTO.Images = TOImagesDTO(projectDetails.Where(x => x.ActionID.Value == projectDetail.ActionID));

                actionsDTO.Add(actionDTO);


            }

            return actionsDTO;


        }

        private static IList<ImageDTO> TOImagesDTO(IEnumerable<ProjectDetail> enumerable)
        {
            IList<ImageDTO> ImageDTOs = new List<ImageDTO>();

            foreach (var projectDetail in enumerable.Select(x => x.ProjectActionImages).FirstOrDefault())
            {
                ImageDTO imageDTO = new ImageDTO();
                imageDTO.ImageID = projectDetail.ImageID.GetValueOrDefault().ToString();
                imageDTO.Description = projectDetail.Image.Description;
                imageDTO.Path = projectDetail.Image.ImagePath;
                imageDTO.CreationDate = projectDetail.Image.CreationDate.GetValueOrDefault();
                imageDTO.Tags = projectDetail.Image.Tags.Split(',');


                //TODO
                //imageDTO.Name = projectDetail.Image



                imageDTO.Comments = TOCommentDTO(projectDetail.Image.ImageDetailComments);

                ImageDTOs.Add(imageDTO);

            }
            return ImageDTOs;

        }

        //private static IList<ImageDTO> TOImagesDTO(ICollection<ProjectActionImage> ProjectActionImages)
        //{
        //    IList<ImageDTO> ImageDTOs = new List<ImageDTO>();

        //    foreach (var projectDetail in ProjectActionImages)
        //    {
        //        ImageDTO imageDTO = new ImageDTO();
        //        imageDTO.ImageID = projectDetail.ImageID.GetValueOrDefault().ToString();
        //        imageDTO.Description = projectDetail.Image.Description;
        //        imageDTO.Path = projectDetail.Image.ImagePath;
        //        imageDTO.CreationDate = projectDetail.Image.CreationDate.GetValueOrDefault();
        //        imageDTO.Tags = projectDetail.Image.Tags.Split(',');


        //        //TODO
        //        //imageDTO.Name = projectDetail.Image



        //        imageDTO.Comments = TOCommentDTO(projectDetail.Image.ImageDetailComments);

        //        ImageDTOs.Add(imageDTO);

        //    }
        //    return ImageDTOs;

        //}

        private static IList<CommentDTO> TOCommentDTO(ICollection<ImageDetailComment> ImageComments)
        {
            List<CommentDTO> imageComments = new List<CommentDTO>();
            foreach (var imgComments in ImageComments)
            {
                CommentDTO commnet = new CommentDTO();

                commnet.CommentedAt = imgComments.Date.GetValueOrDefault();
                commnet.Text = imgComments.Text;
                //TODO
                //commnet.User = imgComments.Date
                imageComments.Add(commnet);

            }

            return imageComments;


        }




        private static UserDTO ToUserDTO(User userDTO)
        {
            //TODO
            throw new NotImplementedException();


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