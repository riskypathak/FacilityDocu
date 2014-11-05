using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Tablet_App.ServiceReference1;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Search;
using System.Xml.Linq;
using Windows.ApplicationModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class editphotos : Page
    {

        FileOpenPicker openPicker = new FileOpenPicker();
        //  ServiceReference1.Service1Client MyService = new ServiceReference1.Service1Client();
        public editphotos()
        {
            this.InitializeComponent();


            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

        }


        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                selectedphoto.Text = "Picked photo: " + file.Name;
            }
            else
            {
                selectedphoto.Text = "Operation cancelled.";
            }
        }
        private void okdetails_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //editPhotos_DetailsText.IsReadOnly = true;
            editdetails_btn.Visibility = Visibility.Visible;
            okdetails_btn.Visibility = Visibility.Collapsed;

        }
       
        private void rigtype_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void category_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void subcategory_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void editdetails_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //rig.Text = rigtype.Text;
            //cat.Text = category.Text;
            //subcat.Text = subcategory.Text;
           // dchange.Text = editPhotos_DetailsText.Text;
           // detailsGrid.Visibility = Visibility.Visible;
        }

        private void comment_txt_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Tag_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        int sel = 0;

        private async void delete_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {

            try
            {
            editpage_listbox.Items.RemoveAt(editpage_listbox.SelectedIndex);
            sel++;
             //StorageFolder d = await StorageFolder.GetFolderFromPathAsync(OfflineData.datap);

             //StorageFile s = await d.GetFileAsync(OfflineData.datap+"\\0.2001.3001.4001\\image"+editpage_listbox.SelectedIndex+".png");
             //await s.DeleteAsync();
             //startload();
            
            }
            catch
            {
                prb.Visibility = Visibility.Visible;

            }

        }

        private void editphotos_updateBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        public async void QuerySend(string query)
        {
            await OfflineData.MyService.QueryExeAsync(query);
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            detailsGrid.Visibility = Visibility.Collapsed;
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            //for (int i = 0; i < OfflineData.ImageID.Count; i++)
            //{
            //    suggestionList[i] = OfflineData.ImageID[i].ToString() + OfflineData.subcategory[i];
            //}
            ////string qry="Update Images set Detail = '"+dchange.Text+"' where ImageID= " +idval[Convert.ToInt32(editpage_listbox.SelectedIndex.ToString())];
            ////QuerySend(qry);
            //for (int i = 0; i < OfflineData.ImageID.Count; i++)
            //{
            //    if (OfflineData.ImageID[i] == editpage_listbox.SelectedIndex)
            //        OfflineData.Detail[i] = dchange.Text;
            //}
            //detailsGrid.Visibility = Visibility.Collapsed;
            //editPhotos_DetailsText.Text = dchange.Text;
        }
        public static editphotos Current;
        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {

        }

        

        private void srch_QueryChanged(SearchBox sender, SearchBoxQueryChangedEventArgs e)
        {
        }

        public string[] searchdata()
        {
            string[] suggestionList=new string[50];

            //suggestionList = new string[OfflineData.ImageID.Count];
            //for (int i = 0; i < OfflineData.ImageID.Count; i++)
            //{
            //    suggestionList[i] = OfflineData.ImageID[i].ToString() + " " + ":" + " " + OfflineData.Action[i];
            //}

            return suggestionList;
        }
        private static readonly string[] suggestionList = new string[1200];
        int i = 0;
        private async void srch_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(srch.QueryText))
                {
                    foreach (string suggestion in searchdata())
                    {
                        SearchSuggestionCollection suggestionCollection = e.Request.SearchSuggestionCollection;

                        if (suggestion.StartsWith(srch.QueryText, StringComparison.CurrentCultureIgnoreCase))
                        {

                            suggestionCollection.AppendQuerySuggestion(suggestion);
                            // suggestionCollection.AppendQuerySuggestion(OfflineData.ImagePath[0]);
                        }
                    }
                }

                if (e.Request.SearchSuggestionCollection.Size > 0)
                {
                    i = 1;
                }
                else
                {
                    i = 0;

                }
            }
            catch
            {

            }
        }

        private void srch_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (i == 1)
                {
                    string[] idg = srch.QueryText.Split(':');
                    int ii = Convert.ToInt32(idg[0]);
                    editpage_listbox.SelectedIndex = ii;
                    page_load(ii);
                }
                else
                {

                    msg.show(srch.QueryText + "  >  No Data Items Match");
                    srch.QueryText = "";
                    t.Visibility = Visibility.Visible;
                    srch.IsEnabled = false;
                    page_load(0);
                }
            }
            catch
            {

            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tx.Text = "";
            tg.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            tx.Text = "";
            tg.Visibility = Visibility.Collapsed;
        }

        private void Tag_Click(object sender, RoutedEventArgs e)
        {
            tg.Visibility = Visibility.Visible;
        }

        private void srch_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void t_Tapped(object sender, TappedRoutedEventArgs e)
        {
            srch.IsEnabled = true;

            t.Visibility = Visibility.Collapsed;
        }




        private void back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            prb.Visibility = Visibility.Collapsed;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void TextBlock_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            OfflineData.editpic = imgstore[editpage_listbox.SelectedIndex+sel];
            this.Frame.Navigate(typeof(edit));
        }

       
        List<string> tid = new List<string>();
        List<string> tnumber = new List<string>();
        List<string> tname = new List<string>();
        List<string> ttag = new List<string>();
        List<string> tdescription = new List<string>();
        string picSize = "";
        string picresol = "";
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            OfflineData.picCount = 0;
            imgview();

        } 
        public void imgview()
        {
            try
            {
                string DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\"+ OfflineData.projectXml + ".xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);

                var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                    .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                 .Elements("modules")
                                 .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                                  .Elements("steps")
                                  .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                                 .Elements("actions")
                                 .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
                                 .Elements("images").Elements("image")
                           select new
                           {
                               id = query.Element("id").Value,
                               number = query.Element("number").Value,
                               name = query.Element("name").Value,
                               tag = query.Element("tags").Value,
                               dec = query.Element("description").Value,
                           };

                foreach (var read in data)
                {

                    tid.Add(read.id);
                    tnumber.Add(read.number);
                    ttag.Add(read.tag);
                    tdescription.Add(read.dec);
                    tname.Add(read.name);


                }
                viewList();
               // msg.show(tname.Count.ToString());
               
            }
            catch
            {
                msg.show("No Project Found");
                this.Frame.GoBack();
            }
        }
        public void refpage()
        {
            editpage_listbox.Items.Clear();
           
            commentview.Items.Clear();
            tid.Clear();
            tnumber.Clear();
            tname.Clear();
            ttag.Clear();
            tdescription.Clear();

            imgview();
        }
        public void showdetails(int index)
        {
            projectIDTxt.Text = OfflineData.tempProjectID;
            rigtypeTxt.Text = OfflineData.tempRigType;
            moduleTxt.Text = OfflineData.tempModuleName;
            stepTxt.Text = OfflineData.tempStepName;
            date.Text = OfflineData.tempProjectCreatedDate;

            imageNameTxt.Text=tname[index];
            descriptionTxt.Text = tdescription[index];
            pID.Text = tid[index];
            pSize.Text = picSize;
            resol.Text = picresol;
            loadComment(index);
        }
        private void editpage_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            page_load(editpage_listbox.SelectedIndex);

        }
        public async void page_load(int selectIndex)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();

                StorageFile imgFile = await StorageFile.GetFileFromPathAsync(OfflineData.datap + "\\" + OfflineData.folderName + "\\"+ tname[selectIndex]+".png");

                using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(fileStream);

                    displayimage.Source = bitmapImage;
                    picresol = bitmapImage.PixelWidth.ToString() + " X " + bitmapImage.PixelHeight.ToString();
                    //Size a = new Size(4,5);
                    // picSize  =   a.ToString();
                    showdetails(selectIndex);
                }

               
            }
            catch
            {

              //   msg.show("Problem to loading data ! ");
            }
        }
      
        List<BitmapImage> imgstore = new List<BitmapImage>();
        public async void viewList()
        {
            BitmapImage bitmapImage = new BitmapImage();
            StorageFolder sf;
            Image ib = new Image();
         
            try
            {
                sf = await StorageFolder.GetFolderFromPathAsync(OfflineData.datap + "\\"+ OfflineData.folderName);
                for (int i = 0; i < tname.Count; i++)
                {
                    StorageFile imgfile = await sf.GetFileAsync(tname[i] + ".png");
                    using (IRandomAccessStream fileStream = await imgfile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                    {
                        ib = new Image();
                        bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(fileStream);
                        imgstore.Add(bitmapImage);
                        ib.Source = bitmapImage;

                    }
                    editpage_listbox.Items.Add(ib);
                   
                }
                editpage_listbox.SelectedIndex = 0;
                page_load(0);

            }
            catch
            {

            }

        }
        public void loadComment(int index)
        {
            TextBlock txt = new TextBlock();
            txt.Width = 526;
            string DBXMLPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
            XDocument loadedData = XDocument.Load(DBXMLPath);

            var data = from query in loadedData.Descendants("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                             .Elements("modules")
                             .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                              .Elements("steps")
                              .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                             .Elements("actions")
                             .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
                             .Elements("images").Elements("image").Where(query => (string)query.Element("name").Value == tname[index])
                              .Elements("comments").Elements("comment")
                       select new
                       {
                           tdate = query.Element("date").Value,
                           tuser = query.Element("user").Value,
                           ttext = query.Element("text").Value,
                          
                       };

            commentview.Items.Clear();

            foreach (var read in data)
            {
                txt.Text = "-------------------------------------------------\n" + read.tuser + "\n" + read.ttext + "\n" + read.tdate;
                commentview.Items.Add(txt.Text);

            }
        }
        private void popen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(add_details));
        }
        private async void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            try
            {

                string testXML = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data\\FacilityDocu\\Data\\" + OfflineData.projectXml + ".xml");
                XDocument loadedData = XDocument.Load(testXML);

                loadedData.Elements("project").Where(query => query.Element("name").Value == OfflineData.tempProjectName)
                                   .Elements("rigs").Elements("rig").Where(query => (string)query.Attribute("type") == OfflineData.tempRigType)
                                .Elements("modules")
                                .Elements("module").Where(query => (string)query.Element("name").Value == OfflineData.tempModuleName)
                                 .Elements("steps")
                                 .Elements("step").Where(query => (string)query.Element("name").Value == OfflineData.tempStepName)
                                .Elements("actions")
                                .Elements("action").Where(query => (string)query.Element("name").Value == OfflineData.tempActionName)
                                .Elements("images").Elements("image").Where(query => (string)query.Element("name").Value == tname[editpage_listbox.SelectedIndex])
                              .Last().Remove();
               
                StorageFolder xmlPath = await ApplicationData.Current.LocalFolder.GetFolderAsync("data\\FacilityDocu\\Data");
                try
                {
                    StorageFile a = await xmlPath.GetFileAsync(OfflineData.projectXml + ".xml");
                    await a.DeleteAsync();
                }
                catch
                {

                }
                StorageFile xmlFile = await xmlPath.CreateFileAsync(OfflineData.projectXml + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await xmlFile.OpenStreamForWriteAsync())
                {
                    loadedData.Save(fileStream);
                }
                try
                {
                    StorageFile delf = await StorageFile.GetFileFromPathAsync(OfflineData.datap + "\\" + OfflineData.folderName + "\\" + tname[editpage_listbox.SelectedIndex] + ".png");
                    await delf.DeleteAsync();
                }
                catch
                {
                    msg.show("file not found");
                }
                refpage();
               
            }
            catch
            {

            }
           // imgview();
            //delete
        }
        private void Edit(object sender, TappedRoutedEventArgs e)
        {
//edit
            OfflineData.ides = tdescription[editpage_listbox.SelectedIndex];
            OfflineData.itag = ttag[editpage_listbox.SelectedIndex];
            OfflineData.picCount = Convert.ToInt32(tnumber[editpage_listbox.SelectedIndex]);

            OfflineData.profileflag = 1;
            OfflineData.pImage.Source = displayimage.Source;

            this.Frame.Navigate(typeof(PhotoDetails));
        }

        public void setvalue()
        {

        }
    }
}
