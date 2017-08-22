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
    }
}
