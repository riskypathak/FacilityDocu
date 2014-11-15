using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Tablet_App.ServiceReference1;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{


    public sealed partial class ActionSelect : Page
    {
        public ActionSelect()
        {
            this.InitializeComponent();

        }

        public async void LoadAllProjects()
        {
            try
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

            }
            catch
            {

            }

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllProjects();
        }

        private void cmbProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml",(e.AddedItems[0] as ProjectDTO).ProjectID)), false);

                cmbRigTypes.ItemsSource = Data.CURRENT_PROJECT.RigTypes;
            }
            catch
            {

            }

        }

        private void Rigtypetext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cmbModules.ItemsSource = (e.AddedItems[0] as RigTypeDTO).Modules;
                Data.CURRENT_RIG = (e.AddedItems[0] as RigTypeDTO);
            }
            catch
            {

            }

        }

        private void cmbModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                cmbSteps.ItemsSource = (e.AddedItems[0] as ModuleDTO).Steps;
                Data.CURRENT_MODULE = (e.AddedItems[0] as ModuleDTO);
            }
            catch
            {


            }
        }

        private void cmbSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cmbActions.ItemsSource = (e.AddedItems[0] as StepDTO).Actions;
                Data.CURRENT_STEP = (e.AddedItems[0] as StepDTO);
            }
            catch
            {


            }
        }

        private void cmbActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.CURRENT_ACTION = (e.AddedItems[0] as ActionDTO);
        }

        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (Data.menuClick == 3)
                {
                    this.Frame.Navigate(typeof(Camera_Page));
                }
                else
                {
                    this.Frame.Navigate(typeof(Gallery));
                }
            }
            catch
            {
                ScreenMessage.Show("Fields cannot be empty");
            }
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
