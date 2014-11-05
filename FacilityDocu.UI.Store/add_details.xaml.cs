using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
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


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 


    public sealed partial class add_details : Page
    {
        List<string> id = new List<string>();
        List<string> number = new List<string>();
        List<string> des = new List<string>();
        public add_details()
        {
            this.InitializeComponent();

        }
       
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/ProjectXml.xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);

               
                var data = from query in loadedData.Descendants("rig")
                           select new
                           {

                               rigtype = (string)query.Attribute("type"),

                           };

                foreach (var read in data)
                {
                    Rigtypetext.Items.Add(read.rigtype);
                }


            }
            catch
            {
                msg.show("File Not Found or Error Loading File Please Try Again Later");
            }



            //read xml file

        }
        private void Rigtypetext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/ProjectXml.xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);

                var data = from query in loadedData.Descendants("rig").Where(query => (string)query.Attribute("type") == Rigtypetext.SelectedItem.ToString())
                               .Elements("modules")
                               .Elements("module")
                           select new
                           {
                               name = query.Element("name").Value,
                                number=query.Element("number").Value,
                                id = query.Element("id").Value,

                           };
                id.Clear();
                number.Clear();
                Categorytext.Items.Clear();

                foreach (var read in data)
                {

                    Categorytext.Items.Add(read.name);
                    id.Add(read.id);
                    number.Add(read.number);
                }


            }
            catch
            {
                msg.show("File Not Found or Error Loading File Please Try Again Later");
            }

        }

        private void Categorytext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



            try
            {

                OfflineData.tempcategoryid = id[Rigtypetext.SelectedIndex];
                OfflineData.tempcategory_no = number[Rigtypetext.SelectedIndex];
                id.Clear();
                number.Clear();

                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/ProjectXml.xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);

                //foreach ( var p in rigs.Elements("rig")) {
                //  Rigtypetext.Items.Add( (string)p.Attribute("type"));

                var data = from query in loadedData.Descendants("rig").Where(query => (string)query.Attribute("type") == Rigtypetext.SelectedItem.ToString())
                               .Elements("modules")
                               .Elements("module").Where(query => (string)query.Element("name").Value == Categorytext.SelectedItem.ToString())
                                .Elements("steps")
                                .Elements("step")
                           select new
                           {
                               name = query.Element("name").Value,
                              number=query.Element("number").Value,
                               id = query.Element("id").Value,

                           };

                Subcategorytext.Items.Clear();
                foreach (var read in data)
                {

                    Subcategorytext.Items.Add(read.name);
                    id.Add(read.id);
                    number.Add(read.number);
                }



            }
            catch
            {
                msg.show("File Not Found or Error Loading File Please Try Again Later");

            }
        }

        private void Subcategorytext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                OfflineData.tempsubcategoryid = id[Categorytext.SelectedIndex];
                OfflineData.tempsubcategory_no = number[Categorytext.SelectedIndex];
                id.Clear();
                number.Clear();
                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/ProjectXml.xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);

                //foreach ( var p in rigs.Elements("rig")) {
                //  Rigtypetext.Items.Add( (string)p.Attribute("type"));

                var data = from query in loadedData.Descendants("rig").Where(query => (string)query.Attribute("type") == Rigtypetext.SelectedItem.ToString())
                               .Elements("modules")
                               .Elements("module").Where(query => (string)query.Element("name").Value == Categorytext.SelectedItem.ToString())
                                .Elements("steps")
                                .Elements("step").Where(query => (string)query.Element("name").Value == Subcategorytext.SelectedItem.ToString())
                               .Elements("actions")
                               .Elements("action")
                           select new
                           {
                               name = query.Element("name").Value,
                               number=query.Element("number").Value,
                               id = query.Element("id").Value,
                               dec=query.Element("description").Value,

                           };

                Actiontext.Items.Clear();
                foreach (var read in data)
                {


                    Actiontext.Items.Add(read.name);
                    id.Add(read.id);
                    number.Add(read.number);
                    des.Add(read.dec);

                }
            }
            catch
            {
                msg.show("File Not Found or Error Loading File Please Try Again Later");

            }
        }

        private void Actiontext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OfflineData.tempactionid = id[Subcategorytext.SelectedIndex];
            OfflineData.tempaction_no = number[Subcategorytext.SelectedIndex];
            OfflineData.tempaction_description = des[Subcategorytext.SelectedIndex];
            id.Clear();
            number.Clear();
            des.Clear();

            //action box changed

        }



        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }
        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {

            try
            {
                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/ProjectXml.xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);

                //foreach ( var p in rigs.Elements("rig")) {
                //  Rigtypetext.Items.Add( (string)p.Attribute("type"));
                try
                {
                    var data = from query in loadedData.Descendants("project").Where(query => (string)query.Element("name") == Projectnametext.ToString())

                               select new
                               {
                                  
                                   number = query.Element("number").Value,
                                   id = query.Element("id").Value,
                                    cdate=query.Element("createdtime").Value,
                                    udate=query.Element("updatetime").Value,
                               };
                    foreach (var read in data)
                    {
                        OfflineData.tempprojectid = read.id;

                    }

                }
                catch
                {
                    OfflineData.tempprojectid = "109";
                }
                OfflineData.temprigtype = Rigtypetext.SelectedItem.ToString();
                OfflineData.tempcategory = Categorytext.SelectedItem.ToString();
                OfflineData.tempsubcategory = Subcategorytext.SelectedItem.ToString();
                OfflineData.tempproject_name = Projectnametext.Text;
                OfflineData.tempaction = Actiontext.SelectedItem.ToString();

                //   await MyService.InsertImageAsync(new ServiceReference1.Imagesclass { getDetail = Detailstext.Text });  
                this.Frame.Navigate(typeof(Camera_Page));
            }
            catch
            {
                msg.show("Fields cannot be empty");
            }
        }


    }
}
