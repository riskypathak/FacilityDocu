﻿using System;
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

            if (Data.CURRENT_PROJECT != null)
            {
                cmbProjects.SelectedItem = Data.CURRENT_PROJECT;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllProjects();
        }

        private void cmbProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(Path.Combine(Data.ProjectXmlPath, string.Format("{0}.xml", (e.AddedItems[0] as ProjectDTO).ProjectID)), false);
            cmbRigTypes.ItemsSource = Data.CURRENT_PROJECT.RigTypes;
        }

        private void Rigtypetext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbModules.ItemsSource = (e.AddedItems[0] as RigTypeDTO).Modules;
            Data.CURRENT_RIG = (e.AddedItems[0] as RigTypeDTO);
        }

        private void cmbModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbSteps.ItemsSource = (e.AddedItems[0] as ModuleDTO).Steps;
            Data.CURRENT_MODULE = (e.AddedItems[0] as ModuleDTO);
        }

        private void cmbSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbActions.ItemsSource = (e.AddedItems[0] as StepDTO).Actions;
            Data.CURRENT_STEP = (e.AddedItems[0] as StepDTO);
        }

        private void cmbActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data.CURRENT_ACTION = (e.AddedItems[0] as ActionDTO);
        }

        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {
            if (cmbActions.SelectedIndex != -1
                && cmbProjects.SelectedIndex != -1
                && cmbModules.SelectedIndex != -1
                && cmbSteps.SelectedIndex != -1)
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
            else
            {
                ScreenMessage.Show("All fields are mandatory.");
            }
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
