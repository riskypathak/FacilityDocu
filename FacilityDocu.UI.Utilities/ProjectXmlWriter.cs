using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FacilityDocu.UI.Utilities.Services;

namespace FacilityDocu.UI.Utilities
{
    public static class ProjectXmlWriter
    {
        public static void Write(ProjectDTO project)
        {
            XElement xProject = new XElement("Project");

            xProject.Add(new XElement("id", project.ProjectID));
            xProject.Add(new XElement("template", project.Template));
            xProject.Add(new XElement("createby", project.CreatedBy));
            xProject.Add(new XElement("createdtime", project.CreationDate));
            xProject.Add(new XElement("description", project.Description));
            xProject.Add(new XElement("updatetime", project.LastUpdatedAt));
            xProject.Add(new XElement("updateby", project.LastUpdatedBy));

            WriteRig(project.RigTypes.ToList(), xProject);
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
                xStepAction.Add(new XElement("risks", stepAction.Description));
                xStepAction.Add(new XElement("liftinggears", stepAction.LiftingGears));
                xStepAction.Add(new XElement("dimensions", stepAction.Dimensions));

                WriteImage(stepAction.Images, xStep);
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
            }
        }
    }
}
