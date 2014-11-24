using Tablet_App.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tablet_App
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
                IList<RigTypeDTO> rigTypes = ReadRigTypes(project, xProject);

                project.RigTypes = new System.Collections.ObjectModel.ObservableCollection<RigTypeDTO>();
                foreach (var rgt in rigTypes)
                {
                    project.RigTypes.Add(rgt);
                }
            }

            return project;
        }

        private static IList<RigTypeDTO> ReadRigTypes(ProjectDTO project, XElement xProject)
        {
            IList<RigTypeDTO> rigTypes = new List<RigTypeDTO>();

            foreach (XElement xRigType in xProject.Element("rigs").Elements("rig"))
            {
                RigTypeDTO rigType = new RigTypeDTO();
                rigType.Name = Convert.ToString(xRigType.Attribute("type").Value);

                IList<ModuleDTO> modules = ReadModules(xRigType);

                rigType.Modules = new System.Collections.ObjectModel.ObservableCollection<ModuleDTO>();

                foreach (var mdl in modules)
                {
                    rigType.Modules.Add(mdl);
                }

                rigTypes.Add(rigType);
            }

            return rigTypes;
        }

        private static IList<ModuleDTO> ReadModules(XElement xRigType)
        {
            IList<ModuleDTO> modules = new List<ModuleDTO>();

            foreach (XElement xModule in xRigType.Element("modules").Elements("module"))
            {

                ModuleDTO module = new ModuleDTO();

                module.ModuleID = Convert.ToString(xModule.Element("id").Value);
                module.Name = Convert.ToString(xModule.Element("name").Value);
                module.Number = Convert.ToString(xModule.Element("number").Value);

                IList<StepDTO> steps = ReadSteps(xModule);

                module.Steps = new System.Collections.ObjectModel.ObservableCollection<StepDTO>();

                foreach (var stp in steps)
                {
                    module.Steps.Add(stp);
                }

                modules.Add(module);
            }

            return modules;
        }
        private static IList<StepDTO> ReadSteps(XElement xModule)
        {
            IList<StepDTO> steps = new List<StepDTO>();

            foreach (XElement xStep in xModule.Element("steps").Elements("step"))
            {
                StepDTO step = new StepDTO();
                step.StepID = Convert.ToInt32(xStep.Element("id").Value);
                step.Name = Convert.ToString(xStep.Element("name").Value);
                step.Number = Convert.ToString(xStep.Element("number").Value);

                IList<ActionDTO> actions = ReadActions(xStep);

                step.Actions = new System.Collections.ObjectModel.ObservableCollection<ActionDTO>();
                foreach (var action in actions)
                {

                    step.Actions.Add(action);
                }
                steps.Add(step);
            }

            return steps;
        }

        private static IList<ActionDTO> ReadActions(XElement xStep)
        {
            IList<ActionDTO> actions = new List<ActionDTO>();

            foreach (XElement xAction in xStep.Element("actions").Elements("action"))
            {
                ActionDTO action = new ActionDTO();
                IList<ImageDTO> images = ReadImages(xAction);

                action.ActionID = Convert.ToString(xAction.Element("id").Value);
                action.Name = Convert.ToString(xAction.Element("name").Value);
                action.Number = Convert.ToString(xAction.Element("number").Value);
                action.Description = Convert.ToString(xAction.Element("description").Value);

                action.Images = new System.Collections.ObjectModel.ObservableCollection<ImageDTO>();

                foreach (var img in images)
                {
                    action.Images.Add(img);
                }

                actions.Add(action);
            }

            return actions;
        }

        private static IList<ImageDTO> ReadImages(XElement xAction)
        {
            IList<ImageDTO> images = null;

            if (xAction.Element("images") != null)
            {
                images = new List<ImageDTO>();

                foreach (XElement xImage in xAction.Element("images").Elements("image"))
                {
                    ImageDTO image = new ImageDTO();

                    image.ImageID = Convert.ToString(xImage.Element("id").Value);
                    image.CreationDate = Convert.ToDateTime(xImage.Element("creationdate").Value);
                    image.Description = Convert.ToString(xImage.Element("description").Value);
                    image.Number = Convert.ToString(xImage.Element("number").Value);
                    image.Path = Convert.ToString(xImage.Element("path").Value);
                    image.Tags = new System.Collections.ObjectModel.ObservableCollection<string>();

                    foreach (string tag in Convert.ToString(xImage.Element("tags").Value).Split(';').ToList())
                    {
                        image.Tags.Add(tag);
                    }

                    IList<CommentDTO> comments = ReadComments(xImage);

                    image.Comments = new System.Collections.ObjectModel.ObservableCollection<CommentDTO>();

                    foreach (var comment in comments)
                    {
                        image.Comments.Add(comment);
                    }

                    images.Add(image);
                }
            }
            return images;

        }

        private static IList<CommentDTO> ReadComments(XElement xImage)
        {
            IList<CommentDTO> comments = new List<CommentDTO>();

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
