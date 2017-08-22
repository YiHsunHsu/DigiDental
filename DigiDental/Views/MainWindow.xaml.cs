using DigiDental.Class;
using DigiDental.ViewModels;
using DigiDental.ViewModels.Class;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        //view model
        private MainWindowViewModel mwvm;
        //private ListFunction lf;
        //private TemplateFunction tf;
        // db
        private DBRegistrations dbr;
        private DBImages dbi;
        //class
        private PatientsFolder pf;
        //dialog view
        private ProgressDialog pd;

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
                        mwvm = new MainWindowViewModel(HostName, Agencys, Patients, DateTime.Now);
                    }
                    DataContext = mwvm;

                    //mwvm.SelectedDate = DateTime.Now;

                    if (dde == null)
                    {
                        dde = new DigiDentalEntities();
                    }

                    if (dbr == null)
                    {
                        dbr = new DBRegistrations();
                    }

                    if (dbi == null)
                    {
                        dbi = new DBImages();
                    }

                    //載入 TabControl 改放到 mwvm
                    //LoadFunctions();
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
        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            Button btnImport = (Button)sender;
            btnImport.IsEnabled = false;
            btnImport.Refresh();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.DefaultExt = ".png";
            ofd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? ofdResult = ofd.ShowDialog();
            if (ofdResult.HasValue && ofdResult.Value)//OpenFileDialog 選確定
            {
                if (Directory.Exists(Agencys.Agency_ImagePath))
                {
                    //讀寫Registrations
                    //確認掛號資料
                    DateTime RegistrationDate = mwvm.SelectedDate;
                    int Registration_ID = dbr.CreateRegistrationsAndGetID(Patients, RegistrationDate);
                    
                    pf = new PatientsFolder(Agencys, Patients, RegistrationDate);
                    #region 小圖路徑(未使用)
                    ////..\病患資料夾\掛號日期\Small
                    //string PatientFolderPathSmall = pf.PatientFolderPathSmall;
                    ////Agencys_ImagePath\病患資料夾\掛號日期\Small
                    //string PatientFullFolderPathSmall = pf.PatientFullFolderPathSmall;
                    #endregion

                    if (!Directory.Exists(pf.PatientFullFolderPath))
                    {
                        Directory.CreateDirectory(pf.PatientFullFolderPathOriginal);
                        //Directory.CreateDirectory(PatientFullFolderPathSmall);
                    }

                    pd = new ProgressDialog();

                    pd.Dispatcher.Invoke(() =>
                    {
                        pd.PMinimum = 0;
                        pd.PValue = 0;
                        pd.PMaximum = ofd.FileNames.Count();
                        pd.PText = "圖片匯入中，請稍後( 0" + " / " + pd.PMaximum + " )";
                        pd.Show();
                    });

                    Task t = Task.Factory.StartNew(() =>
                    {
                        foreach (string fileName in ofd.FileNames)
                        {
                            string extension = Path.GetExtension(fileName).ToUpper();
                            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            //複製原圖到目的Original
                            File.Copy(fileName, pf.PatientFullFolderPathOriginal + @"\" + newFileName + @"ori" + extension);

                            string imagePath = @"\" + pf.PatientFolderPathOriginal + @"\" + newFileName + @"ori" + extension;
                            string imageFileName = newFileName + @"ori" + extension;
                            string imageSize = "Original";
                            string imageExtension = extension;

                            //寫資料庫
                            dbi.InsertImage(imagePath, imageFileName, imageSize, imageExtension, Registration_ID);

                            #region 產生小圖(未使用)
                            //產生縮圖到Small
                            //ImageProcess.SaveThumbPic(fileName, 300, PatientFullFolderPathSmall + @"\" + newFileName + @"sml" + extension);

                            //寫資料庫
                            //dde.Images.Add(new Images
                            //{
                            //    Image_Path = @"\" + PatientFolderPathSmall + @"\" + newFileName + @"sml" + extension,
                            //    Image_FileName = newFileName + @"sml" + extension,
                            //    Image_Size = "Small",
                            //    Image_Extension = extension,
                            //    Registration_ID = Registration_ID
                            //});
                            //dde.SaveChanges();
                            #endregion


                            pd.Dispatcher.Invoke(() =>
                            {
                                pd.PValue++;
                                pd.PText = "圖片匯入中，請稍後( " + pd.PValue + " / " + pd.PMaximum + " )";
                            });

                            Thread.Sleep(200);
                        }
                    }).ContinueWith(cw =>
                    {
                        pd.Dispatcher.Invoke(() =>
                        {
                            pd.PText = "匯入完成";
                            pd.Close();
                        });
                        ReloadRegistration(Registration_ID, RegistrationDate);
                        GC.Collect();
                    }, CancellationToken.None,TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    MessageBox.Show("影像資料夾有問題，請檢查設定是否有誤", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            btnImport.IsEnabled = true;
            btnImport.Refresh();
        }

        private void Button_Stretch_Panel(object sender, RoutedEventArgs e)
        {
            if (IsStretch)
            {
                GridLeft.Width = new GridLength(0, GridUnitType.Pixel);
                ButtonStretch.Content = ">";
                IsStretch = false;
            }
            else
            {
                GridLeft.Width = new GridLength(200, GridUnitType.Pixel);
                ButtonStretch.Content = "<";
                IsStretch = true;
            }
        }

        private void Button_WifiImport_Click(object sender, RoutedEventArgs e)
        {
            Button btnWifiImport = (Button)sender;

            btnWifiImport.Dispatcher.Invoke(() =>
            {
                btnWifiImport.IsEnabled = false;
            });

            if (!string.IsNullOrEmpty(Agencys.Agency_WifiCardPath))
            {
                if (Directory.Exists(Agencys.Agency_WifiCardPath))
                {
                    //讀寫Registrations
                    //確認掛號資料
                    DateTime RegistrationDate = mwvm.SelectedDate;
                    int Registration_ID = dbr.CreateRegistrationsAndGetID(Patients, RegistrationDate);

                    pf = new PatientsFolder(Agencys, Patients, RegistrationDate);

                    if (!Directory.Exists(pf.PatientFullFolderPath))
                    {
                        Directory.CreateDirectory(pf.PatientFullFolderPathOriginal);
                    }

                    pd = new ProgressDialog();

                    pd.Dispatcher.Invoke(() =>
                    {
                        pd.PMinimum = 0;
                        pd.PValue = 0;
                        pd.PMaximum = Directory.GetFiles(Agencys.Agency_WifiCardPath).Count();
                        pd.PText = "圖片匯入中，請稍後( 0" + " / " + pd.PMaximum + " )";
                        pd.Show();
                    });

                    Task t = Task.Factory.StartNew(() =>
                    {
                        foreach (string f in Directory.GetFiles(Agencys.Agency_WifiCardPath))
                        {
                            string extension = Path.GetExtension(f).ToUpper();
                            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");

                            File.Copy(f, pf.PatientFullFolderPathOriginal + @"\" + newFileName + @"ori" + extension);

                            string imagePath = @"\" + pf.PatientFolderPathOriginal + @"\" + newFileName + @"ori" + extension;
                            string imageFileName = newFileName + @"ori" + extension;
                            string imageSize = "Original";
                            string imageExtension = extension;
                            //寫資料庫
                            dbi.InsertImage(imagePath, imageFileName, imageSize, imageExtension, Registration_ID);

                            File.Delete(f);

                            pd.Dispatcher.Invoke(() =>
                            {
                                pd.PValue++;
                                pd.PText = "圖片匯入中，請稍後( " + pd.PValue + " / " + pd.PMaximum + " )";
                            });

                            Thread.Sleep(200);
                        }
                    }).ContinueWith(cw =>
                    {
                        pd.Dispatcher.Invoke(() =>
                        {
                            pd.PText = "匯入完成";
                            pd.Close();
                        });

                        ReloadRegistration(Registration_ID, RegistrationDate);
                        GC.Collect();
                    }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                    
                }
                else
                {
                    MessageBox.Show("Wifi Card實體資料夾位置尚未建立", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("尚未設置Wifi Card資料夾位置", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            btnWifiImport.Dispatcher.Invoke(() =>
            {
                btnWifiImport.IsEnabled = true;
            });
        }
        #region MenuItem Functions

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
        /// 重新載入 掛號(Registrations) 圖片
        /// </summary>
        /// <param name="registrationID">Registration_ID</param>
        /// <param name="registrationDate">Registration_Date</param>
        public void ReloadRegistration(int registrationID, DateTime registrationDate)
        {
            //匯入之後重新載入  取掛號資訊清單 Registration
            var queryRegistrations = from qr in dde.Registrations
                                     where qr.Patient_ID == Patients.Patient_ID
                                     orderby qr.Registration_Date descending
                                     select qr;
            mwvm.RegistrationsCollection = new ObservableCollection<Registrations>(queryRegistrations.ToList());
            mwvm.SelectedItem = new CusComboBoxItem(registrationDate.ToString("yyyy-MM-dd"), registrationID);
        }

        ///// <summary>
        ///// 載入Functions 建立Tab
        ///// </summary>
        //private void LoadFunctions()
        //{
        //    var queryFunctions = from qf in dde.Functions
        //                         where qf.Function_IsEnable == true
        //                         select qf;
        //    if (queryFunctions.Count() > 0)
        //    {
        //        //建立Tabcontrol Functions 功能頁面
        //        foreach (var qf in queryFunctions)
        //        {
        //            TabItem tiFunction = new TabItem();
        //            switch (qf.Function_ID)
        //            {
        //                case 1:
        //                    tiFunction.Header = qf.Function_Title;
        //                    if (lf == null)
        //                    {
        //                        lf = new ListFunction(Agencys, Patients);
        //                    }
        //                    tiFunction.Content = lf;
        //                    break;
        //                case 2:
        //                    tiFunction.Header = qf.Function_Title;
        //                    if (tf == null)
        //                    {
        //                        tf = new TemplateFunction();
        //                    }
        //                    tiFunction.Content = tf;
        //                    break;
        //            }
        //            if (qf.Function_ID == Agencys.Function_ID)
        //            {
        //                tiFunction.IsSelected = true;
        //            }
        //            FunctionsTab.Items.Add(tiFunction);
        //        }
        //    }
        //}

        #endregion


    }
}
