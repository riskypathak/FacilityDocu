using System;
using System.Collections.Generic;
using System.IO;
using Tablet_App.ServiceReference1;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Linq;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace Tablet_App
{
    public sealed partial class ActionSelect : Page
    {
        public ActionSelect()
        {
            this.InitializeComponent();
            Data.CURRENT_ACTION = null;
            Data.CURRENT_MODULE = null;
            Data.CURRENT_PROJECT = null;
            Data.CURRENT_RIG = null;
            Data.CURRENT_STEP = null;

            cmbProjects.Items.Clear();
            cmbRigTypes.Items.Clear();
            cmbModules.Items.Clear();
            cmbSteps.Items.Clear();
            cmbActions.Items.Clear();

        }

        public async void LoadAllProjects()
        {
            IReadOnlyList<StorageFile> selectFiles;
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".xml");
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(Data.ProjectXmlPath);
            var folderFile = sf.CreateFileQueryWithOptions(queryOptions);
            selectFiles = await folderFile.GetFilesAsync();
            IList<ProjectDTO> projects = new List<ProjectDTO>();
            foreach (var projectFile in selectFiles)
            {
                projects.Add(ProjectXmlReader.ReadProjectXml(projectFile.Path, true));
            }
            cmbProjects.ItemsSource = projects;

            if(!string.IsNullOrEmpty(Data.ACTION_SELECT_PROJECT_ID))
            {
                cmbProjects.SelectedItem = projects.SingleOrDefault(P => P.ProjectID == Data.ACTION_SELECT_PROJECT_ID);
            }
            else
            {
                cmbProjects.SelectedIndex = -1;
            }

            //if (Data.CURRENT_PROJECT != null && cmbProjects.Items.Contains(Data.CURRENT_PROJECT))
            //{
            //    cmbProjects.SelectedItem = Data.CURRENT_PROJECT;
            //}
       
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllProjects();
        }

        private async void cmbProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProjects.SelectedIndex != -1)
            {
                Data.ACTION_SELECT_PROJECT_ID = (cmbProjects.SelectedItem as ProjectDTO).ProjectID;
                Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", (cmbProjects.SelectedItem as ProjectDTO).ProjectID)), false);
                cmbRigTypes.ItemsSource = Data.CURRENT_PROJECT.RigTypes;

                if (!string.IsNullOrEmpty(Data.ACTION_SELECT_RIGTYPE_ID))
                {
                    cmbRigTypes.SelectedItem = Data.CURRENT_PROJECT.RigTypes.SingleOrDefault(r=>r.Name == Data.ACTION_SELECT_RIGTYPE_ID);
                }
                else
                {
                    cmbRigTypes.SelectedIndex = -1;
                }
            }
            else
            {
                cmbRigTypes.SelectedIndex = -1;
            }
        }

        private void Rigtypetext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRigTypes.SelectedIndex != -1)
            {
                Data.ACTION_SELECT_RIGTYPE_ID = (cmbRigTypes.SelectedItem as RigTypeDTO).Name;
                cmbModules.ItemsSource = (cmbRigTypes.SelectedItem as RigTypeDTO).Modules;

                Data.CURRENT_RIG = (cmbRigTypes.SelectedItem as RigTypeDTO);

                if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Camera_Page))
                {
                    if (!string.IsNullOrEmpty(Data.ACTION_SELECT_MODULE_ID))
                    {
                        cmbModules.SelectedItem = Data.CURRENT_RIG.Modules.SingleOrDefault(r => r.ModuleID == Data.ACTION_SELECT_MODULE_ID);
                    }
                    else
                    {
                        cmbModules.SelectedIndex = -1;
                    }
                }
            }
            else
            {
                cmbModules.SelectedIndex = -1;
            }
        }

        private void cmbModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbModules.SelectedIndex != -1)
            {
                Data.ACTION_SELECT_MODULE_ID = (cmbModules.SelectedItem as ModuleDTO).ModuleID;
                cmbSteps.ItemsSource = (cmbModules.SelectedItem as ModuleDTO).Steps;
                Data.CURRENT_MODULE = (cmbModules.SelectedItem as ModuleDTO);

                if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Camera_Page))
                {
                    if (!string.IsNullOrEmpty(Data.ACTION_SELECT_STEP_ID))
                    {
                        cmbSteps.SelectedItem = Data.CURRENT_MODULE.Steps.SingleOrDefault(r => r.StepID == Data.ACTION_SELECT_STEP_ID);
                    }
                    else
                    {
                        cmbSteps.SelectedIndex = -1;
                    }
                }
            }
            else
            {
                cmbSteps.SelectedIndex = -1;
            }
        }

        private void cmbSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSteps.SelectedIndex != -1)
            {
                Data.ACTION_SELECT_STEP_ID = (cmbSteps.SelectedItem as StepDTO).StepID;
                cmbActions.ItemsSource = (cmbSteps.SelectedItem as StepDTO).Actions;
                Data.CURRENT_STEP = (cmbSteps.SelectedItem as StepDTO);

                if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Camera_Page))
                {
                    if (!string.IsNullOrEmpty(Data.ACTION_SELECT_ACTION_ID))
                    {
                        cmbActions.SelectedItem = Data.CURRENT_STEP.Actions.SingleOrDefault(r => r.ActionID == Data.ACTION_SELECT_ACTION_ID);
                    }
                    else
                    {
                        cmbActions.SelectedIndex = -1;
                    }
                }
            }
            else
            {
                cmbActions.SelectedIndex = -1;
            }
        }

        private void cmbActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbActions.SelectedIndex != -1)
            {
                Data.ACTION_SELECT_ACTION_ID = (cmbActions.SelectedItem as ActionDTO).ActionID;
                Data.CURRENT_ACTION = (cmbActions.SelectedItem as ActionDTO);
            }
        }

        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {
            if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Camera_Page))
            {
                if (cmbActions.SelectedIndex != -1 && cmbProjects.SelectedIndex != -1
                && cmbModules.SelectedIndex != -1 && cmbSteps.SelectedIndex != -1)
                {
                    this.Frame.Navigate(typeof(Camera_Page));
                    return;
                }
                else
                {
                    ScreenMessage.Show("All fields are mandatory.");
                }
            }
            else if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Gallery))
            {
                if (cmbRigTypes.SelectedIndex != -1 && cmbProjects.SelectedIndex != -1)
                {
                    this.Frame.Navigate(typeof(Gallery));
                    return;
                }
                else
                {
                    ScreenMessage.Show("Project & RigType fileds are mandatory.");
                }
            }
        }
        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}