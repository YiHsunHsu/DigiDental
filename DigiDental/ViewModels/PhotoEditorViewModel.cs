using DigiDental.ViewModels.Class;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels
{
    public class PhotoEditorViewModel : ViewModelBase.ViewModelBase
    {
        private ImageInfo imageInfo;
        public ImageInfo ImageInfo
        {
            get { return imageInfo; }
            set
            {
                imageInfo = value;
                BitmapSource = SetNewBitmapImage(imageInfo.ImagesCollection.Image_Path);
                OnPropertyChanged("ImageInfo");
            }
        }

        private BitmapSource bitmapSource;
        public BitmapSource BitmapSource
        {
            get { return bitmapSource; }
            set
            {
                bitmapSource = value;
                OnPropertyChanged("BitmapSource");
            }
        }

        private ObservableCollection<ImageInfo> imagesCollection;
        public ObservableCollection<ImageInfo> ImagesCollection
        {
            get { return imagesCollection; }
            set { imagesCollection = value; }
        }

        private BitmapImage SetNewBitmapImage(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = fs;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            fs.Close();
            return bi;
        }
    }
}
