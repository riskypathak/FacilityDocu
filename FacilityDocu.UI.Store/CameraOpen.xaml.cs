using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        BitmapImage bitmapImage;
        int tagcom = 0;
        int takepic = 0;
        public Camera_Page()
        {
            this.InitializeComponent();

        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            media.Dispose();
            this.Frame.Navigate(typeof(add_details));
        }

        private async void capturePreview_Tapped(object sender, TappedRoutedEventArgs e)
        {


           
                StorageFolder facilityDocuFol = await StorageFolder.GetFolderFromPathAsync(OfflineData.datap);

                string fileName = "image" + OfflineData.picCount.ToString() + ".png";
                fileName = Path.GetFileName(fileName);
                await facilityDocuFol.CreateFolderAsync(OfflineData.folderName, CreationCollisionOption.OpenIfExists);

                StorageFile file = await facilityDocuFol.CreateFileAsync(OfflineData.folderName + "\\" + fileName, CreationCollisionOption.ReplaceExisting);
               
                await media.CapturePhotoToStorageFileAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreatePng(), file);

                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    preview_pic.Source = bitmapImage;
                    OfflineData.editpic = bitmapImage;

                   
                }
                OfflineData.tempPhotoloc = file;
                media.Dispose();
                OfflineData.imagebk.Add(bitmapImage);
                this.Frame.Navigate(typeof(edit));
                try
                {
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
            media.Dispose();
            this.Frame.Navigate(typeof(add_details));
        }

      
        private async void Button_Click(object sender, TappedRoutedEventArgs e)
        {
           
                media.Dispose();
                OfflineData.imagebk.Add(bitmapImage);
                this.Frame.Navigate(typeof(CropPage));
          
        }

        private void backButton_Copy_Tapped(object sender, TappedRoutedEventArgs e)
        {
           
            preview_grid.Visibility = Visibility.Collapsed;
        }
    }

}
