using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
        int tagcom = 0;
        int takepic = 0;
        BitmapImage bitmapImage;
        public Camera_Page()
        {
            this.InitializeComponent();

        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(add_details));
        }

        private async void capturePreview_Tapped(object sender, TappedRoutedEventArgs e)
        {


            try
            {  
                string fileName = "image" + takepic.ToString() + ".png";
                fileName = Path.GetFileName(fileName);

              string  foldername = "0" + "." + OfflineData.tempcategoryid + "." + OfflineData.tempsubcategoryid + "." + OfflineData.tempactionid;
              OfflineData.foldername = foldername;
                OfflineData.folderpath = OfflineData.datap + "\\" + foldername;
                

                StorageFolder sf = await KnownFolders.PicturesLibrary.CreateFolderAsync("mohan", CreationCollisionOption.ReplaceExisting);
               

                await sf.CreateFolderAsync(foldername, CreationCollisionOption.ReplaceExisting);

                StorageFile file = await sf.CreateFileAsync(foldername + "\\" + fileName, CreationCollisionOption.GenerateUniqueName);
               
                await media.CapturePhotoToStorageFileAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreateJpeg(), file);

                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    preview_pic.Source = bitmapImage;
                    if (takepic == 0)
                    {
                        OfflineData.editpic = bitmapImage;
                    }
                }
               

                preview_grid.Visibility = Visibility.Visible;
             
             
            }
            catch
            {
                msg.show(" its failed !  try aggain");
            }

        }

        //cancel button click
        private void cancel_Click(object sender, RoutedEventArgs e)
        {

            preview_grid.Visibility = Visibility.Collapsed;


        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                media = new MediaCapture();
                await media.InitializeAsync();
                this.capturePreview.Source = media;
                await media.StartPreviewAsync();
                capturePreview.IsTapEnabled = true;
            }
            catch
            {
                msg.show("Error loading Camera Device");
            }


        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(add_details));
        }

        //tc => tag and comment grid
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tc.Visibility = Visibility.Collapsed;
        }

        private void comment_Click(object sender, RoutedEventArgs e)
        {
            tc.Visibility = Visibility.Visible;
            tagcom = 1;
        }

        private void Tag_Click(object sender, RoutedEventArgs e)
        {
            tagcom = 2;
            tc.Visibility = Visibility.Visible;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //done
            if (tagcom == 1)
            {
                OfflineData.tempimagecomment = txt.Text;
                txt.Text = "";

            }
            else
            {
                OfflineData.tempimagetag = txt.Text;
                txt.Text = "";
            }
            tc.Visibility = Visibility.Collapsed;
        }

        private async void ok_Click(object sender, TappedRoutedEventArgs e)
        {
            //   this.Frame.Navigate(typeof(CropPage));
            if (OfflineData.tempimagecomment != "" && OfflineData.tempimagetag != "")
            {
                media.Dispose();
                OfflineData.imagebk.Add(bitmapImage);
                this.Frame.Navigate(typeof(edit));
            }
            else
            {

                var msg = new MessageDialog("Do you want to continue without putting tags and comments?");
                msg.Commands.Add(new UICommand("Yes", (UICommandInvokedHandler) =>
                {
                    //  Application
                    media.Dispose();
                    OfflineData.imagebk.Add(bitmapImage);
                    this.Frame.Navigate(typeof(edit));

                }
                ));
                msg.Commands.Add(new UICommand("No"));
                await msg.ShowAsync();
            }
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OfflineData.imagebk.Add(bitmapImage);
            preview_grid.Visibility = Visibility.Collapsed;
            takepic++;
        }
    }

}
