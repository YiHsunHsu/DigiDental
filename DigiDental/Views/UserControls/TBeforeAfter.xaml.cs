using DigiDental.Class;
using DigiDental.ViewModels.Class;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// TBeforeAfter.xaml 的互動邏輯
    /// </summary>
    public partial class TBeforeAfter : UserControl
    {
        public Agencys Agencys { get; set; }
        public Patients Patients { get; set; }
        public Templates Templates { get; set; }

        private ObservableCollection<TemplateImages> tICollection;

        private DigiDentalEntities dde;
        private TemplateImages tI;
        public TBeforeAfter(Agencys agencys, Patients patients, Templates templates)
        {
            InitializeComponent();

            Agencys = agencys;

            Patients = patients;

            Templates = templates;

            LoadBitmapImage lbi = new LoadBitmapImage();

            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }

            var IsImageExist = (from iie in dde.TemplateImages
                                where iie.Template_ID == Templates.Template_ID &&
                                iie.Patient_ID == Patients.Patient_ID
                                select new
                                {
                                    TemplateImage_ID = iie.TemplateImage_ID,
                                    TemplateImage_Number = iie.TemplateImage_Number,
                                    Template_ID = iie.Template_ID,
                                    Image_ID = iie.Image_ID,
                                    Image_Path = Agencys.Agency_ImagePath + iie.Image_Path,
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

                ProgressDialog pd = new ProgressDialog();
                pd.Dispatcher.Invoke(() =>
                {
                    pd.Show();
                    pd.PText = "圖片載入中( 0 / " + tICollection.Count + " )";
                    pd.PMinimum = 0;
                    pd.PValue = 0;
                    pd.PMaximum = tICollection.Count;
                });

                //multi - thread
                Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(tICollection, imgs =>
                    {
                        BitmapImage bi = new BitmapImage();
                        if (File.Exists(imgs.Image_Path))
                        {
                            FileStream fs = new FileStream(imgs.Image_Path, FileMode.Open);
                            bi.BeginInit();
                            bi.StreamSource = fs;
                            bi.DecodePixelWidth = 0;
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.EndInit();
                            bi.Freeze();
                            fs.Close();
                        }
                        if (!string.IsNullOrEmpty(imgs.Image_Path))
                        {
                            switch (imgs.TemplateImage_Number)
                            {
                                case "0":
                                    //BEFORE TemplateImage_Number = 0
                                    ImageBefore.Dispatcher.Invoke(() =>
                                    {
                                        ImageBefore.Source = bi;
                                    });
                                    break;
                                case "1":
                                    //AFTER TemplateImage_Number = 1
                                    ImageAfter.Dispatcher.Invoke(() =>
                                    {
                                        ImageAfter.Source = bi;
                                    });
                                    break;
                            }
                        }
                    });
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

        private void Image_Before_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Image img = e.Source as Image;

                ImageInfo dragImage = new ImageInfo();
                dragImage = ((ImageInfo)e.Data.GetData(DataFormats.Text));

                LoadBitmapImage lbi = new LoadBitmapImage();
                img.Source = lbi.SettingBitmapImage(dragImage.Image_FullPath, 0);

                //BEFORE TemplateImage_Number = 0
                InsertOrUpdateImage(dragImage, "0");
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("移動圖片發生錯誤，聯絡資訊人員", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Image_After_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Image img = e.Source as Image;

                ImageInfo dragImage = new ImageInfo();
                dragImage = ((ImageInfo)e.Data.GetData(DataFormats.Text));

                LoadBitmapImage lbi = new LoadBitmapImage();
                img.Source = lbi.SettingBitmapImage(dragImage.Image_FullPath, 0);

                //AFTER TemplateImage_Number = 1
                InsertOrUpdateImage(dragImage, "1");
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("移動圖片發生錯誤，聯絡資訊人員", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertOrUpdateImage(ImageInfo imageInfo, string tiNumber)
        {
            var IsImageExist = from iie in dde.TemplateImages
                               where iie.Template_ID == Templates.Template_ID &&
                               iie.TemplateImage_Number == tiNumber &&
                               iie.Patient_ID == Patients.Patient_ID
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
                    Template_ID = Templates.Template_ID,
                    Image_ID = imageInfo.Image_ID,
                    Image_Path = imageInfo.Image_Path,
                    Patient_ID = Patients.Patient_ID
                });
                dde.SaveChanges();
            }
        }
    }
}
