using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FacilityDocu.UI.Utilities.Services;
using System.IO;

namespace FacilityDocu.UI.Utilities
{
    public static class ProjectXmlWriter
    {
        public static void Write(ProjectDTO project, string projectXMLPath)
        {
            XElement xProject = new XElement("project");

            xProject.Add(new XElement("id", project.ProjectID));
            xProject.Add(new XElement("template", project.Template));
            xProject.Add(new XElement("createdby", project.CreatedBy));
            xProject.Add(new XElement("createdtime", project.CreationDate));
            xProject.Add(new XElement("description", project.Description));
            xProject.Add(new XElement("updatedtime", project.LastUpdatedAt));
            xProject.Add(new XElement("updatedby", project.LastUpdatedBy));

            WriteRig(project.RigTypes.ToList(), xProject);

            xProject.Save(Path.Combine(Path.GetFullPath(projectXMLPath), string.Format("{0}.xml",project.ProjectID)));
        }

        private static void WriteRig(IList<RigTypeDTO> rigTypes, XElement xProject)
        {
            XElement xRigs = new XElement("rigs");
            xProject.Add(xRigs);

            foreach (RigTypeDTO rigType in rigTypes)
            {
                XElement xRig = new XElement("rig");
                xRigs.Add(xRig);

                xRig.Add(new XAttribute("type", rigType.Name));

                WriteModule(rigType.Modules, xRig);
            }
        }

        private static void WriteModule(IList<ModuleDTO> modules, XElement xRig)
        {
            XElement xModules = new XElement("modules");
            xRig.Add(xModules);

            foreach (ModuleDTO module in modules)
            {
                XElement xModule = new XElement("module");
                xModules.Add(xModule);

                xModule.Add(new XElement("id", module.ModuleID));
                xModule.Add(new XElement("number", module.ModuleID));
                xModule.Add(new XElement("name", module.Name));

                WriteStep(module.Steps, xModule);
            }
        }

        private static void WriteStep(IList<StepDTO> steps, XElement xModule)
        {
            XElement xSteps = new XElement("steps");
            xModule.Add(xSteps);

            foreach (StepDTO step in steps)
            {
                XElement xStep = new XElement("step");
                xSteps.Add(xStep);

                xStep.Add(new XElement("id", step.StepID));
                xStep.Add(new XElement("number", step.StepID));
                xStep.Add(new XElement("name", step.Name));

                WriteAction(step.Actions, xStep);
            }
        }

        private static void WriteAction(IList<ActionDTO> actions, XElement xStep)
        {
            XElement xStepActions = new XElement("actions");
            xStep.Add(xStepActions);

            foreach (ActionDTO stepAction in actions)
            {
                XElement xStepAction = new XElement("action");
                xStepActions.Add(xStepAction);

                xStepAction.Add(new XElement("id", stepAction.ActionID));
                xStepAction.Add(new XElement("number", stepAction.ActionID));
                xStepAction.Add(new XElement("name", stepAction.Name));
                xStepAction.Add(new XElement("description", stepAction.Description));
                xStepAction.Add(new XElement("risks", stepAction.Risks));
                xStepAction.Add(new XElement("liftinggears", stepAction.LiftingGears));
                xStepAction.Add(new XElement("dimensions", stepAction.Dimensions));

                WriteImage(stepAction.Images, xStepAction);
                WriteTools(stepAction.Tools, xStepAction);
                WriteResources(stepAction.Resources, xStepAction);
            }
        }

        private static void WriteTools(IList<ToolDTO> tools, XElement xAction)
        {
            XElement xTools = new XElement("tools");
            xAction.Add(xTools);

            foreach (ToolDTO tool in tools)
            {
                XElement xTool = new XElement("tool");
                xTools.Add(xTool);

                xTool.Add(new XElement("id", tool.ToolID));
                xTool.Add(new XElement("name", tool.Name));
            }
        }

        private static void WriteResources(IList<ResourceDTO> resources, XElement xAction)
        {
            XElement xResources = new XElement("resources");
            xAction.Add(xResources);

            foreach (ResourceDTO resource in resources)
            {
                XElement xResource = new XElement("resource");
                xResources.Add(xResource);

                xResource.Add(new XElement("id", resource.ResourceID));
                xResource.Add(new XElement("name", resource.Name));
                xResource.Add(new XElement("count", resource.ResourceCount));
            }
        }


        private static void WriteImage(IList<ImageDTO> images, XElement xAction)
        {
            XElement xImages = new XElement("images");
            xAction.Add(xImages);

            foreach (ImageDTO image in images)
            {
                XElement xImage = new XElement("image");
                xImages.Add(xImage);

                xImage.Add(new XElement("id", image.ImageID));
                xImage.Add(new XElement("number", image.Number));
                xImage.Add(new XElement("creationdate", image.CreationDate));
                xImage.Add(new XElement("description", image.Description));
                xImage.Add(new XElement("path", image.Path));
                xImage.Add(new XElement("tags", image.Tags));

                WriteImageComments(image.Comments, xImage);
            }
        }

        private static void WriteImageComments(IList<CommentDTO> comments, XElement xImage)
        {
            XElement xComments = new XElement("comments");
            xImage.Add(xComments);

            foreach (CommentDTO comment in comments)
            {
                XElement xComment = new XElement("comment");
                xComments.Add(xComment);

                xComment.Add(new XElement("text", comment.Text));
                xComment.Add(new XElement("date", comment.CommentedAt));
                xComment.Add(new XElement("user", comment.User));
            }
        }
    }
}
