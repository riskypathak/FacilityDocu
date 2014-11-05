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
        int pathStatus = 0;
        public Settings()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
        public async void viewPath()
        {
            int status = 0;
            //load the paths from the config xml  file
            try
            {
                try
                {
                    string DBXMLPath = System.IO.Path.Combine(KnownFolders.PicturesLibrary.Path, "Application\\Config.xml");
                    StorageFile a = await ApplicationData.Current.LocalFolder.GetFileAsync("Application\\Config.xml");  
                     XDocument loadedData = XDocument.Load(a.Path );
                    var data = from query in loadedData.Descendants("paths")
                               select new
                               {
                                   xmlPath = query.Element("xml_path").Value,
                                   dataPath = query.Element("data_path").Value,
                                   backupPath = query.Element("backup_path").Value,
                               };
                    foreach (var read in data)
                    {

                        xml_path.Text = read.xmlPath;
                        data_path.Text = read.dataPath;
                        backup_path.Text = read.backupPath;


                    }
                    status = 1;
                   
                }
                catch
                {

                }
                if (status != 1)
                {
                    OfflineData.xmlp = ApplicationData.Current.LocalFolder.Path + "\\Data\\FacilityDocu\\Data";
                    StorageFolder picfol = await KnownFolders.PicturesLibrary.CreateFolderAsync("FacilityDocuImages", CreationCollisionOption.OpenIfExists);
                    OfflineData.datap = picfol.Path;
                    StorageFolder picfol1 = await KnownFolders.PicturesLibrary.CreateFolderAsync("FacilityDocuBackup", CreationCollisionOption.OpenIfExists);
                    OfflineData.backupp = picfol1.Path;
                    xml_path.Text = OfflineData.xmlp;
                    data_path.Text = OfflineData.datap;
                    backup_path.Text = OfflineData.backupp;
                  //  msg.show(OfflineData.xmlp);
                }

               
            }
                    catch
                    {

                    }

        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            viewPath();

        }
        XmlDocument dom;
        public void setXmlData()
        {
            try
            {
                dom = new XmlDocument();
                XmlElement x;
                x = dom.CreateElement("setting");
                dom.AppendChild(x);
               
                    XmlElement x1 = dom.CreateElement("paths");

                    XmlElement x11 = dom.CreateElement("xml_path");

                    x11.InnerText = OfflineData.xmlp;

                    x1.AppendChild(x11);
                
                XmlElement x12 = dom.CreateElement("data_path");

                x12.InnerText = OfflineData.datap;

                x1.AppendChild(x12);

                XmlElement x13 = dom.CreateElement("backup_path");

                x13.InnerText = OfflineData.backupp;

                x1.AppendChild(x13);

                x.AppendChild(x1);
            }
            catch
            {
                MessageDialog msgsh = new MessageDialog("Problem Occured To Do PerForm", "Failed");
                msgsh.ShowAsync();
            }
        }
        public async void writeConfigXml()
        {
            
               
                Windows.Storage.StorageFolder storage_folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");


                Windows.Storage.StorageFile filepath = await storage_folder.GetFileAsync("Application\\Config.xml");
                // await aaa.RenameAsync("Employee1.xml", NameCollisionOption.GenerateUniqueName  );
               await filepath.DeleteAsync();
                //await  filepath.OpenStreamForWriteAsync();

               StorageFile st = await storage_folder.CreateFileAsync("Application\\Config.xml", CreationCollisionOption.GenerateUniqueName);
                await dom.SaveToFileAsync(st);
                viewPath();
                try
                {
            }
            catch
            {
                MessageDialog showmsg = new MessageDialog("Error to Reset Handling");
                showmsg.ShowAsync();
            }


        }
        public async void checkpath(string path)
        {
            pathStatus = 0;
            string[] tempPath = path.Split('\\');
            string getPath;
            string pp="";
            getPath = tempPath[0];
            int status = 0;
            ////////////check path status
            StorageFolder sf;
            try
            {
                for (int i = 1; i < tempPath.Count(); i++)
                    {
                try
                {
                    pp = getPath;
                        getPath = getPath + "\\" + tempPath[i];
                     sf=   await StorageFolder.GetFolderFromPathAsync(getPath);
                    
                }
                catch
                {
                    status = 1;
                }
                    if(status==1)
                    {
                        status = 0;
                        sf = await StorageFolder.GetFolderFromPathAsync(pp);
                        await sf.CreateFolderAsync(getPath);
                        
                    }

                    }  
            }
            catch
            {
                pathStatus = 1;
            }

           
        }
        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
          //  checkpath(xml_path.Text);
            if(pathStatus==0)
            {
                OfflineData.xmlp = xml_path.Text;
                MessageDialog msgsh = new MessageDialog("Sucess Fully save your Data", "Done");

                await msgsh.ShowAsync();
            }
            else
            {
                 OfflineData.xmlp = KnownFolders.PicturesLibrary.Path;
                 MessageDialog msgsh = new MessageDialog("Unsuported Path default path becomes >>" + OfflineData.xmlp, "Invalid");
                await msgsh.ShowAsync();
               
            }
          //  checkpath(data_path.Text);
            if (pathStatus == 0)
            {
                OfflineData.datap = data_path.Text;
                MessageDialog msgsh = new MessageDialog("Sucess Fully save your Data", "Done");

                await msgsh.ShowAsync();
            }
            else
            {
                OfflineData.datap = KnownFolders.PicturesLibrary.Path;
                MessageDialog msgsh = new MessageDialog("Unsuported Path default path becomes >>" + OfflineData.datap, "Invalid");
                await msgsh.ShowAsync();
            }
            checkpath(backup_path.Text);
            if (pathStatus == 0)
            {
                OfflineData.backupp = backup_path.Text;
                MessageDialog msgsh = new MessageDialog("Sucess Fully save your Data", "Done");

                await msgsh.ShowAsync();
            }
            else
            {
                OfflineData.backupp = KnownFolders.PicturesLibrary.Path;
                MessageDialog msgsh = new MessageDialog("Unsuported Path default path becomes >>" + OfflineData.backupp, "Invalid");
                await msgsh.ShowAsync();
               
            }
              
                setXmlData();
                writeConfigXml();

        }

        private async void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
           
            //load the paths from the config xml  file
            try
            {
               
                   
                   OfflineData.xmlp = Package.Current.InstalledLocation.Path + "Data\\FacilityDocu\\Data";
                    StorageFolder picfol = await KnownFolders.PicturesLibrary.CreateFolderAsync("FacilityDocuImages", CreationCollisionOption.OpenIfExists);
                    OfflineData.datap = picfol.Path;
                    StorageFolder picfol1 = await KnownFolders.PicturesLibrary.CreateFolderAsync("FacilityDocuBackup", CreationCollisionOption.OpenIfExists);
                    OfflineData.backupp = picfol1.Path;
                    //xml_path.Text = OfflineData.xmlp;
                    data_path.Text = OfflineData.datap;
                    backup_path.Text = OfflineData.backupp;
                    //   msg.show(OfflineData.xmlp);
                
            setXmlData();
            writeConfigXml();
                     MessageDialog msgsh = new MessageDialog("Sucessfully data reset.", "Complete");

            await msgsh.ShowAsync();
                   
                    }
            catch
            {
msg.show("Reset Unsuccessful");
            }

           
        }


    }
}
