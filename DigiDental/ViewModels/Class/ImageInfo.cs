using System.IO;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels.Class
{
    public class ImageInfo
    {
        private Images imagesCollection;
        public Images ImagesCollection
        {
            get { return imagesCollection; }
            set
            {
                imagesCollection = value;
            }
        }
        private BitmapImage bitmapImageSet;
        public BitmapImage BitmapImageSet
        {
            get { return bitmapImageSet; }
            set
            {
                bitmapImageSet = value;
            }
        }

        public ImageInfo() { }
    }
}
