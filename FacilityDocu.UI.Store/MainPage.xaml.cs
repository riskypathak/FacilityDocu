using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//test

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///

    /// public class conMyservice

    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OfflineData.menuClick = 3;
            this.Frame.Navigate(typeof(add_details));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OfflineData.menuClick = 2;
            this.Frame.Navigate(typeof(add_details));
        }
        public async void projctlist()
        {
            try
            {
                string DBXMLPath1 = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/MyProjectList.xml");
                XElement loadedData1 = XElement.Load(DBXMLPath1);

                StorageFile xmlFile1 = await ApplicationData.Current.LocalFolder.CreateFileAsync("Application\\MyProjectList.xml", CreationCollisionOption.FailIfExists);
                using (Stream fileStream = await xmlFile1.OpenStreamForWriteAsync())
                {
                    loadedData1.Save(fileStream);
                }
            }
            catch
            { }
        }
        public async void appned()
        {
            try
            {
                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/Config.xml");
                XElement loadedData = XElement.Load(DBXMLPath);

                StorageFile xmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Application\\Config.xml", CreationCollisionOption.FailIfExists);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }


               
            }
            catch
            {

            }
        }
        public async void mainxml()
        {
            try
            {
                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/ProjectXML.xml");
                XElement loadedData = XElement.Load(DBXMLPath);

                StorageFile xmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Data\\FacilityDocu\\ProjectXML\\ProjectXML.xml", CreationCollisionOption.FailIfExists);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
            }
            catch
            {

            }
        }
        public async void initapp()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data\\FacilityDocu\\Data", CreationCollisionOption.OpenIfExists);
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data\\FacilityDocu\\ProjectXML", CreationCollisionOption.OpenIfExists);
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("Application", CreationCollisionOption.OpenIfExists);

                appned();
                mainxml();
                projctlist();
            }
            catch
            {

            }

        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OfflineData.profileflag = 0;
            OfflineData.picCount = 0;
            int status = 0;
          
            try
            {
              
                
                try
                {
                    StorageFile fl = await ApplicationData.Current.LocalFolder.CreateFileAsync("Application\\Config.xml", CreationCollisionOption.OpenIfExists);
                    //string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/Config.xml");
                    XDocument loadedData = XDocument.Load(fl.Path);
                    var data = from query in loadedData.Descendants("paths")
                               select new
                               {
                                   xmlPath = query.Element("xml_path").Value,
                                   dataPath = query.Element("data_path").Value,
                                   backupPath = query.Element("backup_path").Value,
                               };
                    foreach (var read in data)
                    {

                        OfflineData.xmlp = read.xmlPath;
                        OfflineData.datap = read.dataPath;
                        OfflineData.backupp = read.backupPath;


                    }
                    status = 1;
                }
                catch
                {
                    initapp();
                }
                if (status != 1)
                {
                    OfflineData.xmlp = ApplicationData.Current.LocalFolder.Path + "Data\\FacilityDocu\\Data";
                    StorageFolder picfol = await KnownFolders.PicturesLibrary.CreateFolderAsync("FacilityDocuImages", CreationCollisionOption.OpenIfExists);
                    OfflineData.datap = picfol.Path;
                    StorageFolder picfol1 = await KnownFolders.PicturesLibrary.CreateFolderAsync("FacilityDocuBackup", CreationCollisionOption.OpenIfExists);
                    OfflineData.backupp = picfol1.Path;
                    //   msg.show(OfflineData.xmlp);
                }
              
               
            }
            catch
            {
                msg.show("Error to Loading Page");
            }
               

           
        }


    }

    public class msg
    {
        public static async void show(string ms)
        {
            try
            {
                MessageDialog showmsg = new MessageDialog(ms);
                await showmsg.ShowAsync();
            }
            catch
            {

            }

        }
    }



}
