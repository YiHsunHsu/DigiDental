using DigiDental.Class;
using DigiDental.ViewModels.Class;
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

        private Image i;
        private Rectangle r;

        public EditorRotate(ObservableCollection<ImageInfo> imagesCollection, ImageInfo imageInfo)
        {
            InitializeComponent();

            ImagesCollection = imagesCollection;
            ImageInfo = imageInfo;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi = new LoadBitmapImage().SettingBitmapImage(ImageInfo.Image_FullPath, 0);

            Border border = new Border();
            if (bi.PixelWidth > bi.PixelHeight)
            {
                border.MaxWidth = (Cvs.ActualWidth / 5) * 3;
                border.Width = border.MaxWidth;
                border.Height = (border.MaxWidth / bi.PixelWidth) * bi.PixelHeight;
            }
            else
            {
                border.MaxHeight = (Cvs.ActualHeight / 5) * 3;
                border.Height = border.MaxHeight;
                border.Width = (border.MaxHeight / bi.PixelHeight) * bi.PixelWidth;
            }
            
            i = new Image();
            i.Source = bi;
            border.Child = i;

            double imageStartX = (Cvs.ActualWidth - border.Width) / 2;
            double imageStartY = (Cvs.ActualHeight - border.Height) / 2;

            Cvs.Children.Add(border);

            Canvas.SetTop(border, imageStartY);
            Canvas.SetLeft(border, imageStartX);

            r = new Rectangle
            {
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Width = border.Width,
                Height = border.Height
            };
            Cvs.Children.Add(r);

            Canvas.SetTop(r, imageStartY);
            Canvas.SetLeft(r, imageStartX);
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

        private RotateTransform rotateTransform;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rotateTransform = new RotateTransform();
            rotateTransform.Angle = e.NewValue;
            i.LayoutTransform = rotateTransform;
            lbAngle.Content = e.NewValue.ToString("0.0");
        }
    }
}
