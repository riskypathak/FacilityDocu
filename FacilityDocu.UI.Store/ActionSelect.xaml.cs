using System;
using System.Collections.Generic;
using System.IO;
using Tablet_App.ServiceReference1;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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

            if (cmbProjects.Items.Count > Data.selectedIndexProject)
            {
                cmbProjects.SelectedIndex = Data.selectedIndexProject;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllProjects();
        }

        private async void cmbProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.selectedIndexProject = (sender as ComboBox).SelectedIndex;
            Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", (cmbProjects.SelectedItem as ProjectDTO).ProjectID)), false);
            cmbRigTypes.ItemsSource = Data.CURRENT_PROJECT.RigTypes;

            if (cmbRigTypes.Items.Count > Data.selectedIndexRigType)
            {
                cmbRigTypes.SelectedIndex = Data.selectedIndexRigType;
            }
        }

        private void Rigtypetext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.selectedIndexRigType = (sender as ComboBox).SelectedIndex;
            cmbModules.ItemsSource = (cmbRigTypes.SelectedItem as RigTypeDTO).Modules;
            Data.CURRENT_RIG = (cmbRigTypes.SelectedItem as RigTypeDTO);

            if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Camera_Page))
            {
                if (cmbModules.Items.Count > Data.selectedIndexModule)
                {
                    cmbModules.SelectedIndex = Data.selectedIndexModule;
                }
            }
        }

        private void cmbModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.selectedIndexModule = (sender as ComboBox).SelectedIndex;
            cmbSteps.ItemsSource = (cmbModules.SelectedItem as ModuleDTO).Steps;
            Data.CURRENT_MODULE = (cmbModules.SelectedItem as ModuleDTO);

            if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Camera_Page))
            {
                if (cmbSteps.Items.Count > Data.selectedIndexStep)
                {
                    cmbSteps.SelectedIndex = Data.selectedIndexStep;
                }
            }
        }

        private void cmbSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.selectedIndexStep = (sender as ComboBox).SelectedIndex;
            cmbActions.ItemsSource = (cmbSteps.SelectedItem as StepDTO).Actions;
            Data.CURRENT_STEP = (cmbSteps.SelectedItem as StepDTO);

            if (Data.menuClick != null && Data.menuClick.GetType() == typeof(Camera_Page))
            {
                if (cmbActions.Items.Count > Data.selectedIndexAction)
                {
                    cmbActions.SelectedIndex = Data.selectedIndexAction;
                }
            }
        }

        private void cmbActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.selectedIndexAction = (sender as ComboBox).SelectedIndex;
            Data.CURRENT_ACTION = (cmbActions.SelectedItem as ActionDTO);
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