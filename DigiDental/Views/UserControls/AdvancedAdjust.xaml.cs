using DigiDental.Class;
using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.UserControlViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// AdvancedAdjust.xaml 的互動邏輯
    /// </summary>
    public partial class AdvancedAdjust : UserControl
    {
        public ObservableCollection<ImageInfo> ImagesCollection { get; set; }
        //public ImageInfo ImageInfo
        //{
        //    get { return aavm.ImageInfo; }
        //    set { aavm.ImageInfo = value; }
        //}

        public ImageInfo ImageInfo{ get; set; }

        private Image i;
        private Rectangle r;
        private RotateTransform rt;
        private AdvancedAdjustViewModel aavm;
        public AdvancedAdjust(ObservableCollection<ImageInfo> imagesCollection, ImageInfo imageInfo)
        {
            InitializeComponent();

            if (aavm == null)
            {
                aavm = new AdvancedAdjustViewModel();
            }

            ImagesCollection = imagesCollection;

            ImageInfo = imageInfo;

            DataContext = aavm;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            BaseAdjust ba = new BaseAdjust(ImagesCollection, ImageInfo);
            Content = ba;
        }
        private void Button_Undo_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            BaseAdjust ba = new BaseAdjust(ImagesCollection, ImageInfo);
            Content = ba;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            double av = e.NewValue;

            rt = new RotateTransform();
            rt.Angle = av;

            i.RenderTransformOrigin = new Point(0.5, 0.5);
            i.RenderTransform = rt;

            newHeight = i.ActualHeight;
            newWidth = i.ActualWidth;


            var quadrant = (int)(Math.Floor(av / (Math.PI / 2))) & 3;
            var sign_alpha = (quadrant & 1) == 0 ? av : Math.PI - av;
            var alpha = (sign_alpha % Math.PI + Math.PI) % Math.PI;

            double w = newWidth * Math.Cos(alpha) + newHeight * Math.Sin(alpha);
            double h = newWidth * Math.Sin(alpha) + newHeight * Math.Cos(alpha);

            var gamma = newWidth < newHeight ? Math.Atan2(w, h) : Math.Atan2(h, w);

            var delta = Math.PI - alpha - gamma;

            var length = newWidth < newHeight ? newHeight : newWidth;
            var d = length * Math.Cos(alpha);
            var a = d * Math.Sin(alpha) / Math.Sin(delta);

            var y = a * Math.Cos(gamma);
            var x = y * Math.Tan(gamma);

            //return {
            //    x: x,
            //    y: y,
            //    w: bb.w - 2 * x,
            //    h: bb.h - 2 * y
            //};

            r.Stroke = Brushes.White;
            r.StrokeThickness = 1;
            r.Width = w - 2 * x;
            r.Height = h - 2 * y;




            //cvs.Children.Add(r);
            Canvas.SetTop(r, ((cvs.ActualHeight - h) / 2) + x);
            Canvas.SetLeft(r, ((cvs.ActualWidth - w) / 2) + y);
            //aavm.AngleValue = av;

            //lbSize.Content = cvs.ActualWidth + "/" + cvs.ActualHeight;
        }

        private void Button_TEST_Click(object sender, RoutedEventArgs e)
        {
            //Canvas c = new Canvas();
            //c = cvs;
            //Rect x = new Rect();
            //x = GetBounds(ii, cvs);
            //var myRect = new System.Windows.Shapes.Rectangle();
            //myRect.Stroke = System.Windows.Media.Brushes.Black;
            //myRect.Fill = System.Windows.Media.Brushes.SkyBlue;
            //myRect.HorizontalAlignment = HorizontalAlignment.Left;
            //myRect.VerticalAlignment = VerticalAlignment.Center;
            //myRect.Height = x.Height;
            //myRect.Width = x.Width;
            
            //cvs.Children.Add(myRect);
            //Canvas.SetTop(ii, 0);
            //Canvas.SetLeft(ii, 0);
        }
        public Rect GetBounds(FrameworkElement of, FrameworkElement from)
        {
            // Might throw an exception if of and from are not in the same visual tree
            GeneralTransform transform = of.TransformToVisual(from);

            return transform.TransformBounds(new Rect(0, 0, of.ActualWidth, of.ActualHeight));
        }

        private double oriWidth;
        private double oriHeight;
        private double newWidth;
        private double newHeight;

        private Point ImageStartPoint;
        private Point ImageEndPoint;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi = new LoadBitmapImage().SettingBitmapImage(ImageInfo.Image_FullPath, 0);

            Border border = new Border();
            if (bi.PixelWidth > bi.PixelHeight)
            {
                border.MaxWidth = (cvs.ActualWidth / 5) * 3;
                border.Width = border.MaxWidth;
                border.Height = (border.MaxWidth / bi.PixelWidth) * bi.PixelHeight;
            }
            else
            {
                border.MaxHeight = (cvs.ActualHeight / 5) * 3;
                border.Height = border.MaxHeight;
                border.Width = (border.MaxHeight / bi.PixelHeight) * bi.PixelWidth;
            }

            oriHeight = border.Height;
            oriWidth = border.Width;

            i = new Image();
            i.Source = bi;
            border.Child = i;

            double imageStartX = (cvs.ActualWidth - border.Width) / 2;
            double imageStartY = (cvs.ActualHeight - border.Height) / 2;

            ImageStartPoint = new Point(imageStartX, imageStartY);
            ImageEndPoint = new Point(imageStartX + border.Width, imageStartY + border.Height);

            cvs.Children.Add(border);

            Canvas.SetTop(border, imageStartY);
            Canvas.SetLeft(border, imageStartX);
            
            r = new Rectangle
            {
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Width = border.Width,
                Height = border.Height
            };
            cvs.Children.Add(r);

            Canvas.SetTop(r, imageStartY);
            Canvas.SetLeft(r, imageStartX);
        }

        private Point CropStart;
        private Point CropEnd;
        private Rectangle CropRectangle;
        private bool IsCropIng = false;
        private void Cvs_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((bool)ButtonCrop.IsChecked)
            {
                CropStart = e.GetPosition(cvs);
                if (CropStart.X > ImageStartPoint.X &&
                    CropStart.X < ImageEndPoint.X &&
                    CropStart.Y > ImageStartPoint.Y &&
                    CropStart.Y < ImageEndPoint.Y)
                {

                    IsCropIng = true;
                    if (CropRectangle != null)
                    {
                        cvs.Children.Remove(CropRectangle);
                        CropRectangle = null;
                    }

                    CropRectangle = new Rectangle()
                    {
                        Stroke = Brushes.Red,
                        StrokeThickness = 1
                    };
                    cvs.Children.Add(CropRectangle);
                    Canvas.SetTop(CropRectangle, CropStart.Y);
                    Canvas.SetLeft(CropRectangle, CropStart.X);
                }
            }
        }

        private void Cvs_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if ((bool)ButtonCrop.IsChecked && IsCropIng)
                {
                    Point movingPoint = e.GetPosition(cvs);
                    double cropWidth;
                    double cropHeight;
                    if (movingPoint.X > CropStart.X)
                    {
                        if (movingPoint.X < ImageEndPoint.X)
                        {
                            cropWidth = movingPoint.X - CropStart.X;
                        }
                        else
                        {
                            cropWidth = ImageEndPoint.X - CropStart.X;
                        }
                        Canvas.SetLeft(CropRectangle, CropStart.X);
                    }
                    else
                    {
                        if (movingPoint.X > ImageStartPoint.X)
                        {
                            cropWidth = CropStart.X - movingPoint.X;
                            Canvas.SetLeft(CropRectangle, movingPoint.X);
                        }
                        else
                        {
                            cropWidth = CropStart.X - ImageStartPoint.X;
                            Canvas.SetRight(CropRectangle, CropStart.X);
                        }
                    }
                    if (movingPoint.Y > CropStart.Y)
                    {
                        if (movingPoint.Y < ImageEndPoint.Y)
                        {
                            cropHeight = movingPoint.Y - CropStart.Y;
                        }
                        else
                        {
                            cropHeight = ImageEndPoint.Y - CropStart.Y;
                        }
                        Canvas.SetTop(CropRectangle, CropStart.Y);
                    }
                    else
                    {
                        if (movingPoint.Y > ImageStartPoint.Y)
                        {
                            cropHeight = CropStart.Y - movingPoint.Y;
                            Canvas.SetTop(CropRectangle, movingPoint.Y);
                        }
                        else
                        {
                            cropHeight = CropStart.Y - ImageStartPoint.Y;
                            Canvas.SetBottom(CropRectangle, CropStart.Y);
                        }
                    }
                    CropRectangle.Width = cropWidth;
                    CropRectangle.Height = cropHeight;
                }
            }
        }

        private void Cvs_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((bool)ButtonCrop.IsChecked)
            {
                CropEnd = e.GetPosition(cvs);
                IsCropIng = false;
            }
        }
    }
}
