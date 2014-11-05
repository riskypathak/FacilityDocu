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
        public static Project ToProject(ProjectDTO projectDTO)
        {
            Project project = new Project();

            project.Description = projectDTO.Description;
            project.ProjectID = Convert.ToInt32(projectDTO.ProjectID);

            project.ProjectDetails = ToProjectDetail(projectDTO);

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
                            projectDetail.ActionID = Convert.ToInt32(actionDTO.ActionID);
                            projectDetail.StepID = Convert.ToInt32(stepDTO.StepID);

                            projectDetails.Add(projectDetail);
                        }
                    }
                }
            }

            return projectDetails;
        }
    }
}