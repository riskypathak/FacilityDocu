using System;
using Tablet_App.ServiceReference1;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tablet_App;
using System.IO;
using System.Net.Http;
using Windows.Storage;


namespace Tablet_App
{
    public static class ProjectXmlWriter
    {

        public static async void Write(ProjectDTO project)
        {
            XElement xProject = new XElement("project");

            xProject.Add(new XElement("id", project.ProjectID));
            xProject.Add(new XElement("template", project.Template));
            xProject.Add(new XElement("closed", project.Closed));
            xProject.Add(new XElement("createdby", project.CreatedBy.Name));
            xProject.Add(new XElement("createdtime", project.CreationDate));
            xProject.Add(new XElement("description", project.Description));
            xProject.Add(new XElement("updatedtime", project.LastUpdatedAt));
            xProject.Add(new XElement("updatedby", project.LastUpdatedBy.Name));

            WriteRig(project.RigTypes.ToList(), xProject);

            StorageFolder xmlPath = await StorageFolder.GetFolderFromPathAsync(Data.ProjectXmlPath);

            StorageFile xmlFile = await xmlPath.CreateFileAsync(project.ProjectID.ToString() + ".xml", CreationCollisionOption.ReplaceExisting);
            using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
            {
                xProject.Save(fileStream);
            }

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

            int count = 1;
            foreach (ModuleDTO module in modules)
            {
                XElement xModule = new XElement("module");
                xModules.Add(xModule);

                xModule.Add(new XElement("id", module.ModuleID));
                xModule.Add(new XElement("number", count.ToString("00")));
                xModule.Add(new XElement("name", module.Name));

                WriteStep(module.Steps, xModule);
                count++;
            }
        }

        private static void WriteStep(IList<StepDTO> steps, XElement xModule)
        {
            XElement xSteps = new XElement("steps");
            xModule.Add(xSteps);

            int count = 1;
            foreach (StepDTO step in steps)
            {
                XElement xStep = new XElement("step");
                xSteps.Add(xStep);

                xStep.Add(new XElement("id", step.StepID));
                xStep.Add(new XElement("number", count.ToString("00")));
                xStep.Add(new XElement("name", step.Name));

                WriteAction(step.Actions, xStep);
                count++;
            }
        }

        private static void WriteAction(IList<ActionDTO> actions, XElement xStep)
        {
            XElement xStepActions = new XElement("actions");
            xStep.Add(xStepActions);

            int count = 1;
            foreach (ActionDTO stepAction in actions)
            {
                XElement xStepAction = new XElement("action");
                xStepActions.Add(xStepAction);

                xStepAction.Add(new XElement("id", stepAction.ActionID));
                xStepAction.Add(new XElement("number", count.ToString("00")));
                xStepAction.Add(new XElement("name", stepAction.Name));
                xStepAction.Add(new XElement("description", stepAction.Description));

                WriteImage(stepAction.Images, xStepAction);

                count++;
            }
        }

        private static void WriteImage(IList<ImageDTO> images, XElement xAction)
        {
            XElement xImages = new XElement("images");
            xAction.Add(xImages);

            int count = 1;

            foreach (ImageDTO image in images)
            {
                XElement xImage = new XElement("image");
                xImages.Add(xImage);

                xImage.Add(new XElement("id", image.ImageID));
                xImage.Add(new XElement("number", count.ToString("00")));
                xImage.Add(new XElement("creationdate", image.CreationDate));
                xImage.Add(new XElement("description", image.Description));

                xImage.Add(new XElement("path", SaveImage(image)));
                xImage.Add(new XElement("tags", string.Join(";",image.Tags.ToArray())));

                WriteImageComments(image.Comments, xImage);

                count++;
            }
        }
        
        private static string SaveImage(ImageDTO image)
        {
            string savedPath = Path.Combine(Data.ImagesPath, string.Format("{0}.jpg", image.ImageID.ToString()));

            return savedPath;
        }

        private static void WriteImageComments(IList<CommentDTO> comments, XElement xImage)
        {
            XElement xComments = new XElement("comments");
            xImage.Add(xComments);

            if (comments != null)
            {
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
}
