using DigiDental.Class;
using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.UserControlViewModels;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// BaseAdjust.xaml 的互動邏輯
    /// </summary>
    public partial class BaseAdjust : UserControl
    {
        public ObservableCollection<ImageInfo> ImagesCollection { get; set; }
        public ImageInfo ImageInfo
        {
            get { return bavm.ImageInfo; }
            set { bavm.ImageInfo = value; }
        }
        private BitmapSource BitmapSource
        {
            get { return bavm.BitmapSource; }
            set { bavm.BitmapSource = value; }
        }
        private BaseAdjustViewModel bavm;
        TransformedBitmap tb;
        ScaleTransform st;
        double scaleValue = 1;
        public BaseAdjust(ObservableCollection<ImageInfo> imagesCollection, ImageInfo imageInfo)
        {
            InitializeComponent();

            if (bavm == null)
            {
                bavm = new BaseAdjustViewModel();
            }

            ImagesCollection = imagesCollection;

            ImageInfo = imageInfo;

            DataContext = bavm;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (ImageInfo.Image_Extension.ToUpper())
                {
                    case ".JPG":
                    case ".JPEG":
                        JpegBitmapEncoder jbe = new JpegBitmapEncoder();
                        ImageProcess.SaveUsingEncoder(BitmapSource, ImageInfo.Image_FullPath, jbe);
                        break;
                    case ".PNG":
                        PngBitmapEncoder pbe = new PngBitmapEncoder();
                        ImageProcess.SaveUsingEncoder(BitmapSource, ImageInfo.Image_FullPath, pbe);
                        break;
                    case ".GIF":
                        GifBitmapEncoder gbe = new GifBitmapEncoder();
                        ImageProcess.SaveUsingEncoder(BitmapSource, ImageInfo.Image_FullPath, gbe);
                        break;
                }
                MessageBox.Show("儲存成功", "提示", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("寫入失敗", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".png";
                sfd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
                if (sfd.ShowDialog() == true)
                {
                    string extension = sfd.SafeFileName.Substring(sfd.SafeFileName.IndexOf('.'), (sfd.SafeFileName.Length - sfd.SafeFileName.IndexOf('.')));

                    switch (extension.ToUpper())
                    {
                        case ".JPG":
                        case ".JPEG":
                            JpegBitmapEncoder jbe = new JpegBitmapEncoder();
                            ImageProcess.SaveUsingEncoder(BitmapSource, sfd.FileName, jbe);
                            break;
                        case ".PNG":
                            PngBitmapEncoder pbe = new PngBitmapEncoder();
                            ImageProcess.SaveUsingEncoder(BitmapSource, sfd.FileName, pbe);
                            break;
                        case ".GIF":
                            GifBitmapEncoder gbe = new GifBitmapEncoder();
                            ImageProcess.SaveUsingEncoder(BitmapSource, sfd.FileName, gbe);
                            break;
                    }

                    MessageBox.Show("檔案建立成功，存放位置於" + sfd.FileName, "提示", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("寫入失敗", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            tb = new TransformedBitmap(BitmapSource, new RotateTransform(-90));
            BitmapSource = tb;
            GC.Collect();
        }
        private void Button_RotateRight_Click(object sender, RoutedEventArgs e)
        {
            tb = new TransformedBitmap(BitmapSource, new RotateTransform(90));
            BitmapSource = tb;
            GC.Collect();
        }

        private void Button_MirrorHorizontal_Click(object sender, RoutedEventArgs e)
        {
            tb = new TransformedBitmap(BitmapSource, new ScaleTransform(-1, 1));
            BitmapSource = tb;
            GC.Collect();
        }

        private void Button_MirrorVertical_Click(object sender, RoutedEventArgs e)
        {
            tb = new TransformedBitmap(BitmapSource, new ScaleTransform(1, -1));
            BitmapSource = tb;
            GC.Collect();
        }

        private void Button_AdvancedAdjust_Click(object sender, RoutedEventArgs e)
        {
            AdvancedAdjust aa = new AdvancedAdjust(ImagesCollection, ImageInfo);
            Content = aa;
        }
        private void Button_LastPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesCollection.IndexOf(ImageInfo) > 0)
            {
                ImageInfo = ImagesCollection[ImagesCollection.IndexOf(ImageInfo) - 1];
                GC.Collect();
            }
        }
        private void Button_NextPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesCollection.IndexOf(ImageInfo) < ImagesCollection.Count - 1)
            {
                ImageInfo = ImagesCollection[ImagesCollection.IndexOf(ImageInfo) + 1];
                GC.Collect();
            }
        }
        private void Button_ZoomIn_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Button_ZoomOut_Click(object sender, RoutedEventArgs e)
        {
        }
        private void ImageEdi_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (scaleValue < 3)
                {
                    scaleValue += 0.1;
                }
            }
            else
            {
                if (scaleValue > 1)
                {
                    scaleValue -= 0.1;
                }
            }
            st = new ScaleTransform(scaleValue, scaleValue, e.GetPosition(ImageEdi).X, e.GetPosition(ImageEdi).Y);
            ImageEdi.RenderTransform = st;
            e.Handled = true;
        }


        private void Button_Crop_Click(object sender, RoutedEventArgs e)
        {
            EditorCrop editorCrop = new EditorCrop(ImagesCollection, ImageInfo);
            Content = editorCrop;
        }

        private void Button_Rotate_Click(object sender, RoutedEventArgs e)
        {
            EditorRotate editorRotate = new EditorRotate(ImagesCollection, ImageInfo);
            Content = editorRotate;
        }
    }
}
