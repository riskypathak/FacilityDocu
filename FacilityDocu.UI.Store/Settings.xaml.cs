using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
//test

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        private void btnBack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        public void SetControls()
        {
            txtProjectPath.Text = Data.ProjectXmlPath;
            txtImagesPath.Text = Data.ImagesPath;
            txtBackupPath.Text = Data.BackupPath;

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetControls();

        }

        private async void btnSave_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                StorageFolder projectFolder = await StorageFolder.GetFolderFromPathAsync(txtProjectPath.Text);
                await projectFolder.CreateFileAsync("temp");

                StorageFile tempFile = await projectFolder.GetFileAsync("temp");
                await tempFile.DeleteAsync();
            }
            catch (Exception)
            {
                (new MessageDialog("ProjectXml path is either doesnot exist or does not have read/write access", "Error")).ShowAsync();
                return;
            }

            try
            {
                StorageFolder imagesFolder = await StorageFolder.GetFolderFromPathAsync(txtImagesPath.Text);
                await imagesFolder.CreateFileAsync("temp");

                StorageFile tempFile = await imagesFolder.GetFileAsync("temp");
                await tempFile.DeleteAsync();
            }
            catch (Exception)
            {
                (new MessageDialog("Data path is either doesnot exist or does not have read/write access", "Error")).ShowAsync();
                return;
            }

            try
            {
                StorageFolder backupFolder = await StorageFolder.GetFolderFromPathAsync(txtBackupPath.Text);
                await backupFolder.CreateFileAsync("temp");

                StorageFile tempFile = await backupFolder.GetFileAsync("temp");
                await tempFile.DeleteAsync();
            }
            catch (Exception)
            {
                (new MessageDialog("Backup path is either doesnot exist or does not have read/write access", "Error")).ShowAsync();
                return;
            }

            StorageFile configFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("config.xml", CreationCollisionOption.ReplaceExisting);

            var stream = await configFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

            using (var outputStream = stream.AsStreamForWrite())
            {

                XElement xmltree = new XElement("settings",
                       new XElement("projectxml", txtProjectPath.Text),
                       new XElement("images", txtImagesPath.Text),
                       new XElement("backup", txtBackupPath.Text));

                xmltree.Save(outputStream);
            }

            Data.ProjectXmlPath = txtProjectPath.Text;
            Data.ImagesPath = txtImagesPath.Text;
            Data.BackupPath = txtBackupPath.Text;

            await (new MessageDialog("Path changed sucessfully", "Success")).ShowAsync();
        }

      
        private async void btnReset_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                StorageFolder projectXmlPath = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProjectXML", CreationCollisionOption.OpenIfExists);
                StorageFolder imagesPath = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                StorageFolder backupPath = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Backup", CreationCollisionOption.OpenIfExists);

                txtBackupPath.Text = backupPath.Path;
                txtImagesPath.Text = imagesPath.Path;
                txtProjectPath.Text = projectXmlPath.Path;
            }
            catch
            {
                ScreenMessage.Show("Reset Unsuccessful");
            }
        }

        private void btnSync_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                //test
                //SyncManager manager = new SyncManager(new List<int> { 1, 2 });
                //manager.UpdateProjectXml();
                SyncManager manager = new SyncManager();
                IList<int> projectIds = manager.IsSyncRequired();

                SyncManager updateCall = new SyncManager(projectIds);
                updateCall.UpdateProjectXml();
            }
            catch
            {

            }
        }


    }
}
