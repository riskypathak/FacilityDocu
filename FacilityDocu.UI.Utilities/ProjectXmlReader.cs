﻿using FacilityDocu.DTO;

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
            project.Client = (xProject.Element("client") != null) ? xProject.Element("client").Value : null;
            project.Location = (xProject.Element("location") != null) ? xProject.Element("location").Value : null;
            project.ProjectNumber = (xProject.Element("projectnumber") != null) ? xProject.Element("projectnumber").Value : null;
            project.Persons = (xProject.Element("persons") != null) ? xProject.Element("persons").Value : null;

            project.ProjectID = Convert.ToString(xProject.Element("id").Value);
            project.Template = Convert.ToBoolean(xProject.Element("template").Value);
            project.Closed = Convert.ToBoolean(xProject.Element("closed").Value);

            if (!onlyProjectAttributes)
            {
                IList<RigTypeDTO> rigTypes = ReadRigTypes(project, xProject);
                project.RigTypes = rigTypes.ToArray();
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
                rigType.RigTypeID = Convert.ToString(xRigType.Attribute("id").Value);

                IList<ModuleDTO> modules = ReadModules(xRigType);
                rigType.Modules = modules.ToArray();
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
                module.Steps = steps.ToArray();
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
                step.StepID = Convert.ToString(xStep.Element("id").Value);
                step.Name = Convert.ToString(xStep.Element("name").Value);
                step.Number = Convert.ToString(xStep.Element("number").Value);

                IList<ActionDTO> actions = ReadActions(xStep);

                step.Actions = actions.ToArray();
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
                action.ActionID = Convert.ToString(xAction.Element("id").Value);
                action.Name = Convert.ToString(xAction.Element("name").Value);
                action.Number = Convert.ToString(xAction.Element("number").Value);
                action.Description = Convert.ToString(xAction.Element("description").Value);
                action.Risks = Convert.ToString(xAction.Element("risks").Value);
                action.LiftingGears = Convert.ToString(xAction.Element("liftinggears").Value);
                action.Dimensions = Convert.ToString(xAction.Element("dimensions").Value);
                action.Tools = Convert.ToString(xAction.Element("tools").Value);
                action.People = Convert.ToString(xAction.Element("people").Value);
                action.Machines = Convert.ToString(xAction.Element("machines").Value);

                action.PublishedAt = Convert.ToDateTime(xAction.Element("publishedat").Value);
                action.PublishedBy = new UserDTO() { UserName = Convert.ToString(xAction.Element("publishedby").Value) };
                action.LastUpdatedAt = Convert.ToDateTime(xAction.Element("lastupdatedat").Value);
                action.LastUpdatedBy = new UserDTO() { UserName = Convert.ToString(xAction.Element("lastupdatedby").Value) };


                action.IsNameWarning = Convert.ToBoolean(xAction.Element("namewarning").Value);
                action.IsDescriptionwarning = Convert.ToBoolean(xAction.Element("descriptionwarning").Value);
                action.ImportantName = Convert.ToString(xAction.Element("importantname").Value);
                action.ImportantDescription = Convert.ToString(xAction.Element("importantdescription").Value);
                action.IsAnalysis = Convert.ToBoolean(xAction.Element("isanalysis").Value);

                IList<ImageDTO> images = ReadImages(xAction);
                action.Images = images.ToArray();

                IList<AttachmentDTO> attachments = ReadAttachments(xAction);
                action.Attachments = attachments.ToArray();

                IList<RiskAnalysisDTO> analaysis = ReadAnalsysis(xAction);
                action.RiskAnalysis = analaysis.ToArray();

                actions.Add(action);
            }
            return actions;
        }

        private static IList<AttachmentDTO> ReadAttachments(XElement xAction)
        {
            IList<AttachmentDTO> attachments = new List<AttachmentDTO>();

            if (xAction.Element("attachments") != null)
            {
                foreach (XElement xAttachment in xAction.Element("attachments").Elements("attachment"))
                {
                    AttachmentDTO attachment = new AttachmentDTO();
                    attachment.AttachmentID = Convert.ToString(xAttachment.Element("id").Value);
                    attachment.Name = Convert.ToString(xAttachment.Element("name").Value);
                    attachment.Path = Convert.ToString(xAttachment.Element("path").Value);

                    attachments.Add(attachment);
                }
            }
            return attachments;
        }

        private static IList<RiskAnalysisDTO> ReadAnalsysis(XElement xAction)
        {
            IList<RiskAnalysisDTO> analysiss = new List<RiskAnalysisDTO>();

            foreach (XElement xAnalysis in xAction.Element("risksAnalysis").Elements("riskAnalysis"))
            {
                RiskAnalysisDTO analysise = new RiskAnalysisDTO();
                analysise.RiskAnalysisID = Convert.ToString(xAnalysis.Element("id").Value);
                analysise.Activity = Convert.ToString(xAnalysis.Element("activity").Value);
                analysise.L = Convert.ToString(xAnalysis.Element("l").Value);
                analysise.S = Convert.ToInt16(xAnalysis.Element("s").Value);
                analysise.Controls = Convert.ToString(xAnalysis.Element("controls").Value);
                analysise.Danger = Convert.ToString(xAnalysis.Element("danger").Value);
                analysise.Responsible = Convert.ToString(xAnalysis.Element("responsible").Value);

                analysiss.Add(analysise);
            }
            return analysiss;
        }

        private static IList<ToolDTO> ReadTools(XElement xAction)
        {
            IList<ToolDTO> tools = new List<ToolDTO>();

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
            IList<ResourceDTO> resources = new List<ResourceDTO>();

            foreach (XElement xResource in xAction.Element("resources").Elements("resource"))
            {
                ResourceDTO resource = new ResourceDTO();
                resource.ResourceID = Convert.ToString(xResource.Element("id").Value);
                resource.Name = Convert.ToString(xResource.Element("name").Value);
                resource.Type = Convert.ToString(xResource.Element("type").Value);

                resource.ResourceCount = Convert.ToString(xResource.Element("count").Value);

                resources.Add(resource);
            }
            return resources;
        }

        private static IList<ImageDTO> ReadImages(XElement xAction)
        {
            IList<ImageDTO> images = new List<ImageDTO>();

            foreach (XElement xImage in xAction.Element("images").Elements("image"))
            {
                ImageDTO image = new ImageDTO();
                image.ImageID = Convert.ToString(xImage.Element("id").Value);
                image.CreationDate = Convert.ToDateTime(xImage.Element("creationdate").Value);
                image.Description = Convert.ToString(xImage.Element("description").Value);
                image.Number = Convert.ToString(xImage.Element("number").Value);
                image.Path = Convert.ToString(xImage.Element("path").Value);
                image.Tags = Convert.ToString(xImage.Element("tags").Value).Split(';');
                image.Used = Convert.ToBoolean(xImage.Element("used").Value);

                IList<CommentDTO> comments = ReadComments(xImage);
                image.Comments = comments.ToArray();

                images.Add(image);
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
