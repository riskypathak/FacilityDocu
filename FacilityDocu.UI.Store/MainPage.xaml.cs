﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

//test

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
  //start page
namespace Tablet_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {
            Data.menuClick = new Camera_Page();
            this.Frame.Navigate(typeof(ActionSelect));

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }

        private void btnGallery_Click(object sender, RoutedEventArgs e)
        {
            Data.menuClick = new Gallery();
            this.Frame.Navigate(typeof(ActionSelect));
        }

        public async Task CheckConfigXML()
        {
            bool isInit = true;

            StorageFile configFile;
            try
            {
                configFile = await ApplicationData.Current.LocalFolder.GetFileAsync("config.xml");
            }
            catch (FileNotFoundException)
            {
                isInit = false;
            }

            if (!isInit)
            {
                await Initialize();
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckConfigXML();
            await Read();

        }

        private static async Task Read()
        {
            StorageFile configFile;
            configFile = await ApplicationData.Current.LocalFolder.GetFileAsync("config.xml");

            XDocument loadedData = XDocument.Load(configFile.Path);

            var data = from query in loadedData.Elements("settings")
                       select new
                       {
                           ProjectXmlPath = query.Element("projectxml").Value,
                           ImagesPath = query.Element("images").Value,
                           BackupPath = query.Element("backup").Value,
                       };


            foreach (var read in data)
            {

                Data.ProjectXmlPath = read.ProjectXmlPath;
                Data.ImagesPath = read.ImagesPath;
                Data.BackupPath = read.BackupPath;
            }
        }

        public async Task Initialize()
        {
            StorageFile configFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("config.xml", CreationCollisionOption.ReplaceExisting);

            StorageFolder projectXmlPath = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProjectXML", CreationCollisionOption.OpenIfExists);
            StorageFolder imagesPath = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
            StorageFolder backupPath = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backup", CreationCollisionOption.OpenIfExists);

            var stream = await configFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

            using (var outputStream = stream.AsStreamForWrite())
            {

                XElement xmltree = new XElement("settings",
                       new XElement("projectxml", projectXmlPath.Path),
                       new XElement("images", imagesPath.Path),
                       new XElement("backup", backupPath.Path));

                xmltree.Save(outputStream);
            }
        }
    }

    public class ScreenMessage
    {
        public static async void Show(string message)
        {
            MessageDialog messageDialog = new MessageDialog(message, "PhotoDocu");
            await messageDialog.ShowAsync();
        }
    }
}
