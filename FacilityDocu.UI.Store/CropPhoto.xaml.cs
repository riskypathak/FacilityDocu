using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CropPage : Tablet_App.Common.LayoutAwarePage
    {
        // ServiceReference1.Service1Client MyService;

        InkManager MyInkManager = new InkManager();
        string DrawingTool;
        double X1, X2, Y1, Y2, StrokeThickness = 1;
        Line NewLine;
        Ellipse NewEllipse;
        Point StartPoint, PreviousContactPoint, CurrentContactPoint;
        Polyline Pencil;
        Rectangle NewRectangle;
        Color BorderColor = Colors.Black, FillColor;
        uint PenID, TouchID;
        SelectedRegion selectedRegion;
        double sourceImageScale = 1;

        StorageFile sourceImageFile = null;

        uint sourceImagePixelWidth;
        uint sourceImagePixelHeight;
        Dictionary<uint, Point?> pointerPositionHistory = new Dictionary<uint, Point?>();
        double cornerSize;

        double CornerSize
        {
            get
            {
                if (cornerSize <= 0)
                {
                    cornerSize = (double)Application.Current.Resources["Size"];
                }

                return cornerSize;
            }
        }
        public CropPage()
        {
            this.InitializeComponent();



            selectRegion.ManipulationMode = ManipulationModes.Scale |
                ManipulationModes.TranslateX | ManipulationModes.TranslateY;

            selectedRegion = new SelectedRegion { MinSelectRegionSize = 2 * CornerSize };
            this.DataContext = selectedRegion;
        }

        #region Drawing Tools Click Events
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            selectedRegion.PropertyChanged += selectedRegion_PropertyChanged;

            AddCornerEvents(topLeftCorner);
            AddCornerEvents(topRightCorner);
            AddCornerEvents(bottomLeftCorner);
            AddCornerEvents(bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            selectRegion.ManipulationDelta += selectRegion_ManipulationDelta;
            selectRegion.ManipulationCompleted += selectRegion_ManipulationCompleted;

            this.sourceImage.SizeChanged += sourceImage_SizeChanged;


        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            selectedRegion.PropertyChanged -= selectedRegion_PropertyChanged;

            RemoveCornerEvents(topLeftCorner);
            RemoveCornerEvents(topRightCorner);
            RemoveCornerEvents(bottomLeftCorner);
            RemoveCornerEvents(bottomRightCorner);

            // Handle the manipulation events of the selectRegion
            selectRegion.ManipulationDelta -= selectRegion_ManipulationDelta;
            selectRegion.ManipulationCompleted -= selectRegion_ManipulationCompleted;

            this.sourceImage.SizeChanged -= sourceImage_SizeChanged;

        }
        private void AddCornerEvents(Control corner)
        {
            corner.PointerPressed += Corner_PointerPressed;
            corner.PointerMoved += Corner_PointerMoved;
            corner.PointerReleased += Corner_PointerReleased;
        }
        private void RemoveCornerEvents(Control corner)
        {
            corner.PointerPressed -= Corner_PointerPressed;
            corner.PointerMoved -= Corner_PointerMoved;
            corner.PointerReleased -= Corner_PointerReleased;
        }
        //..//
        #region Open an image, handle the select region changed event and save the image.
        //..//
        void sourceImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (e.NewSize.IsEmpty || double.IsNaN(e.NewSize.Height) || e.NewSize.Height <= 0)
            {
                this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                // this.saveImageButton.IsEnabled = false;
                this.selectedRegion.OuterRect = Rect.Empty;
                this.selectedRegion.ResetCorner(0, 0, 0, 0);
            }
            else
            {
                this.imageCanvas.Visibility = Windows.UI.Xaml.Visibility.Visible;
                // this.saveImageButton.IsEnabled = true;

                this.imageCanvas.Height = e.NewSize.Height;
                this.imageCanvas.Width = e.NewSize.Width;
                this.selectedRegion.OuterRect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

                if (e.PreviousSize.IsEmpty || double.IsNaN(e.PreviousSize.Height) || e.PreviousSize.Height <= 0)
                {
                    this.selectedRegion.ResetCorner(0, 0, e.NewSize.Width, e.NewSize.Height);
                }
                else
                {
                    double scale = e.NewSize.Height / e.PreviousSize.Height;
                    this.selectedRegion.ResizeSelectedRect(scale);
                    UpdatePreviewImage();
                }

            }


        }

        void selectedRegion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            double widthScale = imageCanvas.Width / this.sourceImagePixelWidth;
            double heightScale = imageCanvas.Height / this.sourceImagePixelHeight;

            this.selectInfoInBitmapText.Text = string.Format("Start Point: ({0},{1})  Size: {2}*{3}",
                Math.Floor(this.selectedRegion.SelectedRect.X / widthScale),
                Math.Floor(this.selectedRegion.SelectedRect.Y / heightScale),
                Math.Floor(this.selectedRegion.SelectedRect.Width / widthScale),
                Math.Floor(this.selectedRegion.SelectedRect.Height / heightScale));
        }

        #endregion
        //..//
        #region Select Region methods
        //..//
        private void Corner_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            (sender as UIElement).CapturePointer(e.Pointer);

            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(this);

            // Record the start point of the pointer.
            pointerPositionHistory[pt.PointerId] = pt.Position;

            e.Handled = true;
        }
        void Corner_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(this);
            uint ptrId = pt.PointerId;

            if (pointerPositionHistory.ContainsKey(ptrId) && pointerPositionHistory[ptrId].HasValue)
            {
                Point currentPosition = pt.Position;
                Point previousPosition = pointerPositionHistory[ptrId].Value;

                double xUpdate = currentPosition.X - previousPosition.X;
                double yUpdate = currentPosition.Y - previousPosition.Y;

                this.selectedRegion.UpdateCorner((sender as ContentControl).Tag as string, xUpdate, yUpdate);

                pointerPositionHistory[ptrId] = currentPosition;
            }

            e.Handled = true;
        }
        private void Corner_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            uint ptrId = e.GetCurrentPoint(this).PointerId;
            if (this.pointerPositionHistory.ContainsKey(ptrId))
            {
                this.pointerPositionHistory.Remove(ptrId);
            }

            (sender as UIElement).ReleasePointerCapture(e.Pointer);

            UpdatePreviewImage();
            e.Handled = true;


        }
        void selectRegion_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            this.selectedRegion.UpdateSelectedRect(e.Delta.Scale, e.Delta.Translation.X, e.Delta.Translation.Y);
            e.Handled = true;
        }
        void selectRegion_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            UpdatePreviewImage();
        }
        async void UpdatePreviewImage()
        {
            try
            {
                //  StorageFile sourceImageFile = Camera_Page.stf();
                double sourceImageWidthScale = imageCanvas.Width / this.sourceImagePixelWidth;
                double sourceImageHeightScale = imageCanvas.Height / this.sourceImagePixelHeight;


                Size previewImageSize = new Size(
                    this.selectedRegion.SelectedRect.Width / sourceImageWidthScale,
                    this.selectedRegion.SelectedRect.Height / sourceImageHeightScale);

                double previewImageScale = 1;

                if (previewImageSize.Width <= imageCanvas.Width &&
                    previewImageSize.Height <= imageCanvas.Height)
                {
                    this.previewImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                }
                else
                {
                    this.previewImage.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;

                    previewImageScale = Math.Min(imageCanvas.Width / previewImageSize.Width,
                        imageCanvas.Height / previewImageSize.Height);
                }


                previewImage.Source = await CropBitmap.GetCroppedBitmapAsync(
                        this.sourceImageFile,
                        new Point(this.selectedRegion.SelectedRect.X / sourceImageWidthScale, this.selectedRegion.SelectedRect.Y / sourceImageHeightScale),
                        previewImageSize,
                        previewImageScale);
            }
            catch
            {
            }

        }

        #endregion
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private ImageSource CurrentEditPic;
        private StorageFile CurrentImageFile;

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage();
            CurrentImageFile = await StorageFile.GetFileFromPathAsync(System.IO.Path.Combine(Data.ImagesPath, "temp.jpg"));

            using (IRandomAccessStream fileStream = await CurrentImageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                await bitmapImage.SetSourceAsync(fileStream);
                sourceImage.Source = CurrentEditPic =  bitmapImage;
            }

            StorageFile imgFile = CurrentImageFile;
            try
            {
                using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    this.sourceImageFile = imgFile;
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                    this.sourceImagePixelHeight = decoder.PixelHeight;
                    this.sourceImagePixelWidth = decoder.PixelWidth;
                }
            }
            catch
            {
                this.Frame.Navigate(typeof(EditPhoto));
            }


            loadPicFrame();
        }
        public async void loadPicFrame()
        {
            try
            {

                using (IRandomAccessStream fileStream = await CurrentImageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    this.sourceImageFile = CurrentImageFile;
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                    this.sourceImagePixelHeight = decoder.PixelHeight;
                    this.sourceImagePixelWidth = decoder.PixelWidth;
                }


                if (this.sourceImagePixelHeight < 2 * this.CornerSize ||
                    this.sourceImagePixelWidth < 2 * this.CornerSize)
                {
                    //this.NotifyUser(string.Format("Please select an image which is larger than {0}*{0}",
                    //    2 * this.CornerSize));
                    return;
                }
                else
                {
                    sourceImageScale = 0;

                    if (this.sourceImagePixelHeight < this.sourceImageGrid.ActualHeight &&
                        this.sourceImagePixelWidth < this.sourceImageGrid.ActualWidth)
                    {
                        this.sourceImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                    }
                    else
                    {
                        sourceImageScale = Math.Min(this.sourceImageGrid.ActualWidth / this.sourceImagePixelWidth,
                             this.sourceImageGrid.ActualHeight / this.sourceImagePixelHeight);
                        this.sourceImage.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                    }


                    this.sourceImage.Source = await CropBitmap.GetCroppedBitmapAsync(
                       this.sourceImageFile,
                       new Point(0, 0),
                       new Size(this.sourceImagePixelWidth, this.sourceImagePixelHeight),
                       sourceImageScale);
                    this.originalImageInfoText.Text = string.Format("Original Image Size: {0}*{1} ",
                        this.sourceImagePixelWidth, this.sourceImagePixelHeight);
                    BitmapImage imgs = (BitmapImage)asd.ImageSource;
                    im = imgs;
                }
            }
            catch
            {
                this.Frame.Navigate(typeof(EditPhoto));
            }
        }

        ImageBrush asd = new ImageBrush();
        BitmapImage im = new BitmapImage();


        private void backButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            fp.Visibility = Visibility.Collapsed;
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                double widthScale = imageCanvas.Width / this.sourceImagePixelWidth;
                double heightScale = imageCanvas.Height / this.sourceImagePixelHeight;

                await CropBitmap.SaveCroppedBitmapAsync(
                    sourceImageFile,
                    sourceImageFile,
                    new Point(this.selectedRegion.SelectedRect.X / widthScale, this.selectedRegion.SelectedRect.Y / heightScale),
                    new Size(this.selectedRegion.SelectedRect.Width / widthScale, this.selectedRegion.SelectedRect.Height / heightScale));

                CurrentEditPic = im;
                this.Frame.Navigate(typeof(EditPhoto));
            }
            catch
            {

            }
        }

        private async void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {

            try
            {
                double widthScale = imageCanvas.Width / this.sourceImagePixelWidth;
                double heightScale = imageCanvas.Height / this.sourceImagePixelHeight;

                await CropBitmap.SaveCroppedBitmapAsync(
                    sourceImageFile,
                    sourceImageFile,
                    new Point(this.selectedRegion.SelectedRect.X / widthScale, this.selectedRegion.SelectedRect.Y / heightScale),
                    new Size(this.selectedRegion.SelectedRect.Width / widthScale, this.selectedRegion.SelectedRect.Height / heightScale));

                CurrentEditPic = im;
                Data.IsFromCrop = true;

                this.Frame.Navigate(typeof(EditPhoto));
            }
            catch
            {

            }
        }


    }
}
        #endregion