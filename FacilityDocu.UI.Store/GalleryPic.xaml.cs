using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Tablet_App.ServiceReference1;
using Windows.ApplicationModel.Search;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{
    public class ImageModel
    {
        public string ImageID { get; set; }
        public BitmapImage Image { get; set; }
        public string Description { get; set; }
    }

    public sealed partial class Gallery : Page
    {
        public IList<ImageModel> Images;

        ImageDTO currentImage;

        FileOpenPicker openPicker = new FileOpenPicker();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Data.CURRENT_ACTION.Images.Count <= 0)
            {
                ScreenMessage.Show("No images for this action. \n Please select another action");
                this.Frame.Navigate(typeof(ActionSelect));
            }
            else
            {
                currentImage = Data.CURRENT_ACTION.Images.First();
                ChangeScreenControls();
            }
        }

        private async void ShowImages()
        {
            Images = new List<ImageModel>();
            foreach (ImageDTO image in Data.CURRENT_ACTION.Images)
            {
                BitmapImage bitmapImage = new BitmapImage();

                StorageFile imgFile = await StorageFile.GetFileFromPathAsync(image.Path);

                using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(fileStream);

                    Images.Add(new ImageModel() { ImageID = image.ImageID, Image = bitmapImage, Description = image.Description });
                }
            }

            lstAllImages.ItemsSource = Images;
        }

        public Gallery()
        {
            this.InitializeComponent();

            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".jpg");
        }

        private void btnBack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ActionSelect));
        }

        private void lstAllImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string selectedImageID = (e.AddedItems[0] as ImageModel).ImageID;
                currentImage = Data.CURRENT_ACTION.Images.Single(i => i.ImageID == selectedImageID);
                ChangeScreenControls();
            }
        }

        public async void ChangeScreenControls()
        {
            BitmapImage bitmapImage = new BitmapImage();
            StorageFile imgFile = await StorageFile.GetFileFromPathAsync(currentImage.Path);

            using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                await bitmapImage.SetSourceAsync(fileStream);
                imgDisplayMain.Source = bitmapImage;
            }

            txtProjectName.Text = Data.CURRENT_PROJECT.Description;
            txtRigType.Text = Data.CURRENT_RIG.Name;
            txtModule.Text = Data.CURRENT_MODULE.Name;
            txtStep.Text = Data.CURRENT_STEP.Name;
            txtImageCreationDate.Text = currentImage.CreationDate.ToString();
            txtActionName.Text = Data.CURRENT_ACTION.Name;
            txtDescriptionEdit.Text = Data.CURRENT_ACTION.Description;

            txtImageName.Text = string.Format("{0}.jpg", currentImage.ImageID);
            txtImageDescription.Text = currentImage.Description;

            txtPICId.Text = currentImage.Number;

            BasicProperties pro = await imgFile.GetBasicPropertiesAsync();
            txtImageSize.Text = string.Format("{0} bytes", pro.Size.ToString());

            txtImageResolution.Text = bitmapImage.PixelWidth.ToString() + " X " + bitmapImage.PixelHeight.ToString();

            lstComments.ItemsSource = currentImage.Comments;

            ShowImages();
        }

        private void btnEditImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdvEditImage.Visibility = Visibility.Visible;
            txtEditComment.Text = string.Empty;
            txtDescriptionEdit.Text = currentImage.Description;

            if (currentImage.Tags != null && currentImage.Tags.Count > 0)
            {
                txtEditTag.Text = string.Join(";", currentImage.Tags.ToArray());
            }
        }

        public IEnumerable<string> searchdata()
        geImge{
            IEnumerable<string> suggestionList;

            suggestionList = Data.CURRENT_ACTION.Images.Select(i => i.Description);

            return suggestionList;
        }

        int i = 0;

        private async void srchSearch_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs e)
        {
            if (!string.IsNullOrEmpty(srchSearch.QueryText))
            {
                foreach (string suggestion in searchdata())
                {
                    SearchSuggestionCollection suggestionCollection = e.Request.SearchSuggestionCollection;

                    if (suggestion.Contains(srchSearch.QueryText))
                    {
                        suggestionCollection.AppendQuerySuggestion(suggestion);
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

        private async void srchSearch_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (i == 1)
                {
                    Images = new List<ImageModel>();
                    foreach (ImageDTO image in Data.CURRENT_ACTION.Images.Where(ii => ii.Description.Contains(srchSearch.QueryText)))
                    {
                        BitmapImage bitmapImage = new BitmapImage();

                        StorageFile imgFile = await StorageFile.GetFileFromPathAsync(image.Path);

                        using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            await bitmapImage.SetSourceAsync(fileStream);

                            Images.Add(new ImageModel() { ImageID = image.ImageID, Image = bitmapImage, Description = image.Description });
                        }
                    }
                    // lstAllImages.Items.Clear();
                    lstAllImages.ItemsSource = Images;
                    //ShowPage(ii);
                }
                else
                {
                    ScreenMessage.Show(srchSearch.QueryText + "  >  No Data Items Match");
                    srchSearch.QueryText = "";
                    rctSearch.Visibility = Visibility.Visible;
                    srchSearch.IsEnabled = false;
                    //ShowPage(o);
                }
            }
            catch
            {

            }

        }

        private void rctSearch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            srchSearch.IsEnabled = true;

            rctSearch.Visibility = Visibility.Collapsed;
        }

        private async void btnDeleteImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Data.CURRENT_ACTION.Images.Remove(currentImage);



            if (Data.CURRENT_ACTION.Images.Count <= 0)
            {
                ScreenMessage.Show("No images for this action. \n Please select another action");
                this.Frame.Navigate(typeof(ActionSelect));
            }
            else
            {
                currentImage = Data.CURRENT_ACTION.Images.First();
                ChangeScreenControls();
            }
        }

        private void btnCancelEdit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdvEditImage.Visibility = Visibility.Collapsed;
        }

        private async void btnSaveEdit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            currentImage.Description = txtDescriptionEdit.Text;
            currentImage.Tags = new System.Collections.ObjectModel.ObservableCollection<string>();

            foreach (string tag in txtEditTag.Text.Split(';').ToList())
            {
                currentImage.Tags.Add(tag);
            }

            if (!string.IsNullOrEmpty(txtEditComment.Text.Trim()))
            {
                string userName = await Data.GetUserName();

                if (currentImage.Comments == null)
                {
                    currentImage.Comments = new System.Collections.ObjectModel.ObservableCollection<CommentDTO>();
                }

                currentImage.Comments.Add(new CommentDTO() { CommentedAt = DateTime.Now, CommentID = DateTime.Now.ToString("yyyyMMddHHmmssfff"), Text = txtEditComment.Text.Trim(), User = userName.ToString() });
            }

            ChangeScreenControls();

            gdvEditImage.Visibility = Visibility.Collapsed;
        }

        private void btnModifyImage_Click(object sender, RoutedEventArgs e)
        {
            Data.MODIFYIMAGE = currentImage;
            this.Frame.Navigate(typeof(EditPhoto));
        }

        private async void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
            Data.CURRENT_PROJECT = await (new SyncManager()).UploadImages(Data.CURRENT_PROJECT.ProjectID);
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
        }

        private void btnSaveNext_Click(object sender, RoutedEventArgs e)
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);

            int currentActionIdex = Data.CURRENT_STEP.Actions.IndexOf(Data.CURRENT_ACTION);

            if (currentActionIdex == Data.CURRENT_STEP.Actions.Count - 1)
            {
                int currentStepIdex = Data.CURRENT_MODULE.Steps.IndexOf(Data.CURRENT_STEP);

                if (currentStepIdex == Data.CURRENT_MODULE.Steps.Count - 1)
                {
                    int currentModuleIdex = Data.CURRENT_RIG.Modules.IndexOf(Data.CURRENT_MODULE);

                    if (currentModuleIdex == Data.CURRENT_RIG.Modules.Count - 1)
                    {
                        int currentRigIndex = Data.CURRENT_PROJECT.RigTypes.IndexOf(Data.CURRENT_RIG);

                        if (currentRigIndex == Data.CURRENT_PROJECT.RigTypes.Count - 1)
                        {
                            ScreenMessage.Show("No more actions :)");
                        }
                        else
                        {
                            Data.CURRENT_RIG = Data.CURRENT_PROJECT.RigTypes[currentRigIndex + 1];
                            Data.CURRENT_MODULE = Data.CURRENT_RIG.Modules[0];
                            Data.CURRENT_STEP = Data.CURRENT_MODULE.Steps[0];
                            Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[0];
                        }
                    }
                    else
                    {
                        Data.CURRENT_MODULE = Data.CURRENT_RIG.Modules[currentModuleIdex + 1];
                        Data.CURRENT_STEP = Data.CURRENT_MODULE.Steps[0];
                        Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[0];
                    }
                }
                else
                {
                    Data.CURRENT_STEP = Data.CURRENT_MODULE.Steps[currentStepIdex + 1];
                    Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[0];
                }
            }
            else
            {
                Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[currentActionIdex + 1];
            }
            ChangeScreenControls();
        }

        private void btnSaveClose_Click(object sender, RoutedEventArgs e)
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
