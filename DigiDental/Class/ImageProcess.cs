using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

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
        public static int[] GetThumbPic_WidthAndHeight(System.Drawing.Image image, int maxPx)
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
    }
}
