using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tablet_App
{

    public sealed partial class EditPhoto : Page
    {
        Windows.Storage.StorageFolder saveFolder = ApplicationData.Current.LocalFolder;
        int redo_undo = 0;
        InkManager MyInkManager = new InkManager();
        string DrawingTool;
        double X1, X2, Y1, Y2, StrokeThickness = 1;
        Point PreviousContactPoint, CurrentContactPoint;

        double width = 0, height = 0;

        Line NewLine;
        Ellipse NewEllipse;
        Polyline Pencil;
        Rectangle NewRectangle;
        Color BorderColor, FillColor;
        uint PenID, TouchID;
        BitmapImage finalImage = new BitmapImage();
        ImageBrush img = new ImageBrush();
        int count = 0;

        StorageFile file;

        BitmapImage bitmapImage;

        private static List<PropertyInfo> LoadColors()
        {
            List<PropertyInfo> colors = new List<PropertyInfo>();

            var t = typeof(Colors);
            var ti = t.GetTypeInfo();
            var dp = ti.DeclaredProperties;
            foreach (var item in dp)
            {
                if (item.Name.Equals("Red")
                    || item.Name.Equals("White")
                    || item.Name.Equals("Black")
                    || item.Name.Equals("Blue")
                    || item.Name.Equals("Green")
                    || item.Name.Equals("Yellow"))
                {
                    colors.Add(item);
                }
            }

            return colors;
        }



        public EditPhoto()
        {
            this.InitializeComponent();

            try
            {
                canvas.PointerMoved += canvas_PointerMoved;
                canvas.PointerReleased += canvas_PointerReleased;
                canvas.PointerPressed += canvas_PointerPressed;
                canvas.PointerExited += canvas_PointerExited;
                canvas.PointerEntered += canvas_PointerEntered;

                for (int i = 30; i < 100; i += 5)
                {
                    ComboBoxItem Items = new ComboBoxItem();
                    Items.Content = i;
                    cmbStrokeThickness.Items.Add(Items);
                }

                cmbStrokeThickness.SelectedIndex = 3;

                cbBorderColor.ItemsSource = LoadColors();

                BorderColor = Colors.White;
            }
            catch
            {
                ScreenMessage.Show("Somethig is missing");
            }

        }
        #region Drawing Tools Click Events

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

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            DrawingTool = "Rectangle";
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
        double xCor = 0;
        double yCor = 0;
        double xf = 0, yf = 0, xl = 0, yl = 0;
        public void initialPoint(PointerRoutedEventArgs e)
        {
            try
            {
                switch (DrawingTool)
                {
                    case "Line":
                        {

                            NewLine = new Line();

                            NewLine.X1 = xCor;
                            NewLine.Y1 = yCor;

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

                    case "Rectangle":
                        {
                            NewRectangle = new Rectangle();
                            X1 = xCor;
                            Y1 = yCor;
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
                            X1 = xCor;
                            Y1 = yCor;
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

                    default:
                        break;
                }
            }
            catch
            {

            }
        }
        public void finalPoint(PointerRoutedEventArgs e)
        {
            try
            {
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

                    case "Rectangle":
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

                    default:
                        break;
                }
            }
            catch
            {

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

                finalPoint(e);
            }
            catch
            {

            }
        }
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

            xCor = e.GetCurrentPoint(canvas).Position.X;
            yCor = e.GetCurrentPoint(canvas).Position.Y;
            initialPoint(e);

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
                xl = X2;
                yl = Y2;

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

        private void cmbStrokeThickness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StrokeThickness = Convert.ToDouble(cmbStrokeThickness.SelectedIndex + 1);
        }

        private async Task SaveToPNG(RenderTargetBitmap bitmap)
        {
            StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(Data.ImagesPath);
            file = await sf.CreateFileAsync("temp.jpg", Windows.Storage.CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenStreamForWriteAsync())
            {
                // Initialization.  
                var pixelBuffer = await bitmap.GetPixelsAsync();
                var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                // convert stream to IRandomAccessStream  
                var randomAccessStream = stream.AsRandomAccessStream();
                // encoding to PNG  
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, randomAccessStream);
                // Finish saving  
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth,
                           (uint)bitmap.PixelHeight, logicalDpi, logicalDpi, pixelBuffer.ToArray());
                // Flush encoder.  
                await encoder.FlushAsync();
            }
        }

        private async Task<RenderTargetBitmap> CanvasToBMP()
        {
            // Initialization.  
            Size canvasSize = this.canvas.RenderSize;
            Point defaultPoint = this.canvas.RenderTransformOrigin;
            // Sezing to output image dimension.  
            this.canvas.Measure(canvasSize);
            this.canvas.UpdateLayout();
            this.canvas.Arrange(new Rect(defaultPoint, canvasSize));
            // Convert canvas to bmp.  
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(this.canvas);


            return bitmap;
        }

        private async Task SaveCanvas()
        {
            try
            {
                canvas.Width = width;
                canvas.Height = height;

                RenderTargetBitmap bitmap = await CanvasToBMP();
                await SaveToPNG(bitmap);
            }

            catch
            {
                ScreenMessage.Show("Error Update Your Process");
            }
        }

        public async void ViewImage()
        {
            ImageBrush img = new ImageBrush();

            StorageFile modifyImage = null;
            if (!Data.IsFromCrop)
            {
                modifyImage = await StorageFile.GetFileFromPathAsync(Data.MODIFYIMAGE.Path);
            }
            else
            {
                modifyImage = await StorageFile.GetFileFromPathAsync(System.IO.Path.Combine(Data.ImagesPath, "temp.jpg"));
                Data.IsFromCrop = false;
            }

            using (IRandomAccessStream fileStream = await modifyImage.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);

                canvas.Width = 1024;
                canvas.Height = 768;
                width = canvas.Width;
                height = canvas.Height;
                imgPreview.Source = bitmapImage;
                img.ImageSource = this.imgPreview.Source;
                canvas.Background = img;
            }
        }

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            ViewImage();
        }

        private async void btnNext_Click(object sender, TappedRoutedEventArgs e)
        {
            await SaveCanvas();
            StorageFile modifyImage = await StorageFile.GetFileFromPathAsync(Data.MODIFYIMAGE.Path);
            await file.CopyAndReplaceAsync(modifyImage);

            this.Frame.Navigate(typeof(Gallery));
        }

        private void btnBack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void btnCrop_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await SaveCanvas();
            this.Frame.Navigate(typeof(CropPage));
        }

        private void cmbResize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = (e.AddedItems[0] as ComboBoxItem).Content.ToString();

            if (selectedValue.Equals("640X480"))
            {
                canvas.Width = 640;
                canvas.Height = 480;
            }
            else if (selectedValue.Equals("1024X768"))
            {
                canvas.Width = 1024;
                canvas.Height = 768;
            }
            else if (selectedValue.Equals("1280X1024"))
            {
                canvas.Width = 1280;
                canvas.Height = 1024;
            }
        }

        private void cmbFillColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillColor = (((e.AddedItems[0] as ComboBoxItem).Content as Rectangle).Fill as SolidColorBrush).Color;
        }

        private void cmbBorderColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BorderColor = (((e.AddedItems[0] as ComboBoxItem).Content as Rectangle).Fill as SolidColorBrush).Color;
        }
    }
}
