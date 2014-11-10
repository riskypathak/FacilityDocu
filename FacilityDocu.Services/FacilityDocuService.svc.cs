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

                    if (_currentProject != null && DateTime.Compare(_currentProject.LastUpdatedAt.GetValueOrDefault(), item.Value) > 0)
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

        public void UpdateProject(ProjectDTO projectDTO)
        {
            using (TabletApp_DatabaseEntities context = new TabletApp_DatabaseEntities())
            {
                if (Helper.IsNew(projectDTO.ProjectID))
                {
                    Project project = DTOConverter.ToProject(projectDTO);
                    project.User = context.Users.Single(u => u.UserName.Equals(projectDTO.CreatedBy.UserName));
                    project.User1 = context.Users.Single(u => u.UserName.Equals(projectDTO.LastUpdatedBy.UserName));

                    context.Projects.Add(project);
                    context.SaveChanges();
                }
                else
                {
                    DTOConverter.IsUpdate = true;

                    Project project = DTOConverter.ToProject(projectDTO);

                    int projectID = Convert.ToInt32(projectDTO.ProjectID);

                    Project existingProject = context.Projects.SingleOrDefault(p => p.ProjectID.Equals(projectID));

                    existingProject.Close = project.Close;
                    existingProject.LastUpdatedAt = project.LastUpdatedAt;



                    foreach (ProjectDetail existingPD in existingProject.ProjectDetails.ToList())
                    {
                        ProjectDetail updateProject = project.ProjectDetails.FirstOrDefault(p => p.ProjectDetailID.Equals(existingPD.ProjectDetailID));

                        existingPD.Dimensions = updateProject.Dimensions;
                        existingPD.LiftingGears = updateProject.LiftingGears;
                        existingPD.Risks = updateProject.Risks;

                        existingPD.ActionName = updateProject.ActionName;
                        existingPD.Description = updateProject.Description;
                    }

                    List<ProjectDetail> newProjectDetails = project.ProjectDetails.Where(p => Helper.IsNew(p.ProjectDetailID.ToString())).ToList();
                    newProjectDetails.ForEach(np => existingProject.ProjectDetails.Add(np));



                    context.SaveChanges();
                }
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

                        int projectDetailID = context.ProjectDetails.First(p => p.ProjectDetailID.Equals(actionID)).ProjectDetailID;

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

        public IList<ToolDTO> GetTools()
        {
            IList<ToolDTO> tools = new List<ToolDTO>();

            using (var context = new TabletApp_DatabaseEntities())
            {
                context.Tools.ToList().ForEach(t => tools.Add(new ToolDTO() { ToolID = t.ToolID.ToString(), Name = t.ToolName }));
            }

            return tools;
        }
    }
}
