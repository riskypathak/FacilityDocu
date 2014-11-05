using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{


    public sealed partial class add_details : Page
    {
        List<string> tempStore1 = new List<string>();
        List<string> tempStore2 = new List<string>();
        List<string> tempStore3 = new List<string>();
        List<string> tempStore4 = new List<string>();
        List<string> tempStore5 = new List<string>();
        int index;
        public add_details()
        {
            this.InitializeComponent();

        }
       public async void projectRead()
        {
            try
            {
                string DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data/FacilityDocu/ProjectXML/ProjectXml.xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);


                var data = from query in loadedData.Descendants("project")
                           select new
                           {
                               pid = (string)query.Element("id"),
                               pname = (string)query.Element("name"),
                               pdescription = (string)query.Element("description"),
                               pcreatedtime = (string)query.Element("createdtime"),
                               pupdatetime = (string)query.Element("updatetime"),

                           };

                foreach (var read in data)
                {

                    tempStore1.Add(read.pid);
                    tempStore2.Add(read.pname);
                    tempStore3.Add(read.pdescription);
                    tempStore4.Add(read.pcreatedtime);
                    tempStore5.Add(read.pupdatetime);
                    if (OfflineData.menuClick == 2)
                    {
                        try
                        {

                            await StorageFile.GetFileFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\Data\\FacilityDocu\\Data\\" + read.pid + "." + read.pname + ".xml");
                            Projectnametext.Items.Add(read.pname);

                        }
                        catch
                        {

                        }

                    }
                    else
                    {
                        Projectnametext.Items.Add(read.pname);
                    }
                }


            }
            catch
            {

            }
        }
       public async void rigtypeRead()
       {
           try
           {
             
                string DBXMLPath;
                if (OfflineData.menuClick == 2)
                {
                    DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");

                }
                else
                {
                    DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\ProjectXML\\ProjectXml.xml");
             
                }
                  XDocument loadedData = XDocument.Load(DBXMLPath);


                var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                           .Elements("rigs").Elements("rig")
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

            }
           
       }
       public async void moduleRead()
       {
           try
           {
               string DBXMLPath;
               if (OfflineData.menuClick == 2)
               {
                   DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");

               }
               else
               {
                   DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\ProjectXML\\ProjectXml.xml");

               } XDocument loadedData = XDocument.Load(DBXMLPath);

               var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                          .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                              .Elements("modules")
                              .Elements("module")
                          select new
                          {
                              mid = query.Element("id").Value,
                              mname = query.Element("name").Value,
                              mnumber = query.Element("number").Value,
                          };

             //  Categorytext.Items.Clear();

               foreach (var read in data)
               {

                   tempStore1.Add(read.mid);
                   tempStore2.Add(read.mname);
                   tempStore3.Add(read.mnumber);

                 //  Categorytext.Items.Add(read.mname);

               }
           }
           catch
           {

           }
       }
       public async void stepRead()
       {
           try
           {
               string DBXMLPath;
               if (OfflineData.menuClick == 2)
               {
                   DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");

               }
               else
               {
                   DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\ProjectXML\\ProjectXml.xml");

               } XDocument loadedData = XDocument.Load(DBXMLPath);

               //foreach ( var p in rigs.Elements("rig")) {
               //  Rigtypetext.Items.Add( (string)p.Attribute("type"));

               var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                          .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                              .Elements("modules")
                              .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                               .Elements("steps")
                               .Elements("step")
                          select new
                          {
                              sname = query.Element("name").Value,
                              snumber = query.Element("number").Value,
                              sid = query.Element("id").Value,

                          };


               foreach (var read in data)
               {
                   tempStore1.Add(read.sid);
                   tempStore2.Add(read.sname);
                   tempStore3.Add(read.snumber);
                   //Subcategorytext.Items.Add(read.sname);

               }
           }
           catch
           {

           }
       }
       public async void actionRead()
       {
           try
           {
           string DBXMLPath;
           if (OfflineData.menuClick == 2)
           {
               DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");

           }
           else
           {
               DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\ProjectXML\\ProjectXml.xml");

           } XDocument loadedData = XDocument.Load(DBXMLPath);

           var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                      .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                          .Elements("modules")
                          .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                           .Elements("steps")
                           .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                          .Elements("actions")
                          .Elements("action")
                      select new
                      {
                          aid = query.Element("id").Value,

                          aname = query.Element("name").Value,
                          anumber = query.Element("number").Value,
                          adec = query.Element("description").Value,

                      };


           foreach (var read in data)
           {

               tempStore1.Add(read.aid);
               tempStore2.Add(read.aname);
               tempStore3.Add(read.anumber);
               tempStore4.Add(read.adec);
             //  Actiontext.Items.Add(read.aname);


           }
       }
        catch
    {

    }
       }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(OfflineData.menuClick==2)
            {
                head.Text = "Select Your Project";
            }
            else
            {
                head.Text = "Add picture details";

            }
            projectRead();

        }
        public void cleartempstore()
        {
            try
            {
                tempStore1.Clear();
                tempStore2.Clear();
                tempStore3.Clear();
                tempStore4.Clear();
                tempStore5.Clear();
            }
            catch
            {

            }
        }
        private void Projectnametext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                projectRead();
                index = Projectnametext.SelectedIndex;
                if (index != 1)
                {
                    OfflineData.tempProjectID = tempStore1[index];
                    OfflineData.tempProjectName = tempStore2[index];
                    OfflineData.tempProjectDescription = tempStore3[index];
                    OfflineData.tempProjectCreatedDate = tempStore4[index];
                    OfflineData.tempProjectUpdateDate = tempStore5[index];
                    cleartempstore();
                }
                rigtypeRead();
            }
            catch
            {

            }

        }
        private void Rigtypetext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Categorytext.Items.Clear();
                //  rigtypeRead();
                OfflineData.tempRigType = Rigtypetext.SelectedItem.ToString();
                string DBXMLPath;
                if (OfflineData.menuClick == 2)
                {
                    DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");

                }
                else
                {
                    DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\ProjectXML\\ProjectXml.xml");

                } XDocument loadedData = XDocument.Load(DBXMLPath);

                var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                           .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                               .Elements("modules")
                               .Elements("module")
                           select new
                           {
                               mid = query.Element("id").Value,
                               mname = query.Element("name").Value,
                               mnumber = query.Element("number").Value,
                           };

                //  Categorytext.Items.Clear();

                foreach (var read in data)
                {

                    //tempStore1.Add(read.mid);
                    //tempStore2.Add(read.mname);
                    //tempStore3.Add(read.mnumber);

                    Categorytext.Items.Add(read.mname);

                }


            }
            catch
            {
               
            }

        }

        private void Categorytext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            try{
                Subcategorytext.Items.Clear();
                moduleRead();
               index=Categorytext.SelectedIndex;
               if (index != -1)
               {
                  
                   OfflineData.tempModuleID = tempStore1[index];
                   OfflineData.tempModuleName = tempStore2[index];
                   OfflineData.tempModuleNo = tempStore3[index]; ;
                   cleartempstore();
               }


               string DBXMLPath;
               if (OfflineData.menuClick == 2)
               {
                   DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");

               }
               else
               {
                   DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\ProjectXML\\ProjectXml.xml");

               } XDocument loadedData = XDocument.Load(DBXMLPath);

               //foreach ( var p in rigs.Elements("rig")) {
               //  Rigtypetext.Items.Add( (string)p.Attribute("type"));

               var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                          .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                              .Elements("modules")
                              .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                               .Elements("steps")
                               .Elements("step")
                          select new
                          {
                              sname = query.Element("name").Value,
                              snumber = query.Element("number").Value,
                              sid = query.Element("id").Value,

                          };


               foreach (var read in data)
               {
                   //tempStore1.Add(read.sid);
                   //tempStore2.Add(read.sname);
                   //tempStore3.Add(read.snumber);
                   Subcategorytext.Items.Add(read.sname);

               }
                
            }
            catch
            {
               

            }
        }

        private void Subcategorytext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            stepRead();
            try
            {
                Actiontext.Items.Clear();
                index = Subcategorytext.SelectedIndex;
                if (index != -1)
                {
                    OfflineData.tempStepID = tempStore1[index];
                    OfflineData.tempStepName = tempStore2[index];
                    OfflineData.tempStepNo = tempStore3[index]; ;
                    cleartempstore();
                }
                string DBXMLPath;
                if (OfflineData.menuClick == 2)
                {
                    DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");

                }
                else
                {
                    DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\ProjectXML\\ProjectXml.xml");

                } XDocument loadedData = XDocument.Load(DBXMLPath);

                var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                           .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                               .Elements("modules")
                               .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                                .Elements("steps")
                                .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                               .Elements("actions")
                               .Elements("action")
                           select new
                           {
                               aid = query.Element("id").Value,

                               aname = query.Element("name").Value,
                               anumber = query.Element("number").Value,
                               adec = query.Element("description").Value,

                           };


                foreach (var read in data)
                {

                    //tempStore1.Add(read.aid);
                    //tempStore2.Add(read.aname);
                    //tempStore3.Add(read.anumber);
                    //tempStore4.Add(read.adec);
                    Actiontext.Items.Add(read.aname);


                }
            }
            catch
            {
               

            }
        }

        private void Actiontext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                actionRead();
                 index = Actiontext.SelectedIndex;
                 if (index != -1)
                 {
                     OfflineData.tempActionID = tempStore1[index];
                     OfflineData.tempActionName = tempStore2[index];
                     OfflineData.tempActionNo = tempStore3[index];
                     OfflineData.tempActionDescription = tempStore4[index];
                     cleartempstore();
                 }
                OfflineData.folderName = OfflineData.tempProjectID + "." + OfflineData.tempRigType + "." + OfflineData.tempModuleID + "." + OfflineData.tempStepID + "." + OfflineData.tempActionID ;
                OfflineData.projectXml = OfflineData.tempProjectID + "." + OfflineData.tempProjectName;

                try
                {
                  //  msg.show(OfflineData.tempStepName);
                    string DBXMLPath;
                    DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.tempProjectID + "." + OfflineData.tempProjectName + ".xml");
   XDocument loadedData = XDocument.Load(DBXMLPath);

                    var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                               .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                   .Elements("modules")
                                   .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                                    .Elements("steps")
                                    .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                                   .Elements("actions")
                                   .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
                                   .Elements("images")
                                   .Elements("image")
                               select new
                               {
                                   anumber = query.Element("number").Value,
                                };

                    OfflineData.picCount = -1;
                    foreach (var read in data)
                    {

                        OfflineData.picCount =Convert.ToInt32(read.anumber);
                       
                    }

                    OfflineData.picCount++;
                }
                catch
                {
                    OfflineData.picCount = 0;
                   
                 }
               
            }
            catch
            {
                OfflineData.picCount = 0;
            }
            //action box changed
          //  msg.show(OfflineData.picCount.ToString());

        }

        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {
            try{
                if(OfflineData.menuClick == 3)
                {
                    OfflineData.profileflag = 0;
                    this.Frame.Navigate(typeof(Camera_Page));
                }
                else
                {
                    OfflineData.profileflag = 1;
                    this.Frame.Navigate(typeof(editphotos));
                }
                
            }
            catch
            {
                msg.show("Fields cannot be empty");
            }
        }
        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }
       

    }
}
