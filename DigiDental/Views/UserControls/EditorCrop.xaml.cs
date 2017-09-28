using DigiDental.Class;
using DigiDental.ViewModels.Class;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// EditorCrop.xaml 的互動邏輯
    /// </summary>
    public partial class EditorCrop : UserControl
    {
        public ObservableCollection<ImageInfo> ImagesCollection { get; set; }
        public ImageInfo ImageInfo { get; set; }
        
        public EditorCrop(ObservableCollection<ImageInfo> imagesCollection, ImageInfo imageInfo)
        {
            InitializeComponent();

            ImagesCollection = imagesCollection;
            ImageInfo = imageInfo;
        }

        private BitmapImage bi;
        /// <summary>
        /// 高的比例
        /// </summary>
        private double heightRatio = 0;
        /// <summary>
        /// 寬的比例
        /// </summary>
        private double widthRatio = 0;
        /// <summary>
        /// Control Image in Canvas
        /// </summary>
        private Image image;
        //圖片的起始(x1,y1)與結束(x2,y2)
        private Point ImageStartPoint;
        private Point ImageEndPoint;

        private Path path;
        private CombinedGeometry combinedGeometry;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Default Photo
            bi = new LoadBitmapImage().SettingBitmapImage(ImageInfo.Image_FullPath, 0);

            SettingImage(bi);
        }

        private void SettingImage(BitmapSource bs)
        {
            try
            {
                image = new Image()
                {
                    Source = bs
                };

                //Control Image (X,Y)
                double imageStartX = 0;
                double imageStartY = 0;

                if (bi.PixelHeight < Cvs.ActualHeight && bi.PixelWidth < Cvs.ActualWidth)
                {
                    heightRatio = 1;
                    widthRatio = 1;
                    image.MaxHeight = bi.PixelHeight;
                    image.Height = image.MaxHeight;
                    image.MaxWidth = bi.PixelWidth;
                    image.Width = image.MaxWidth;
                    imageStartX = (Cvs.ActualWidth - bi.PixelWidth) / 2;
                    imageStartY = (Cvs.ActualHeight - bi.PixelHeight) / 2;
                }
                else
                {
                    //Canvas 的寬高關係
                    if (Cvs.ActualWidth > Cvs.ActualHeight)
                    {
                        //高的比例
                        heightRatio = Cvs.ActualHeight / bs.PixelHeight;
                        //高填滿
                        image.MaxHeight = Cvs.ActualHeight;
                        image.Height = image.MaxHeight;
                        //計算寬比例
                        image.Width = bs.PixelWidth * heightRatio;

                        //如果圖片是橫的 但是寬度超過CVS
                        if (image.Width > Cvs.ActualWidth)
                        {
                            widthRatio = Cvs.ActualWidth / bs.PixelWidth;
                            image.MaxWidth = Cvs.ActualWidth;
                            image.Width = image.MaxWidth;
                            image.Height = bs.PixelHeight * widthRatio;

                            //Image 起始座標 高度置中
                            imageStartX = 0;
                            imageStartY = (Cvs.ActualHeight - image.Height) / 2;
                        }
                        else
                        {
                            //Image 起始座標 寬度置中
                            imageStartX = (Cvs.ActualWidth - image.Width) / 2;
                            imageStartY = 0;
                        }
                    }
                    else
                    {
                        //寬的比例
                        widthRatio = Cvs.ActualWidth / bs.PixelWidth;
                        //寬填滿
                        image.MaxWidth = Cvs.ActualWidth;
                        image.Width = ActualWidth;
                        //計算高比例
                        image.Height = bs.PixelHeight * widthRatio;
                        if (image.Height > Cvs.ActualHeight)
                        {
                            heightRatio = Cvs.ActualHeight / bs.PixelHeight;
                            image.MaxHeight = Cvs.ActualHeight;
                            image.Height = image.MaxHeight;
                            image.Width = bs.PixelWidth * heightRatio;

                            //Image 起始座標 寬度置中
                            imageStartX = (Cvs.ActualWidth - image.Width) / 2;
                            imageStartY = 0;
                        }
                        else
                        {
                            //Image 起始座標 高度置中
                            imageStartX = 0;
                            imageStartY = (Cvs.ActualHeight - image.Height) / 2;
                        }
                    }
                }
                //圖片的起始(x1,y1)與結束(x2,y2)
                ImageStartPoint = new Point(imageStartX, imageStartY);
                ImageEndPoint = new Point(imageStartX + image.Width, imageStartY + image.Height);

                Cvs.Children.Add(image);
                Canvas.SetTop(image, imageStartY);
                Canvas.SetLeft(image, imageStartX);

                //CombinedGeometry.Geometry1 Setting
                //包圍Image的Rect(反灰)
                RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(imageStartX, imageStartY, image.Width, image.Height));

                //Path 的 CombinedGeometry
                combinedGeometry = new CombinedGeometry()
                {
                    GeometryCombineMode = GeometryCombineMode.Xor,
                    Geometry1 = rectangleGeometry
                };

                //建立Rect 覆蓋Image
                path = new Path
                {
                    Fill = (Brush)(new BrushConverter().ConvertFromString("#AAFFFFFF")),
                    Data = combinedGeometry
                };
                Cvs.Children.Add(path);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        /// <summary>
        /// 剪裁模式
        /// </summary>
        private bool IsCropMode = false;
        /// <summary>
        /// 剪裁起點
        /// </summary>
        private Point CropStart;
        /// <summary>
        /// 剪裁終點
        /// </summary>
        private Point CropEnd;
        /// <summary>
        /// CombinedGeometry.Geometry2 Setting
        /// </summary>
        private RectangleGeometry rectangleGeometry2;
        /// <summary>
        /// Path選取的第二Rect 透明區塊
        /// </summary>
        private Rect rect2;
        private void Cvs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CropStart = e.GetPosition(Cvs);
            if (CropStart.X > ImageStartPoint.X && CropStart.X < ImageEndPoint.X &&
                CropStart.Y > ImageStartPoint.Y && CropStart.Y < ImageEndPoint.Y)
            {
                //剪裁模式
                IsCropMode = true;
                //CombinedGeometry.Geometry2 Setting
                rectangleGeometry2 = new RectangleGeometry();
                //Path Rect2 initial
                rect2 = new Rect(CropStart.X, CropStart.Y, 0, 0);
            }
        }

        //剪下的寬高
        double cropWidth;
        double cropHeight;

        private void Cvs_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //剪裁模式
                if (IsCropMode)
                {
                    Point movingPoint = e.GetPosition(Cvs);
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
                        rect2.X = CropStart.X;
                    }
                    else
                    {
                        if (movingPoint.X > ImageStartPoint.X)
                        {
                            cropWidth = CropStart.X - movingPoint.X;
                            rect2.X = movingPoint.X;
                        }
                        else
                        {
                            cropWidth = CropStart.X - ImageStartPoint.X;
                            rect2.X = ImageStartPoint.X;
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
                        rect2.Y = CropStart.Y;
                    }
                    else
                    {
                        if (movingPoint.Y > ImageStartPoint.Y)
                        {
                            cropHeight = CropStart.Y - movingPoint.Y;
                            rect2.Y = movingPoint.Y;
                        }
                        else
                        {
                            cropHeight = CropStart.Y - ImageStartPoint.Y;
                            rect2.Y = ImageStartPoint.Y;
                        }
                    }

                    rect2.Width = cropWidth;
                    rect2.Height = cropHeight;
                    rectangleGeometry2.Rect = rect2;
                    combinedGeometry.Geometry2 = rectangleGeometry2;
                    path.Data = combinedGeometry;
                }
            }
        }

        private void Cvs_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (IsCropMode)
                {
                    CropEnd = e.GetPosition(Cvs);
                    if (CropEnd.X > ImageEndPoint.X)
                    {
                        CropEnd.X = ImageEndPoint.X;
                    }
                    if (CropEnd.X < ImageStartPoint.X)
                    {
                        CropEnd.X = ImageStartPoint.X;
                    }
                    if (CropEnd.Y > ImageEndPoint.Y)
                    {
                        CropEnd.Y = ImageEndPoint.Y;
                    }
                    if (CropEnd.Y < ImageStartPoint.Y)
                    {
                        CropEnd.Y = ImageStartPoint.Y;
                    }
                    
                    IsCropMode = false;

                    Cvs.Children.Remove(image);
                    Cvs.Children.Remove(path);
                    //切割圖片
                    ImageSource imageSource = image.Source;
                    System.Drawing.Bitmap bitmap = ImageProcess.ImageSourceToBitmap(imageSource);
                    BitmapSource bitmapSource = ImageProcess.BitmapToBitmapImage(bitmap);
                    double ratio = heightRatio > 0 ? heightRatio : widthRatio;
                    BitmapSource newBitmapSource = ImageProcess.CutImage(bitmapSource, new Int32Rect((int)((rect2.Left - ImageStartPoint.X) / ratio), (int)((rect2.Top - ImageStartPoint.Y) / ratio), (int)(cropWidth / ratio), (int)(cropHeight / ratio)));
                    
                    SettingImage(newBitmapSource);

                    //執行過編輯 可以復原
                    buttonUndo.IsEnabled = true;
                    //執行過編輯 可以儲存
                    buttonSure.IsEnabled = true;
                }
            }
        }

        private void Button_Sure_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("確定完成裁切並儲存?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                try
                {
                    switch (ImageInfo.Image_Extension.ToUpper())
                    {
                        case ".JPG":
                        case ".JPEG":
                            JpegBitmapEncoder jbe = new JpegBitmapEncoder();
                            ImageProcess.SaveUsingEncoder((BitmapSource)image.Source, ImageInfo.Image_FullPath, jbe);
                            break;
                        case ".PNG":
                            PngBitmapEncoder pbe = new PngBitmapEncoder();
                            ImageProcess.SaveUsingEncoder((BitmapSource)image.Source, ImageInfo.Image_FullPath, pbe);
                            break;
                        case ".GIF":
                            GifBitmapEncoder gbe = new GifBitmapEncoder();
                            ImageProcess.SaveUsingEncoder((BitmapSource)image.Source, ImageInfo.Image_FullPath, gbe);
                            break;
                    }
                    //編輯過 可以復原
                    buttonUndo.IsEnabled = false;
                    //未執行過編輯 不可以儲存
                    buttonSure.IsEnabled = false;
                    MessageBox.Show("儲存成功", "提示", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    Error_Log.ErrorMessageOutput(ex.ToString());
                    MessageBox.Show("寫入失敗", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Undo_Click(object sender, RoutedEventArgs e)
        {
            Cvs.Children.Remove(image);
            Cvs.Children.Remove(path);
            //Default Photo
            bi = new LoadBitmapImage().SettingBitmapImage(ImageInfo.Image_FullPath, 0);
            SettingImage(bi);

            //已復原 不能再復原
            buttonUndo.IsEnabled = false;
            //未執行過編輯 不可以儲存
            buttonSure.IsEnabled = false;
        }

        private void Button_ExitEditor_Click(object sender, RoutedEventArgs e)
        {
            //未執行過編輯 不可以儲存
            buttonSure.IsEnabled = false;

            BaseAdjust ba = new BaseAdjust(ImagesCollection, ImageInfo);
            Content = ba;
        }
    }
}
