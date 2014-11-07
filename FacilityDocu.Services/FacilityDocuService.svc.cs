using FacilityDocu.DTO;
using FacilityDocu.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using FacilityDocu.Services.EntityDTOConverter;
using System.IO;

namespace FacilityDocu.Services
{
    public class FacilityDocuService : IFacilityDocuService
    {
        public Dictionary<int, bool> IsSync(Dictionary<int, DateTime> ProjectsData)
        {
            Dictionary<int, bool> _statusData = new Dictionary<int, bool>();

            using (var _db = new TabletApp_DatabaseEntities())
            {
                foreach (var item in ProjectsData)
                {

                    var _currentProject = _db.Projects.Where(x => x.ProjectID == item.Key).FirstOrDefault();
                    if (_currentProject != null && DateTime.Compare(_currentProject.LastUpdatedAt.GetValueOrDefault(), item.Value) >= 0)
                    {
                        _statusData.Add(item.Key, true);
                    }
                    else
                    {
                        _statusData.Add(item.Key, false);
                    }


                }
            }

            return _statusData;
        }

        public List<ProjectDTO> GetProjectDetails(IList<int> ProjectIDs)
        {
            var _projectData = new List<ProjectDTO>();

            using (var _db = new TabletApp_DatabaseEntities())
            {


                foreach (var id in ProjectIDs)
                {
                    var _currentProject = _db.Projects
                        .FirstOrDefault(x => x.ProjectID == id);
                    if (_currentProject != null)
                    {
                        //_db.Entry(_currentProject).Reference(s => s.User).Load();

                        _projectData.Add(EntityConverter.ToProjectDTO(_currentProject));
                    }
                    else
                    {
                        _projectData.Add(null);
                    }



                }
            }
            return _projectData;

        }

        public bool Login(string userName, string password)
        {
            bool isAuthenticated = false;

            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                isAuthenticated = context.Users.Where(u => u.UserName.Equals(userName) && u.Password.Equals(password)).Count() > 0 ? true : false;
            }

            return isAuthenticated;
        }

        public void UpdateProject(ProjectDTO project)
        {

            int dbGeneratedIdentity = 0;
            //as projectid is strong in dto 
            if (string.IsNullOrEmpty(project.ProjectID) || project.ProjectID == "0")
            {
                using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
                {
                    Project DbProject = new Project();
                    //TODO
                    //DbProject.Close = project.CreatedBy
                    DbProject.Description = project.Description;
                    DbProject.CreationDate = project.CreationDate;
                    //TODO
                    //DbProject.CreatedBy = project.CreatedBy;
                    DbProject.LastUpdatedAt = project.LastUpdatedAt;
                    DbProject.Template = project.Template;
                    //TODO
                    DbProject.Close = false;

                    //TODO project must have a user name as its F.K in project table
                    // DbProject.UserName = project.

                    foreach (RigTypeDTO rigTypeDTO in project.RigTypes)
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
                                    projectDetail.ProjectID = DbProject.ProjectID;
                                    DbProject.ProjectDetails.Add(projectDetail);

                                    foreach (ImageDTO image in actionDTO.Images)
                                    {

                                        ProjectActionImage actionimg = new ProjectActionImage();
                                        actionimg.ID = dbGeneratedIdentity--;
                                        actionimg.ProjectDetailID = projectDetail.ProjectDetailID;

                                        Image img = new Image();
                                        img.ImageID = dbGeneratedIdentity--;
                                        //image dto should have an image name
                                        // img.Path = "Images" + image.name + new GUID();
                                        img.Tags = String.Join(",", image.Tags);

                                        img.Description = image.Description;

                                        actionimg.Image = img;

                                        //TODO no image data(byte)
                                        projectDetail.ProjectActionImages.Add(actionimg);

                                    }



                                }
                            }
                        }
                    }







                    //foreach (ProjectActionRisk projectActionRisk in ProjectDetails.ProjectActionRisks)
                    //{
                    //    projectActionRisk.ID = _tempId--;
                    //    projectActionRisk.ProjectDetailID = ProjectDetails.ProjectDetailID;
                    //    projectActionRisk.ProjectDetail = ProjectDetails;
                    //    context.ProjectActionRisks.Add(projectActionRisk);

                    //}
                    //foreach (ProjectActionDimension ProjectActionDimension in ProjectDetails.ProjectActionDimensions)
                    //{
                    //    ProjectActionDimension.ID = _tempId--;
                    //    ProjectActionDimension.ProjectDetailID = ProjectDetails.ProjectDetailID;
                    //    ProjectActionDimension.ProjectDetail = ProjectDetails;
                    //    context.ProjectActionDimensions.Add(ProjectActionDimension);

                    //}
                    //foreach (ProjectActionTool ProjectActionTools in ProjectDetails.ProjectActionTools)
                    //{
                    //    ProjectActionTools.ID = _tempId--;
                    //    ProjectActionTools.ProjectDetailID = ProjectDetails.ProjectDetailID;
                    //    ProjectActionTools.ProjectDetail = ProjectDetails;
                    //    context.ProjectActionTools.Add(ProjectActionTools);

                    //}






                    //foreach (ProjectActionImage ProjectActionImages in project.RigTypes.ProjectActionImages)
                    //{
                    //    ProjectActionImages.ID = _tempId--;
                    //    ProjectActionImages.ProjectDetailID = ProjectDetails.ProjectDetailID;
                    //    ProjectActionImages.ProjectDetail = ProjectDetails;
                    //    context.ProjectActionImages.Add(ProjectActionImages);

                    //}

                    //foreach (ProjectModuleResource projectModuleResource in project.ProjectModuleResources)
                    //{

                    //    projectModuleResource.ID = _tempId--;
                    //    projectModuleResource.ProjectID = project.ProjectID;
                    //    projectModuleResource.Project = project;


                    //}
                    //foreach (ProjectRiskAnalysi projectRiskAnalysis in project.ProjectRiskAnalysis)
                    //{

                    //    projectRiskAnalysis.ID = _tempId--;
                    //    projectRiskAnalysis.ProjectID = project.ProjectID;
                    //    projectRiskAnalysis.Project = project;


                    //}


                    context.SaveChanges();

                }



            }
            else
            {
                // update existing 
                using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
                {
                    //TODO project id should be int in dto ?
                    Project DbProject = context.Projects.Where(x => x.ProjectID == Convert.ToInt32(project.ProjectID)).FirstOrDefault();
                    if (DbProject != null)
                    {
                        context.ProjectDetails.RemoveRange(context.ProjectDetails.Where(x => x.ProjectID == Convert.ToInt32(project.ProjectID)).ToList());
                        context.ProjectRiskAnalysis.RemoveRange(context.ProjectRiskAnalysis.Where(x => x.ProjectDetailID == Convert.ToInt32(project.ProjectID)).ToList());

                        //TODO delete all other mappings
                        //TODO
                        //DbProject.Close = project.CreatedBy
                        DbProject.Description = project.Description;
                        DbProject.CreationDate = project.CreationDate;
                        //TODO
                        //DbProject.CreatedBy = project.CreatedBy;
                        DbProject.LastUpdatedAt = project.LastUpdatedAt;
                        DbProject.Template = project.Template;
                        //TODO
                        DbProject.Close = false;

                        //TODO project must have a user name as its F.K in project table
                        // DbProject.UserName = project.

                        foreach (RigTypeDTO rigTypeDTO in project.RigTypes)
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
                                        projectDetail.ProjectID = DbProject.ProjectID;
                                        DbProject.ProjectDetails.Add(projectDetail);

                                        foreach (ImageDTO image in actionDTO.Images)
                                        {

                                            ProjectActionImage actionimg = new ProjectActionImage();
                                            actionimg.ID = dbGeneratedIdentity--;
                                            actionimg.ProjectDetailID = projectDetail.ProjectDetailID;

                                            Image img = new Image();
                                            img.ImageID = dbGeneratedIdentity--;
                                            //image dto should have an image name
                                            // img.Path = "Images" + image.name + new GUID();
                                            img.Tags = String.Join(",", image.Tags);

                                            img.Description = image.Description;

                                            actionimg.Image = img;

                                            //TODO no image data(byte)
                                            projectDetail.ProjectActionImages.Add(actionimg);

                                        }



                                    }
                                }
                            }
                        }








                        //_project.Description = project.Description;
                        //_project.CreationDate = project.CreationDate;
                        //_project.CreatedBy = project.CreatedBy;
                        //_project.LastUpdatedAt = project.LastUpdatedAt;
                        //_project.LastUpdatedBy = project.LastUpdatedBy;
                        //_project.Template = project.Template;
                        //_project.UserName = project.UserName;
                        //_project.User = project.User;

                        // context.ProjectDetails.RemoveRange(context.ProjectDetails.Where(x=>x.ProjectID ==project.ProjectID ).ToList()) ;
                        // context.ProjectModuleResources.RemoveRange(context.ProjectModuleResources.Where(x=>x.ProjectID ==project.ProjectID ).ToList()) ;
                        // context.ProjectRiskAnalysis.RemoveRange(context.ProjectRiskAnalysis.Where(x=>x.ProjectID ==project.ProjectID ).ToList()) ;
                        // var _tempId = -1;
                        // foreach (ProjectDetail ProjectDetails in project.ProjectDetails)
                        // {
                        //     ProjectDetails.Project = project;
                        //     ProjectDetails.ProjectDetailID = _tempId--;
                        //     context.ProjectDetails.Add(ProjectDetails);
                        //     context.SaveChanges();

                        //     foreach (ProjectActionRisk projectActionRisk in ProjectDetails.ProjectActionRisks)
                        //     {
                        //         projectActionRisk.ID = _tempId--;
                        //         projectActionRisk.ProjectDetailID = ProjectDetails.ProjectDetailID;
                        //         projectActionRisk.ProjectDetail = ProjectDetails;
                        //         context.ProjectActionRisks.Add(projectActionRisk);

                        //     }
                        //     foreach (ProjectActionDimension ProjectActionDimension in ProjectDetails.ProjectActionDimensions)
                        //     {
                        //         ProjectActionDimension.ID = _tempId--;
                        //         ProjectActionDimension.ProjectDetailID = ProjectDetails.ProjectDetailID;
                        //         ProjectActionDimension.ProjectDetail = ProjectDetails;
                        //         context.ProjectActionDimensions.Add(ProjectActionDimension);

                        //     }
                        //     foreach (ProjectActionTool ProjectActionTools in ProjectDetails.ProjectActionTools)
                        //     {
                        //         ProjectActionTools.ID = _tempId--;
                        //         ProjectActionTools.ProjectDetailID = ProjectDetails.ProjectDetailID;
                        //         ProjectActionTools.ProjectDetail = ProjectDetails;
                        //         context.ProjectActionTools.Add(ProjectActionTools);

                        //     }


                        //     foreach (ProjectActionImage ProjectActionImages in ProjectDetails.ProjectActionImages)
                        //     {
                        //         ProjectActionImages.ID = _tempId--;
                        //         ProjectActionImages.ProjectDetailID = ProjectDetails.ProjectDetailID;
                        //         ProjectActionImages.ProjectDetail = ProjectDetails;
                        //         context.ProjectActionImages.Add(ProjectActionImages);

                        //     }
                        // }
                        // foreach (ProjectModuleResource projectModuleResource in project.ProjectModuleResources)
                        // {

                        //     projectModuleResource.ID = _tempId--;
                        //     projectModuleResource.ProjectID = project.ProjectID;
                        //     projectModuleResource.Project = project;


                        // }
                        // foreach (ProjectRiskAnalysi projectRiskAnalysis in project.ProjectRiskAnalysis)
                        // {

                        //     projectRiskAnalysis.ID = _tempId--;
                        //     projectRiskAnalysis.ProjectID = project.ProjectID;
                        //     projectRiskAnalysis.Project = project;


                        // }


                        context.SaveChanges();


                    }
                }
            }
        }

        public List<ModuleDTO> GetModules(int rigTypeID)
        {
            List<ModuleDTO> modulesDTO = new List<ModuleDTO>();

            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                List<Module> modules = context.Modules.Where(m => m.RigTypeID.Value.Equals(rigTypeID)).ToList();

                modules.ForEach(m => modulesDTO.Add(EntityConverter.ToModuleDTO(m)));
            }

            return modulesDTO;
        }

        public IList<StepDTO> GetSteps(int ModuleID)
        {
            List<StepDTO> stepsDTO = new List<StepDTO>();
            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                foreach (Step step in context.ModuleSteps.Where(x => x.ModuleID == ModuleID).Select(x => x.Step))
                {
                    stepsDTO.Add(EntityConverter.ToStepDTO(step));
                }

                return stepsDTO;
            }
        }

        public IList<ActionDTO> GetActions(int stepID)
        {
            List<ActionDTO> actionDTO = new List<ActionDTO>();
            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {

                var StepActions = context.StepActions.Where(x => x.StepID == stepID);
                foreach (var item in StepActions)
                {

                    actionDTO.Add(EntityConverter.ToActionDTO(item));

                }

                return actionDTO;
            }
        }

        public void UpdateActionImages(ActionDTO action)
        {
            int actionID = Convert.ToInt32(action.ActionID);

            int tempId = 0;
            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                foreach (var item in action.Images)
                {
                    int imageID = Convert.ToInt32(item.ImageID);

                    //insert
                    if (imageID == 0)
                    {
                        Image img = new Image()
                        {
                            CreationDate = item.CreationDate,
                            Description = item.Description,
                            Tags = String.Join(",", item.Tags),
                        };

                        foreach (CommentDTO comment in item.Comments)
                        {
                            ImageComment imgComment = new ImageComment()
                            {
                                CreationDate = comment.CommentedAt,
                                ImageID = img.ImageID,
                                Text = comment.Text,
                                ImageCommentID = tempId--
                            };
                            img.ImageComments.Add(imgComment);
                        }

                        int projectDetailID = context.ProjectDetails.First(p => p.ActionID.Value.Equals(actionID)).ProjectDetailID;

                        // Project action image is collection in Image 
                        ProjectActionImage ProjectActionImg = new ProjectActionImage()
                        {
                            ID = tempId--,
                            ImageID = img.ImageID,
                            ProjectDetailID = projectDetailID
                        };

                        img.ProjectActionImages.Add(ProjectActionImg);

                        context.Images.Add(img);
                        context.SaveChanges();

                        SaveImageToFile(img.ImageID, item);
                    }
                    else
                    {
                        //update
                        Image img = context.Images.First(x => x.ImageID == imageID);



                        img.Description = item.Description;
                        img.Tags = String.Join(",", item.Tags);

                        context.ImageComments.RemoveRange(context.ImageComments.Where(x => x.ImageID.Value == imageID));

                        foreach (CommentDTO commnet in item.Comments)
                        {
                            ImageComment imgComment = new ImageComment()
                            {
                                CreationDate = commnet.CommentedAt,
                                ImageID = img.ImageID,
                                Text = commnet.Text,
                                ImageCommentID = tempId--
                            };
                            img.ImageComments.Add(imgComment);
                        }

                        context.SaveChanges();

                        SaveImageToFile(img.ImageID, item);



                    }

                }
            }
        }

        private void SaveImageToFile(int imageId, ImageDTO item)
        {
            //image extension as for now using jpg
            string extension = "jpeg";
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/Data/Images/{0}.{1}", imageId, extension));

            using (MemoryStream ms = new MemoryStream(item.FileByteStream))
            {
                System.Drawing.Image.FromStream(ms).Save(filePath);
            }
        }
    }
}
