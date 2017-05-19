using FacilityDocu.DTO;
using FacilityDocu.UI.Utilities;

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FacilityDocLaptop
{
    public class ImageModel
    {
        public string ImageID { get; set; }
        public System.Windows.Controls.Image Image { get; set; }
        public string Description { get; set; }
    }

    public partial class HomePage : Window
    {
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        private int currentModuleIndex = 0;
        private int currentStepIndex = 0;
        private int currentActionIndex = 0;
        private int currentAnalysisIndex = 0;

        public IList<ImageModel> Images;

        public HomePage()
        {
            InitializeComponent();

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            this.DataContext = this;

            _dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 60);

            PartialInit();
        }

        partial void PartialInit();

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            SaveProject();
            txbAutoSave.Text = string.Format("Autosaved at {0}", DateTime.Now.ToString("HH:mm tt"));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Empty;
            Exception ex = (e.ExceptionObject as Exception);

            if (ex != null)
            {
                errorMessage = ex.Message + "\n\n" + ex.StackTrace;
            }

            Helper.WriteLog(errorMessage, EventLogEntryType.Error);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Empty;
            Exception ex = e.Exception;

            if (ex != null)
            {
                errorMessage = ex.Message + "\n\n" + ex.StackTrace;
            }

            Helper.WriteLog(errorMessage, EventLogEntryType.Error);
        }

        public void CreateListViewGrid()
        {
            IList<string> projectFiles = Directory.GetFiles(Data.PROJECT_XML_FOLDER, "*.xml");

            IList<ProjectDTO> projects = new List<ProjectDTO>();
            projectFiles.ToList().ForEach(f => projects.Add(ProjectXmlReader.ReadProjectXml(f, true)));

            lsvProjects.ItemsSource = projects.Where(p => !p.Template && !p.Closed);
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            string role = "";
            bool isLogin = Helper.Login(userName.Text, password.Password, out role);

            if (isLogin)
            {
                this.Role = role;
                MakeVisible(gridHome);
                Data.CURRENT_USER = userName.Text;
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
            IList<ProjectDTO> projects = GetProjectsFromLocal();

            cmbTemplates.ItemsSource = projects.Where(p => p.Template);

            txtNewProjectName.Text = string.Empty;

            homePage.Title = "RigDocu - New Project";

            gdvNew.Visibility = Visibility.Visible;
        }

        private static IList<ProjectDTO> GetProjectsFromLocal()
        {
            IList<ProjectDTO> projects = new List<ProjectDTO>();
            IList<string> projectFiles = Directory.GetFiles(Data.PROJECT_XML_FOLDER, "*.xml");
            projectFiles.ToList().ForEach(f => projects.Add(ProjectXmlReader.ReadProjectXml(f, true)));
            return projects;
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

            IList<ImageDTO> images = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images.ToList();

            openFileDialogue.FileNames.ToList().ForEach(i =>
                {
                    int counter = 1;
                    ImageDTO image = new ImageDTO()
                    {
                        Used = true,
                        CreationDate = DateTime.Now,
                        Description = "Image Added",
                        ImageID = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter),
                        Path = System.IO.Path.Combine(Data.PROJECT_IMAGES_FOLDER, string.Format("{0}.jpg", string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter++)))
                    };

                    System.IO.File.Copy(i, image.Path, true);
                    images.Add(image);
                });

            Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images = images.ToArray();

            ChangeScreenControls();
        }

        private void projectName_txt_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter();
        }

        private void projectName_txt_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave();
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

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            SaveProject();

            var unknwonImages = Data.CURRENT_PROJECT.RigTypes.SelectMany(r => r.Modules.SelectMany(m => m.Steps.SelectMany(s => s.Actions.SelectMany
                (a => a.Images.Where(i => !File.Exists(i.Path)).Select(i => i.Path)))));

            //if there are few images which are not known then don't go ahead will publish
            if (unknwonImages.Count() > 0)
            {
                MessageBox.Show(string.Format("Cannot publish as below images are missing.\n{0}", string.Join("\n", unknwonImages.ToArray())));
                return;
            }

            string oldProjectID = Data.CURRENT_PROJECT.ProjectID;

            ProjectDTO updatedProject = (new SyncManager()).UpdateDatabase(Data.CURRENT_PROJECT.ProjectID, false);

            if (updatedProject == null)
            {
                MessageBox.Show("Unable to publish. One or more actions has already a new version on server");
            }
            else
            {
                Data.CURRENT_PROJECT = updatedProject;
                ProjectXmlWriter.Write(Data.CURRENT_PROJECT);

                if (oldProjectID != Data.CURRENT_PROJECT.ProjectID)
                {
                    File.Delete(System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", oldProjectID)));
                }

                unknwonImages = Data.CURRENT_PROJECT.RigTypes.SelectMany(r => r.Modules.SelectMany(m => m.Steps.SelectMany(s => s.Actions.SelectMany
                (a => a.Images.Where(i => !File.Exists(i.Path)).Select(i => i.Path)))));

                //if there are few images which are not known then don't go ahead will publish
                if (unknwonImages.Count() > 0)
                {
                    MessageBox.Show(string.Format("Published but below images couldnot be synced.\n{0}", string.Join("\n", unknwonImages.ToArray())));
                }
                else
                {
                    MessageBox.Show("Published Successfully.");
                }
            }

            MakeVisible(gridHome);
        }

        private void homePage_Loaded(object sender, RoutedEventArgs e)
        {
            CreateListViewGrid();
        }

        private void listView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string projectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", (e.AddedItems[0] as ProjectDTO).ProjectID));
                Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(projectPath, false);

                //Fixed as part of bug where its loading data from other project
                Data.CURRENT_RIG = null;

                ChangeScreenControls();

                MakeVisible(gridEdit);

                lsvProjects.SelectedIndex = -1;
            }
        }

        private void ChangeScreenControls()
        {
            if (Data.CURRENT_RIG == null)
            {
                Data.CURRENT_RIG = Data.CURRENT_PROJECT.RigTypes.FirstOrDefault();
            }

            lstChapters.ItemsSource = Data.CURRENT_RIG.Modules.Where(m => m.Steps.Count() > 0); //Only those modules having step count > 0

            lstSteps.ItemsSource = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps;

            txtUser.Text = Data.CURRENT_USER;
            txtProjectID.Text = Data.CURRENT_PROJECT.ProjectID;
            txtProjectDescription.Text = Data.CURRENT_PROJECT.Description;

            txtRigType.Text = string.Format("RIG - {0}", Data.CURRENT_RIG.Name);

            ModuleDTO module = Data.CURRENT_RIG.Modules[currentModuleIndex];
            txtModule.Text = string.Format("{0} {1}", module.Number, module.Name);

            if (currentStepIndex >= 0 && currentStepIndex < module.Steps.Count())
            {

                StepDTO step = module.Steps[currentStepIndex];
                txtStepName.Text = string.Format("{2}.{0} {1}", step.Number, step.Name, module.Number);

                if (step.Actions.Count > 0)
                {
                    ActionDTO action = step.Actions[currentActionIndex];

                    txtAction.Document.Blocks.Clear();
                    txtAction.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new Run(action.Name)));

                    EnableNameWarning();

                    txtActionDetails.Document.Blocks.Clear();
                    txtActionDetails.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new Run(action.Description)));

                    EnableActionDetailsWarning();

                    txtActionNumber.Text = string.Format("{0}/{1}", currentActionIndex + 1, step.Actions.Count());

                    if (!string.IsNullOrEmpty(action.Dimensions))
                    {
                        string[] splits = action.Description.Trim().Split(new string[] { "Wg" }, StringSplitOptions.RemoveEmptyEntries);

                        if (splits.Length == 2)
                        {
                            double value;
                            if (double.TryParse(splits[1], out value)) this.DimensionWeight = value;

                            string[] splits2 = splits[0].Trim().Split(new string[] { "X" }, StringSplitOptions.RemoveEmptyEntries);

                            if (splits2.Length == 3)
                            {
                                if (double.TryParse(splits2[0], out value)) this.DimensionLength = value;
                                if (double.TryParse(splits2[1], out value)) this.DimensionWidth = value;
                                if (double.TryParse(splits2[2], out value)) this.DimensionHeight = value;
                            }
                        }
                    }

                    SelectedActionItems = new ObservableCollection<string>(action.Tools.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries));
                    SelectedLiftingGears = new ObservableCollection<string>(action.LiftingGears.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries));
                    SelectedRisks = new ObservableCollection<string>(action.Risks.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries));

                    SelectedPeople.Clear();
                    action.People.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(p =>
                    {
                        string[] vals = p.Split(Data.SUBSEPERATOR);
                        SelectedPeople.Add(new ResourceDTO() { Name = vals[0], ResourceCount = vals[1] });
                    });

                    SelectedMachines.Clear();
                    action.Machines.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(p =>
                    {
                        string[] vals = p.Split(Data.SUBSEPERATOR);
                        SelectedMachines.Add(new ResourceDTO() { Name = vals[0], ResourceCount = vals[1] });
                    });

                    ShowImages(action.Images);

                    lstAttachments.ItemsSource = action.Attachments;

                    chkRiskAnalysis.IsChecked = action.IsAnalysis;

                    if (IsAnalysisIndexCorrect())
                    {
                        RiskAnalysisDTO analysis = action.RiskAnalysis[currentAnalysisIndex];
                        txtAnalysisActivity.Text = analysis.Activity;
                        cmbAnalysisL.Text = analysis.L.ToString();
                        cmbAnalysisS.Text = analysis.S.ToString();
                        txtAnalysisControl.Text = analysis.Controls;
                        txtAnalysisDanger.Text = analysis.Activity;
                        ComboBox_SelectionChanged(null, null); //Calculate Risk
                        txtAnalysisResponsible.Text = analysis.Responsible.ToString();

                        txtActivityNumbers.Text = string.Format("{0}/{1}", currentAnalysisIndex + 1, action.RiskAnalysis.Count());
                    }
                    else
                    {
                        txtActivityNumbers.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    if (Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].RiskAnalysis.Count() == 0)
                    {
                        txtAnalysisActivity.Text = string.Empty;
                        cmbAnalysisS.Text = string.Empty;
                        txtAnalysisControl.Text = string.Empty;
                        txtAnalysisDanger.Text = string.Empty;
                        cmbAnalysisL.Text = string.Empty;
                        txtAnalysisRisk.Text = string.Empty;
                        txtAnalysisResponsible.Text = string.Empty;
                        txtActivityNumbers.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }

            if (Data.CURRENT_PROJECT.RigTypes.SingleOrDefault(r => r.RigTypeID.Equals("1")) != null)
            {
                mniUp.IsEnabled = true;
                if (Data.CURRENT_RIG.RigTypeID.Equals("1"))
                {
                    mniUp.IsChecked = true;
                    mniDown.IsChecked = false;
                    mniMove.IsChecked = false;
                }
            }
            else
            {
                mniUp.IsEnabled = false;

            }

            if (Data.CURRENT_PROJECT.RigTypes.SingleOrDefault(r => r.RigTypeID.Equals("2")) != null)
            {
                mniDown.IsEnabled = true;
                if (Data.CURRENT_RIG.RigTypeID.Equals("2"))
                {
                    mniUp.IsChecked = false;
                    mniDown.IsChecked = true;
                    mniMove.IsChecked = false;
                }
            }
            else
            {
                mniDown.IsEnabled = false;

            }

            if (Data.CURRENT_PROJECT.RigTypes.SingleOrDefault(r => r.RigTypeID.Equals("3")) != null)
            {
                mniMove.IsEnabled = true;
                if (Data.CURRENT_RIG.RigTypeID.Equals("3"))
                {
                    mniUp.IsChecked = false;
                    mniDown.IsChecked = false;
                    mniMove.IsChecked = true;
                }
            }
            else
            {
                mniMove.IsEnabled = false;
            }
        }

        private void ShowImages(IList<ImageDTO> images)
        {

            IList<string> imagesNotFound = new List<string>();
            Images = new List<ImageModel>();
            foreach (ImageDTO image in images.Where(i => i.Used == true))
            {
                BitmapImage bitmap = new BitmapImage();



                if (!File.Exists(image.Path))
                {
                    imagesNotFound.Add(image.Path);
                    continue;
                }
                try
                {
                    System.Windows.Controls.Image imagename = new System.Windows.Controls.Image();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(image.Path);
                    imagename.Source = bitmap;
                    bitmap.EndInit();

                    Images.Add(new ImageModel() { ImageID = image.ImageID, Image = imagename, Description = image.Description });
                }
                catch (Exception ex)
                {

                }
            }
            lstPictures.ItemsSource = Images;

            if (imagesNotFound.Count > 0)
            {
                MessageBox.Show(string.Format("Below Images are not present at specified path. This can be a serious issue. {0}", string.Join("\n", imagesNotFound.ToArray())));
            }
        }

        private void btnActionRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentActionIndex < (Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions.Count() - 1))
            {
                SaveActionDetail();
                currentActionIndex++;
                currentAnalysisIndex = 0;
                ChangeScreenControls();
            }
        }

        private void SaveActionDetail()
        {
            if (Data.CURRENT_RIG != null)
            {
                if (currentModuleIndex < 0 || currentStepIndex < 0 || currentActionIndex < 0)
                {

                }

                ActionDTO action = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex];

                action.Dimensions = this.Dimension;
                action.Tools = string.Join(Data.SEPERATOR.ToString(), SelectedActionItems);
                action.LiftingGears = string.Join(Data.SEPERATOR.ToString(), SelectedLiftingGears);
                action.Risks = string.Join(Data.SEPERATOR.ToString(), SelectedRisks);

                action.People = string.Join(Data.SEPERATOR.ToString(), SelectedPeople.Select(p => $"{p.Name}{Data.SUBSEPERATOR}{p.ResourceCount}"));
                action.Machines = string.Join(Data.SEPERATOR.ToString(), SelectedMachines.Select(p => $"{p.Name}{Data.SUBSEPERATOR}{p.ResourceCount}"));

                action.Name = new TextRange(txtAction.Document.ContentStart, txtAction.Document.ContentEnd).Text;
                action.Description = new TextRange(txtActionDetails.Document.ContentStart, txtActionDetails.Document.ContentEnd).Text;
                
                action.IsAnalysis = chkRiskAnalysis.IsChecked.Value;

                SaveAnalysisDetail();

                action.LastUpdatedBy = new UserDTO() { UserName = Data.CURRENT_USER };
                action.LastUpdatedAt = DateTime.Now.ToUniversalTime();
            }
        }

        private void SaveAnalysisDetail()
        {
            if (IsAnalysisIndexCorrect())
            {
                RiskAnalysisDTO analysis = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].
                    RiskAnalysis[currentAnalysisIndex];

                analysis.Activity = txtAnalysisActivity.Text;
                analysis.Danger = txtAnalysisDanger.Text;
                analysis.L = cmbAnalysisL.Text;
                analysis.S = string.IsNullOrEmpty(cmbAnalysisS.Text) ? 0 : Convert.ToInt16(cmbAnalysisS.Text);
                analysis.Controls = txtAnalysisControl.Text;
                analysis.Responsible = txtAnalysisResponsible.Text;
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
            IList<ResourceDTO> masterResources = new List<ResourceDTO>();

            //Data.CURRENT_RIG.Modules.SelectMany(m => m.Steps).SelectMany(s => s.Actions).First().Resources.ToList().ForEach(r =>

            //    masterResources.Add(new ResourceDTO() { ResourceID = r.ResourceID, Type = r.Type, Name = r.Name, ResourceCount = r.ResourceCount })
            //);

            ActionDTO action = new ActionDTO()
            {
                Name = "New Action",
                Description = "New Action's Description",
                Number = (Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions.Count + 1).ToString("00"),
                //Resources = masterResources.ToArray(),
                RiskAnalysis = (new List<RiskAnalysisDTO>() { new RiskAnalysisDTO() }).ToArray(),
                //Tools = (new List<ToolDTO>()).ToArray(),
                Images = (new List<ImageDTO>()).ToArray(),
                ActionID = "0"
            };


            IList<ActionDTO> actions = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions.ToList();
            //actions.Add(action);

            actions.Insert(currentActionIndex + 1, action);

            Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions = actions.ToArray();

            currentActionIndex = currentActionIndex + 1;
            //currentActionIndex = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions.Count() - 1;

            currentAnalysisIndex = 0;
            ChangeScreenControls();
        }

        private void btAnalysisLeft_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentAnalysisIndex > 0)
            {
                SaveAnalysisDetail();
                currentAnalysisIndex--;
                ChangeScreenControls();
            }
        }

        private void btAnalysisRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentAnalysisIndex < (Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].RiskAnalysis.Count() - 1))
            {
                SaveAnalysisDetail();
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

            IList<AttachmentDTO> attachments = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments.ToList();

            openFileDialogue.FileNames.ToList().ForEach(i =>
            {
                int counter = 1;
                AttachmentDTO attachment = new AttachmentDTO()
                {
                    AttachmentID = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter),
                    Name = System.IO.Path.GetFileName(i),
                    Path = System.IO.Path.Combine(Data.PROJECT_ATTACHMENTS_FOLDER, string.Format("{0}.pdf", string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter++)))
                };

                System.IO.File.Copy(i, attachment.Path);
                attachments.Add(attachment);
            });

            Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments = attachments.ToArray();

            ChangeScreenControls();
        }

        private void btnRemoveAttachment_Click_1(object sender, RoutedEventArgs e)
        {
            IList<AttachmentDTO> attachments = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments.ToList();

            AttachmentDTO removeAttachment = attachments.Single(a => a.AttachmentID.Equals((sender as Button).CommandParameter));
            attachments.Remove(removeAttachment);

            Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments = attachments.ToArray();

            ChangeScreenControls();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveProject();
            MessageBox.Show("Project saved locally. \nPlease click on publish to sync with remote data server");
        }

        private void SaveProject()
        {
            SaveActionDetail();

            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
        }

        private void MakeVisible(Grid grid)
        {
            gridLogin.Visibility = Visibility.Collapsed;
            gridHome.Visibility = Visibility.Collapsed;
            gridEdit.Visibility = Visibility.Collapsed;

            grid.Visibility = Visibility.Visible;

            if (grid.Name.Equals("gridEdit"))
            {
                //Also start timer, if not already started
                if (!this._dispatcherTimer.IsEnabled)
                {
                    this._dispatcherTimer.Start();
                }
                dpnMenu.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                //Also stop timer, if started
                if (this._dispatcherTimer.IsEnabled)
                {
                    this._dispatcherTimer.Stop();
                    txbAutoSave.Text = string.Empty;
                }
                dpnMenu.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (grid.Name.Equals("gridHome"))
            {
                if (Helper.isInternetAvailable())
                {
                    imgSync.Source = new BitmapImage(new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "sync.jpg")));
                    imgTemplate.Source = new BitmapImage(new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "template.png")));

                    btnSync.IsEnabled = true;
                    btnTemplate.IsEnabled = true;
                }
                else
                {
                    imgSync.Source = new BitmapImage(new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "syncdisable.png")));
                    imgTemplate.Source = new BitmapImage(new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "templatedisable.png")));

                    btnSync.IsEnabled = false;
                    btnTemplate.IsEnabled = false;
                }
            }
        }

        private void btnCreate_Click_1(object sender, RoutedEventArgs e)
        {
            if ((cmbTemplates.SelectedItem as ProjectDTO) == null
                || string.IsNullOrEmpty(txtNewProjectName.Text))
            {
                return;
            }

            gdvNew.Visibility = Visibility.Collapsed;

            MakeVisible(gridEdit);

            string newProjectID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string oldProjectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", (cmbTemplates.SelectedItem as ProjectDTO).ProjectID));
            string newProjectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", newProjectID));

            System.IO.File.Copy(oldProjectPath, newProjectPath);

            Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(newProjectPath, false);

            Data.CURRENT_PROJECT.ProjectID = newProjectID;
            Data.CURRENT_PROJECT.Description = txtNewProjectName.Text;
            Data.CURRENT_PROJECT.Client = txtNewProjectClient.Text;
            Data.CURRENT_PROJECT.Location = txtNewProjectLocation.Text;
            Data.CURRENT_PROJECT.Persons = txtNewProjectPersons.Text;
            Data.CURRENT_PROJECT.ProjectNumber = txtNewProjectProjectNo.Text;

            Data.CURRENT_PROJECT.Template = false;
            Data.CURRENT_PROJECT.Closed = false;
            Data.CURRENT_PROJECT.CreatedBy = new UserDTO() { UserName = Data.CURRENT_USER };
            Data.CURRENT_PROJECT.CreationDate = DateTime.Now.ToUniversalTime();

            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);

            ChangeScreenControls();

            CreateListViewGrid();
        }

        private void btnExport_Click_1(object sender, RoutedEventArgs e)
        {
            SaveProject();

            ExportOptions exportOptions = new ExportOptions();
            exportOptions.Show();
        }

        private void btnBack_Click_1(object sender, RoutedEventArgs e)
        {
            SaveProject();
            MakeVisible(gridHome);
        }

        private void btnRemoveImage_Click_1(object sender, RoutedEventArgs e)
        {
            SaveActionDetail();
            string imageToRemoveID = (sender as Button).CommandParameter.ToString();

            ImageDTO removeImage = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].
                Actions[currentActionIndex].Images.First(i => i.ImageID.Equals(imageToRemoveID));

            removeImage.Used = false;

            ChangeScreenControls();
        }

        private void btnAddImages_Click_1(object sender, RoutedEventArgs e)
        {
            SaveActionDetail();

            IList<ImageModel> addImages = new List<ImageModel>();

            Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images.Where(i => i.Used == false)
                .ToList().ForEach(i =>
            {
                BitmapImage bitmap = new BitmapImage();
                System.Windows.Controls.Image imagename = new System.Windows.Controls.Image();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(i.Path);
                imagename.Source = bitmap;
                bitmap.EndInit();

                addImages.Add(new ImageModel() { Description = i.Description, ImageID = i.ImageID, Image = imagename });

            });
            lstAddImages.ItemsSource = addImages;
            popImage.IsOpen = true;
        }

        private void btnImageA_Click_1(object sender, RoutedEventArgs e)
        {
            popImage.IsOpen = false;

            foreach (ImageModel selected in lstAddImages.SelectedItems)
            {
                ImageDTO modified = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images.Single(i => i.ImageID.Equals(selected.ImageID));
                modified.Used = true;
            }
            ChangeScreenControls();
        }

        private void btnImageC_Click_1(object sender, RoutedEventArgs e)
        {
            popImage.IsOpen = false;
        }

        private void btnActionDelete_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to really want to remove action?",
    "Action Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            if (currentActionIndex >= 0)
            {
                IList<ActionDTO> actions = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions.ToList();

                if (actions.Count > 1)
                {
                    actions.Remove(Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex]);

                    Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions = actions.ToArray();

                    if (currentActionIndex > 0)
                    {
                        currentActionIndex--;
                    }

                    currentAnalysisIndex = 0;
                    ChangeScreenControls();
                }
                else
                {
                    MessageBox.Show("Cannot Delete. There should be atleast one action in a step");
                }
            }
        }

        private void btnAnalysisAdd_Click_1(object sender, RoutedEventArgs e)
        {
            SaveActionDetail();

            RiskAnalysisDTO RiskAnalysis = new RiskAnalysisDTO()
            {
                RiskAnalysisID = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Activity = "New RiskAnalysis Activity",
                Controls = "New RiskAnalysis's Controls",
                Danger = "New RiskAnalysis's Danger"
            };

            IList<RiskAnalysisDTO> RiskAnalysiss = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
                .Actions[currentActionIndex].RiskAnalysis.ToList();
            RiskAnalysiss.Add(RiskAnalysis);

            Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].RiskAnalysis = RiskAnalysiss.ToArray();

            currentAnalysisIndex = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
                .Actions[currentActionIndex].RiskAnalysis.Count() - 1;
            ChangeScreenControls();
        }

        private void btnAnalysisDelete_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to really want to remove risk analysis activity?",
                "Risk Analysis Activity Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            SaveActionDetail();

            if (IsAnalysisIndexCorrect())
            {
                IList<RiskAnalysisDTO> RiskAnalysiss = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
                    .Actions[currentActionIndex].RiskAnalysis.ToList();

                RiskAnalysiss.Remove(Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
                    .Actions[currentActionIndex].RiskAnalysis[currentAnalysisIndex]);

                Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
                    .Actions[currentActionIndex].RiskAnalysis = RiskAnalysiss.ToArray();

                if (currentAnalysisIndex > 0)
                {
                    currentAnalysisIndex--;
                }

                ChangeScreenControls();
            }
        }

        private void btnWarningAction_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            EnableNameWarning(true);
        }

        private void EnableNameWarning(bool isToggle = false)
        {
            ActionDTO action = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
        .Actions[currentActionIndex];

            Brush color = new SolidColorBrush(Colors.Yellow);

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();

            if (isToggle)
            {
                if (action.IsNameWarning)
                {
                    action.IsNameWarning = false;
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningdisable.png"));
                    color = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    action.IsNameWarning = true;
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningenable.png"));
                    color = new SolidColorBrush(Colors.Yellow);
                }
            }
            else
            {
                if (!action.IsNameWarning)
                {
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningdisable.png"));
                    color = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningenable.png"));
                    color = new SolidColorBrush(Colors.Yellow);
                }
            }
            logo.EndInit();


            txtAction.BorderBrush = color;
            btnWarningAction.Source = logo;
        }

        private void mniImportnat_Click_1(object sender, RoutedEventArgs e)
        {
            txtAction.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, Brushes.Red);
        }

        private void mniDetailImportnat_Click_1(object sender, RoutedEventArgs e)
        {
            txtActionDetails.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, Brushes.Red);
        }

        private void btnWarningActionDescription_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            EnableActionDetailsWarning(true);
        }

        private void EnableActionDetailsWarning(bool isToggle = false)
        {
            ActionDTO action = Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
        .Actions[currentActionIndex];

            Brush color = new SolidColorBrush(Colors.Yellow);

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();

            if (isToggle)
            {
                if (action.IsDescriptionwarning)
                {
                    action.IsDescriptionwarning = false;
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningdisable.png"));
                    color = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    action.IsDescriptionwarning = true;
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningenable.png"));
                    color = new SolidColorBrush(Colors.Yellow);
                }
            }
            else
            {
                if (!action.IsDescriptionwarning)
                {
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningdisable.png"));
                    color = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    logo.UriSource = new Uri(System.IO.Path.Combine(Data.ASSETS_PATH, "warningenable.png"));
                    color = new SolidColorBrush(Colors.Yellow);
                }
            }
            logo.EndInit();


            txtActionDetails.BorderBrush = color;
            btnWarningActionDescription.Source = logo;
        }

        private void btnSync_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The sync process is pulling out latest projects/templates from server to local machine.\nDo you want to sync?", "RigDocu", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {

                (new SyncManager()).Sync();

                MessageBox.Show("Data Sync Done!!!");
                CreateListViewGrid();
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveActionDetail();

            string selectedRigTypeID = Convert.ToString((sender as MenuItem).CommandParameter);

            RigTypeDTO selectedRig = Data.CURRENT_PROJECT.RigTypes.SingleOrDefault(r => r.RigTypeID.Equals(selectedRigTypeID));

            if (selectedRig != null)
            {
                Data.CURRENT_RIG = selectedRig;
                currentModuleIndex = 0;
                currentStepIndex = 0;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void mniLogout_Click_1(object sender, RoutedEventArgs e)
        {
            SaveProject();
            MakeVisible(gridLogin);
        }

        private void mniExit_Click_1(object sender, RoutedEventArgs e)
        {
            SaveProject();

            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to really want to exit?", "Exit Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void txtModule_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            popChapters.IsOpen = true;
        }

        private void lstChapters_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SaveActionDetail();

                currentModuleIndex = (sender as ListBox).SelectedIndex;
                currentStepIndex = 0;
                currentActionIndex = 0;
                currentAnalysisIndex = 0;

                popChapters.IsOpen = false;

                ChangeScreenControls();
            }
        }

        private void lstSteps_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SaveActionDetail();

                currentStepIndex = (sender as ListBox).SelectedIndex;
                currentActionIndex = 0;
                currentAnalysisIndex = 0;

                popSteps.IsOpen = false;

                ChangeScreenControls();
            }
        }

        private void txtStepName_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            popSteps.IsOpen = true;
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            string attachmentID = (sender as TextBlock).Tag.ToString();
            System.Diagnostics.Process.Start(System.IO.Path.Combine(Data.PROJECT_ATTACHMENTS_FOLDER, string.Format("{0}.pdf", attachmentID)));
        }

        private bool IsAnalysisIndexCorrect()
        {
            if (Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].RiskAnalysis.Count() > 0
                && currentAnalysisIndex >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void chkRiskAnalysis_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnTemplate_Click(object sender, RoutedEventArgs e)
        {
            popNewTemplate.IsOpen = true;
            cmbNewTemplates.ItemsSource = GetProjectsFromLocal().Where(p => p.Template);
        }

        private void btnNewTemplateOK_Click(object sender, RoutedEventArgs e)
        {
            popNewTemplate.IsOpen = false;
            CustomTemplate customTemplate = null;

            if (rdbCopyTemplate.IsChecked.Value)
            {
                int templateID = Convert.ToInt32((cmbNewTemplates.SelectedItem as ProjectDTO).ProjectID);
                customTemplate = new CustomTemplate(templateID);
            }
            else
            {
                customTemplate = new CustomTemplate();
            }

            customTemplate.Show();

        }

        private void btnNewTemplateCancel_Click(object sender, RoutedEventArgs e)
        {
            popNewTemplate.IsOpen = false;
        }

        private void btnImportXml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML Files (*.xml)|*.xml";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.Title = "Please select an image file to encrypt.";
            if (dialog.ShowDialog() == true)
            {

            }
        }

        private void chkRiskAnalysis_Click(object sender, RoutedEventArgs e)
        {
            Data.CURRENT_RIG.Modules[currentModuleIndex].Steps[currentStepIndex]
                   .Actions[currentActionIndex].IsAnalysis = (sender as CheckBox).IsChecked.Value;
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin();
            admin.ShowDialog();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dictionary<string, int> mapping = new Dictionary<string, int>()
            { { "A", 1}, { "B", 2},{ "C", 3},{ "D", 4},{ "E", 5}};

            if (!string.IsNullOrEmpty(cmbAnalysisL.Text) && !string.IsNullOrEmpty(cmbAnalysisS.Text))
            {
                string lString = cmbAnalysisL.Text.ToString();
                int l = mapping[lString];
                int s = Convert.ToInt32(cmbAnalysisS.Text.ToString());

                int result = l * s;

                if (result < 5)
                {
                    txtAnalysisRisk.Text = $"Low {lString}{s}";
                    txtAnalysisRisk.Background = Brushes.Green;
                }
                else if (result >= 10)
                {
                    txtAnalysisRisk.Text = $"High {lString}{s}";
                    txtAnalysisRisk.Background = Brushes.Red;
                }
                else
                {
                    txtAnalysisRisk.Text = $"MED {lString}{s}";
                    txtAnalysisRisk.Background = Brushes.Yellow;
                }
            }
            else
            {
                txtAnalysisRisk.Text = "";
                txtAnalysisRisk.Background = Brushes.White;
            }
        }
    }
}
