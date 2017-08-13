using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace DigiDental.Class
{
    public class LoadBitmapImage
    {
        /// <summary>
        /// 建立圖片BitmapImage
        /// </summary>
        /// <param name="filePath">影像路徑</param>
        /// <param name="decodePixelWidth">影像解析</param>
        public BitmapImage SettingBitmapImage(string filePath, int decodePixelWidth)
        {
            BitmapImage bi = new BitmapImage();
            try
            {
                if (File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open);
                    bi.BeginInit();
                    bi.StreamSource = fs;
                    bi.DecodePixelWidth = decodePixelWidth;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    bi.Freeze();
                    fs.Close();
                }
                return bi;
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                return bi;
            }
        }
    }
}
