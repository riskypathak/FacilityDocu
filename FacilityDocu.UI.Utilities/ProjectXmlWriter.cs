using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FacilityDocu.UI.Utilities.Services;
using System.IO;
using System.Net;

namespace FacilityDocu.UI.Utilities
{
    public static class ProjectXmlWriter
    {
        public static void Write(ProjectDTO project)
        {
            XElement xProject = new XElement("project");

            xProject.Add(new XElement("id", project.ProjectID));
            xProject.Add(new XElement("template", project.Template));
            xProject.Add(new XElement("closed", project.Closed));
            xProject.Add(new XElement("createdby", project.CreatedBy.UserName));
            xProject.Add(new XElement("createdtime", project.CreationDate));
            xProject.Add(new XElement("description", project.Description));
            xProject.Add(new XElement("client", project.Client));
            xProject.Add(new XElement("location", project.Location));
            xProject.Add(new XElement("projectnumber", project.ProjectNumber));
            xProject.Add(new XElement("persons", project.Persons));

            WriteRig(project.RigTypes.ToList(), xProject);

            xProject.Save(Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", project.ProjectID)));
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
                xRig.Add(new XAttribute("id", rigType.RigTypeID));

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

                xStepAction.Add(new XElement("publishedat", stepAction.PublishedAt));
                xStepAction.Add(new XElement("publishedby", stepAction.PublishedBy));
                xStepAction.Add(new XElement("lastupdatedat", stepAction.LastUpdatedAt));
                xStepAction.Add(new XElement("lastupdatedby", stepAction.LastUpdatedBy));

                xStepAction.Add(new XElement("namewarning", stepAction.IsNameWarning));
                xStepAction.Add(new XElement("descriptionwarning", stepAction.IsDescriptionwarning));
                xStepAction.Add(new XElement("importantname", stepAction.ImportantName));
                xStepAction.Add(new XElement("importantdescription", stepAction.ImportantDescription));
                xStepAction.Add(new XElement("isanalysis", stepAction.IsAnalysis));

                xStepAction.Add(new XElement("id", stepAction.ActionID));
                xStepAction.Add(new XElement("number", count.ToString("00")));
                xStepAction.Add(new XElement("name", stepAction.Name));
                xStepAction.Add(new XElement("description", stepAction.Description));
                xStepAction.Add(new XElement("risks", stepAction.Risks));
                xStepAction.Add(new XElement("liftinggears", stepAction.LiftingGears));
                xStepAction.Add(new XElement("dimensions", stepAction.Dimensions));

                WriteImage(stepAction.Images, xStepAction);
                WriteAttachments(stepAction.Attachments, xStepAction);
                WriteTools(stepAction.Tools, xStepAction);
                WriteResources(stepAction.Resources, xStepAction);
                WriteRiskAnalysis(stepAction.RiskAnalysis, xStepAction);

                count++;
            }
        }

        private static void WriteAttachments(AttachmentDTO[] attachments, XElement xAction)
        {
            XElement xAttachments = new XElement("attachments");
            xAction.Add(xAttachments);

            if (attachments != null)
            {
                foreach (AttachmentDTO attachment in attachments)
                {
                    XElement xAttachment = new XElement("attachment");
                    xAttachments.Add(xAttachment);

                    xAttachment.Add(new XElement("id", attachment.AttachmentID));
                    xAttachment.Add(new XElement("name", attachment.Name));
                    xAttachment.Add(new XElement("path", SaveAttachment(attachment)));
                }
            }
        }

        private static void WriteRiskAnalysis(IList<RiskAnalysisDTO> riskAnalysiss, XElement xAction)
        {
            XElement xRiskAnalysiss = new XElement("risksAnalysis");
            xAction.Add(xRiskAnalysiss);

            foreach (RiskAnalysisDTO riskAnalysis in riskAnalysiss)
            {
                XElement xRiskAnalysis = new XElement("riskAnalysis");
                xRiskAnalysiss.Add(xRiskAnalysis);

                xRiskAnalysis.Add(new XElement("id", riskAnalysis.RiskAnalysisID));
                xRiskAnalysis.Add(new XElement("activity", riskAnalysis.Activity));
                xRiskAnalysis.Add(new XElement("danger", riskAnalysis.Danger));
                xRiskAnalysis.Add(new XElement("k", riskAnalysis.K));
                xRiskAnalysis.Add(new XElement("b", riskAnalysis.B));
                xRiskAnalysis.Add(new XElement("e", riskAnalysis.E));
                xRiskAnalysis.Add(new XElement("risk", riskAnalysis.Risk));
                xRiskAnalysis.Add(new XElement("controls", riskAnalysis.Controls));
                xRiskAnalysis.Add(new XElement("k_", riskAnalysis.K_));
                xRiskAnalysis.Add(new XElement("b_", riskAnalysis.B_));
                xRiskAnalysis.Add(new XElement("e_", riskAnalysis.E_));
                xRiskAnalysis.Add(new XElement("risk_", riskAnalysis.Risk_));
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
                xResource.Add(new XElement("type", resource.Type));
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
                xImage.Add(new XElement("used", image.Used));

                xImage.Add(new XElement("path", SaveImage(image)));
                xImage.Add(new XElement("tags", image.Tags));

                WriteImageComments(image.Comments, xImage);

                count++;
            }
        }

        private static string SaveImage(ImageDTO image)
        {
            string savedPath = Path.Combine(Data.PROJECT_IMAGES_FOLDER, string.Format("{0}.jpg", image.ImageID));

            if (Data.SYNC_DOWNLOAD_UPDATE && !image.Path.StartsWith("http")) 
            {
                savedPath = image.Path;
            }
            else if (Data.SYNC_DOWNLOAD || image.Path.StartsWith("http"))
            {
                using (WebClient webClient = new WebClient())
                {
                    //if image path is not webpath then it means that this action has not been updated
                    //and hence we dont require it to get it from server.
                    try
                    {
                        if (image.Path.StartsWith("http"))
                        {
                            if (File.Exists(savedPath))
                            {
                                File.Delete(savedPath);
                            }
                            webClient.DownloadFile(image.Path, savedPath);
                        }
                    }
                    catch(Exception)
                    {
                        //If not able to delete then just eat the exception
                    }
                }
            }

            return savedPath;
        }

        private static string SaveAttachment(AttachmentDTO attachment)
        {
            string savedPath = Path.Combine(Data.PROJECT_ATTACHMENTS_FOLDER, string.Format("{0}.pdf", attachment.AttachmentID));

            if (Data.SYNC_DOWNLOAD_UPDATE && !attachment.Path.StartsWith("http"))
            {
                savedPath = attachment.Path;
            }
            else if (Data.SYNC_DOWNLOAD || attachment.Path.StartsWith("http"))
            {
                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        if (attachment.Path.StartsWith("http"))
                        {
                            if (File.Exists(savedPath))
                            {
                                File.Delete(savedPath);
                            }
                            webClient.DownloadFile(attachment.Path, savedPath);
                        }
                    }
                    catch (Exception)
                    {
                        //If not able to delete then just eat the exception
                    }
                }
            }

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
