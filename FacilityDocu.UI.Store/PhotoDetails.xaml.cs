using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PhotoDetails : Page
    {
        public PhotoDetails()
        {
            this.InitializeComponent();
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
        int addi = 0;
        public async void addinxml()
        {


            int status = 1;
            try
            {
                string DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
                XDocument loadedData;
                loadedData = XDocument.Load(DBXMLPath);
                try
                {



                    var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                               .Elements("rigs").Elements("rig")
                               select new
                               {
                                   anumber = query.Attribute("type").Value,
                               };
                    foreach (var read in data)
                    {
                        if (read.anumber == OfflineData.tempRigType)
                        {
                            status = 0;
                        }


                    }
                    if (status == 1)
                    {
                        updateFromRig();
                        return;
                    }

                    status = 1;


                    try
                    {
                        var data1 = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                   .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                   .Elements("modules").Elements("module")
                                    select new
                                    {
                                        anumber = query.Element("name").Value,
                                    };
                        foreach (var read in data1)
                        {
                            if (read.anumber == OfflineData.tempModuleName)
                            {
                                status = 0;
                            }


                        }
                        if (status == 1)
                        {
                            updateFromModule();
                            return;
                        }
                        status = 1;
                        try
                        {
                            var data2 = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                    .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                    .Elements("modules").Elements("module").Where(query => (string)query.Element("name") == OfflineData.tempModuleName)
                                     .Elements("steps").Elements("step")

                                        select new
                                        {
                                            anumber = query.Element("name").Value,
                                        };
                            foreach (var read in data2)
                            {

                                if (read.anumber == OfflineData.tempStepName)
                                {
                                    status = 0;
                                }

                            }
                            if (status == 1)
                            {
                                updateFromStep();
                                return;
                            }
                            status = 1;
                            try
                            {
                                var data3 = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                                              .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                                              .Elements("modules").Elements("module").Where(query => (string)query.Element("name") == OfflineData.tempModuleName)
                                                               .Elements("steps").Elements("step").Where(query => (string)query.Element("name") == OfflineData.tempStepName)
                                                               .Elements("actions").Elements("action")
                                            select new
                                            {
                                                anumber = query.Element("name").Value,
                                            };
                                foreach (var read in data3)
                                {

                                    if (read.anumber == OfflineData.tempActionName)
                                    {
                                        status = 0;
                                    }

                                }
                                if (status == 1)
                                {
                                    updateFromAction();
                                    return;
                                }
                                status = 1;
                            }
                            catch
                            {

                                updateFromAction();
                            }
                        }
                        catch
                        {

                            updateFromStep();
                        }
                    }
                    catch
                    {

                        updateFromModule();
                    }
                }
                catch
                {

                    updateFromRig();
                }


            }
            catch
            {

                Save_Project();
            }



        }
        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {
            if (OfflineData.profileflag != 1)
            {
                addi = 0;
                if (OfflineData.picCount == 0)
                {
                    OfflineData.tempimage_no = OfflineData.picCount.ToString();
                    OfflineData.tempimageid = OfflineData.picCount.ToString();
                    OfflineData.imageName = "image" + OfflineData.picCount.ToString();
                    OfflineData.tempimagetag = tagtext.Text;
                    OfflineData.tempimagecomment = commenttext.Text;
                    OfflineData.tempimage_description = descriptiontext.Text;
                    addinxml();

                }
                else
                {
                    OfflineData.tempimagetag = tagtext.Text;
                    OfflineData.tempimage_description = descriptiontext.Text;
                    addImage();

                }
            }
            else
            {
                int indx = OfflineData.picCount;
                // addComment(indx.ToString());
                Update(indx.ToString());

            }



        }
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            addi = 1;
            //camera pge
            if (OfflineData.picCount == 0)
            {

                OfflineData.tempimage_no = OfflineData.picCount.ToString();
                OfflineData.tempimageid = OfflineData.picCount.ToString();
                OfflineData.imageName = "image" + OfflineData.picCount.ToString();
                OfflineData.tempimagetag = tagtext.Text;
                OfflineData.tempimagecomment = commenttext.Text;
                OfflineData.tempimage_description = descriptiontext.Text;
                addinxml();

            }
            else
            {
                OfflineData.tempimagetag = tagtext.Text;
                OfflineData.tempimage_description = descriptiontext.Text;
                addImage();

            }

            this.Frame.Navigate(typeof(Camera_Page));
        }
        public async void addImage()
        {
            DateTime dt = DateTime.Now;
            string dtt = dt.ToString("r");
            XDocument loadedData = new XDocument();
            XElement newComment;
            try
            {
                string testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
                loadedData = XDocument.Load(testXML);

                newComment = new XElement("image",
                                                 new XElement("id", OfflineData.picCount.ToString()),
                                                 new XElement("number", OfflineData.picCount.ToString()),
                                                 new XElement("name", "image" + OfflineData.picCount.ToString()),
                                                  new XElement("tags", OfflineData.tempimagetag),
                                                   new XElement("description", OfflineData.tempimage_description),
                                                    new XElement("comments")
                                             );

                loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                  .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
               .Elements("modules")
               .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                .Elements("steps")
                .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
               .Elements("actions")
               .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
               .Elements("images").Last().Add(newComment);

                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");

                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                if (commenttext.Text != "")
                {
                    addComment(OfflineData.picCount.ToString());
                }
                else
                {
                    OfflineData.picCount++;
                }
                if (addi != 1)
                {
                    msg.show("Successfully Saved");
                    this.Frame.Navigate(typeof(MainPage));
                }


            }
            catch
            {

            }

        }
        public async void addComment(string index)
        {
            DateTime dt = DateTime.Now;
            string dtt = dt.ToString("r");
            try
            {
                XDocument loadedData = new XDocument();
                XElement newComment;
                string testXML;

                testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
                loadedData = XDocument.Load(testXML);

                newComment = new XElement("comment",
                                                 new XElement("text", commenttext.Text),
                                                 new XElement("user", OfflineData.cuser),
                                                 new XElement("date", dtt)
                                             );

                loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
             .Elements("modules")
             .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
              .Elements("steps")
              .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
             .Elements("actions")
             .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
             .Elements("images").Elements("image").Where(query => (string)query.Element("name").Value == OfflineData.imageName)
            .Elements("comments").Last().Add(newComment);

                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");

                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                OfflineData.picCount++;
            }
            catch
            {

            }
        }
        public async void Update(string index)
        {
            DateTime dt = DateTime.Now;
            string dtt = dt.ToString("r");
            XDocument loadedData = new XDocument();
            XElement newdes;
            try
            {
                string testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
                loadedData = XDocument.Load(testXML);

                newdes = new XElement("description", descriptiontext.Text
                                             );

                loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                   .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                .Elements("modules")
                                .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                                 .Elements("steps")
                                 .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                                .Elements("actions")
                                .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
                                .Elements("images").Elements("image").Where(query => (string)query.Element("name").Value == OfflineData.imageName)
                              .Last().Element("description").ReplaceWith(newdes);


                if (commenttext.Text != "")
                {
                    XElement newComment1 = new XElement("comment",
                                                    new XElement("text", commenttext.Text),
                                                    new XElement("user", OfflineData.cuser),
                                                    new XElement("date", dtt)
                                                );

                    loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                    .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                 .Elements("modules")
                 .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                  .Elements("steps")
                  .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                 .Elements("actions")
                 .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
                 .Elements("images").Elements("image").Where(query => (string)query.Element("name").Value == OfflineData.imageName)
                .Elements("comments").Last().Add(newComment1);

                }

                XElement newtag = new XElement("tags", tagtext.Text
                                                );

                loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                    .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                 .Elements("modules")
                                 .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                                  .Elements("steps")
                                  .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                                 .Elements("actions")
                                 .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
                                 .Elements("images").Elements("image").Where(query => (string)query.Element("name").Value == OfflineData.imageName)
                                .Last().Element("tags").ReplaceWith(newtag);


                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");

                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                if (addi != 1)
                {
                    msg.show("Successfully Saved");
                    // this.Frame.Navigate(typeof(MainPage));
                }
            }
            catch
            {
                msg.show("Successfully  Saved");
            }
            this.Frame.GoBack();
        }
        public async void updateFromRig()
        {
            string testXML;
            testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
            XDocument loadedData = XDocument.Load(testXML);

            XElement rigup = new XElement("rig",
                           new XAttribute("type", OfflineData.tempRigType),

                           new XElement("modules",
                new XElement("module",
                  new XElement("id", OfflineData.tempModuleID),
                  new XElement("name", OfflineData.tempModuleName),
                  new XElement("number", OfflineData.tempModuleNo),

                  new XElement("steps",
                          new XElement("step",
                              new XElement("id", OfflineData.tempStepID),
                              new XElement("name", OfflineData.tempStepName),
                              new XElement("number", OfflineData.tempStepNo),

                              new XElement("actions",
                          new XElement("action",
                              new XElement("id", OfflineData.tempActionID),
                              new XElement("name", OfflineData.tempActionName),
                              new XElement("number", OfflineData.tempActionNo),
                              new XElement("description", OfflineData.tempActionDescription),

                              new XElement("images",
                                  new XElement("image",
                                      new XElement("id", OfflineData.tempimageid),
                                      new XElement("number", OfflineData.tempimage_no),
                                      new XElement("name", OfflineData.imageName),
                                      new XElement("tags", OfflineData.tempimagetag),
                                      new XElement("description", OfflineData.tempimage_description),
                                      new XElement("comments"


                                              )))))))
                                                       )));

            loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                              .Elements("rigs").Last().Add(rigup);
            try
            {
                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");

                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                if (commenttext.Text != "")
                {
                    addComment(OfflineData.picCount.ToString());
                }
                else
                {
                    OfflineData.picCount++;
                }
                if (addi != 1)
                {
                    msg.show("Successfully Saved");
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
            catch
            {

            }

        }
        public async void updateFromModule()
        {
            string testXML;
            testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
            XDocument loadedData = XDocument.Load(testXML);


            XElement moduleup = new XElement("module",
                   new XElement("id", OfflineData.tempModuleID),
                   new XElement("name", OfflineData.tempModuleName),
                   new XElement("number", OfflineData.tempModuleNo),

                   new XElement("steps",
                           new XElement("step",
                               new XElement("id", OfflineData.tempStepID),
                               new XElement("name", OfflineData.tempStepName),
                               new XElement("number", OfflineData.tempStepNo),

                               new XElement("actions",
                           new XElement("action",
                               new XElement("id", OfflineData.tempActionID),
                               new XElement("name", OfflineData.tempActionName),
                               new XElement("number", OfflineData.tempActionNo),
                               new XElement("description", OfflineData.tempActionDescription),

                               new XElement("images",
                                   new XElement("image",
                                       new XElement("id", OfflineData.tempimageid),
                                       new XElement("number", OfflineData.tempimage_no),
                                       new XElement("name", OfflineData.imageName),
                                       new XElement("tags", OfflineData.tempimagetag),
                                       new XElement("description", OfflineData.tempimage_description),
                                       new XElement("comments"


                                               )))))))
                                                        );

            loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                            .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                         .Elements("modules").Last().Add(moduleup);
            try
            {
                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");

                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                if (commenttext.Text != "")
                {
                    addComment(OfflineData.picCount.ToString());
                }
                else
                {
                    OfflineData.picCount++;
                }
                if (addi != 1)
                {
                    msg.show("Successfully Saved");
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
            catch
            {

            }
        }
        public async void updateFromStep()
        {
            string testXML;
            testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
            XDocument loadedData = XDocument.Load(testXML);



            XElement stepup = new XElement("step",
                            new XElement("id", OfflineData.tempStepID),
                            new XElement("name", OfflineData.tempStepName),
                            new XElement("number", OfflineData.tempStepNo),

                            new XElement("actions",
                        new XElement("action",
                            new XElement("id", OfflineData.tempActionID),
                            new XElement("name", OfflineData.tempActionName),
                            new XElement("number", OfflineData.tempActionNo),
                            new XElement("description", OfflineData.tempActionDescription),

                            new XElement("images",
                                new XElement("image",
                                    new XElement("id", OfflineData.tempimageid),
                                    new XElement("number", OfflineData.tempimage_no),
                                    new XElement("name", OfflineData.imageName),
                                    new XElement("tags", OfflineData.tempimagetag),
                                    new XElement("description", OfflineData.tempimage_description),
                                    new XElement("comments"


                                            ))))));

            loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                         .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                      .Elements("modules")
                                      .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                                       .Elements("steps").Last().Add(stepup);
            try
            {
                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");

                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                if (commenttext.Text != "")
                {
                    addComment(OfflineData.picCount.ToString());
                }
                else
                {
                    OfflineData.picCount++;
                }
                if (addi != 1)
                {
                    msg.show("Successfully Saved");
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
            catch
            {

            }
        }
        public async void updateFromAction()
        {
            string testXML;
            testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
            XDocument loadedData = XDocument.Load(testXML);

            XElement actionup = new XElement("action",
                   new XElement("id", OfflineData.tempActionID),
                   new XElement("name", OfflineData.tempActionName),
                   new XElement("number", OfflineData.tempActionNo),
                   new XElement("description", OfflineData.tempActionDescription),

                   new XElement("images",
                       new XElement("image",
                           new XElement("id", OfflineData.tempimageid),
                           new XElement("number", OfflineData.tempimage_no),
                           new XElement("name", OfflineData.imageName),
                           new XElement("tags", OfflineData.tempimagetag),
                           new XElement("description", OfflineData.tempimage_description),
                           new XElement("comments"


                                   ))));

            loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                 .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
              .Elements("modules")
              .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
               .Elements("steps")
               .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
              .Elements("actions").Last().Add(actionup);
            try
            {
                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");

                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                if (commenttext.Text != "")
                {
                    addComment(OfflineData.picCount.ToString());
                }
                else
                {
                    OfflineData.picCount++;
                }
                if (addi != 1)
                {
                    msg.show("Successfully Saved");
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
            catch
            {

            }
        }
        public async void Save_Project()
        {
            XDocument projectSave = new XDocument(
                new XElement("project",
                    new XElement("id", OfflineData.tempProjectID),
                    new XElement("name", OfflineData.tempProjectName),
                    new XElement("createdby", OfflineData.tempProjectCreatedBy),
                    new XElement("createdtime", OfflineData.tempProjectCreatedDate),
                    new XElement("updatetime", OfflineData.tempProjectUpdateDate),
                    new XElement("description", OfflineData.tempProjectDescription),

                    new XElement("rigs",
                    new XElement("rig",
                        new XAttribute("type", OfflineData.tempRigType),

                        new XElement("modules",
             new XElement("module",
               new XElement("id", OfflineData.tempModuleID),
               new XElement("name", OfflineData.tempModuleName),
               new XElement("number", OfflineData.tempModuleNo),

               new XElement("steps",
                       new XElement("step",
                           new XElement("id", OfflineData.tempStepID),
                           new XElement("name", OfflineData.tempStepName),
                           new XElement("number", OfflineData.tempStepNo),

                           new XElement("actions",
                       new XElement("action",
                           new XElement("id", OfflineData.tempActionID),
                           new XElement("name", OfflineData.tempActionName),
                           new XElement("number", OfflineData.tempActionNo),
                           new XElement("description", OfflineData.tempActionDescription),

                           new XElement("images",
                               new XElement("image",
                                   new XElement("id", OfflineData.tempimageid),
                                   new XElement("number", OfflineData.tempimage_no),
                                   new XElement("name", OfflineData.imageName),
                                   new XElement("tags", OfflineData.tempimagetag),
                                   new XElement("description", OfflineData.tempimage_description),
                                   new XElement("comments"


                                           )))))))
                                                    ))))));


            Windows.Storage.StorageFolder xmlFolderPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("Data\\FacilityDocu\\Data");

            StorageFile xmlFile = await xmlFolderPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);

            using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
            {
                projectSave.Save(fileStream);
            }
            if (commenttext.Text != "")
            {
                addComment(OfflineData.picCount.ToString());
            }
            else
            {
                OfflineData.picCount++;
            }
            if (addi != 1)
            {
                msg.show("Successfully Saved");
                this.Frame.Navigate(typeof(MainPage));
            }
            //  OfflineData.picCount++;
        }
        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (OfflineData.profileflag != 1)
            {
                img.Source = OfflineData.editpic;
                OfflineData.imageName = "image" + OfflineData.picCount.ToString();
            }
            else
            {
                OfflineData.imageName = "image" + OfflineData.picCount.ToString();

                aphoto.Visibility = Visibility.Collapsed;
                img.Source = OfflineData.pImage.Source;
                descriptiontext.Text = OfflineData.ides;
                tagtext.Text = OfflineData.itag;
            }
        }


    }
}
