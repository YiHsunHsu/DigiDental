using DigiDental.Class;
using DigiDental.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels.Class
{
    public class DBTemplateImages
    {
        private ObservableCollection<TemplateImages> tICollection;

        private DigiDentalEntities dde;
        private LoadBitmapImage lbi;
        private TemplateImages tI;
        public DBTemplateImages()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
        }

        public void ShowTemplateImage(Agencys agencys, Patients patients, Templates templates, int templateImagePixelWidth, Grid mainGrid)
        {
            try
            {
                var IsImageExist = (from iie in dde.TemplateImages
                                    where iie.Template_ID == templates.Template_ID &&
                                    iie.Patient_ID == patients.Patient_ID
                                    select new
                                    {
                                        TemplateImage_ID = iie.TemplateImage_ID,
                                        TemplateImage_Number = iie.TemplateImage_Number,
                                        Template_ID = iie.Template_ID,
                                        Image_ID = iie.Image_ID,
                                        Image_Path = agencys.Agency_ImagePath + iie.Image_Path,
                                        Patient_ID = iie.Patient_ID
                                    }).ToList().Select(s => new TemplateImages
                                    {
                                        TemplateImage_ID = s.TemplateImage_ID,
                                        TemplateImage_Number = s.TemplateImage_Number,
                                        Template_ID = s.Template_ID,
                                        Image_ID = s.Image_ID,
                                        Image_Path = s.Image_Path,
                                        Patient_ID = s.Patient_ID
                                    });
                if (IsImageExist.Count() > 0)
                {
                    tICollection = new ObservableCollection<TemplateImages>(IsImageExist);
                    lbi = new LoadBitmapImage();

                    ProgressDialog pd = new ProgressDialog();
                    pd.Show();
                    pd.PText = "圖片載入中( 0 / " + tICollection.Count + " )";
                    pd.PMinimum = 0;
                    pd.PValue = 0;
                    pd.PMaximum = tICollection.Count;

                    //multi - thread
                    Task.Factory.StartNew(() =>
                    {
                        foreach (TemplateImages imgs in tICollection)
                        {
                            if (!string.IsNullOrEmpty(imgs.Image_Path) && File.Exists(imgs.Image_Path))
                            {
                                mainGrid.Dispatcher.Invoke(() =>
                                {
                                    ((Image)mainGrid.FindName("Image" + imgs.TemplateImage_Number)).Source = lbi.SettingBitmapImage(imgs.Image_Path, templateImagePixelWidth);
                                });

                            }
                            pd.PValue++;
                            pd.PText = "圖片載入中( " + pd.PValue + " / " + tICollection.Count + " )";
                        }
                    }).ContinueWith(t =>
                    {
                        pd.Dispatcher.Invoke(() =>
                        {
                            pd.PText = "載入完成";
                            pd.Close();
                        });

                        GC.Collect();
                    });
                }
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }
        public void InsertOrUpdateImage(Patients patients, Templates templates, ImageInfo imageInfo, string tiNumber)
        {
            var IsImageExist = from iie in dde.TemplateImages
                               where iie.Template_ID == templates.Template_ID &&
                               iie.TemplateImage_Number == tiNumber &&
                               iie.Patient_ID == patients.Patient_ID
                               select iie;
            if (IsImageExist.Count() > 0)
            {
                tI = new TemplateImages();
                tI = IsImageExist.First();
                tI.Image_ID = imageInfo.Image_ID;
                tI.Image_Path = imageInfo.Image_Path;
                dde.SaveChanges();
            }
            else
            {
                dde.TemplateImages.Add(new TemplateImages()
                {
                    TemplateImage_Number = tiNumber,
                    Template_ID = templates.Template_ID,
                    Image_ID = imageInfo.Image_ID,
                    Image_Path = imageInfo.Image_Path,
                    Patient_ID = patients.Patient_ID
                });
                dde.SaveChanges();
            }
        }
    }
}
