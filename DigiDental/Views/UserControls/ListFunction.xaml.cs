using DigiDental.Class;
using DigiDental.ViewModels.UserControlViewModels;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// ListFunction.xaml 的互動邏輯
    /// </summary>
    public partial class ListFunction : UserControl
    {
        private DigiDentalEntities dde;
        public ObservableCollection<Registrations> RegistrationsCollection
        {
            get { return lfvm.RegistrationsCollection; }
            set { lfvm.RegistrationsCollection = value; }
        }
        public Agencys Agencys
        {
            get { return lfvm.Agencys; }
            set { lfvm.Agencys = value; }
        }
        private Patients Patients { get; set; }
        private ListFunctionViewModel lfvm;
        public ListFunction(Agencys agencys, Patients patients, ObservableCollection<Registrations> registrationsCollection)
        {
            InitializeComponent();

            if (lfvm == null)
            {
                lfvm = new ListFunctionViewModel();
            }
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
            Agencys = agencys;
            Patients = patients;
            RegistrationsCollection = registrationsCollection;

            DataContext = lfvm;
        }
        

        private void Button_ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (lfvm.ColumnCount > 1)
                lfvm.ColumnCount--;
        }
        private void Button_ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (lfvm.ColumnCount < 5)
                lfvm.ColumnCount++;
        }

        private void Button_ListAll_Click(object sender, RoutedEventArgs e)
        {
            //載入Images
            //取圖片清單 Images
            var queryImages = from r in RegistrationsCollection
                              join i in dde.Images
                              on r.Registration_ID equals i.Registration_ID into ri
                              from qri in ri.DefaultIfEmpty()
                              where qri.Image_Size.Equals("Original")
                                    && qri.Image_IsEnable == true
                              select new Images()
                              {
                                  Image_ID = qri.Image_ID,
                                  Image_Path = Agencys.Agency_ImagePath + qri.Image_Path,
                                  Image_FileName = qri.Image_FileName,
                                  Image_Size = qri.Image_Size,
                                  Image_Extension = qri.Image_Extension,
                                  Image_IsEnable = qri.Image_IsEnable,
                                  Registration_ID = qri.Registration_ID
                              };
            lfvm.ImagesCollection = new ObservableCollection<Images>(queryImages);
        }

        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.DefaultExt = ".png";
            ofd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? ofdResult = ofd.ShowDialog();
            if (ofdResult.HasValue && ofdResult.Value)//OpenFileDialog 選確定
            {
                //讀寫Registrations
                //確認掛號資料
                int Registration_ID;
                DateTime RegistrationDate = lfvm.SelectedDate;
                var queryRegistration = from qr in dde.Registrations
                                        where qr.Patient_ID == Patients.Patient_ID && qr.Registration_Date == RegistrationDate
                                        select qr;
                if (queryRegistration.Count() == 0)
                {
                    var newRegistration = new Registrations
                    {
                        Patient_ID = Patients.Patient_ID,
                        Registration_Date = RegistrationDate
                    };
                    dde.Registrations.Add(newRegistration);
                    dde.SaveChanges();
                    Registration_ID = newRegistration.Registration_ID;
                }
                else
                {
                    Registration_ID = queryRegistration.First().Registration_ID;
                }

                //..\病患資料夾\掛號日期
                string PatientFolderPath = Patients.Patient_ID + @"\" + RegistrationDate.ToString("yyyyMMdd");
                //..\病患資料夾\掛號日期\Original
                string PatientFolderPathOriginal = PatientFolderPath + @"\Original";
                //..\病患資料夾\掛號日期\Small
                string PatientFolderPathSmall = PatientFolderPath + @"\Small";
                //Agencys_ImagePath\病患資料夾\掛號日期
                string PatientFullFolderPath = Agencys.Agency_ImagePath + @"\" + PatientFolderPath;
                //Agencys_ImagePath\病患資料夾\掛號日期\Original
                string PatientFullFolderPathOriginal = PatientFullFolderPath + @"\Original";
                //Agencys_ImagePath\病患資料夾\掛號日期\Small
                string PatientFullFolderPathSmall = PatientFullFolderPath + @"\Small";

                if (!Directory.Exists(PatientFullFolderPath))
                {
                    Directory.CreateDirectory(PatientFullFolderPathOriginal);
                    Directory.CreateDirectory(PatientFullFolderPathSmall);
                }

                foreach (string fileName in ofd.FileNames)
                {
                    string extension = Path.GetExtension(fileName).ToUpper();
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    //複製原圖到目的Original
                    File.Copy(fileName, PatientFullFolderPathOriginal + @"\" + newFileName + @"ori" + extension);

                    //寫資料庫                    
                    dde.Images.Add(new Images
                    {
                        Image_Path = @"\" + PatientFolderPathOriginal + @"\" + newFileName + @"ori" + extension,
                        Image_FileName = newFileName + @"ori" + extension,
                        Image_Size = "Original",
                        Image_Extension = extension,
                        Registration_ID = Registration_ID
                    });
                    dde.SaveChanges();

                    //產生縮圖到Small
                    ImageProcess.SaveThumbPic(fileName, 300, PatientFullFolderPathSmall + @"\" + newFileName + @"sml" + extension);

                    //寫資料庫
                    dde.Images.Add(new Images
                    {
                        Image_Path = @"\" + PatientFolderPathSmall + @"\" + newFileName + @"sml" + extension,
                        Image_FileName = newFileName + @"sml" + extension,
                        Image_Size = "Small",
                        Image_Extension = extension,
                        Registration_ID = Registration_ID
                    });
                    dde.SaveChanges();

                    Thread.Sleep(200);
                }

                //匯入之後重新載入  取掛號資訊清單 Registration
                var queryRegistrations = from qr in dde.Registrations
                                         where qr.Patient_ID == Patients.Patient_ID
                                         orderby qr.Registration_Date descending
                                         select qr;
                lfvm.RegistrationsCollection = new ObservableCollection<Registrations>(queryRegistrations.ToList());
            }
        }

        private void ListBox_PhotoEditor_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lfvm.ShowImages.IndexOf(lfvm.SelectedImage) != -1)
            {
                PhotoEditor pe = new PhotoEditor(lfvm.ShowImages, lfvm.ShowImages.IndexOf(lfvm.SelectedImage));
                pe.Show();
            }
        }
    }
}
