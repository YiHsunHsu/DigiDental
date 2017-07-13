using DigiDental.Class;
using DigiDental.ViewModels;
using DigiDental.ViewModels.Class;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigiDental.Views
{
    /// <summary>
    /// PhotoEditor.xaml 的互動邏輯
    /// </summary>
    public partial class PhotoEditor : Window
    {
        public ImageInfo ImageInfo
        {
            get { return pevm.ImageInfo; }
            set { pevm.ImageInfo = value; }
        }
        private BitmapSource BitmapSource
        {
            get { return pevm.BitmapSource; }
            set { pevm.BitmapSource = value; }
        }
        public ObservableCollection<ImageInfo> ImagesCollection
        {
            get { return pevm.ImagesCollection; }
            set { pevm.ImagesCollection = value; }
        }
        private PhotoEditorViewModel pevm;
        TransformedBitmap tb;
        public PhotoEditor(ObservableCollection<ImageInfo> imagesCollection, int SelectedIndex)
        {
            InitializeComponent();

            if (pevm == null)
            {
                pevm = new PhotoEditorViewModel();
            }

            ImagesCollection = imagesCollection;

            ImageInfo = imagesCollection[SelectedIndex];

            DataContext = pevm;
        }
        #region EVENT
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

        private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point xy;
            var position = e.GetPosition(ImageEdi);
            xy = position;
            lbPosition.Content = xy.X + "," + xy.Y;
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

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            switch (ImageInfo.ImagesCollection.Image_Extension.ToUpper())
            {
                case ".JPG":
                case ".JPEG":
                    JpegBitmapEncoder jbe = new JpegBitmapEncoder();
                    ImageProcess.SaveUsingEncoder(BitmapSource, ImageInfo.ImagesCollection.Image_Path, jbe);
                    break;
                case ".PNG":
                    PngBitmapEncoder pbe = new PngBitmapEncoder();
                    ImageProcess.SaveUsingEncoder(BitmapSource, ImageInfo.ImagesCollection.Image_Path, pbe);
                    break;
                case ".GIF":
                    GifBitmapEncoder gbe = new GifBitmapEncoder();
                    ImageProcess.SaveUsingEncoder(BitmapSource, ImageInfo.ImagesCollection.Image_Path, gbe);
                    break;
            }
        }
        #endregion
    }
}
