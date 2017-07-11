using DigiDental.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// PhotoEditor.xaml 的互動邏輯
    /// </summary>
    public partial class PhotoEditor : Window
    {
        public Images Images
        {
            get { return pevm.Images; }
            set { pevm.Images = value; }
        }
        public ObservableCollection<Images> ImagesCollection
        {
            get { return pevm.ImagesCollection; }
            set { pevm.ImagesCollection = value; }
        }

        public int RotateAngle
        {
            get { return pevm.RotateAngle; }
            set { pevm.RotateAngle = value; }
        }
        private PhotoEditorViewModel pevm;
        public PhotoEditor(ObservableCollection<Images> imagesCollection, int SelectedIndex)
        {
            InitializeComponent();

            if (pevm == null)
            {
                pevm = new PhotoEditorViewModel();
            }

            ImagesCollection = imagesCollection;
            Images = imagesCollection[SelectedIndex];

            DataContext = pevm;
        }

        private void Button_LastPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesCollection.IndexOf(Images) > 0)
            {
                Images = ImagesCollection[ImagesCollection.IndexOf(Images) - 1];
            }
        }
        private void Button_NextPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesCollection.IndexOf(Images) < ImagesCollection.Count - 1)
            {
                Images = ImagesCollection[ImagesCollection.IndexOf(Images) + 1];
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
            RotateAngle -= 90;
        }
        private void Button_RotateRight_Click(object sender, RoutedEventArgs e)
        {
            RotateAngle += 90;
        }
    }
}
