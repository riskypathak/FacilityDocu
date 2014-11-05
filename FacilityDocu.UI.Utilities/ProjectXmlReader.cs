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
        public static ProjectDTO ReadProjectXml(string xmlPath, bool onlyProjectAttributes)
        {
            XDocument xdoc = XDocument.Load(xmlPath);

            ProjectDTO project = new ProjectDTO();

            XElement xProject = xdoc.Elements("project").Single();

            project.CreatedBy = new UserDTO() { UserName = xProject.Element("createdby").Value };
            project.CreationDate = Convert.ToDateTime(xProject.Element("createdtime").Value);
            project.Description = xProject.Element("description").Value;
            project.LastUpdatedAt = Convert.ToDateTime(xProject.Element("updatedtime").Value);
            project.LastUpdatedBy = new UserDTO() { UserName = xProject.Element("updatedby").Value };

            project.ProjectID = Convert.ToString(xProject.Element("id").Value);
            project.Template = Convert.ToBoolean(xProject.Element("template").Value);

            if (!onlyProjectAttributes)
            {
                IList<Services.RigTypeDTO> rigTypes = ReadRigTypes(project, xProject);
                project.RigTypes = rigTypes.ToArray();
            }

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

                IList<Services.ResourceDTO> resources = ReadResources(xAction);
                action.Resources = resources.ToArray();

                IList<Services.ToolDTO> tools = ReadTools(xAction);
                action.Tools = tools.ToArray();

                actions.Add(action);
            }
            return actions;
        }

        private static IList<ToolDTO> ReadTools(XElement xAction)
        {
            IList<Services.ToolDTO> tools = new List<Services.ToolDTO>();

            foreach (XElement xTool in xAction.Element("tools").Elements("tool"))
            {
                ToolDTO toole = new ToolDTO();
                toole.ToolID = Convert.ToString(xTool.Element("id").Value);
                toole.Name = Convert.ToString(xTool.Element("name").Value);

                tools.Add(toole);
            }
            return tools;
        }

        private static IList<ResourceDTO> ReadResources(XElement xAction)
        {
            IList<Services.ResourceDTO> resources = new List<Services.ResourceDTO>();

            foreach (XElement xResource in xAction.Element("resources").Elements("resource"))
            {
                ResourceDTO resource = new ResourceDTO();
                resource.ResourceID = Convert.ToString(xResource.Element("id").Value);
                resource.Name = Convert.ToString(xResource.Element("name").Value);
                resource.ResourceCount = Convert.ToString(xResource.Element("count").Value);

                resources.Add(resource);
            }
            return resources;
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

                IList<Services.CommentDTO> comments = ReadComments(xImage);
                image.Comments = comments.ToArray();

                images.Add(image);
            }
            return images;
        }

        private static IList<CommentDTO> ReadComments(XElement xImage)
        {
            IList<Services.CommentDTO> comments = new List<Services.CommentDTO>();

            foreach (XElement xComment in xImage.Element("comments").Elements("comment"))
            {
                CommentDTO comment = new CommentDTO();
                comment.Text = Convert.ToString(xComment.Element("text").Value);
                comment.User = Convert.ToString(xComment.Element("user").Value);
                comment.CommentedAt = Convert.ToDateTime(xComment.Element("date").Value);

                comments.Add(comment);
            }
            return comments;
        }
    }
}
