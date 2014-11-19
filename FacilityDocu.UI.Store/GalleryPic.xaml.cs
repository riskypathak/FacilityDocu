using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tablet_App.ServiceReference1;
using Windows.ApplicationModel.Search;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
    public enum SelectionMode
    {
        RigType,
        Module,
        Step,
        Action
    }
    public sealed partial class Gallery : Page
    {
        private SelectionMode selectionMode;
        public IList<ImageModel> Images;
        private ImageDTO currentImage;
        private FileOpenPicker openPicker = new FileOpenPicker();
        private IEnumerable<ImageDTO> allImages = new List<ImageDTO>();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Data.CURRENT_RIG != null && Data.CURRENT_MODULE == null)
            {
                selectionMode = SelectionMode.RigType;
                allImages = Data.CURRENT_RIG.Modules.SelectMany(m => m.Steps).SelectMany(s => s.Actions)
                    .SelectMany(a => a.Images.Where(i => !string.IsNullOrEmpty(i.Path)));
                if (allImages.Count() <= 0)
                {
                    ScreenMessage.Show("No images for this rig. \n Please select another rig/project");
                    this.Frame.Navigate(typeof(ActionSelect));
                    return;
                }
            }
            else if (Data.CURRENT_MODULE != null && Data.CURRENT_STEP == null)
            {
                selectionMode = SelectionMode.Module;
                allImages = Data.CURRENT_MODULE.Steps.SelectMany(s => s.Actions)
                    .SelectMany(a => a.Images.Where(i => !string.IsNullOrEmpty(i.Path)));
                if (allImages.Count() <= 0)
                {
                    ScreenMessage.Show("No images for this module. \n Please select another module");
                    this.Frame.Navigate(typeof(ActionSelect));
                    return;
                }
            }
            else if (Data.CURRENT_STEP != null && Data.CURRENT_ACTION == null)
            {
                selectionMode = SelectionMode.Step;
                allImages = Data.CURRENT_STEP.Actions
                    .SelectMany(a => a.Images.Where(i => !string.IsNullOrEmpty(i.Path)));
                if (allImages.Count() <= 0)
                {
                    ScreenMessage.Show("No images for this step. \n Please select another step");
                    this.Frame.Navigate(typeof(ActionSelect));
                    return;
                }
            }
            else if (Data.CURRENT_ACTION != null)
            {
                selectionMode = SelectionMode.Action;
                allImages = Data.CURRENT_ACTION.Images.Where(i => !string.IsNullOrEmpty(i.Path));
                if (allImages.Count() <= 0)
                {
                    ScreenMessage.Show("No images for this action. \n Please select another action");
                    this.Frame.Navigate(typeof(ActionSelect));
                    return;
                }
            }
            if (Data.MODIFYIMAGE != null)
            {
                currentImage = Data.MODIFYIMAGE;
                Data.MODIFYIMAGE = null;
            }
            else
            {
                currentImage = allImages.First();
            }
            SetData(currentImage.ImageID);
            ChangeScreenControls();
        }

        private async void ShowImages()
        {
            Images = new List<ImageModel>();
            foreach (ImageDTO image in allImages)
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

        private async void btnBack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog msgDialog = new MessageDialog("There might be some unsaved changes.\nDo you want to move back without saving?", "FacilityDocu");
            //OK Button
            UICommand okBtn = new UICommand("Yes");
            okBtn.Invoked = OkBtn_Back_Click;
            msgDialog.Commands.Add(okBtn);
            //Cancel Button
            UICommand cancelBtn = new UICommand("No");
            msgDialog.Commands.Add(cancelBtn);
            //Show message
            await msgDialog.ShowAsync();
        }

        private async void OkBtn_Back_Click(IUICommand command)
        {
            this.Frame.Navigate(typeof(ActionSelect));
        }

        private void lstAllImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string selectedImageID = (e.AddedItems[0] as ImageModel).ImageID;
                currentImage = allImages.Single(i => i.ImageID == selectedImageID);
                SetData(selectedImageID);
                ChangeScreenControls();
            }
        }

        private static void SetData(string selectedImageID)
        {
            Data.CURRENT_MODULE = Data.CURRENT_RIG.Modules.Single(m => m.Steps.Any(s => s.Actions.Any(a => a.Images.Any(i => i.ImageID == selectedImageID))));
            Data.CURRENT_STEP = Data.CURRENT_MODULE.Steps.Single(s => s.Actions.Any(a => a.Images.Any(i => i.ImageID == selectedImageID)));
            Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions.Single(a => a.Images.Any(i => i.ImageID == selectedImageID));
        }

        public async Task ChangeScreenControls()
        {
            ShowImages();
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
        {
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
                    ScreenMessage.Show(srchSearch.QueryText + " > No Data Items Match");
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
            MessageDialog msgDialog = new MessageDialog("Do you really want to delete image?\nThis will remove image and its properties from action.", "FacilityDocu");
            //OK Button
            UICommand okBtn = new UICommand("Yes");
            okBtn.Invoked = OkBtn_Delete_Click;
            msgDialog.Commands.Add(okBtn);
            //Cancel Button
            UICommand cancelBtn = new UICommand("No");
            msgDialog.Commands.Add(cancelBtn);
            //Show message
            await msgDialog.ShowAsync();
        }

        private async void OkBtn_Delete_Click(IUICommand command)
        {
            Data.CURRENT_ACTION.Images.Remove(currentImage);
            if (allImages.Count() <= 0)
            {
                ScreenMessage.Show("No images now. \n Please select another action");
                this.Frame.Navigate(typeof(ActionSelect));
            }
            else
            {
                currentImage = allImages.First();
                await ChangeScreenControls();
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
            if (selectionMode == SelectionMode.RigType)
            {
                Data.CURRENT_MODULE = null;
            }
            else if (selectionMode == SelectionMode.Module)
            {
                Data.CURRENT_MODULE = null;
                Data.CURRENT_STEP = null;
            }
            else if (selectionMode == SelectionMode.Step)
            {
                Data.CURRENT_MODULE = null;
                Data.CURRENT_STEP = null;
                Data.CURRENT_ACTION = null;
            }
            Data.MODIFYIMAGE = currentImage;
            this.Frame.Navigate(typeof(EditPhoto));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgDialog = new MessageDialog("Do you really want to reset this image to its original?", "FacilityDocu");
            //OK Button
            UICommand okBtn = new UICommand("Yes");
            okBtn.Invoked = OkBtn_Reset_Click;
            msgDialog.Commands.Add(okBtn);
            //Cancel Button
            UICommand cancelBtn = new UICommand("No");
            msgDialog.Commands.Add(cancelBtn);
            //Show message
            await msgDialog.ShowAsync();
        }

        private async void OkBtn_Reset_Click(IUICommand command)
        {
            StorageFile modifiedFile = await StorageFile.GetFileFromPathAsync(Path.Combine(Data.ImagesPath, string.Format("{0}.jpg", currentImage.ImageID)));
            StorageFile originalFile = await StorageFile.GetFileFromPathAsync(Path.Combine(Data.ImagesPath, string.Format("{0}.jpg_org", currentImage.ImageID)));
            await originalFile.CopyAndReplaceAsync(modifiedFile);
            ChangeScreenControls();
        }

        private void btnSaveClose_Click(object sender, RoutedEventArgs e)
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}