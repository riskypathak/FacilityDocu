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
            this.Frame.Navigate(typeof(MainPage));
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
            editPhotos_DetailsText.IsReadOnly = true;
            editdetails_btn.Visibility = Visibility.Visible;
            okdetails_btn.Visibility = Visibility.Collapsed;

        }

        public async void page_load(int selectIndex)
        {
            projectInnfo();
            try
            {

                projectID.Text = tProjectID;
             
                datetime.Text = tCreationDate;
                projectname.Text = tProjectName;
                rigtype.Text = "RIG " + tRigType;
                category.Text = tCategoryName;
                subcategory.Text =tStepName;
                //rigtypeID=rigtypeD[0];
                // description.Text = projectD[1];
                //  editPhotos_DetailsText.Text = OfflineData.Detail[id];
                // created by.Text = projectD[3];

                //imageID.Text = projectD[7];

            //    int id = OfflineData.ImageID[selectIndex];
                BitmapImage bitmapImage = new BitmapImage();

                StorageFile imgFile = await KnownFolders.PicturesLibrary.GetFileAsync("mohan\\0.2001.3001.4001\\image"+(editpage_listbox.SelectedIndex+sel).ToString()+".png");

                using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap


                    await bitmapImage.SetSourceAsync(fileStream);



                    displayimage.Source = bitmapImage;
                }

            //    //  imagedetailID.Text = imageD[0];
            //    //  tag.Text = imageD[1];
            //    comment_txt.Text = OfflineData.Comment[id];
            //    //  imageID.Text = imageD[3];
            //    //datetime.Text = imageD[4];
          
            //    // imageID = rigtypeD[0];

            }
            catch
            {

                //   msg.show("Problem to loading data ! ");
            }
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

           


            startload();

        }
        List<int> imageCount = new List<int>();
        List<string> folderName = new List<string>();
        string tProjectID;
        string tProjectName;
        string tCreatedBy;
        string tCreationDate;
        string tRigType;
        string tCategoryName;
        string tCategoryno;
        string tStepno;
        string tStepName;
        string tActionno;
        string tActionNmae;


        public void projectInnfo()
        {
            
                string DBXMLPath = System.IO.Path.Combine(OfflineData.xmlp, "\\" + folderName[0] + ".xml");
                XDocument loadedData = XDocument.Load(Package.Current.InstalledLocation.Path+ "/Assets/a.xml");
                var data = from query in loadedData.Descendants("project")
                           select new
                           {
                               id = query.Element("id").Value,
                               name = query.Element("name").Value,
                               createdby = query.Element("createdby").Value,
                               creationDate=query.Element("creationdate").Value,
                             
                           };
                foreach (var read in data)
                {
                    tProjectID = read.id;
                    tProjectName = read.name;
                    tCreatedBy = read.createdby;
                    tCreationDate = read.creationDate;
                }


                var data1 = from query in loadedData.Descendants("project").Elements("rig")
                            select new
                            {
                                rigtype = query.Attribute("type").Value,
                            };
                foreach (var read in data1)
                {
                  
                    tRigType = read.rigtype;

                }
                var data2 = from query in loadedData.Descendants("project").Elements("rig").Elements("category")
                            select new
                            {
                                categoryname = query.Element("name").Value,
                                categorynumber =query.Element("number").Value,
                              
                            };
                foreach (var read in data2)
                {

                    //tRigType = read.rigtype;
                    tCategoryName = read.categoryname;
                    tCategoryno = read.categorynumber;

                }

                var data3 = from query in loadedData.Descendants("project").Elements("rig").Elements("category")
                            .Elements("step")
                            select new
                            {
                                stepname = query.Element("name").Value,
                                stepnumber = query.Element("number").Value,

                            };
                foreach (var read in data3)
                {

                    //tRigType = read.rigtype;
                    tStepName = read.stepname;
                    tStepno= read.stepnumber;

                }
                var data4 = from query in loadedData.Descendants("project").Elements("rig").Elements("category")
                               .Elements("step").Elements("action")
                            select new
                            {
                                actionname = query.Element("name").Value,
                                actionnumber = query.Element("number").Value,

                            };
                foreach (var read in data4)
                {

                    //tRigType = read.rigtype;
                    tActionNmae = read.actionname;
                    tActionno = read.actionnumber;

                }

                try { 

            }
            catch
            {
                msg.show("asdasdasd");
            }
        }
        public async void startload()
        {
           
                string DBXMLPath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "Assets/MyProjectList.xml");
                XDocument loadedData = XDocument.Load(DBXMLPath);

                         var data = from query in loadedData.Descendants("myproject")
                           select new
                           {
                             folderName=query.Element("name").Value ,
                             imageCount = query.Element("contain").Value,
                             
                           };
               
                foreach (var read in data)
                    {
                        imageCount.Add(Convert.ToInt32(read.imageCount));
                        folderName.Add(read.folderName);
               
                     }
               
                viewList(0);
                page_load(0);
                try
                {

            }
            catch
            {
                msg.show("File Not Found or Error Loading File Please Try Again Later");
            }


            
            
        }
        List<BitmapImage> imgstore = new List<BitmapImage>();
        public async void viewList(int index)
        {
            BitmapImage bitmapImage = new BitmapImage();
             StorageFolder sf ;
             Image ib=new Image();
             try
             {
               //sf= await StorageFolder.GetFolderFromPathAsync(OfflineData.datap+"\\0.2001.3001.4001");
               for (int i = 0; i < imageCount[index]; i++)
               {
                   StorageFile imgfile = await KnownFolders.PicturesLibrary.GetFileAsync("mohan\\0.2001.3001.4001\\image"+i.ToString()+".png");
                   using (IRandomAccessStream fileStream = await imgfile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                   {
                       ib = new Image();
                       bitmapImage = new BitmapImage();
                       await bitmapImage.SetSourceAsync(fileStream);
                       imgstore.Add(bitmapImage);
                       ib.Source = bitmapImage;
                       editpage_listbox.Items.Add(ib);
                      
                       
                   }
               }

               
            }
            catch
            {

            }
           
        }
        private async void editphotos_updateBtn_Click(object sender, RoutedEventArgs e)
        {
            //////////////////////////////////////
            //ServiceReference1.Imagesclass objimageclass = new ServiceReference1.Imagesclass();
            //objimageclass.getDetail = editPhotos_DetailsText.Text;
            //await MyService.UpdateImageAsync(objimageclass,14); 

        }




        string TempRigType = "";
        int countchange = 0;
        List<string> querySave;
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
            rig.Text = rigtype.Text;
            cat.Text = category.Text;
            subcat.Text = subcategory.Text;
            dchange.Text = editPhotos_DetailsText.Text;
            detailsGrid.Visibility = Visibility.Visible;
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

        private async void editpage_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             page_load(0);
           // page_load(editpage_listbox.SelectedIndex);


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



        private void rld_Tapped(object sender, TappedRoutedEventArgs e)
        {

            editpage_listbox.Items.Clear();
            startload();


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

    }
}
