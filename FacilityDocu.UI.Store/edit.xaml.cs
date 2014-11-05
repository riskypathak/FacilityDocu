using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{

    public sealed partial class edit : Page
    {


        Windows.Storage.StorageFolder saveFolder = Package.Current.InstalledLocation;
        int redo_undo = 0;
        InkManager MyInkManager = new InkManager();
        string DrawingTool;
        double X1, X2, Y1, Y2, StrokeThickness = 1;
        Point StartPoint, PreviousContactPoint, CurrentContactPoint;
        double w = 0, h = 0;
        Line NewLine;
        Ellipse NewEllipse;
        Polyline Pencil;
        Rectangle NewRectangle;
        Color BorderColor = Colors.Black, FillColor, temp;
        uint PenID, TouchID;
        BitmapImage finalImage = new BitmapImage();
        ImageBrush img = new ImageBrush();
        int count = 0;

        int dn = 0;
        StorageFile file;
        BitmapImage bitmapImage;
        StorageFile sourceImageFile = null;
        double sourceImageScale = 1;
        uint sourceImagePixelWidth;
        uint sourceImagePixelHeight;
        SelectedRegion selectedRegion;
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
        public edit()
        {


            this.InitializeComponent();
            try
            {
                canvas.PointerMoved += canvas_PointerMoved;
                canvas.PointerReleased += canvas_PointerReleased;
                canvas.PointerPressed += canvas_PointerPressed;
                canvas.PointerExited += canvas_PointerExited;
                canvas.PointerEntered += canvas_PointerEntered;

                for (int i = 1; i < 30; i++)
                {
                    ComboBoxItem Items = new ComboBoxItem();
                    Items.Content = i;
                    cbStrokeThickness.Items.Add(Items);
                }
                for (int i = 30; i < 100; i += 5)
                {
                    ComboBoxItem Items = new ComboBoxItem();
                    Items.Content = i;
                    cbStrokeThickness.Items.Add(Items);
                }
                cbStrokeThickness.SelectedIndex = 10;

                cbFillColor.Items.Add("");
                var colors = typeof(Colors).GetTypeInfo().DeclaredProperties;
                foreach (var item in colors)
                {
                    cbBorderColor.Items.Add(item);
                    cbFillColor.Items.Add(item);
                }
                cbBorderColor.SelectedIndex = 7;
                selectRegion.ManipulationMode = ManipulationModes.Scale |
                    ManipulationModes.TranslateX | ManipulationModes.TranslateY;

                selectedRegion = new SelectedRegion { MinSelectRegionSize = 2 * CornerSize };
                this.DataContext = selectedRegion;
            }
            catch
            {
                msg.show("Somethig is missing");
            }

        }

        #region Drawing Tools Click Events
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


            base.OnNavigatedTo(e);

            selectedRegion.PropertyChanged += selectedRegion_PropertyChanged;

            //  this.sourceImage.ImageFailed += this.sourceImage_ImageFailed;

            // Handle the pointer events of the corners. 
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

            //  this.sourceImage.ImageFailed -= this.sourceImage_ImageFailed;

            // Handle the pointer events of the corners. 
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

            //this.selectInfoInBitmapText.Text = string.Format("Start Point: ({0},{1})  Size: {2}*{3}",
            //    Math.Floor(this.selectedRegion.SelectedRect.X / widthScale),
            //    Math.Floor(this.selectedRegion.SelectedRect.Y / heightScale),
            //    Math.Floor(this.selectedRegion.SelectedRect.Width / widthScale),
            //    Math.Floor(this.selectedRegion.SelectedRect.Height / heightScale));
        }
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


                //  OfflineData.cropImg.Source = previewImage.Source;
            }
            catch
            {
                msg.show("error in crop processing");
            }

        }
        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //cropage
            StorageFile imgFile;

            sourceImage.Source =null;
                string foldername = OfflineData.tempprojectid + "." + OfflineData.tempcategoryid + "." + OfflineData.tempsubcategoryid + "." + OfflineData.tempactionid;
               imgFile = await StorageFile.GetFileFromPathAsync(OfflineData.datap + "\\" + foldername + "\\" + OfflineData.tempproject_name+getimg_pos+".png");
             
                 using (IRandomAccessStream fileStream = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {

                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                    this.sourceImagePixelHeight = decoder.PixelHeight;
                    this.sourceImagePixelWidth = decoder.PixelWidth;
                    this.sourceImageFile = imgFile;
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

                    sourceImage.Source = OfflineData.imagebk[getimg_pos];
                    //this.originalImageInfoText.Text = string.Format("Original Image Size: {0}*{1} ",
                    //    this.sourceImagePixelWidth, this.sourceImagePixelHeight);

                    crop_grid.Visibility = Visibility.Visible;
                }
            try
            {
            }
            catch
            {
                msg.show("Error In Handling");
                crop_grid.Visibility = Visibility.Collapsed;
            }

        }
        
        
        private void btnPencil_Click(object sender, RoutedEventArgs e)
        {
            DrawingTool = "Pencil";
        }

        private void btnLine_Click(object sender, RoutedEventArgs e)
        {
            DrawingTool = "Line";
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            DrawingTool = "Ellipse";
        }

        private void btnRectagle_Click(object sender, RoutedEventArgs e)
        {
            DrawingTool = "Rectagle";
        }

        private void btnEraser_Click(object sender, RoutedEventArgs e)
        {
            DrawingTool = "Eraser";
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            DrawingTool = "Select";
        }

        #endregion

        #region Pointer Events

        void canvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (DrawingTool == "Select")
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        }

        void canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            sset = 0;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        }
        int sset = 0;
        void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (DrawingTool != "Eraser" && DrawingTool != "Select")
            {
                sset = 1;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Cross, 1);
            }
            else if (DrawingTool == "Select")
            {
                sset = 0;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
            }
            else
            {
                sset = 1;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
            }
            switch (DrawingTool)
            {
                case "Line":
                    {

                        NewLine = new Line();

                        NewLine.X1 = e.GetCurrentPoint(canvas).Position.X;
                        NewLine.Y1 = e.GetCurrentPoint(canvas).Position.Y;
                        NewLine.X2 = NewLine.X1 + 1;
                        NewLine.Y2 = NewLine.Y1 + 1;
                        NewLine.StrokeThickness = StrokeThickness;
                        NewLine.Stroke = new SolidColorBrush(BorderColor);
                        canvas.Children.Add(NewLine);


                    }
                    break;

                case "Pencil":
                    {

                        var MyDrawingAttributes = new InkDrawingAttributes();
                        MyDrawingAttributes.Size = new Size(StrokeThickness, StrokeThickness);
                        MyDrawingAttributes.Color = BorderColor;
                        MyDrawingAttributes.FitToCurve = true;
                        MyInkManager.SetDefaultDrawingAttributes(MyDrawingAttributes);

                        PreviousContactPoint = e.GetCurrentPoint(canvas).Position;
                        //PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;  to identify the pointer device
                        if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed)
                        {
                            // Pass the pointer information to the InkManager.
                            MyInkManager.ProcessPointerDown(e.GetCurrentPoint(canvas));
                            PenID = e.GetCurrentPoint(canvas).PointerId;
                            e.Handled = true;
                        }
                    }
                    break;

                case "Rectagle":
                    {
                        NewRectangle = new Rectangle();
                        X1 = e.GetCurrentPoint(canvas).Position.X;
                        Y1 = e.GetCurrentPoint(canvas).Position.Y;
                        X2 = X1;
                        Y2 = Y1;
                        NewRectangle.Width = X2 - X1;
                        NewRectangle.Height = Y2 - Y1;
                        NewRectangle.StrokeThickness = StrokeThickness;
                        NewRectangle.Stroke = new SolidColorBrush(BorderColor);
                        NewRectangle.Fill = new SolidColorBrush(FillColor);
                        canvas.Children.Add(NewRectangle);
                        NewRectangle.ManipulationMode = ManipulationModes.All;
                       

                    }
                    break;

                case "Ellipse":
                    {



                        NewEllipse = new Ellipse();
                        X1 = e.GetCurrentPoint(canvas).Position.X;
                        Y1 = e.GetCurrentPoint(canvas).Position.Y;
                        X2 = X1;
                        Y2 = Y1;
                        NewEllipse.Width = X2 - X1;
                        NewEllipse.Height = Y2 - Y1;
                        NewEllipse.StrokeThickness = StrokeThickness;
                        NewEllipse.Stroke = new SolidColorBrush(BorderColor);
                        NewEllipse.Fill = new SolidColorBrush(FillColor);
                        canvas.Children.Add(NewEllipse);

                    }
                    break;

                case "Eraser":
                    {
                        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.UniversalNo, 1);
                        StartPoint = e.GetCurrentPoint(canvas).Position;
                        Pencil = new Polyline();
                        Pencil.Stroke = new SolidColorBrush(Colors.Wheat);
                        Pencil.StrokeThickness = 10;
                        canvas.Children.Add(Pencil);
                    }
                    break;

                default:
                    break;
            }
        }

        void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (DrawingTool != "Eraser" && DrawingTool != "Select")
                    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Cross, 1);
                else if (DrawingTool == "Select")
                    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                else
                    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.UniversalNo, 1);

                switch (DrawingTool)
                {
                    case "Pencil":
                        {


                            if (e.Pointer.PointerId == PenID || e.Pointer.PointerId == TouchID)
                            {
                                // Distance() is an application-defined function that tests
                                // whether the pointer has moved far enough to justify 
                                // drawing a new line.
                                CurrentContactPoint = e.GetCurrentPoint(canvas).Position;
                                X1 = PreviousContactPoint.X;
                                Y1 = PreviousContactPoint.Y;
                                X2 = CurrentContactPoint.X;
                                Y2 = CurrentContactPoint.Y;

                                if (Distance(X1, Y1, X2, Y2) > 2.0)
                                {
                                    Line line = new Line()
                                    {
                                        X1 = X1,
                                        Y1 = Y1,
                                        X2 = X2,
                                        Y2 = Y2,
                                        StrokeThickness = StrokeThickness,
                                        Stroke = new SolidColorBrush(BorderColor)
                                    };

                                    PreviousContactPoint = CurrentContactPoint;
                                    canvas.Children.Add(line);
                                    MyInkManager.ProcessPointerUpdate(e.GetCurrentPoint(canvas));
                                }
                            }
                        }
                        break;

                    case "Line":
                        {
                            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
                            {
                                NewLine.X2 = e.GetCurrentPoint(canvas).Position.X;
                                NewLine.Y2 = e.GetCurrentPoint(canvas).Position.Y;

                            }


                        }
                        break;

                    case "Rectagle":
                        {
                            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
                            {
                                X2 = e.GetCurrentPoint(canvas).Position.X;
                                Y2 = e.GetCurrentPoint(canvas).Position.Y;
                                if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
                                    NewRectangle.Margin = new Thickness(X1, Y1, X2, Y2);
                                else if ((X2 - X1) < 0 && (Y2 - Y1) > 0)
                                    NewRectangle.Margin = new Thickness(X2, Y1, X1, Y2);
                                else if ((X2 - X1) > 0 && (Y2 - Y1) < 0)
                                    NewRectangle.Margin = new Thickness(X1, Y2, X2, Y1);
                                else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
                                    NewRectangle.Margin = new Thickness(X2, Y2, X1, Y1);
                                NewRectangle.Width = Math.Abs(X2 - X1);
                                NewRectangle.Height = Math.Abs(Y2 - Y1);

                            }
                        }
                        break;

                    case "Ellipse":
                        {

                            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
                            {
                                X2 = e.GetCurrentPoint(canvas).Position.X;
                                Y2 = e.GetCurrentPoint(canvas).Position.Y;
                                if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
                                    NewEllipse.Margin = new Thickness(X1, Y1, X2, Y2);
                                else if ((X2 - X1) < 0 && (Y2 - Y1) > 0)
                                    NewEllipse.Margin = new Thickness(X2, Y1, X1, Y2);
                                else if ((X2 - X1) > 0 && (Y2 - Y1) < 0)
                                    NewEllipse.Margin = new Thickness(X1, Y2, X2, Y1);
                                else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
                                    NewEllipse.Margin = new Thickness(X2, Y2, X1, Y1);
                                NewEllipse.Width = Math.Abs(X2 - X1);
                                NewEllipse.Height = Math.Abs(Y2 - Y1);



                            }



                        }
                        break;

                    case "Eraser":
                        {
                            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == true)
                            {
                                if (StartPoint != e.GetCurrentPoint(canvas).Position)
                                {
                                    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.UniversalNo, 1);
                                    Pencil.Points.Add(e.GetCurrentPoint(canvas).Position);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                //
            }
        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            double d = 0;
            d = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            return d;
        }

        void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

            if (e.Pointer.PointerId == PenID || e.Pointer.PointerId == TouchID)
                MyInkManager.ProcessPointerUp(e.GetCurrentPoint(canvas));

            if (sset == 1)
            {
                btn_Undo.IsEnabled = true;

                SaveCanvas();

            }
            TouchID = 0;
            PenID = 0;
            e.Handled = true;
            Pencil = null;
            NewLine = null;
            NewRectangle = null;
            NewEllipse = null;

        }

        #endregion


        private void cbStrokeThickness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StrokeThickness = Convert.ToDouble(cbStrokeThickness.SelectedIndex + 1);
        }
        private void cbBorderColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBorderColor.SelectedIndex == 0)
            {
                BorderColor = temp;
            }
            else if (cbBorderColor.SelectedIndex != -1)
            {
                var pi = cbBorderColor.SelectedItem as PropertyInfo;
                BorderColor = (Color)pi.GetValue(null);
            }
        }
        private void cbFillColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFillColor.SelectedIndex != -1)
            {
                var pi = cbFillColor.SelectedItem as PropertyInfo;
                FillColor = (Color)pi.GetValue(null);
            }
        }



        #region Show Message method
        /// <summary>  
        /// Show Message method  
        /// </summary>  
        /// <param name="content">Content parameter</param>  
        /// <param name="title">Title parameter</param>  
        private async void ShowMessage(string content, string title)
        {
            try
            {
                MessageDialog msg = new MessageDialog(content, title);
                await msg.ShowAsync();
            }
            catch
            {

            }

        }
        #endregion
        #region Create File method.
        /// <summary>  
        /// Create File method.  
        /// </summary>  
        /// <param name="filename">File name parameter</param>  
        /// <returns>Returns storage file type object</returns>  

        #endregion
        #region Save to PNG method.
       
        private async Task SaveToPNG(RenderTargetBitmap bitmap)
        {

            try
            {
                file = await KnownFolders.PicturesLibrary.CreateFileAsync("image"+getimg_pos.ToString()+ ".png",CreationCollisionOption.ReplaceExisting);
                //file = await sf.CreateFileAsync
                //                 ("image"+getimg_pos.ToString()+ ".png", Windows.Storage.CreationCollisionOption.ReplaceExisting);

                if (bitmap == null)
                {

                    this.ShowMessage("Something goes wrong, try again later", "Error");

                    return;
                }

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    // Initialization.  
                    var pixelBuffer = await bitmap.GetPixelsAsync();
                    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                    // convert stream to IRandomAccessStream  
                    var randomAccessStream = stream.AsRandomAccessStream();
                    // encoding to PNG  
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
                    // Finish saving  
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth,
                               (uint)bitmap.PixelHeight, logicalDpi, logicalDpi, pixelBuffer.ToArray());
                    // Flush encoder.  
                    await encoder.FlushAsync();

                }


                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    nj.Source = bitmapImage;
                   // img.ImageSource = bitmapImage;
                    finalImage = bitmapImage;
                    OfflineData.imagebk[redo_undo]=bitmapImage;
                    img.ImageSource = OfflineData.imagebk[redo_undo];
                    canvas.Background = img;
                    canvas.Children.Clear();
                    count++;
                    redo_undo++;


                }



            }
            catch (Exception ex)
            {
                //  this.ShowMessage("File not supported or not found", "Error");
            }


        }


        #endregion
        #region Canvas to BMP method

        private async Task<RenderTargetBitmap> CanvasToBMP()
        {
            // Initialization  
            RenderTargetBitmap bitmap = null;
            try
            {
                // Initialization.  
                Size canvasSize = this.canvas.RenderSize;
                Point defaultPoint = this.canvas.RenderTransformOrigin;
                // Sezing to output image dimension.  
                this.canvas.Measure(canvasSize);
                this.canvas.UpdateLayout();
                this.canvas.Arrange(new Rect(defaultPoint, canvasSize));
                // Convert canvas to bmp.  
                var bmp = new RenderTargetBitmap();
                await bmp.RenderAsync(this.canvas);
                // Setting.  
                bitmap = bmp as RenderTargetBitmap;
            }
            catch
            {
                SaveCanvas();

            }
            return bitmap;
        }
        #endregion


        #region Save Canvas method

        private async void SaveCanvas()
        {


            try
            {
                redo_undo = count;
                canvas.Width = w;

                canvas.Height = h;

                RenderTargetBitmap bitmap = await this.CanvasToBMP();

                await this.SaveToPNG(bitmap);
            }

            catch
            {
                msg.show("Error Update Your Process");

            }
        }
        #endregion


        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                btn_Undo.IsEnabled = true;

                if (count != redo_undo)
                {
                    count++;
                    ImageBrush img = new ImageBrush();
                    img.ImageSource = OfflineData.undoImage[count];
                    canvas.Background = img;
                    nj.Source = img.ImageSource;

                }
                else
                {
                    btnRedo.IsEnabled = false;
                }
            }

            catch
            {
                msg.show("Redo cannot be performed");
            }
        }
        private void btn_Undo_Click(object sender, RoutedEventArgs e)
        {
            //red--;
            try
            {
                btnRedo.IsEnabled = true;

                if (count != 0)
                {
                    count--;
                    ImageBrush img = new ImageBrush();
                    img.ImageSource = OfflineData.undoImage[0];
                    canvas.Background = img;
                    nj.Source = img.ImageSource;
                }
                else
                {
                    btn_Undo.IsEnabled = false;
                }
               

            }

            catch
            {
                msg.show("Undo cannot be performed");
            }
        }


        private void zoomout_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                double rat;
                rat = canvas.Width / canvas.Height;
                if (rat > 1)
                {
                    canvas.Width = canvas.Width - rat * 10;
                    canvas.Height = canvas.Height - 10;
                }
                else
                {
                    canvas.Width = canvas.Width - 10;
                    canvas.Height = canvas.Height - rat * 10;
                }
            }
            catch
            {

            }
        }

        private void zoomin_btn_Click(object sender, RoutedEventArgs e)
        {
            double rat;
            rat = canvas.Width / canvas.Height;
            if (rat > 1)
            {
                canvas.Width = canvas.Width + rat * 10;
                canvas.Height = canvas.Height + 10;
            }
            else
            {
                canvas.Width = canvas.Width + 10;
                canvas.Height = canvas.Height + rat * 10;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            resize_bk.Visibility = Visibility.Collapsed;
            canvas.Height = h;
            canvas.Width = w;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            double wdv, htv;
            try
            {
                wdv = Convert.ToInt32(wd.Text);
                htv = Convert.ToInt32(ht.Text);
                h = htv;
                w = wdv;
                canvas.Height = h;
                canvas.Width = w;
                resize_bk.Visibility = Visibility.Collapsed;
            }
            catch
            {
                this.ShowMessage("Enter the valid data in Pixel Form ", "Invalid");
            }
        }

        private void resize_btn_Click(object sender, RoutedEventArgs e)
        {
            resize_bk.Visibility = Visibility.Visible;
        }
        Image target = new Image();
        private void ht_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (ht.Text != "")
                {
                    er.Text = "";
                    canvas.Height = Convert.ToInt32(ht.Text);
                }
                else
                {
                    er.Text = "";
                }

            }
            catch
            {
                er.Text = "Invaid Data Entered In Height Box ..";
            }
        }

        private void wd_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                if (wd.Text != "")
                {
                    er.Text = "";
                    canvas.Width = Convert.ToInt32(wd.Text);
                }
                else
                {
                    er.Text = "";
                }

            }
            catch
            {
                er.Text = "Invaid Data Entered In width Box ..";
            }
        }

      
       

        public void viewimg(int indx)
        {
            BitmapImage bi = new BitmapImage();
            try
            {

                ImageBrush img = new ImageBrush();

                bi = OfflineData.imagebk[indx];

                nj.Source = bi;
                img.ImageSource = bi;
               
                canvas.Background = img;
                SaveCanvas();
            }
            catch
            {

            }

        }
        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {

            try
            {
                Image a;
                 BitmapImage bi = new BitmapImage();
                 ImageBrush img = new ImageBrush();

                for (int lop = 0; lop < OfflineData.imagebk.Count; lop++)
                {
                    a = new Image();
                    a.Source = OfflineData.imagebk[lop];


                }
               

                    bi = OfflineData.editpic;

                    canvas.Width = bi.PixelWidth * 1.5;
                    canvas.Height = bi.PixelHeight * 1.5;
                    w = canvas.Width;
                    h = canvas.Height;
                    nj.Source = bi;
                    img.ImageSource = bi;
                    canvas.Background = img;
                    SaveCanvas();

                }
                catch
                {
                    msg.show("Error To Loading Page");

                }

        }

       


        private async void Done_Click(object sender, TappedRoutedEventArgs e)
        {

            //DateTime dt = DateTime.Now;
            //string dtt = dt.ToString("r");
            dn = 1;
            writeXml.Save_Project();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }
      
        int getimg_pos = 0;
        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {


            SaveCanvas();
            OfflineData.undoImage.Clear();
           
            redo_undo = 0;
            count = 0;
            btn_Undo.IsEnabled = false;
            btnRedo.IsEnabled = false;
            getimg_pos++;
            if (getimg_pos == OfflineData.imagebk.Count)
            {
                getimg_pos = 0;
            }
            // OfflineData.imagebk[prev_imgsel] = bitmapImage;
            viewimg(getimg_pos);
        }

        private void cropDone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ImageBrush img = new ImageBrush();
            img.ImageSource = previewImage.Source;

            canvas.Width = previewImage.Width;
            canvas.Height = previewImage.Height;
            canvas.Background = img;
            crop_grid.Visibility = Visibility.Collapsed;
            SaveCanvas();
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            crop_grid.Visibility = Visibility.Collapsed;
        }







    }
}
