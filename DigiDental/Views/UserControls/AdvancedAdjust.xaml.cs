using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.UserControlViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// AdvancedAdjust.xaml 的互動邏輯
    /// </summary>
    public partial class AdvancedAdjust : UserControl
    {
        public ObservableCollection<ImageInfo> ImagesCollection { get; set; }
        public ImageInfo ImageInfo
        {
            get { return aavm.ImageInfo; }
            set { aavm.ImageInfo = value; }
        }

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
    }
}
