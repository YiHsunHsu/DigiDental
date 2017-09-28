using DigiDental.Class;
using System;

namespace DigiDental.ViewModels.Class
{
    public class DBImages
    {
        private DigiDentalEntities dde;
        public DBImages()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
        }
        /// <summary>
        /// 寫入Images(新增圖片) 回傳Image_ID
        /// </summary>
        /// <param name="imagePath">圖片名稱(含路徑)</param>
        /// <param name="imageFileName">圖片名稱</param>
        /// <param name="imageSize">圖片規格(Original/Small)</param>
        /// <param name="imageExtension">圖片副檔名</param>
        /// <param name="registrationID">掛號流水號</param>
        public int InsertImage(string imagePath, string imageFileName, string imageSize, string imageExtension, int registrationID)
        {
            var newImage = new Images
            {
                Image_Path = imagePath,
                Image_FileName = imageFileName,
                Image_Size = imageSize,
                Image_Extension = imageExtension,
                Registration_ID = registrationID
            };
            dde.Images.Add(newImage);
            dde.SaveChanges();
            return newImage.Image_ID;
        }

        /// <summary>
        /// 寫入Images(新增圖片) 回傳ImageInfo
        /// </summary>
        /// <param name="imagePath">圖片名稱(含路徑)</param>
        /// <param name="imageFileName">圖片名稱</param>
        /// <param name="imageSize">圖片規格</param>
        /// <param name="imageExtension">圖片副檔名</param>
        /// <param name="registrationID">掛號流水號</param>
        /// <param name="registrationDate">掛號日</param>
        /// <param name="agencyImagePath">伺服器影像路徑</param>
        /// <param name="decodePixelWidth">圖片解析</param>
        /// <returns></returns>
        public ImageInfo InsertImageReturnImageInfo(string imagePath, string imageFileName, string imageSize, string imageExtension, int registrationID, DateTime registrationDate, string agencyImagePath, int decodePixelWidth)
        {
            var newImage = new Images
            {
                Image_Path = imagePath,
                Image_FileName = imageFileName,
                Image_Size = imageSize,
                Image_Extension = imageExtension,
                Registration_ID = registrationID
            };
            dde.Images.Add(newImage);
            dde.SaveChanges();
            ImageInfo ii = new ImageInfo()
            {
                Registration_Date = registrationDate,
                Image_ID = newImage.Image_ID,
                Image_Path = newImage.Image_Path,
                Image_FullPath = agencyImagePath + @"\" + newImage.Image_Path,
                Image_FileName = newImage.Image_FileName,
                Image_Extension = newImage.Image_Extension,
                Registration_ID = newImage.Registration_ID,
                CreateDate = newImage.CreateDate,
                BitmapImageSet = new LoadBitmapImage().SettingBitmapImage(agencyImagePath + @"\" + newImage.Image_Path, decodePixelWidth)
            };
            return ii;
        }
    }
}
