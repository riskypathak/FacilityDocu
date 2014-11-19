﻿using FacilityDocu.UI.Utilities;
using FacilityDocu.UI.Utilities.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private int currentRigIndex = 0;
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

            listView.ItemsSource = projects.Where(p => !p.Template && !p.Closed);
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            bool isLogin = Helper.Login(userName.Text, password.Password);

            if (isLogin)
            {
                MakeVisible(gridHomePage);
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
            IList<ProjectDTO> projects = new List<ProjectDTO>();
            IList<string> projectFiles = Directory.GetFiles(Data.PROJECT_XML_FOLDER, "*.xml");
            projectFiles.ToList().ForEach(f => projects.Add(ProjectXmlReader.ReadProjectXml(f, true)));

            cmbTemplates.ItemsSource = projects.Where(p => p.Template);

            txtNewProjectName.Text = string.Empty;

            homePage.Title = "FacilityDocu - New Project";

            gdvNew.Visibility = Visibility.Visible;
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

            string oldProjectID = Data.CURRENT_PROJECT.ProjectID;
            Data.CURRENT_PROJECT = (new SyncManager()).UpdateDatabase(Data.CURRENT_PROJECT.ProjectID, false);
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);

            if (oldProjectID != Data.CURRENT_PROJECT.ProjectID)
            {
                File.Delete(System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", oldProjectID)));
            }

            MessageBox.Show("Published");

            MakeVisible(gridHomePage);
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

                ChangeScreenControls();

                MakeVisible(editPage_grid);

                listView.SelectedIndex = -1;
            }
        }

        private void ChangeScreenControls()
        {
            lstChapters.ItemsSource = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules;
            lstSteps.ItemsSource = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps;

            txtUser.Text = Data.CURRENT_USER;
            txtProjectID.Text = Data.CURRENT_PROJECT.ProjectID;
            txtProjectDescription.Text = Data.CURRENT_PROJECT.Description;

            txtRigType.Text = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Name;

            ModuleDTO module = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex];
            txtModule.Text = string.Format("{0} {1}", module.Number, module.Name);

            StepDTO step = module.Steps[currentStepIndex];
            txtStepName.Text = string.Format("{0} {1}", step.Number, step.Name);

            if (step.Actions.Length > 0)
            {
                ActionDTO action = step.Actions[currentActionIndex];

                txtAction.Document.Blocks.Clear();
                txtAction.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new Run(action.Name)));

                EnableNameWarning();

                txtActionDetails.Document.Blocks.Clear();
                txtActionDetails.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new Run(action.Description)));

                EnableActionDetailsWarning();

                txtActionNumber.Text = action.Number.Trim();
                txtActionDimensions.Text = action.Dimensions.Trim();
                txtActionLiftingGears.Text = action.LiftingGears.Trim();
                txtActionRisks.Text = action.Risks.Trim();

                lstActionTools.ItemsSource = action.Tools;

                lstActionResourcesP.ItemsSource = action.Resources.Where(r => r.Type.Equals("people")).ToList();
                lstActionResourcesM.ItemsSource = action.Resources.Where(r => r.Type.Equals("machine")).ToList();

                ShowImages(action.Images);

                lstAttachments.ItemsSource = action.Attachments;

                if (IsAnalysisIndexCorrect())
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

                    txtActivityNumbers.Text = string.Format("RiskyAnalysis#{0} of {1}", currentAnalysisIndex + 1, action.RiskAnalysis.Count());
                }
                else
                {
                    txtActionNumber.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            if (currentRigIndex == 0)
            {
                mniUp.IsChecked = true;
                mniDown.IsChecked = false;
                mniMove.IsChecked = false;
            }
            else if (currentRigIndex == 1)
            {
                mniUp.IsChecked = false;
                mniDown.IsChecked = true;
                mniMove.IsChecked = false;
            }
            else if (currentRigIndex == 2)
            {
                mniUp.IsChecked = false;
                mniDown.IsChecked = false;
                mniMove.IsChecked = true;
            }
        }

        private void ShowImages(IList<ImageDTO> images)
        {
            Images = new List<ImageModel>();
            foreach (ImageDTO image in images.Where(i => i.Used == true))
            {
                BitmapImage bitmap = new BitmapImage();
                System.Windows.Controls.Image imagename = new System.Windows.Controls.Image();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(image.Path);
                imagename.Source = bitmap;
                //imagename.Width = 150;
                //imagename.Height = 150;
                bitmap.EndInit();

                Images.Add(new ImageModel() { ImageID = image.ImageID, Image = imagename, Description = image.Description });
            }
            lstPictures.ItemsSource = Images;
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
                SaveActionDetail();

                currentStepIndex--;
                currentActionIndex = 0;

                ChangeScreenControls();
            }
        }

        private void btnActionRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentActionIndex < (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions.Count() - 1))
            {
                SaveActionDetail();
                currentActionIndex++;
                currentAnalysisIndex = 0;
                ChangeScreenControls();
            }
        }

        private void SaveActionDetail()
        {
            ActionDTO action = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex];

            action.Dimensions = txtActionDimensions.Text;
            action.LiftingGears = txtActionLiftingGears.Text;
            action.Name = new TextRange(txtAction.Document.ContentStart, txtAction.Document.ContentEnd).Text;
            action.Description = new TextRange(txtActionDetails.Document.ContentStart, txtActionDetails.Document.ContentEnd).Text;
            action.Risks = txtActionRisks.Text;

            SaveAnalysisDetail();
        }

        private void SaveAnalysisDetail()
        {
            if (IsAnalysisIndexCorrect())
            {
                RiskAnalysisDTO analysis = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].
                    RiskAnalysis[currentAnalysisIndex];

                analysis.Activity = txtAnalysisActivity.Text;
                txtAnalysisActivity.Text = analysis.Activity;
                analysis.B = Convert.ToDouble(txtAnalysisB.Text);
                analysis.B = Convert.ToDouble(txtAnalysisB_.Text);
                analysis.Controls = txtAnalysisControl.Text;
                analysis.Activity = txtAnalysisDanger.Text;
                analysis.E = Convert.ToDouble(txtAnalysisE.Text);
                analysis.E_ = Convert.ToDouble(txtAnalysisE_.Text);
                analysis.K = Convert.ToDouble(txtAnalysisK.Text);
                analysis.K_ = Convert.ToDouble(txtAnalysisK_.Text);
                analysis.Risk = Convert.ToDouble(txtAnalysisRisk.Text);
                analysis.Risk_ = Convert.ToDouble(txtAnalysisRisk_.Text);
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
            if (currentAnalysisIndex < (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentAnalysisIndex].RiskAnalysis.Count() - 1))
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

            IList<AttachmentDTO> attachments = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Attachments.ToList();

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
            SaveProject();
            MessageBox.Show("Project saved locally. \nPlease click on publish to sync with remote data server");
        }

        private void SaveProject()
        {
            SaveActionDetail();

            Data.CURRENT_PROJECT.LastUpdatedBy = new UserDTO() { UserName = Data.CURRENT_USER };
            Data.CURRENT_PROJECT.LastUpdatedAt = DateTime.Now.ToUniversalTime();

            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
        }

        private void MakeVisible(Grid grid)
        {
            grdViewLogin.Visibility = Visibility.Collapsed;
            gridHomePage.Visibility = Visibility.Collapsed;
            editPage_grid.Visibility = Visibility.Collapsed;

            grid.Visibility = Visibility.Visible;

            if (grid.Name.Equals("editPage_grid"))
            {
                dpnMenu.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                dpnMenu.Visibility = System.Windows.Visibility.Collapsed;
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

            MakeVisible(editPage_grid);

            string newProjectID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string oldProjectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", (cmbTemplates.SelectedItem as ProjectDTO).ProjectID));
            string newProjectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", newProjectID));

            System.IO.File.Copy(oldProjectPath, newProjectPath);

            Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(newProjectPath, false);

            Data.CURRENT_PROJECT.ProjectID = newProjectID;
            Data.CURRENT_PROJECT.Description = txtNewProjectName.Text;
            Data.CURRENT_PROJECT.Template = false;
            Data.CURRENT_PROJECT.Closed = false;
            Data.CURRENT_PROJECT.CreatedBy = new UserDTO() { UserName = Data.CURRENT_USER };
            Data.CURRENT_PROJECT.LastUpdatedBy = new UserDTO() { UserName = Data.CURRENT_USER };
            Data.CURRENT_PROJECT.LastUpdatedAt = DateTime.Now.ToUniversalTime();
            Data.CURRENT_PROJECT.CreationDate = DateTime.Now.ToUniversalTime();

            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);

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

        private void btnToolAdd_Click_1(object sender, RoutedEventArgs e)
        {
            Helper.GetTools();

            var notToolIds = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Tools.Select(n => n.ToolID);

            lstAddTools.ItemsSource = Data.AVAILABLE_TOOLS.Where(t => !notToolIds.Contains(t.ToolID));

            popTool.IsOpen = true;
        }

        private void btnToolA_Click_1(object sender, RoutedEventArgs e)
        {
            popTool.IsOpen = false;

            foreach (ToolDTO selected in lstAddTools.SelectedItems)
            {
                IList<ToolDTO> tools = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Tools.ToList();
                tools.Add(selected);


                Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Tools = tools.ToArray();
            }
            ChangeScreenControls();
        }

        private void btnToolC_Click_1(object sender, RoutedEventArgs e)
        {
            popTool.IsOpen = false;
        }

        private void btnRemove_Click_1(object sender, RoutedEventArgs e)
        {
            string toolToRemoveID = (sender as Button).CommandParameter.ToString();

            IList<ToolDTO> tools = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Tools.ToList();

            ToolDTO toolToRemove = tools.Single(t => t.ToolID.Equals(toolToRemoveID));
            tools.Remove(toolToRemove);


            Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Tools = tools.ToArray();

            ChangeScreenControls();
        }

        private void btnRemoveImage_Click_1(object sender, RoutedEventArgs e)
        {
            string imageToRemoveID = (sender as Button).CommandParameter.ToString();

            ImageDTO removeImage = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].
                Actions[currentActionIndex].Images.Single(i => i.ImageID.Equals(imageToRemoveID));

            removeImage.Used = false;

            ChangeScreenControls();
        }

        private void btnAddImages_Click_1(object sender, RoutedEventArgs e)
        {
            IList<ImageModel> addImages = new List<ImageModel>();

            Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images.Where(i => i.Used == false)
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
                ImageDTO modified = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].Images.Single(i => i.ImageID.Equals(selected.ImageID));
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
            if (currentActionIndex >= 0)
            {
                IList<ActionDTO> actions = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions.ToList();

                if (actions.Count > 1)
                {
                    actions.Remove(Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex]);

                    Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions = actions.ToArray();

                    currentActionIndex--;
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
            RiskAnalysisDTO RiskAnalysis = new RiskAnalysisDTO()
            {
                RiskAnalysisID = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Activity = "New RiskAnalysis Activity",
                Controls = "New RiskAnalysis's Controls",
                Danger = "New RiskAnalysis's Danger"
            };

            IList<RiskAnalysisDTO> RiskAnalysiss = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex]
                .Actions[currentActionIndex].RiskAnalysis.ToList();
            RiskAnalysiss.Add(RiskAnalysis);

            Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].RiskAnalysis = RiskAnalysiss.ToArray();

            currentAnalysisIndex = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex]
                .Actions[currentActionIndex].RiskAnalysis.Count() - 1;
            ChangeScreenControls();
        }

        private void btnAnalysisDelete_Click_1(object sender, RoutedEventArgs e)
        {
            if (IsAnalysisIndexCorrect())
            {
                IList<RiskAnalysisDTO> RiskAnalysiss = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex]
                    .Actions[currentActionIndex].RiskAnalysis.ToList();

                RiskAnalysiss.Remove(Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex]
                    .Actions[currentActionIndex].RiskAnalysis[currentAnalysisIndex]);

                Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex]
                    .Actions[currentActionIndex].RiskAnalysis = RiskAnalysiss.ToArray();

                currentAnalysisIndex--;
                ChangeScreenControls();
            }
        }

        private void btnWarningAction_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            EnableNameWarning(true);
        }

        private void EnableNameWarning(bool isToggle = false)
        {
            ActionDTO action = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex]
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
                    color = new SolidColorBrush(Colors.Black);
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
                    color = new SolidColorBrush(Colors.Black);
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
            ActionDTO action = Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex]
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
                    color = new SolidColorBrush(Colors.Black);
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
                    color = new SolidColorBrush(Colors.Black);
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
            (new SyncManager()).Sync();

            MessageBox.Show("Data Sync Done!!!");
            CreateListViewGrid();

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            int rigIndex = Convert.ToInt32((sender as MenuItem).CommandParameter);




            currentRigIndex = rigIndex;

            currentModuleIndex = 0;
            currentStepIndex = 0;
            currentActionIndex = 0;

            ChangeScreenControls();
        }

        private void mniLogout_Click_1(object sender, RoutedEventArgs e)
        {
            MakeVisible(grdViewLogin);
        }

        private void mniExit_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtModule_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            lstChapters.Visibility = System.Windows.Visibility.Visible;

            Mouse.Capture(this, CaptureMode.SubTree);
            AddHandler();
        }

        private void AddHandler()
        {
            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            lstChapters.Visibility = System.Windows.Visibility.Collapsed;
            lstSteps.Visibility = System.Windows.Visibility.Collapsed;
            ReleaseMouseCapture();
        }

        private void lstChapters_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            currentModuleIndex = (sender as ListBox).SelectedIndex;
            currentStepIndex = 0;
            currentActionIndex = 0;
            currentAnalysisIndex = 0;

            lstChapters.Visibility = System.Windows.Visibility.Collapsed;
            ReleaseMouseCapture();

            ChangeScreenControls();
        }

        private void lstSteps_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            currentStepIndex = (sender as ListBox).SelectedIndex;
            currentActionIndex = 0;
            currentAnalysisIndex = 0;

            lstSteps.Visibility = System.Windows.Visibility.Collapsed;
            ReleaseMouseCapture();

            ChangeScreenControls();
        }

        private void txtStepName_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            lstSteps.Visibility = System.Windows.Visibility.Visible;

            Mouse.Capture(this, CaptureMode.SubTree);
            AddHandler();
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            string attachmentID = (sender as TextBlock).Tag.ToString();
            System.Diagnostics.Process.Start(System.IO.Path.Combine(Data.PROJECT_ATTACHMENTS_FOLDER, string.Format("{0}.pdf", attachmentID)));
        }

        private bool IsAnalysisIndexCorrect()
        {
            if (Data.CURRENT_PROJECT.RigTypes[currentRigIndex].Modules[currentModuleIndex].Steps[currentStepIndex].Actions[currentActionIndex].RiskAnalysis.Count() > 0
                && currentAnalysisIndex >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
