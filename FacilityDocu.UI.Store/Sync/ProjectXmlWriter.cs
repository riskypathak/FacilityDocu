﻿using System;
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
        public static async Task Write(ProjectDTO project)
        {
            XElement xProject = new XElement("project");
            xProject.Add(new XElement("id", project.ProjectID));
            xProject.Add(new XElement("template", project.Template));
            xProject.Add(new XElement("closed", project.Closed));
            xProject.Add(new XElement("createdby", project.CreatedBy.Name));
            xProject.Add(new XElement("createdtime", project.CreationDate));
            xProject.Add(new XElement("description", project.Description));

            WriteRig(project.RigTypes.ToList(), xProject);
            StorageFolder xmlPath = await StorageFolder.GetFolderFromPathAsync(Data.ProjectXmlPath);
            StorageFile xmlFile = await xmlPath.CreateFileAsync(project.ProjectID.ToString() + ".xml", CreationCollisionOption.ReplaceExisting);
            using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
            {
                xProject.Save(fileStream);
            }

            await Backup(xmlFile);
        }

        private async static Task Backup(StorageFile xmlFile)
        {
            string newName = string.Format("{0}_{1}", xmlFile.Name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            IStorageFolder backupFolder = await StorageFolder.GetFolderFromPathAsync(Data.BackupPath);
            await xmlFile.CopyAsync(backupFolder, newName, NameCollisionOption.ReplaceExisting);
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

                string actionName = "";
                string actionDescription = "";

                if (stepAction.Name.Contains("xaml"))
                {
                    XElement xname = XElement.Parse(stepAction.Name);
                    actionName = xname.Value;
                }
                else
                {
                    actionName = stepAction.Name;
                }

                if (stepAction.Description.Contains("xaml"))
                {
                    XElement xdesc = XElement.Parse(stepAction.Description);
                    actionDescription = xdesc.Value;
                }
                else
                {
                    actionDescription = stepAction.Description;
                }

                xStepAction.Add(new XElement("id", stepAction.ActionID));
                xStepAction.Add(new XElement("number", count.ToString("00")));
                xStepAction.Add(new XElement("name", actionName));
                xStepAction.Add(new XElement("description", actionDescription));
                WriteImage(stepAction.Images, xStepAction);
                count++;
            }
        }

        private static async void WriteImage(IList<ImageDTO> images, XElement xAction)
        {
            XElement xImages = new XElement("images");
            xAction.Add(xImages);

            if (!Data.SYNC_PROCESS)
            {
                int count = 1;
                foreach (ImageDTO image in images)
                {
                    XElement xImage = new XElement("image");
                    xImages.Add(xImage);
                    xImage.Add(new XElement("id", image.ImageID));
                    xImage.Add(new XElement("number", count.ToString("00")));
                    xImage.Add(new XElement("creationdate", image.CreationDate));
                    xImage.Add(new XElement("description", image.Description));
                    xImage.Add(new XElement("tags", string.Join(";", image.Tags.ToArray())));

                    string savedPath = Path.Combine(Data.ImagesPath, string.Format("{0}.jpg", image.ImageID.ToString()));

                    if(Data.PUBLISH_PROCESS)
                    {
                        //savedPath = string.Empty;
                    }
                    
                    xImage.Add(new XElement("path", savedPath));
                    WriteImageComments(image.Comments, xImage);

                    count++;

                    //This wont be called ever
                    await SaveImage(image);
                }
            }
        }

        private static async Task SaveImage(ImageDTO image)
        {
            if (Data.SYNC_PROCESS)
            {
                using (HttpClient webClient = new HttpClient())
                {
                    IStorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Data.ImagesPath);
                    IStorageFile file = await folder.CreateFileAsync(string.Format("{0}.jpg", image.ImageID.ToString()), CreationCollisionOption.ReplaceExisting);

                    byte[] imageData = await webClient.GetByteArrayAsync(image.Path);
                    using (Stream stream = await file.OpenStreamForWriteAsync())
                    {
                        stream.Seek(0, SeekOrigin.End);
                        await stream.WriteAsync(imageData, 0, imageData.Length);
                    }
                    await FileIO.WriteBytesAsync(file, imageData);
                }
            }
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