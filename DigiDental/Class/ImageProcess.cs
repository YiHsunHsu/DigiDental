using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigiDental.Class
{
    public class ImageProcess
    {
        /// <summary>
        ///  寬高誰較長就縮誰  - 計算方法
        /// </summary>
        /// <param name="image">System.Drawing.Image 的物件</param>
        /// <param name="maxPx">寬或高超過多少像素就要縮圖</param>
        /// <returns>回傳int陣列，索引0為縮圖後的寬度、索引1為縮圖後的高度</returns>
        public static int[] GetThumbPic_WidthAndHeight(Image image, int maxPx)
        {
            int fixWidth = 0;
            int fixHeight = 0;

            if (image.Width > maxPx || image.Height > maxPx)//如果圖片的寬大於最大值或高大於最大值就往下執行
            {
                if (image.Width >= image.Height)
                //圖片的寬大於圖片的高  
                {
                    fixHeight = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                    //設定修改後的圖高  
                    fixWidth = maxPx;
                }
                else
                {
                    fixWidth = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                    //設定修改後的圖寬  
                    fixHeight = maxPx;
                }
            }
            else//圖片沒有超過設定值，不執行縮圖
            {
                fixHeight = image.Height;
                fixWidth = image.Width;
            }
            int[] fixWidthAndfixHeight = { fixWidth, fixHeight };
            return fixWidthAndfixHeight;
        }

        /// <summary>
        /// 產生縮圖並儲存 寬高誰較長就縮誰
        /// </summary>
        /// <param name="srcImagePath">來源圖片的路徑</param>
        /// <param name="maxPix">超過多少像素就要等比例縮圖</param>
        /// <param name="saveThumbFilePath">縮圖的儲存檔案路徑</param>
        public static void SaveThumbPic(string srcImagePath, int maxPix, string saveThumbFilePath)
        {
            //讀取原始圖片 
            using (FileStream fs = new FileStream(srcImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                //取得原始圖片 
                Bitmap bitmap = new Bitmap(fs);

                //圖片寬高
                int ImgWidth = bitmap.Width;
                int ImgHeight = bitmap.Height;

                // 計算維持比例的縮圖大小 
                int[] thumbnailScaleWidth = GetThumbPic_WidthAndHeight(bitmap, maxPix);
                int AfterImgWidth = thumbnailScaleWidth[0];
                int AfterImgHeight = thumbnailScaleWidth[1];

                // 產生縮圖 
                using (var bmp = new Bitmap(AfterImgWidth, AfterImgHeight))
                {
                    using (var gr = Graphics.FromImage(bmp))
                    {
                        gr.CompositingQuality = CompositingQuality.HighQuality;
                        gr.SmoothingMode = SmoothingMode.HighQuality;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.DrawImage(bitmap, new Rectangle(0, 0, AfterImgWidth, AfterImgHeight), 0, 0, ImgWidth, ImgHeight, GraphicsUnit.Pixel);
                        bmp.Save(saveThumbFilePath);
                    }
                }
            }
        }

        /// <summary>
        /// 儲存 BitmapIimage 影像
        /// </summary>
        /// <param name="bitmapSource">BitmapSource資料來源</param>
        /// <param name="fileName">儲存路徑</param>
        /// <param name="encoder">編碼</param>
        public static void SaveUsingEncoder(BitmapSource bitmapSource, string fileName, BitmapEncoder encoder)
        {
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        /// <summary>
        /// 切图
        /// </summary>
        /// <param name="bitmapSource">圖源</param>
        /// <param name="cut">裁切區</param>
        /// <returns></returns>
        public static BitmapSource CutImage(BitmapSource bitmapSource, Int32Rect cut)
        {
            //計算Stride
            var stride = bitmapSource.Format.BitsPerPixel * cut.Width;
            //
            byte[] data = new byte[cut.Height * stride];
            //CopyPixels
            bitmapSource.CopyPixels(cut, data, stride, 0);

            return BitmapSource.Create(cut.Width, cut.Height, 0, 0, PixelFormats.Bgr32, null, data, stride);
        }

        // ImageSource --> Bitmap
        public static Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            BitmapSource m = (BitmapSource)imageSource;

            Bitmap bmp = new Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
            new Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bmp.UnlockBits(data);

            return bmp;
        }

        // Bitmap --> BitmapImage
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }
    }
}
