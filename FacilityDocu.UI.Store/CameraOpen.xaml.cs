using System;
using Tablet_App.ServiceReference1;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.MediaProperties;
using System.Diagnostics;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 



    public sealed partial class Camera_Page : Page
    {
        MediaCapture media;
        ImageDTO currentImage;

        public Camera_Page()
        {
            this.InitializeComponent();
        }

        private void btnBack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            WriteImages();

            media.Dispose();
            this.Frame.Navigate(typeof(ActionSelect));
        }
        public async void SetResolution()
        {
            System.Collections.Generic.IReadOnlyList<IMediaEncodingProperties> res;
            res = this.media.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);
            uint maxResolution = 0;
            int indexMaxResolution = 0;

            if (res.Count >= 1)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    VideoEncodingProperties vp = (VideoEncodingProperties)res[i];

                    if (vp.Width > maxResolution)
                    {
                        indexMaxResolution = i;
                        maxResolution = vp.Width;
                       // Debug.WriteLine("Resolution: " + vp.Width);
                    }
                }
                await this.media.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, res[indexMaxResolution]);
            }
        }


        private async void capturePreview_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                StorageFolder imagesFolder = await StorageFolder.GetFolderFromPathAsync(Data.ImagesPath);

                string imageID = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = string.Format("{0}.jpg", imageID);
               // CameraCaptureUIMaxPhotoResolution.HighestAvailable ;
               
                Windows.Media.MediaProperties.ImageEncodingProperties imgProperties = Windows.Media.MediaProperties.ImageEncodingProperties.CreateJpeg();
                imgProperties.Height = 1024;
                imgProperties.Width = 1280;
          
                StorageFile file = await imagesFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await media.CapturePhotoToStorageFileAsync(imgProperties, file);

                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    preview_pic.Source = bitmapImage;
                    

                    currentImage = new ImageDTO()
                    {
                        CreationDate = DateTime.Now,
                        ImageID = imageID,
                        Path = System.IO.Path.Combine(Data.ImagesPath, string.Format("{0}.jpg", imageID)),
                        Tags = new System.Collections.ObjectModel.ObservableCollection<string>(),
                        Comments = new System.Collections.ObjectModel.ObservableCollection<CommentDTO>()
                    };
                }

                string originalFileName = string.Format("{0}.jpg_org", imageID);

                await file.CopyAsync(imagesFolder, originalFileName, NameCollisionOption.ReplaceExisting);

                IStorageFolder backupFolder = await StorageFolder.GetFolderFromPathAsync(Data.BackupPath);
                await file.CopyAsync(backupFolder, file.Name, NameCollisionOption.ReplaceExisting);
                
                //Copying original file too
                await file.CopyAsync(backupFolder, originalFileName, NameCollisionOption.ReplaceExisting);

                gdvPreview.Visibility = Visibility.Visible;
                ChangeScreenControls();
            }
            catch
            {
            }

        }

        private void ChangeScreenControls()
        {
            txtProjectID.Text = Data.CURRENT_PROJECT.Description;
            txtRigType.Text = "RIG " + Data.CURRENT_RIG.Name;
            txtModule.Text = Data.CURRENT_MODULE.Name;
            txtStep.Text = Data.CURRENT_STEP.Name;
            txtActionName.Text = Data.CURRENT_ACTION.Name;
            txtActionDescription.Text = Data.CURRENT_ACTION.Description;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            gdvPreview.Visibility = Visibility.Collapsed;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                media = new MediaCapture();

                var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture); 
                //getting all camera devices and then will select last device

                if (devices.Count > 1)
                {
                    await media.InitializeAsync(new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = devices[1].Id
                    });
                }
                else
                {
                    await media.InitializeAsync();
                }

                this.capturePreview.Source = media;
                await media.StartPreviewAsync();
                capturePreview.IsTapEnabled = true;

                var captureUI = new Windows.Media.Capture.CameraCaptureUI();
                captureUI.PhotoSettings.MaxResolution = Windows.Media.Capture.CameraCaptureUIMaxPhotoResolution.HighestAvailable;

            }
            catch
            {
                ScreenMessage.Show("Error loading Camera Device");
            }
        }

        private void btnBack_Copy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdvPreview.Visibility = Visibility.Collapsed;
        }

        private void cancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdvImageDetailEdit.Visibility = Visibility.Collapsed;
        }

        private async void ok_Tapped(object sender, TappedRoutedEventArgs e)
        {
            currentImage.Description = txtImageDescription.Text;
            currentImage.Tags = new System.Collections.ObjectModel.ObservableCollection<string>();

            foreach (string tag in txtTags.Text.Split(';'))
            {
                currentImage.Tags.Add(tag);
            }

            if (!string.IsNullOrEmpty(txtComment.Text.Trim()))
            {
                string userName = await Data.GetUserName();

                if (currentImage.Comments == null)
                {
                    currentImage.Comments = new System.Collections.ObjectModel.ObservableCollection<CommentDTO>();
                }

                currentImage.Comments.Add(new CommentDTO() { CommentedAt = DateTime.Now, CommentID = DateTime.Now.ToString("yyyyMMddHHmmssfff"), Text = txtComment.Text.Trim(), User = userName.ToString() });
            }

            gdvImageDetailEdit.Visibility = Visibility.Collapsed;

        }

        private void Edit_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdvImageDetailEdit.Visibility = Visibility.Visible;
            txtComment.Text = "";
            txtImageDescription.Text = "";
            txtTags.Text = "";
        }

        private void discard_New_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gdvPreview.Visibility = Visibility.Collapsed;
            txtComment.Text = "";
            txtImageDescription.Text = "";
            txtTags.Text = "";
        }

        private void Save_New_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Data.CURRENT_ACTION.Images.Add(currentImage);
            gdvPreview.Visibility = Visibility.Collapsed;
            txtComment.Text = "";
            txtImageDescription.Text = "";
            txtTags.Text = "";
        }

        private void Save_Next_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Data.CURRENT_ACTION.Images.Add(currentImage);

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
                            (new MessageDialog("No more actions :)")).ShowAsync();
                        }
                        else
                        {
                            Data.CURRENT_RIG = Data.CURRENT_PROJECT.RigTypes[currentRigIndex + 1];
                            Data.CURRENT_MODULE = Data.CURRENT_RIG.Modules[0];
                            Data.CURRENT_STEP = Data.CURRENT_MODULE.Steps[0];
                            Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[0];
                            gdvPreview.Visibility = Visibility.Collapsed;           //
                        }
                    }
                    else
                    {
                        Data.CURRENT_MODULE = Data.CURRENT_RIG.Modules[currentModuleIdex + 1];
                        Data.CURRENT_STEP = Data.CURRENT_MODULE.Steps[0];     
                        Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[0];   
                        gdvPreview.Visibility = Visibility.Collapsed;        //
                    }
                }
                else
                {
                    gdvPreview.Visibility = Visibility.Collapsed;        //
                    Data.CURRENT_STEP = Data.CURRENT_MODULE.Steps[currentStepIdex + 1];
                    Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[0];
                }
            }
            else
            {
                Data.CURRENT_ACTION = Data.CURRENT_STEP.Actions[currentActionIdex + 1];
                gdvPreview.Visibility = Visibility.Collapsed;          //
            }

            ChangeScreenControls();
            txtComment.Text = "";
            txtImageDescription.Text = "";
            txtTags.Text = "";
        }

        private void Save_Close_btn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Data.CURRENT_ACTION.Images.Add(currentImage);

            WriteImages();
            media.Dispose();
            this.Frame.Navigate(typeof(MainPage));
            txtComment.Text = "";
            txtImageDescription.Text = "";
            txtTags.Text = "";
            ScreenMessage.Show("Successfully saved!");
        }

        private void WriteImages()
        {
            ProjectXmlWriter.Write(Data.CURRENT_PROJECT);
        }

        private void gallery()
        {
            this.Frame.Navigate(typeof(Gallery));
        }
        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Data.CURRENT_ACTION.Images.Add(currentImage);
            WriteImages();
            media.Dispose();
            gallery();
        }
    }
}
