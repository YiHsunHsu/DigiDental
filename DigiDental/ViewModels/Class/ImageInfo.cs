using System;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels.Class
{
    public class ImageInfo
    {
        public DateTime Registration_Date { get; set; }
        public int Image_ID { get; set; }
        public string Image_Path { get; set; }
        public string Image_FullPath { get; set; }
        public string Image_FileName { get; set; }
        public string Image_Extension { get; set; }
        public int Registration_ID { get; set; }
        public DateTime CreateDate { get; set; }
        public BitmapImage BitmapImageSet { get; set; }

        public ImageInfo() { }
    }
}
