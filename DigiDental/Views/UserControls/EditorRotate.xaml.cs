using DigiDental.Class;
using DigiDental.ViewModels.Class;
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
    /// EditorRotate.xaml 的互動邏輯
    /// </summary>
    public partial class EditorRotate : UserControl
    {
        public ObservableCollection<ImageInfo> ImagesCollection { get; set; }
        public ImageInfo ImageInfo { get; set; }
        
        public EditorRotate(ObservableCollection<ImageInfo> imagesCollection, ImageInfo imageInfo)
        {
            InitializeComponent();

            ImagesCollection = imagesCollection;
            ImageInfo = imageInfo;
        }
        

        private Image ImageRotated;

        BitmapImage bi;
        /// <summary>
        /// 高的比例
        /// </summary>
        private double heightRatio = 0;
        /// <summary>
        /// 寬的比例
        /// </summary>
        private double widthRatio = 0;
        private Path path;
        private CombinedGeometry combinedGeometry;
        /// <summary>
        /// CombinedGeometry.Geometry2 Setting
        /// </summary>
        private RectangleGeometry rectangleGeometry2;
        /// <summary>
        /// Path選取的第二Rect 透明區塊
        /// </summary>
        /// 
        private Rect rect2;
        double imageCenterX;
        double imageCenterY;
        double imageStartX;
        double imageStartY;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetImageDefault(ImageInfo);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("確定完成旋轉並儲存?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                try
                {
                    double ratio = heightRatio == 0 ? widthRatio : heightRatio;

                    double oriWidth = rect2WAndH[0] / ratio;
                    double oriHeight = rect2WAndH[1] / ratio;

                    double sourceCenX = bi.PixelWidth / 2;
                    double sourceCenY = bi.PixelHeight / 2;

                    double oriStartX = sourceCenX - (oriWidth / 2);
                    double oriStartY = sourceCenY - (oriHeight / 2);

                    RotateAndSaveImage(bi, rotateAngle, (int)oriStartX, (int)oriStartY, (int)oriWidth, (int)oriHeight, ImageInfo.Image_FullPath);
                    MessageBox.Show("儲存成功", "提示", MessageBoxButton.OK);

                    Cvs.Children.Clear();
                    SetImageDefault(ImageInfo);
                }
                catch (Exception ex)
                {
                    Error_Log.ErrorMessageOutput(ex.ToString());
                }
            }
        }

        private void Button_Undo_Click(object sender, RoutedEventArgs e)
        {
            Cvs.Children.Clear();
            SetImageDefault(ImageInfo);
        }

        private void Button_ExitEditor_Click(object sender, RoutedEventArgs e)
        {
            BaseAdjust ba = new BaseAdjust(ImagesCollection, ImageInfo);
            Content = ba;

            GC.Collect();
        }

        private RotateTransform rtf;
        private double rotateAngle;
        private double[] rect2WAndH;
        double rect2StartX;
        double rect2StartY;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                rotateAngle = e.NewValue;

                if (rotateAngle != 0)
                {
                    ButtonSave.IsEnabled = true;
                    ButtonUndo.IsEnabled = true;
                }

                rtf = new RotateTransform();
                rtf.Angle = rotateAngle;
                ImageRotated.RenderTransformOrigin = new Point(0.5, 0.5);
                ImageRotated.RenderTransform = rtf;

                imageCenterX = Cvs.ActualWidth / 2;
                imageCenterY = Cvs.ActualHeight / 2;

                rect2WAndH = GetLargestRectangle(ImageRotated.Width, ImageRotated.Height, rotateAngle);

                double halfWidth = rect2WAndH[0] / 2;
                double halfHeight = rect2WAndH[1] / 2;

                rect2StartX = imageCenterX - halfWidth;
                rect2StartY = imageCenterY - halfHeight;

                //XOR的區域
                rect2 = new Rect(rect2StartX, rect2StartY, rect2WAndH[0], rect2WAndH[1]);
                //CombinedGeometry.Geometry2 Setting
                rectangleGeometry2 = new RectangleGeometry(rect2);
                //Path 的 CombinedGeometry
                combinedGeometry.Geometry2 = rectangleGeometry2;
                path.Data = combinedGeometry;
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        /// <summary>
        /// 取得旋轉中的框框
        /// </summary>
        /// <param name="imageWidth">image寬</param>
        /// <param name="imageHeight">image長</param>
        /// <param name="rotAngDeg">旋轉角度</param>
        /// <returns>回傳框框長寬</returns>
        public double[] GetLargestRectangle(double imageWidth, double imageHeight, double rotAngDeg)
        {
            double angle = Math.PI * rotAngDeg / 180;
            double minSide, maxSide;
            if (imageWidth <= imageHeight)
            {
                minSide = imageWidth;
                maxSide = imageHeight;
            }
            else
            {
                minSide = imageHeight;
                maxSide = imageWidth;
            }

            double absAngle = Math.Abs(angle);

            double sinAngle = Math.Sin(absAngle);
            double cosAngle = Math.Cos(absAngle);
            double maxOuter = sinAngle * minSide + cosAngle * maxSide;
            double minOuter = sinAngle * maxSide + cosAngle * minSide;
            double sinAcosA = sinAngle * cosAngle;
            double c = maxSide * sinAcosA / (2 * maxSide * sinAcosA + minSide);
            double minInner = minOuter - (2 * minOuter * c);
            double maxInner = maxOuter - (2 * maxOuter * c);

            double w, h;

            if (imageWidth <= imageHeight)
            {
                w = minInner;
                h = maxInner;
            }
            else
            {
                w = maxInner;
                h = minInner;
            }

            double[] wh = new double[2] { w, h };
            return wh;
        }

        public void RotateAndSaveImage(BitmapImage sourceImage,double angle,
                              int startX,int startY,int width,int height,
                              string filePath)
        {
            try
            {
                TransformGroup transformGroup = new TransformGroup();
                RotateTransform rotateTransform = new RotateTransform(angle);
                rotateTransform.CenterX = sourceImage.PixelWidth / 2.0;
                rotateTransform.CenterY = sourceImage.PixelHeight / 2.0;
                transformGroup.Children.Add(rotateTransform);
                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X = -startX;
                translateTransform.Y = -startY;
                transformGroup.Children.Add(translateTransform);

                DrawingVisual vis = new DrawingVisual();
                DrawingContext cont = vis.RenderOpen();
                cont.PushTransform(transformGroup);
                cont.DrawImage(sourceImage, new Rect(new Size(sourceImage.PixelWidth, sourceImage.PixelHeight)));
                cont.Close();

                RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
                rtb.Render(vis);

                System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private void SetImageDefault(ImageInfo imageInfo)
        {
            try
            {
                bi = new LoadBitmapImage().SettingBitmapImage(ImageInfo.Image_FullPath, 0);
                ImageRotated = new Image()
                {
                    Source = bi
                };

                //Control Image (X,Y)
                imageStartX = 0;
                imageStartY = 0;

                if (bi.PixelHeight < Cvs.ActualHeight && bi.PixelWidth < Cvs.ActualWidth)
                {
                    heightRatio = 1;
                    widthRatio = 1;
                    ImageRotated.MaxHeight = bi.PixelHeight;
                    ImageRotated.Height = ImageRotated.MaxHeight;
                    ImageRotated.MaxWidth = bi.PixelWidth;
                    ImageRotated.Width = ImageRotated.MaxWidth;
                    imageStartX = (Cvs.ActualWidth - bi.PixelWidth) / 2;
                    imageStartY = (Cvs.ActualHeight - bi.PixelHeight) / 2;
                }
                else
                {
                    //Canvas 的寬高關係
                    if (Cvs.ActualWidth > Cvs.ActualHeight)
                    {
                        //高的比例
                        heightRatio = Cvs.ActualHeight / bi.PixelHeight;
                        //高填滿
                        ImageRotated.MaxHeight = Cvs.ActualHeight;
                        ImageRotated.Height = ImageRotated.MaxHeight;
                        //計算寬比例
                        ImageRotated.Width = bi.PixelWidth * heightRatio;

                        //如果圖片是橫的 但是寬度超過CVS
                        if (ImageRotated.Width > Cvs.ActualWidth)
                        {
                            widthRatio = Cvs.ActualWidth / bi.PixelWidth;
                            ImageRotated.MaxWidth = Cvs.ActualWidth;
                            ImageRotated.Width = ImageRotated.MaxWidth;
                            ImageRotated.Height = bi.PixelHeight * widthRatio;

                            //Image 起始座標 高度置中
                            imageStartX = 0;
                            imageStartY = (Cvs.ActualHeight - ImageRotated.Height) / 2;
                        }
                        else
                        {
                            //Image 起始座標 寬度置中
                            imageStartX = (Cvs.ActualWidth - ImageRotated.Width) / 2;
                            imageStartY = 0;
                        }
                    }
                    else
                    {
                        //寬的比例
                        widthRatio = Cvs.ActualWidth / bi.PixelWidth;
                        //寬填滿
                        ImageRotated.MaxWidth = Cvs.ActualWidth;
                        ImageRotated.Width = ActualWidth;
                        //計算高比例
                        ImageRotated.Height = bi.PixelHeight * widthRatio;
                        if (ImageRotated.Height > Cvs.ActualHeight)
                        {
                            heightRatio = Cvs.ActualHeight / bi.PixelHeight;
                            ImageRotated.MaxHeight = Cvs.ActualHeight;
                            ImageRotated.Height = ImageRotated.MaxHeight;
                            ImageRotated.Width = bi.PixelWidth * heightRatio;

                            //Image 起始座標 寬度置中
                            imageStartX = (Cvs.ActualWidth - ImageRotated.Width) / 2;
                            imageStartY = 0;
                        }
                        else
                        {
                            //Image 起始座標 高度置中
                            imageStartX = 0;
                            imageStartY = (Cvs.ActualHeight - ImageRotated.Height) / 2;
                        }
                    }
                }

                Cvs.Children.Add(ImageRotated);

                Canvas.SetTop(ImageRotated, imageStartY);
                Canvas.SetLeft(ImageRotated, imageStartX);

                //CombinedGeometry.Geometry1 Setting
                //包圍Image的Rect(反灰)
                RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(0, 0, Cvs.ActualWidth, Cvs.ActualHeight));

                rect2 = new Rect(imageStartX, imageStartY, ImageRotated.Width, ImageRotated.Height);
                //CombinedGeometry.Geometry2 Setting
                //包圍Image的Rect(透明區)初始與圖片一樣
                rectangleGeometry2 = new RectangleGeometry(rect2);

                //Path 的 CombinedGeometry
                combinedGeometry = new CombinedGeometry()
                {
                    GeometryCombineMode = GeometryCombineMode.Xor,
                    Geometry1 = rectangleGeometry,
                    Geometry2 = rectangleGeometry2
                };

                //建立Rect 覆蓋Image
                path = new Path
                {
                    Fill = (Brush)(new BrushConverter().ConvertFromString("#AA000000")),
                    Data = combinedGeometry
                };

                Cvs.Children.Add(path);

                ButtonSave.IsEnabled = false;
                ButtonUndo.IsEnabled = false;

                RotateAngle.Value = 0;
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }
    }
}
