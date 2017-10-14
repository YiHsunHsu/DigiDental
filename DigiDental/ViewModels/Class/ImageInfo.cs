using System;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels.Class
{
    public class ImageInfo : ViewModelBase.ViewModelBase
    {
        public DateTime Registration_Date { get; set; }
        public int Image_ID { get; set; }
        public string Image_Path { get; set; }
        public string Image_FullPath { get; set; }
        public string Image_FileName { get; set; }
        public string Image_Extension { get; set; }
        public int Registration_ID { get; set; }
        public DateTime CreateDate { get; set; }

        private BitmapImage bitmapImageSet;
        public BitmapImage BitmapImageSet
        {
            get { return bitmapImageSet; }
            set
            {
                bitmapImageSet = value;
                OnPropertyChanged("BitmapImageSet");
            }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                }
            }
        }

        public ImageInfo() { }
    }
}
