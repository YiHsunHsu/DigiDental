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
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

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
        private ProcessingDialog processingDialog;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Loading Loading = new Loading();
                bool? result = Loading.ShowDialog();
                if ((bool)result)
                {
                    //Client 電腦名稱
                    HostName = Loading.HostName;
                    //Agencys 載入的機構設定
                    Agencys = Loading.Agencys;
                    //Patients載入的病患 或 沒有
                    Patients = Loading.Patients;
                    
                    if (Patients != null)
                    {
                        mwvm = new MainWindowViewModel(HostName, Agencys, Patients, DateTime.Now);
                    }
                    else
                    {
                        mwvm = new MainWindowViewModel();
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

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Multiselect = true,
                DefaultExt = ".png",
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };

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
                            ImageInfo importImage = dbi.InsertImageReturnImageInfo(imagePath, imageFileName, imageSize, imageExtension, Registration_ID, RegistrationDate, Agencys.Agency_ImagePath, 800);

                            //加入showImage清單
                            mwvm.ShowImages.Add(importImage);
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

        bool isStop = false; //接ProcessingDialog 回傳值 停止
        private void ToggleButton_WifiImport_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            if (toggleButton.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(Agencys.Agency_WifiCardPath))
                {
                    if (Directory.Exists(Agencys.Agency_WifiCardPath))
                    {
                        //讀寫Registrations
                        //確認掛號資料
                        DateTime RegistrationDate = mwvm.SelectedDate;
                        int Registration_ID = dbr.CreateRegistrationsAndGetID(Patients, RegistrationDate);

                        processingDialog = new ProcessingDialog();
                        Task task = Task.Factory.StartNew(() =>
                        {
                            processingDialog.Dispatcher.Invoke(() =>
                            {
                                processingDialog.PText = "圖片偵測中";
                                processingDialog.PIsIndeterminate = true;
                                processingDialog.ButtonContentVisibility = Visibility.Hidden;
                                processingDialog.ReturnValueCallback += new ProcessingDialog.ReturnValueDelegate(this.SetReturnValueCallbackFun);

                                processingDialog.Show();
                            });
                            int imageCount = 0;
                            while (true)
                            {
                                //偵測資料夾
                                foreach (string f in Directory.GetFiles(Agencys.Agency_WifiCardPath))
                                {
                                    Thread.Sleep(500);


                                    pf = new PatientsFolder(Agencys, Patients, RegistrationDate);

                                    if (!Directory.Exists(pf.PatientFullFolderPath))
                                    {
                                        Directory.CreateDirectory(pf.PatientFullFolderPathOriginal);
                                    }

                                    string extension = Path.GetExtension(f).ToUpper();
                                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");

                                    File.Copy(f, pf.PatientFullFolderPathOriginal + @"\" + newFileName + @"ori" + extension);

                                    string imagePath = @"\" + pf.PatientFolderPathOriginal + @"\" + newFileName + @"ori" + extension;
                                    string imageFileName = newFileName + @"ori" + extension;
                                    string imageSize = "Original";
                                    string imageExtension = extension;

                                    //寫資料庫
                                    ImageInfo importImage = dbi.InsertImageReturnImageInfo(imagePath, imageFileName, imageSize, imageExtension, Registration_ID, RegistrationDate, Agencys.Agency_ImagePath, 800);

                                    File.Delete(f);

                                    //加入showImage清單
                                    mwvm.ShowImages.Add(importImage);

                                    //已匯入
                                    imageCount++;
                                    processingDialog.Dispatcher.Invoke(() =>
                                    {
                                        processingDialog.PText = "圖片匯入中，已匯入" + imageCount + "張";
                                    });
                                }
                                //按停止
                                if (isStop)
                                {
                                    isStop = false;
                                    return;
                                }
                            }
                        }).ContinueWith(cw =>
                        {
                            //結束
                            processingDialog.PText = "處理完畢";
                            processingDialog.Close();

                            toggleButton.IsChecked = false;

                            ReloadRegistration(Registration_ID, RegistrationDate);

                            GC.Collect();
                        }, TaskScheduler.FromCurrentSynchronizationContext());
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
            }
        }

        private void SetReturnValueCallbackFun(bool isDetecting)
        {
            if (isDetecting)
            {
                isStop = isDetecting;
            }
        }

        #region MenuItem Functions

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_PatientAddEdit_Click(object sender, RoutedEventArgs e)
        {
            Patient patient = new Patient(Agencys);
            patient.ShowDialog();
        }

        private void MenuItem_PatientSearch_Click(object sender, RoutedEventArgs e)
        {
            PatientSearch patientSearch = new PatientSearch();
            patientSearch.ShowDialog();
            if (patientSearch.DialogResult == true)
            {
                Patients = patientSearch.Patients;
                mwvm = new MainWindowViewModel(HostName, Agencys, Patients, DateTime.Now);
                DataContext = mwvm;
            }
        }

        private void MenuItem_PatientCategory_Click(object sender, RoutedEventArgs e)
        {
            PatientCategory pc = new PatientCategory();
            pc.ShowDialog();
            mwvm.PatientCategoryInfo = (from pcs in dde.PatientCategories
                                        where pcs.Patients.Any(p => p.Patient_ID == Patients.Patient_ID)
                                        select new PatientCategoryInfo()
                                        {
                                            PatientCategory_ID = pcs.PatientCategory_ID,
                                            PatientCategory_Title = pcs.PatientCategory_Title,
                                            IsChecked = true
                                        }).ToList();
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
            //選擇日期(會重載)
            //mwvm.SelectedItem = new CusComboBoxItem(registrationDate.ToString("yyyy-MM-dd"), registrationID);
        }

        #endregion

        private void Image_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Image img = e.Source as Image;

                ImageInfo dragImage = new ImageInfo();
                dragImage = ((ImageInfo)e.Data.GetData(DataFormats.Text));

                pf = new PatientsFolder(Agencys, Patients);
                if (!Directory.Exists(pf.PatientFullPatientPhotoPath))
                {
                    Directory.CreateDirectory(pf.PatientFullPatientPhotoPath);
                }
                string newPatientPhotoName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + dragImage.Image_Extension;
                string newPatientPhotoPath = pf.PatientFullPatientPhotoPath + @"\" + newPatientPhotoName;
                File.Copy(dragImage.Image_FullPath, newPatientPhotoPath);
                Thread.Sleep(200);
                
                mwvm.PatientPhoto = new LoadBitmapImage().SettingBitmapImage(newPatientPhotoPath, 400);

                //update database Patients Patient_Photo
                Patients patients = (from q in dde.Patients
                             where q.Patient_ID == Patients.Patient_ID
                             select q).First();
                patients.Patient_Photo = pf.PatientPhotoPath + @"\" + newPatientPhotoName;
                patients.UpdateDate = DateTime.Now;
                dde.SaveChanges();

                Patients = patients;
                mwvm.Patients = patients;
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("移動圖片發生錯誤，聯絡資訊人員", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            Patient patient = new Patient(Agencys, Patients);
            patient.ShowDialog();
            if (patient.DialogResult == true)
            {
                using (var dde = new DigiDentalEntities())
                {
                    Patients patients = (from p in dde.Patients
                                         where p.Patient_ID == Patients.Patient_ID
                                         select p).First();
                    Patients = patients;
                    mwvm.Patients = patients;
                    if (!string.IsNullOrEmpty(patients.Patient_Photo))
                    {
                        mwvm.PatientPhoto = new LoadBitmapImage().SettingBitmapImage(Agencys.Agency_ImagePath + @"\" + Patients.Patient_Photo, 400);
                    }
                    else
                    {
                        mwvm.PatientPhoto = new BitmapImage(new Uri(@"C:\Users\Eason_Hsu\Desktop\icon\user.png"));
                    }
                    mwvm.PatientCategoryInfo = (from pc in dde.PatientCategories
                                                where pc.Patients.Any(p => p.Patient_ID == Patients.Patient_ID)
                                                select new PatientCategoryInfo()
                                                {
                                                    PatientCategory_ID = pc.PatientCategory_ID,
                                                    PatientCategory_Title = pc.PatientCategory_Title,
                                                    IsChecked = true
                                                }).ToList();
                }
            }
        }
    }
}
