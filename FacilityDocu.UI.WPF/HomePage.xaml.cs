using FacilityDocu.UI.Utilities;
using FacilityDocu.UI.Utilities.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace FacilityDocLaptop
{

    public partial class HomePage : Window
    {
        Services.IFacilityDocuService service;

        private int currentRigIndex = 0;
        private int currentModuleIndex = 0;
        private int currentStepIndex = 0;
        private int currentActionIndex = 0;
        private int currentAnalysisIndex = 0;

        string projectid;
        string[] sourcePath = new string[100];
        string[] pictureFilename = new string[100];
        string[] totalpictureinoneindex = new string[100];
        int imagecountdiff;
        string actionidstring;
        int liftinggearcount;
        int imagecount;
        int riskcount;
        int dimensioncount;
        int toolcount;
        int resourcecount;
        int actioncount;
        int stepcount;
        int modulecount;
        int rigtypecount;
        int rigtypeindex = 0;
        int moduleindex = 0;
        int stepindex = 0;
        int actionindex = 0;
        int resourceindex = 0;
        int toolsindex = 0;
        int dimensionindex = 0;
        int riskindex = 0;
        int imageindex = 0;
        int liftinggearindex = 0;
        string XMLPath;
        string DATAPath;
        string BACKUPPath;
        string pdf_ProjectName;
        string pdf_RigType;
        string pdf_Module;

        string[] projectname = new string[100];
        string[] createdby = new string[100];
        string[] createdtime = new string[100];
        string[] updatedby = new string[100];
        string[] updatetime = new string[100];
        string[] xmlfilesArray = new string[100];
        string[] templatename = new string[100];
        string[] closed = new string[100];


        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();

        XElement xelement;


        List<string> rigdata = new List<string>();
        List<string> rigdata1 = new List<string>();
        List<string> moduledataname = new List<string>();
        List<string> moduledatanumber = new List<string>();
        List<string> moduleid = new List<string>();
        List<string> stepdataname = new List<string>();
        List<string> stepdatanumber = new List<string>();
        List<string> stepid = new List<string>();
        List<string> actionid = new List<string>();
        List<string> actiondata = new List<string>();
        List<string> actiondatanumber = new List<string>();
        List<string> actiondatadetails = new List<string>();
        List<string> actiondetailsdata = new List<string>();
        List<string> resourcedata = new List<string>();
        List<string> liftinggearsdata = new List<string>();
        List<string> toolsdata = new List<string>();
        List<string> dimensiondata = new List<string>();
        List<string> riskdata = new List<string>();
        List<string> imagedata = new List<string>();
        List<string> imageiddata = new List<string>();
        //PDF
        List<string> pdf_imagedataid = new List<string>();
        List<string> pdf_imagedataname = new List<string>();

        public class Grid_XMLRead
        {
            public int projectID { get; set; }
            public string projectName { get; set; }
            public string description { get; set; }
            public string createdBy { get; set; }
            public string createdTime { get; set; }
            public string updatedBy { get; set; }
            public string updatedTime { get; set; }
        }

        public HomePage()
        {
            InitializeComponent();

            this.DataContext = this;

            service = new Services.FacilityDocuServiceClient();
        }

        public void CreateListViewGrid()
        {
            IList<string> projectFiles = Directory.GetFiles(Data.PROJECT_XML_FOLDER, "*.xml");

            IList<ProjectDTO> projects = new List<ProjectDTO>();
            projectFiles.ToList().ForEach(f => projects.Add(ProjectXmlReader.ReadProjectXml(f, true)));

            listView.ItemsSource = projects.Where(p => !p.Template && !p.Closed);
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            bool isLogin = false;

            try
            {
                isLogin = service.Login(userName.Text, password.Password);
            }
            catch (Exception)
            {
                isLogin = true;
            }

            if (isLogin)
            {
                MakeVisible(gridHomePage);
            }
            else
            {
                password.Password = "";
                MessageBox.Show("Invalid Username or Password");
            }
        }

        public void MouseEnter()
        {
            if (this.Cursor != Cursors.Wait)

                Mouse.OverrideCursor = Cursors.Hand;
        }
        public void MouseLeave()
        {
            if (this.Cursor != Cursors.Wait)

                Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void NewProject_btn_Click(object sender, RoutedEventArgs e)
        {
            IList<ProjectDTO> projects = new List<ProjectDTO>();
            IList<string> projectFiles = Directory.GetFiles(Data.PROJECT_XML_FOLDER, "*.xml");
            projectFiles.ToList().ForEach(f => projects.Add(ProjectXmlReader.ReadProjectXml(f, true)));

            cmbTemplates.ItemsSource = projects.Where(p => p.Template);

            txtNewProjectName.Text = string.Empty;

            homePage.Title = "FacilityDocu - New Project";

            gdvNew.Visibility = Visibility.Visible;
        }

        private void ImportPhoto_btn_Click(object sender, RoutedEventArgs e)
        {
            importPhotos_grid.Visibility = Visibility.Visible;
        }

        private void importPhotos_back_Click(object sender, RoutedEventArgs e)
        {
            importPhotos_grid.Visibility = Visibility.Collapsed;
        }

        private void addPictures_btn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialogue = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png"
            };
            var result = openFileDialogue.ShowDialog();

            if (result == false) return;

            IList<ImageDTO> images = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images.ToList();

            openFileDialogue.FileNames.ToList().ForEach(i =>
                {
                    int counter = 1;
                    ImageDTO image = new ImageDTO()
                    {
                        CreationDate = DateTime.Now,
                        Description = "Image Added",
                        ImageID = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter),
                        Path = System.IO.Path.Combine(Data.PROJECT_IMAGES_FOLDER, string.Format("{0}.jpg", string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter++)))
                    };

                    System.IO.File.Copy(i, image.Path);
                    images.Add(image);
                });

            Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images = images.ToArray();

            ChangeScreenControls();

        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ProjectList_StylusInRange(object sender, StylusEventArgs e)
        {
        }

        private void editRisk_btn_Click(object sender, RoutedEventArgs e)
        {
            homePage.Title = "FacilityDocu - Edit Risk";
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) //_ProjectName_HyperLink
        {
        }

        private void txtblockActivity_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter();
        }

        private void txtblockActivity_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave();
        }

        private void editHierarchy_btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //homePage.Title = "FacilityDocu - Edit Hierarchy";
            //editPage_grid.Visibility = Visibility.Collapsed;
            //grid_EditHierarchy.Visibility = Visibility.Visible;

            //for (int i = 0; i < moduledataname.Count; i++)
            //    listboxModules.Items.Add(moduledataname[i]);

            //for (int i = 0; i < stepdataname.Count; i++)
            //listboxSteps.Items.Add(stepdataname[i]);

            //for (int i = 0; i < actiondata.Count; i++)
            //    listboxActions.Items.Add(actiondata[i]);
        }

        private void editHierarchy_btn_MouseEnter_1(object sender, MouseEventArgs e)
        {
            MouseEnter();
        }

        private void editHierarchy_btn_MouseLeave_1(object sender, MouseEventArgs e)
        {
            MouseLeave();
        }

        private void projectName_txt_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter();
        }

        private void projectName_txt_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e) //Logout Button
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to log out?", "Log out Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //log out activity
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e) //RigType Previous Button
        {


        }

        private void btnNext_Click(object sender, RoutedEventArgs e)  //RigType Next Button
        {

        }

        public void imageload()
        {
            for (int i = 0; i < imagedata.Count; i++)
            {
                BitmapImage bimg = new BitmapImage();
                System.Windows.Controls.Image imagename = new System.Windows.Controls.Image();
                bimg.BeginInit();
                bimg.UriSource = new Uri(imagedata[imageindex]);
                imagename.Source = bimg;
                imagename.Width = 150;
                imagename.Height = 150;
                bimg.EndInit();
                lstPictures.Items.Add(imagename);
            }
        }

        public void createfolder()
        {

            if (!Directory.Exists(imagedata[imageindex]))
            {
                Directory.CreateDirectory(imagedata[imageindex]);
            }

        }

        public void ReadPathFromConfigXML()
        {
            XElement xelement = XElement.Load("../../Assets/Config.xml");
            IEnumerable<XElement> name = xelement.Elements();

            var readPath = from query in xelement.Descendants("paths")



                           select new
                           {
                               xmlpath = query.Element("xmlp").Value,
                               datapath = query.Element("datap").Value,
                               backuppath = query.Element("backp").Value,
                           };

            foreach (var names in readPath)
            {
                XMLPath = names.xmlpath.ToString();
                DATAPath = names.datapath.ToString();
                BACKUPPath = names.backuppath.ToString();


            }
            // MessageBox.Show(DATAPath);
        }

        private void listboxTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txtK_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //char c = Convert.ToChar(e.Text);
            //if (Char.IsNumber(c))
            //    e.Handled = false;
            //else
            //    e.Handled = true;

            //base.OnPreviewTextInput(e);

            //   Regex rx = new Regex("\\b(?<word>\\w+)\\s+(\\k<word>)\\b",
            //  RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            // Regex rx = new Regex(@"^-?\d+(\.\d{2})?$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        public void ProjectSave()
        {
        }

        public void ReadXML()
        {


            XElement xelement = XElement.Load(System.IO.Path.GetFullPath("Data/ProjectXml") + "/" + "ProjectXML.xml");
            IEnumerable<XElement> name = xelement.Elements();

            var readProjectName = from query in xelement.Descendants("project")
                            .Elements("name")

                                  select new
                                  {
                                      name = query.Value,
                                  };

            foreach (var names in readProjectName)
            {
                pdf_ProjectName = names.name.ToString();
                // MessageBox.Show(ProjectName);
            }


            //RIG

            var readRigType = from query in xelement.Descendants("project")
                            .Elements("rigs")
                            .Elements("rig")
                              select new
                              {
                                  name = query.Attribute("type").Value,
                              };



            foreach (var names in readRigType)
            {
                pdf_RigType = names.name.ToString();
                //MessageBox.Show(names.name);
            }

            //MODULE
            var readModule = from query in xelement.Descendants("project")
                                 .Elements("rigs")
                                 .Elements("rig")
                                 .Elements("modules")
                                 .Elements("module")
                                 .Elements("name")
                             select new
                             {
                                 name = query.Value,

                             };
            foreach (var names in readModule)
            {
                pdf_Module = names.name.ToString();
                // MessageBox.Show(names.name);
            }


            //STEP
            var readStep = from query in xelement.Descendants("project")
                                 .Elements("rigs")
                                 .Elements("rig")
                                 .Elements("modules")
                                 .Elements("module")
                                 .Elements("steps")
                                 .Elements("step")

                           select new
                           {
                               name = query.Element("name").Value,


                           };
            foreach (var names in readStep)
            {
                //MessageBox.Show(names.name);
            }

            //ACTION
            var readAction = from query in xelement.Descendants("project")
                                 .Elements("rigs")
                                 .Elements("rig")
                                 .Elements("modules")
                                 .Elements("module")
                                 .Elements("steps")
                                 .Elements("step")
                                 .Elements("actions")
                                 .Elements("action")
                                 .Elements("name")

                             select new
                             {
                                 name = query.Value,


                             };
            foreach (var names in readAction)
            {
                MessageBox.Show(names.name);
                // ActionName = names.name;
            }


            //Image
            var readImage = from query in xelement.Descendants("project")
                                 .Elements("rigs")
                                 .Elements("rig")
                                 .Elements("modules")
                                 .Elements("module")
                                 .Elements("steps")
                                 .Elements("step")
                                 .Elements("actions")
                                 .Elements("action")
                                 .Elements("images")
                                 .Elements("image")

                            select new
                            {
                                ImgID = query.Element("id").Value,
                                ImgName = query.Element("name").Value,


                            };
            foreach (var names in readImage)
            {
                pdf_imagedataid.Add(names.ImgID.ToString());
                pdf_imagedataname.Add(names.ImgName.ToString());
                //  MessageBox.Show(names.name);
                // ActionName = names.name;
            }
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            ReadXML();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {


        }

        private void homePage_Loaded(object sender, RoutedEventArgs e)
        {
            CreateListViewGrid();
        }

        private void listView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            string projectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", (e.AddedItems[0] as ProjectDTO).ProjectID));
            Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(projectPath, false);

            ChangeScreenControls();

            editPage_grid.Visibility = Visibility.Visible;
            gridHomePage.Visibility = Visibility.Collapsed;
        }

        private void ChangeScreenControls()
        {
            txtProjectID.Text = Data.CURRENT_PROJECT.ProjectID;
            txtProjectDescription.Text = Data.CURRENT_PROJECT.Description;

            txtRigType.Text = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Name;

            ModuleDTO module = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex];
            txtModule.Text = string.Format("{0} {1}", module.Number, module.Name);

            StepDTO step = module.Steps[currentStepIndex];
            txtStepName.Text = string.Format("{0} {1}", step.Number, step.Name);

            ActionDTO action = step.Actions[currentActionIndex];
            txtAction.Text = action.Name.Trim();
            txtActionNumber.Text = action.Number.Trim();
            txtActionDetails.Text = action.Description.Trim();
            txtActionDimensions.Text = action.Dimensions.Trim();
            txtActionLiftingGears.Text = action.LiftingGears.Trim();
            txtActionRisks.Text = action.Risks.Trim();

            lstActionTools.ItemsSource = action.Tools;

            lstActionResourcesP.ItemsSource = action.Resources.Where(r => r.Type.Equals("people")).ToList();
            lstActionResourcesM.ItemsSource = action.Resources.Where(r => r.Type.Equals("machine")).ToList();

            lstPictures.Items.Clear();
            action.Images.ToList().ForEach(i => AddActionImage(i.Path));

            txtAction.IsReadOnly = true;
            txtActionDetails.IsReadOnly = true;
            txtActionDimensions.IsReadOnly = true;
            txtActionRisks.IsReadOnly = true;
            lstActionTools.IsEnabled = false;
            lstActionResourcesM.IsEnabled = false;
            lstActionResourcesP.IsEnabled = false;


            lstAttachments.ItemsSource = action.Attachments;

            if (action.RiskAnalysis.Length > 0)
            {
                RiskAnalysisDTO analysis = action.RiskAnalysis[currentAnalysisIndex];
                txtAnalysisActivity.Text = analysis.Activity;
                txtAnalysisB.Text = analysis.B.ToString();
                txtAnalysisB_.Text = analysis.B_.ToString();
                txtAnalysisControl.Text = analysis.Controls;
                txtAnalysisDanger.Text = analysis.Activity;
                txtAnalysisE.Text = analysis.E.ToString();
                txtAnalysisE_.Text = analysis.E_.ToString();
                txtAnalysisK.Text = analysis.K.ToString();
                txtAnalysisK_.Text = analysis.K_.ToString();
                txtAnalysisRisk.Text = analysis.Risk.ToString();
                txtAnalysisRisk_.Text = analysis.Risk_.ToString();
            }
            txtAnalysisActivity.IsReadOnly = true;
            txtAnalysisB.IsReadOnly = true;
            txtAnalysisB_.IsReadOnly = true;
            txtAnalysisControl.IsReadOnly = true;
            txtAnalysisDanger.IsReadOnly = true;
            txtAnalysisE.IsReadOnly = true;
            txtAnalysisE_.IsReadOnly = true;
            txtAnalysisK.IsReadOnly = true;
            txtAnalysisK_.IsReadOnly = true;
            txtAnalysisRisk.IsReadOnly = true;
            txtAnalysisRisk_.IsReadOnly = true;

        }

        private void AddActionImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            System.Windows.Controls.Image imagename = new System.Windows.Controls.Image();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath);
            imagename.Source = bitmap;
            imagename.Width = 150;
            imagename.Height = 150;
            bitmap.EndInit();
            lstPictures.Items.Add(imagename);
        }

        private void btnUpNext_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentRigIndex > 0)
            {
                currentRigIndex--;
                currentModuleIndex = 0;
                currentStepIndex = 0;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnDownNext_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentRigIndex < (Data.CURRENT_PROJECT.RigTypes.Count() - 1))
            {
                currentRigIndex++;
                currentModuleIndex = 0;
                currentStepIndex = 0;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnModuleLeft_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentModuleIndex > 0)// || currentRigIndex > (Data.CURRENT_PROJECT .RigTypes.Count() - 1) )
            {
                currentModuleIndex--;
                currentStepIndex = 0;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnModuleRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentModuleIndex < (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules.Count() - 1))
            {
                currentModuleIndex++;
                currentStepIndex = 0;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnStepRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentStepIndex < (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps.Count() - 1))
            {
                currentStepIndex++;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnStepLeft_Click(object sender, RoutedEventArgs e)
        {
            if (currentStepIndex > 0)// || currentRigIndex > (Data.CURRENT_PROJECT .RigTypes.Count() - 1) )
            {
                currentStepIndex--;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnActionRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentActionIndex < (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions.Count() - 1))
            {
                currentActionIndex++;
                currentAnalysisIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnActionLeft_Click(object sender, RoutedEventArgs e)
        {
            if (currentActionIndex > 0)
            {
                currentActionIndex--;
                currentAnalysisIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnActionAdd_Click_1(object sender, RoutedEventArgs e)
        {
            ActionDTO action = new ActionDTO()
                {
                    Name = "New Action",
                    Description = "New Action's Description",
                    Dimensions = "New Action's Dimesnions",
                    LiftingGears = "New Action's Lifting Gears",
                    Risks = "New Action's Risks",
                    Number = (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions.Length + 1).ToString("00"),
                    Resources = (new List<ResourceDTO>()).ToArray(),
                    RiskAnalysis = (new List<RiskAnalysisDTO>() { new RiskAnalysisDTO() }).ToArray(),
                    Tools = (new List<ToolDTO>()).ToArray(),
                    Images = (new List<ImageDTO>()).ToArray(),
                    ActionID = "0"
                };

            IList<ActionDTO> actions = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions.ToList();
            actions.Add(action);

            Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions = actions.ToArray();

            currentActionIndex = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions.Count() - 1;
            ChangeScreenControls();
        }

        private void btnActionEdit_Click_1(object sender, RoutedEventArgs e)
        {
            txtAction.IsReadOnly = false;
            txtActionDetails.IsReadOnly = false;
            txtActionDimensions.IsReadOnly = false;
            txtActionRisks.IsReadOnly = false;
            lstActionTools.IsEnabled = true;
            lstActionResourcesM.IsEnabled = true;
            lstActionResourcesP.IsEnabled = true;
        }

        private void btAnalysisLeft_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentAnalysisIndex > 0)
            {
                currentAnalysisIndex--;
                ChangeScreenControls();
            }
        }

        private void btAnalysisRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentAnalysisIndex < (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentAnalysisIndex].RiskAnalysis.Count() - 1))
            {
                currentAnalysisIndex++;

                ChangeScreenControls();
            }
        }

        private void btnAddAttachments_Click_1(object sender, RoutedEventArgs e)
        {
            var openFileDialogue = new OpenFileDialog()
            {
                Multiselect = true
            };

            var result = openFileDialogue.ShowDialog();

            if (result == false) return;

            IList<AttachmentDTO> attachments = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments.ToList();

            openFileDialogue.FileNames.ToList().ForEach(i =>
            {
                int counter = 1;
                AttachmentDTO attachment = new AttachmentDTO()
                {
                    AttachmentID = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter),
                    Name = System.IO.Path.GetFileName(i),
                    Path = System.IO.Path.Combine(Data.PROJECT_ATTACHMENTS_FOLDER, string.Format("{0}.atc", string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter++)))
                };

                System.IO.File.Copy(i, attachment.Path);
                attachments.Add(attachment);
            });

            Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments = attachments.ToArray();

            ChangeScreenControls();
        }

        private void btnRemoveAttachment_Click_1(object sender, RoutedEventArgs e)
        {
            IList<AttachmentDTO> attachments = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments.ToList();

            AttachmentDTO removeAttachment = attachments.Single(a => a.AttachmentID.Equals((sender as Button).CommandParameter));
            attachments.Remove(removeAttachment);

            Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments = attachments.ToArray();

            ChangeScreenControls();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
            MessageBox.Show("Project saved locally. \nPlease click on publish to sync with remote data server");
        }

        private void MakeVisible(Grid grid)
        {
            grdViewLogin.Visibility = Visibility.Collapsed;
            gridHomePage.Visibility = Visibility.Collapsed;
            editPage_grid.Visibility = Visibility.Collapsed;

            grid.Visibility = Visibility.Visible;

        }

        private void btnCreate_Click_1(object sender, RoutedEventArgs e)
        {
            if ((cmbTemplates.SelectedItem as ProjectDTO) == null
                || string.IsNullOrEmpty(txtNewProjectName.Text))
            {
                return;
            }

            gdvNew.Visibility = Visibility.Collapsed;

            MakeVisible(editPage_grid);

            string oldProjectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", (cmbTemplates.SelectedItem as ProjectDTO).ProjectID));
            string newProjectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", DateTime.Now.ToString("yyyyMMddHHmmssfff")));

            System.IO.File.Copy(oldProjectPath, newProjectPath);

            Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(newProjectPath, false);

            Data.CURRENT_PROJECT.Description = txtNewProjectName.Text;
            Data.CURRENT_PROJECT.Template = false;
            Data.CURRENT_PROJECT.Closed = false;

            ChangeScreenControls();
        }

        private void btnExport_Click_1(object sender, RoutedEventArgs e)
        {
            IList<string> outputs = Helper.GeneratePdf(Data.CURRENT_PROJECT);

            MessageBox.Show(string.Concat("Files Generated at\n", string.Join("\n", outputs.ToArray())));
        }

        private void btnBack_Click_1(object sender, RoutedEventArgs e)
        {
            MakeVisible(gridHomePage);
        }
    }
}
