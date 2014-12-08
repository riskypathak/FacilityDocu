using System;
using System.IO;
using System.Linq;
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
        InkManager MyInkManager = new InkManager();
        string DrawingTool = "NotSelect";
        double X1, X2, Y1, Y2, StrokeThickness = 1;
        Point PreviousContactPoint, CurrentContactPoint;

        double width = 0, height = 0;

        Line NewLine;
        TextBox NewText;
        Ellipse NewEllipse;
        Rectangle NewRectangle;
        Polyline Pencil;
        UIElement selectedItem;
        Color BorderColor, FillColor;
        uint PenID, TouchID;
        BitmapImage finalImage = new BitmapImage();
        ImageBrush img = new ImageBrush();
        int selectedIndex = 0;
        StorageFile file;

        BitmapImage bitmapImage;
        int redoundoCount = 0;

        public EditPhoto()
        {
            this.InitializeComponent();

            canvas.PointerMoved += canvas_PointerMoved;
            canvas.PointerReleased += canvas_PointerReleased;
            canvas.PointerPressed += canvas_PointerPressed;
            canvas.PointerExited += canvas_PointerExited;
            canvas.PointerEntered += canvas_PointerEntered;

            for (int i = 2; i <= 10; i += 2)
            {
                ComboBoxItem Items = new ComboBoxItem();
                Items.Content = i;
                cmbStrokeThickness.Items.Add(Items);
            }

            cmbStrokeThickness.SelectedIndex = 0;
            cbFillColor.SelectedIndex = 0;
            cbBorderColor.SelectedIndex = 0;
        }
        #region Drawing Tools Click Events

        private void defaultBtnColor()
        {
            Brush bkcolor = new SolidColorBrush(Colors.Silver);
            btnPencil.Background = bkcolor;
            btnLine.Background = bkcolor;
            btnEllipse.Background = bkcolor;
            btnRectangle.Background = bkcolor;
            btnText.Background = bkcolor;

        }
        private void btnPencil_Click(object sender, RoutedEventArgs e)
        {
            defaultBtnColor();
            if (DrawingTool != "Pencil")
            {
                Brush bkcolor = new SolidColorBrush(Colors.Red);
                btnPencil.Background = bkcolor;
                DrawingTool = "Pencil";
                if (redoundoCount != canvas.Children.Count)
                {
                    canvasSet();
                }

            }
            else
            {

                DrawingTool = "NotSelect";

            }


        }

        private void btnLine_Click(object sender, RoutedEventArgs e)
        {

            defaultBtnColor();
            if (DrawingTool != "Line")
            {
                Brush bkcolor = new SolidColorBrush(Colors.Red);
                btnLine.Background = bkcolor;
                DrawingTool = "Line";
                if (redoundoCount != canvas.Children.Count)
                {
                    canvasSet();
                }

            }
            else
            {

                DrawingTool = "NotSelect";

            }
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {

            defaultBtnColor();
            if (DrawingTool != "Ellipse")
            {
                Brush bkcolor = new SolidColorBrush(Colors.Red);
                btnEllipse.Background = bkcolor;
                DrawingTool = "Ellipse";
                if (redoundoCount != canvas.Children.Count)
                {
                    canvasSet();
                }

            }
            else
            {

                DrawingTool = "NotSelect";

            }
        }

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {

            defaultBtnColor();
            if (DrawingTool != "Rectangle")
            {
                Brush bkcolor = new SolidColorBrush(Colors.Red);
                btnRectangle.Background = bkcolor;
                DrawingTool = "Rectangle";
                if (redoundoCount != canvas.Children.Count)
                {
                    canvasSet();
                }

            }
            else
            {

                DrawingTool = "NotSelect";

            }
        }
        private void btnText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            defaultBtnColor();
            if (DrawingTool != "Text")
            {
                Brush bkcolor = new SolidColorBrush(Colors.Red);
                btnText.Background = bkcolor;
                DrawingTool = "Text";
                if (redoundoCount != canvas.Children.Count)
                {
                    canvasSet();
                }

            }
            else
            {

                DrawingTool = "NotSelect";

            }
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
            if (sset == 1 && DrawingTool != "NotSelect")
            {
                xl = X2;
                yl = Y2;
                btn_Redo.IsEnabled = false;
                btn_Undo.IsEnabled = true;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Cross, 1);

                SaveCanvas();
            }
            else
            {
                sset = 0;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            }


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
                            NewLine.ManipulationDelta += MoveableContainer_ManipulationDelta;
                            NewLine.ManipulationMode = ManipulationModes.All;
                            redoundoCount++;



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
                            redoundoCount++;

                            NewRectangle.ManipulationMode = ManipulationModes.All;
                            NewRectangle.ManipulationDelta += MoveableContainer_ManipulationDelta;



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
                            redoundoCount++;
                            NewEllipse.ManipulationDelta += MoveableContainer_ManipulationDelta;
                            NewEllipse.ManipulationMode = ManipulationModes.All;
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
                                    redoundoCount++;

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
                if (DrawingTool == "NotSelect")
                {
                    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                    //  moveChildrens(e);
                }
                else
                    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Cross, 1);

                finalPoint(e);
            }
            catch
            {

            }
        }
        void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

            if (DrawingTool == "NotSelect")
            {
                sset = 0;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);

            }
            else if (DrawingTool == "Text")
            {
                NewText = new TextBox();
                NewText.Foreground = new SolidColorBrush(BorderColor);
                NewText.Background = new SolidColorBrush(FillColor);
                NewText.FontSize = 15 + (StrokeThickness - 2) * 2;
                NewText.Margin = new Thickness(e.GetCurrentPoint(canvas).Position.X, e.GetCurrentPoint(canvas).Position.Y, 0, 0);
                NewText.AcceptsReturn = true;
                canvas.Children.Add(NewText);
                NewText.ManipulationMode = ManipulationModes.All;
                NewText.ManipulationDelta += MoveableContainer_ManipulationDelta;
                redoundoCount++;
                defaultBtnColor();
                DrawingTool = "NotSelect";

            }
            else
            {
                sset = 1;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Cross, 1);
            }

            xCor = e.GetCurrentPoint(canvas).Position.X;
            yCor = e.GetCurrentPoint(canvas).Position.Y;
            initialPoint(e);

        }

        private UIElement FindCanvasChild(DependencyObject dependencyObject)
        {
            // throw new NotImplementedException();
            while (dependencyObject != null)
            {
                // If the current object is a UIElement which is a child of the
                // Canvas, exit the loop and return it.
                UIElement elem = dependencyObject as UIElement;
                if (elem != null && canvas.Children.Contains(elem))
                    break;

                // VisualTreeHelper works with objects of type Visual or Visual3D.
                // If the current object is not derived from Visual or Visual3D,
                // then use the LogicalTreeHelper to find the parent element.
                //if (dependencyObject is Visual || depObj is Visual3D)
                //    depObj = VisualTreeHelper.GetParent(depObj);
                //else
                //    depObj = LogicalTreeHelper.GetParent(depObj);
            }
            return dependencyObject as UIElement;
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

            if (sset == 1 && DrawingTool != "NotSelect")
            {
                xl = X2;
                yl = Y2;
                btn_Redo.IsEnabled = false;
                btn_Undo.IsEnabled = true;

            }
            TouchID = 0;
            PenID = 0;
            e.Handled = true;
            Pencil = null;
            NewLine = null;
            NewRectangle = null;
            NewEllipse = null;
            SaveCanvas();
            sset = 0;
        }
        #endregion

        private void cmbStrokeThickness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StrokeThickness = Convert.ToDouble((e.AddedItems[0] as ComboBoxItem).Content);
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


            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap
                bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                imgPreview.Source = bitmapImage;
            }
        }

        private async Task<RenderTargetBitmap> CanvasToBMP()
        {
            // Initialization.  
            Size canvasSize = this.canvas.RenderSize;
            Point defaultPoint = this.canvas.RenderTransformOrigin;
            // Sezing to output image dimension.  
            this.canvasMain.Measure(canvasSize);
            this.canvasMain.UpdateLayout();
            this.canvasMain.Arrange(new Rect(defaultPoint, canvasSize));
            // Convert canvas to bmp.  
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(this.canvasMain);


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

            IStorageFolder backupFolder = await StorageFolder.GetFolderFromPathAsync(Data.BackupPath);
            await file.CopyAsync(backupFolder, file.Name, NameCollisionOption.ReplaceExisting);

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

            width = canvas.Width;
            height = canvas.Height;
        }

        private void cmbFillColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillColor = (((e.AddedItems[0] as ComboBoxItem).Content as Rectangle).Fill as SolidColorBrush).Color;
        }

        private void cmbBorderColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BorderColor = (((e.AddedItems[0] as ComboBoxItem).Content as Rectangle).Fill as SolidColorBrush).Color;

        }

        private void canvasSet()
        {
            while (canvas.Children.Count > redoundoCount)
            {
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            redoundoCount = canvas.Children.Count;
        }
        private void btn_Redo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SaveCanvas();
            defaultBtnColor();
            DrawingTool = "NotSelect";
            if (canvas.Children.Count != redoundoCount)
            {
                btn_Undo.IsEnabled = true;

                canvas.Children.Cast<UIElement>().ElementAt(redoundoCount).Visibility = Visibility.Visible;
                redoundoCount++;

                if (canvas.Children.Count == redoundoCount)
                {
                    btn_Redo.IsEnabled = false;
                }
            }
        }

        private void btn_Undo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SaveCanvas();
            defaultBtnColor();
            DrawingTool = "NotSelect";
            if (redoundoCount != 0)
            {
                btn_Redo.IsEnabled = true;
                redoundoCount--;
                canvas.Children.Cast<UIElement>().ElementAt(redoundoCount).Visibility = Visibility.Collapsed;

                if (redoundoCount == 0)
                {
                    btn_Undo.IsEnabled = false;
                }
            }
        }

        private void MoveableContainer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            if (DrawingTool == "NotSelect")
            {

                selectedItem = e.Container;
                var x = Canvas.GetLeft(selectedItem);
                var y = Canvas.GetTop(selectedItem);
                Canvas.SetLeft(selectedItem, x + e.Delta.Translation.X);
                Canvas.SetTop(selectedItem, y + e.Delta.Translation.Y);

            }

        }

        private void canvas_PointerMoved_1(object sender, PointerRoutedEventArgs e)
        {
        }
        int degree = 0;
        private void imageRotate()
        {
            canvasMain.Width = canvas.Width;
            canvasMain.Height = canvas.Height;
            RotateTransform rotateTransform2 = new RotateTransform();
            rotateTransform2.Angle = degree;
            canvas.RenderTransform = rotateTransform2;
        }

        private void btn_RotateLeft_Tapped(object sender, TappedRoutedEventArgs e)
        {
            degree = degree - 90;
            imageRotate();
           
        }

        private void btn_RotateRight_Tapped(object sender, TappedRoutedEventArgs e)
        {
            degree = degree + 90;
            imageRotate();
        }

       
       


    }
}
