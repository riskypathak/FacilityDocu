using FacilityDocu.UI.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FacilityDocu.UI.Utilities
{
    public static class ProjectXmlReader
    {
        public static ProjectDTO ReadProjectXml(string xmlPath)
        {
            XDocument xdoc = XDocument.Load(xmlPath);

            ProjectDTO project = new ProjectDTO();

            XElement xProject = xdoc.Elements("project").Single();

            project.CreatedBy = new UserDTO();
            project.CreatedBy.UserName = xProject.Element("createdby").Value;
            project.CreationDate = Convert.ToDateTime(xProject.Element("createdtime").Value);
            project.Description = xProject.Element("description").Value;
            project.LastUpdatedAt = Convert.ToDateTime(xProject.Element("updatetime").Value);
            project.LastUpdatedBy.UserName = xProject.Element("updateby").Value;

            project.ProjectID = Convert.ToString(xProject.Element("id").Value);
            project.Template = Convert.ToBoolean(xProject.Element("template").Value);

            IList<Services.RigTypeDTO> rigTypes = ReadRigTypes(project, xProject);

            project.RigTypes = rigTypes.ToArray();
            return project;
        }

        private static IList<Services.RigTypeDTO> ReadRigTypes(ProjectDTO project, XElement xProject)
        {
            IList<Services.RigTypeDTO> rigTypes = new List<Services.RigTypeDTO>();

            foreach (XElement xRigType in xProject.Element("rigs").Elements("rig"))
            {
                RigTypeDTO rigType = new RigTypeDTO();
                rigType.Name = Convert.ToString(xRigType.Attribute("type").Value);

                IList<Services.ModuleDTO> modules = ReadModules(xRigType);
                rigType.Modules = modules.ToArray();
                rigTypes.Add(rigType);
            }

            return rigTypes;
        }

        private static IList<ModuleDTO> ReadModules(XElement xRigType)
        {
            IList<Services.ModuleDTO> modules = new List<Services.ModuleDTO>();

            foreach (XElement xModule in xRigType.Element("modules").Elements("module"))
            {
                ModuleDTO module = new ModuleDTO();
                module.ModuleID = Convert.ToString(xModule.Element("id").Value);
                module.Name = Convert.ToString(xModule.Element("name").Value);

                IList<Services.StepDTO> steps = ReadSteps(xModule);
                module.Steps = steps.ToArray();
                modules.Add(module);
            }
            return modules;
        }

        private static IList<StepDTO> ReadSteps(XElement xModule)
        {
            IList<Services.StepDTO> steps = new List<Services.StepDTO>();

            foreach (XElement xStep in xModule.Element("steps").Elements("step"))
            {
                StepDTO step = new StepDTO();
                step.StepID = Convert.ToInt32(xStep.Element("id").Value);
                step.Name = Convert.ToString(xStep.Element("name").Value);
                IList<Services.ActionDTO> actions = ReadActions(xStep);

                step.Actions = actions.ToArray();
                steps.Add(step);
            }
            return steps;
        }

        private static IList<ActionDTO> ReadActions(XElement xStep)
        {
            IList<Services.ActionDTO> actions = new List<Services.ActionDTO>();

            foreach (XElement xAction in xStep.Element("actions").Elements("action"))
            {
                ActionDTO action = new ActionDTO();
                action.ActionID = Convert.ToString(xAction.Element("id").Value);
                action.Name = Convert.ToString(xAction.Element("name").Value);
                action.Description = Convert.ToString(xAction.Element("description").Value);
                action.Risks = Convert.ToString(xAction.Element("risks").Value);
                action.LiftingGears = Convert.ToString(xAction.Element("liftinggears").Value);
                action.Dimensions = Convert.ToString(xAction.Element("dimensions").Value);

                IList<Services.ImageDTO> images = ReadImages(xAction);

                action.Images = images.ToArray();


                actions.Add(action);
            }
            return actions;
        }

        private static IList<ImageDTO> ReadImages(XElement xAction)
        {
            IList<Services.ImageDTO> images = new List<Services.ImageDTO>();

            foreach (XElement xImage in xAction.Element("images").Elements("image"))
            {
                ImageDTO image = new ImageDTO();
                image.ImageID = Convert.ToString(xImage.Element("id").Value);
                image.CreationDate = Convert.ToDateTime(xImage.Element("creationdate").Value);
                image.Description = Convert.ToString(xImage.Element("description").Value);
                image.Number = Convert.ToString(xImage.Element("number").Value);
                image.Path = Convert.ToString(xImage.Element("path").Value);
                image.Tags = Convert.ToString(xImage.Element("tags").Value).Split(';');

                images.Add(image);
            }
            return images;
        }
    }
}
