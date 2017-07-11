using DigiDental.Class;
using DigiDental.ViewModels;
using DigiDental.Views.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private DigiDentalEntities dde;

        private bool IsStretch = true;
        private string HostName { get; set; }
        private Agencys Agencys { get; set; }
        private Patients Patients { get; set; }
        public ObservableCollection<Registrations> RegistrationsCollection { get; set; }
        public ObservableCollection<Images> ImagesCollection { get; set; }

        private MainWindowViewModel mwvm;
        private ListFunction lf;
        private TemplateFunction tf;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Loading Loading = new Loading();
                bool? result = Loading.ShowDialog();
                if ((bool)result)
                {
                    if (mwvm == null)
                    {
                        //Client 電腦名稱
                        HostName = Loading.HostName;
                        //Agencys 載入的機構設定
                        Agencys = Loading.Agencys;
                        //Patients載入的病患 或 沒有
                        Patients = Loading.Patients;
                        mwvm = new MainWindowViewModel(HostName, Agencys, Patients);
                    }
                    DataContext = mwvm;

                    if(dde == null)
                    {
                        dde = new DigiDentalEntities();
                    }

                    LoadFunctions();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                Application.Current.Shutdown();
            }
        }

        private void Button_Stretch_Panel(object sender, RoutedEventArgs e)
        {
            if (IsStretch)
            {
                GridLife.Width = new GridLength(0, GridUnitType.Pixel);
                IsStretch = false;
            }
            else
            {
                GridLife.Width = new GridLength(150, GridUnitType.Pixel);
                IsStretch = true;
            }
        }
        #region MenuItem Functions
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MenuItem_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.DefaultExt = ".png";
            ofd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? ofdResult = ofd.ShowDialog();
            if (ofdResult.HasValue && ofdResult.Value)//OpenFileDialog 選確定
            {
                if (dde == null)
                {
                    dde = new DigiDentalEntities();
                }
                //讀寫Registrations
                //確認掛號資料
                int Registration_ID;
                DateTime RegistrationDate = DateTime.Now.Date;
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
                    string extension = Path.GetExtension(fileName);
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
                lf.RegistrationsCollection = new ObservableCollection<Registrations>(queryRegistrations.ToList());
            }
        }
        private void MenuItem_Setting_Click(object sender, RoutedEventArgs e)
        {
            Settings Settings = new Settings(Agencys);
            if (Settings.ShowDialog() == true)
            {
                Agencys = Settings.Agencys;
                mwvm.Agencys = Agencys;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 載入Functions 建立Tab
        /// </summary>
        private void LoadFunctions()
        {
            var queryFunctions = from qf in dde.Functions
                                 where qf.Function_IsEnable == true
                                 select qf;
            if (queryFunctions.Count() > 0)
            {
                //取掛號資訊清單 Registration
                var queryRegistrations = from qr in dde.Registrations
                                         where qr.Patient_ID == Patients.Patient_ID
                                         orderby qr.Registration_Date descending
                                         select qr;
                RegistrationsCollection = new ObservableCollection<Registrations>(queryRegistrations.ToList());

                //建立Tabcontrol Functions 功能頁面
                foreach (var qf in queryFunctions)
                {
                    TabItem tiFunction = new TabItem();
                    switch (qf.Function_ID)
                    {
                        case 1:

                            tiFunction.Header = qf.Function_Title;
                            if (lf == null)
                            {
                                lf = new ListFunction(Agencys, Patients, RegistrationsCollection);
                            }
                            tiFunction.Content = lf;
                            break;
                        case 2:
                            tiFunction.Header = qf.Function_Title;
                            if (tf == null)
                            {
                                tf = new TemplateFunction();
                            }
                            tiFunction.Content = tf;
                            break;
                    }
                    if (qf.Function_ID == Agencys.Function_ID)
                    {
                        tiFunction.IsSelected = true;
                    }
                    FunctionsTab.Items.Add(tiFunction);
                }
            }
        }
        #endregion
    }
}
