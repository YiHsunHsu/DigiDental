using DigiDental.Class;
using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.ViewModelBase;
using DigiDental.Views;
using DigiDental.Views.UserControls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels
{
    public class MainWindowViewModel : ViewModelBase.ViewModelBase
    {
        public string HostName { get; set; }
        /// <summary>
        /// 診所資料
        /// </summary>
        private Agencys agencys;
        public Agencys Agencys
        {
            get { return agencys; }
            set
            {
                agencys = value;
                if (!string.IsNullOrEmpty(agencys.Agency_ImagePath))
                    ShowImportFunction = true;
                else
                    ShowImportFunction = false;

                if (lf != null)
                {
                    lf.Agencys = agencys;
                }

                if (tf != null)
                {
                    tf.Agencys = agencys;
                }
            }
        }

        /// <summary>
        /// binding 匯入圖片 IsEnable
        /// </summary>
        private bool showImportFunction = false;
        public bool ShowImportFunction
        {
            get { return showImportFunction; }
            set
            {
                showImportFunction = value;
                OnPropertyChanged("ShowImportFunction");
            }
        }

        /// <summary>
        /// 病患資料
        /// </summary>
        private Patients patients;
        public Patients Patients
        {
            get { return patients; }
            set { patients = value; }
        }

        /// <summary>
        /// 選擇匯入的日期(改變的話重新載入圖片)
        /// </summary>
        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    OnPropertyChanged("SelectedDate");
                    SetImagesCollectionByDate(selectedDate);
                }
            }
        }

        /// <summary>
        /// 載入的所有掛號日
        /// </summary>
        private ObservableCollection<Registrations> registrationsCollection;
        public ObservableCollection<Registrations> RegistrationsCollection
        {
            get { return registrationsCollection; }
            set
            {
                registrationsCollection = value;
                if (registrationsCollection.Count > 0)
                {
                    //建立ComboBox選項
                    //RegistrationsCollection 項目變動時 更動
                    CusComboBoxItem = new MTObservableCollection<CusComboBoxItem>();
                    CusComboBoxItem.Add(new CusComboBoxItem("全部", -1));
                    foreach (Registrations r in registrationsCollection)
                    {
                        CusComboBoxItem.Add(new CusComboBoxItem(r.Registration_Date.ToString("yyyy-MM-dd"), r.Registration_ID));
                    }
                }
            }
        }

        /// <summary>
        /// 用來Binding ComboBox 的ItemsSource
        /// </summary>
        private MTObservableCollection<CusComboBoxItem> cusComboBoxItem;
        public MTObservableCollection<CusComboBoxItem> CusComboBoxItem
        {
            get { return cusComboBoxItem; }
            set
            {
                cusComboBoxItem = value;
                OnPropertyChanged("CusComboBoxItem");
            }
        }

        /// <summary>
        /// ComboBox 的SelectionChanged
        /// </summary>
        private CusComboBoxItem selectedItem;
        public CusComboBoxItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged("SelectedItem");
                    if (selectedItem != null)
                    {
                        if (selectedItem.SelectedValue.Equals(-1))
                        {
                            SetImagesCollectionAll();
                        }
                        else
                        {
                            SetImagesCollectionByDate(DateTime.Parse(selectedItem.DisplayName));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 載入所有影像
        /// </summary>
        private ObservableCollection<ImageInfo> imageInfo;
        public ObservableCollection<ImageInfo> ImageInfo
        {
            get { return imageInfo; }
            set
            {
                imageInfo = value;

                try
                {
                    ShowImages = new MTObservableCollection<ImageInfo>();

                    pd = new ProgressDialog();

                    pd.Dispatcher.Invoke(() =>
                    {
                        pd.PText = "圖片載入中( 0 / " + imageInfo.Count + " )";
                        pd.PMinimum = 0;
                        pd.PValue = 0;
                        pd.PMaximum = imageInfo.Count;
                        pd.Show();
                    });

                    //multi - thread
                    Task t = Task.Factory.StartNew(() =>
                    {
                        Parallel.ForEach(imageInfo, imgs =>
                        {
                            BitmapImage bi = new BitmapImage();

                            if (File.Exists(imgs.Image_FullPath))
                            {
                                FileStream fs = new FileStream(imgs.Image_FullPath, FileMode.Open);
                                bi.BeginInit();
                                bi.StreamSource = fs;
                                bi.DecodePixelWidth = 800;
                                bi.CacheOption = BitmapCacheOption.OnLoad;
                                bi.EndInit();
                                bi.Freeze();
                                fs.Close();
                            }
                            ShowImages.Add(new ImageInfo()
                            {
                                Registration_Date = imgs.Registration_Date,
                                Image_ID = imgs.Image_ID,
                                Image_Path = imgs.Image_Path,
                                Image_FullPath = imgs.Image_FullPath,
                                Image_FileName = imgs.Image_FileName,
                                Image_Extension = imgs.Image_Extension,
                                Registration_ID = imgs.Registration_ID,
                                CreateDate = imgs.CreateDate,
                                BitmapImageSet = bi
                            });

                            pd.Dispatcher.Invoke(() =>
                            {
                                pd.PValue++;
                                pd.PText = "圖片載入中( " + pd.PValue + " / " + imageInfo.Count + " )";
                            });
                        });
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).ContinueWith(cw =>
                    {
                        pd.Dispatcher.Invoke(() =>
                        {
                            pd.PText = "載入完成";
                            pd.Close();
                        });

                        ////更新TabControl 分頁   的ImageInfo 來源
                        ////TabSelectedChanged 重新刷新TAB頁面內的來源
                        UpdateImageInfo();

                        GC.Collect();
                    });
                }
                catch (Exception ex)
                {
                    Error_Log.ErrorMessageOutput(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 用來Binding Image
        /// </summary>
        private MTObservableCollection<ImageInfo> showImages;
        public MTObservableCollection<ImageInfo> ShowImages
        {
            get { return showImages; }
            set { showImages = value; }
        }

        /// <summary>
        /// binding Tab ItemSource來源
        /// </summary>
        private ObservableCollection<TabItem> functionsTab;
        public ObservableCollection<TabItem> FunctionsTab
        {
            get { return functionsTab; }
            set
            {
                functionsTab = value;
                OnPropertyChanged("FunctionsTab");
            }
        }

        /// <summary>
        /// Selected Tab頁面(載入圖片)
        /// </summary>
        private TabItem functionsTabItem;
        public TabItem FunctionsTabItem
        {
            get { return functionsTabItem; }
            set
            {
                functionsTabItem = value;
                OnPropertyChanged("FunctionsTabItem");
                //更新TabControl 分頁   的ImageInfo 來源
                //TabSelectedChanged 重新刷新TAB頁面內的來源
                UpdateImageInfo();
            }
        }

        #region MVVM TabControl 建構子
        //private DigiDentalEntities dde;

        //private ObservableCollection<TabItemModel> tabs;
        //public ObservableCollection<TabItemModel> Tabs
        //{
        //    get
        //    {
        //        if (tabs == null)
        //        {
        //            tabs = new ObservableCollection<TabItemModel>();
        //        }
        //        return tabs;
        //    }
        //}
        //SelectedItem Binding 
        //private object tabControlSelectedItem;
        //public object TabControlSelectedItem
        //{
        //    get { return tabControlSelectedItem; }
        //    set
        //    {
        //        if (tabControlSelectedItem != value)
        //        {
        //            tabControlSelectedItem = value;
        //            //OnPropertyChanged("TabControlSelectedItem");
        //        }
        //    }
        //}
        #endregion
        /// <summary>
        /// DigiDentalEntities
        /// </summary>
        private DigiDentalEntities dde;
        /// <summary>
        /// UserControl (Tab頁面)
        /// </summary>
        private ListFunction lf;
        /// <summary>
        /// UserControl (Tab頁面)
        /// </summary>
        private TemplateFunction tf;
        /// <summary>
        /// ProgressDialog(進度條)
        /// </summary>
        private ProgressDialog pd;
        public MainWindowViewModel(string hostName, Agencys agencys, Patients patients, DateTime selectedDate)
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }

            HostName = hostName;
            Agencys = agencys;
            Patients = patients;

            
            //取掛號資訊清單 Registration
            var queryRegistrations = from qr in dde.Registrations
                                     where qr.Patient_ID == Patients.Patient_ID
                                     orderby qr.Registration_Date descending
                                     select qr;
            RegistrationsCollection = new ObservableCollection<Registrations>(queryRegistrations.ToList());

            SelectedDate = selectedDate;

            LoadFunctions();            
        }

        #region METHOD
        /// <summary>
        /// 載入影像 條件日期
        /// </summary>
        /// <param name="date">條件日</param>
        private void SetImagesCollectionByDate(DateTime date)
        {
            //載入Images
            //取圖片清單 Images
            var queryImages = from r in RegistrationsCollection
                              where r.Registration_Date.Date == date.Date
                              join i in dde.Images
                              on r.Registration_ID equals i.Registration_ID into ri
                              from qri in ri.DefaultIfEmpty()
                              where qri.Image_Size.Equals("Original")
                                    && qri.Image_IsEnable == true
                              select new ImageInfo()
                              {
                                  Registration_Date = qri.Registrations.Registration_Date,
                                  Image_ID = qri.Image_ID,
                                  Image_Path = qri.Image_Path,
                                  Image_FullPath = Agencys.Agency_ImagePath + qri.Image_Path,
                                  Image_FileName = qri.Image_FileName,
                                  Image_Extension = qri.Image_Extension,
                                  Registration_ID = qri.Registration_ID,
                                  CreateDate = qri.CreateDate
                              };
            ImageInfo = new ObservableCollection<ImageInfo>(queryImages);
        }

        /// <summary>
        /// 載入影像 全部
        /// </summary>
        private void SetImagesCollectionAll()
        {
            //載入Images
            //取圖片清單 Images
            var queryImages = from r in RegistrationsCollection
                              join i in dde.Images
                              on r.Registration_ID equals i.Registration_ID into ri
                              from qri in ri.DefaultIfEmpty()
                              where qri.Image_Size.Equals("Original")
                                    && qri.Image_IsEnable == true
                              select new ImageInfo()
                              {
                                  Registration_Date = qri.Registrations.Registration_Date,
                                  Image_ID = qri.Image_ID,
                                  Image_Path = qri.Image_Path,
                                  Image_FullPath = Agencys.Agency_ImagePath + qri.Image_Path,
                                  Image_FileName = qri.Image_FileName,
                                  Image_Extension = qri.Image_Extension,
                                  Registration_ID = qri.Registration_ID,
                                  CreateDate = qri.CreateDate
                              };
            ImageInfo = new ObservableCollection<ImageInfo>(queryImages);
        }

        /// <summary>
        /// 更新TabControl 分頁的ImageInfo 來源
        /// </summary>
        private void UpdateImageInfo()
        {
            FunctionsTabItem.Dispatcher.Invoke(() =>
            {
                switch (FunctionsTabItem.Uid)
                {
                    case "1":
                        FunctionsTabItem.Content = lf;
                        lf.Dispatcher.Invoke(() =>
                        {
                            lf.ShowImages = ShowImages;
                        });
                        break;
                    case "2":
                        FunctionsTabItem.Content = tf;
                        tf.Dispatcher.Invoke(() =>
                        {
                            tf.ShowImages = ShowImages;
                        });
                        break;
                }
            });
        }

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
                foreach (var qf in queryFunctions)
                {
                    TabItem fTabItem = new TabItem();
                    fTabItem.Header = qf.Function_Title;
                    fTabItem.Uid = qf.Function_ID.ToString();
                    switch (fTabItem.Uid)
                    {
                        case "1":
                            if (lf == null)
                            {
                                lf = new ListFunction(Agencys, ShowImages);
                            }
                            break;
                        case "2":
                            if (tf == null)
                            {
                                tf = new TemplateFunction(Agencys, Patients, ShowImages);
                                tf.ReturnValueCallback += new TemplateFunction.ReturnValueDelegate(RenewUsercontrol);
                            }
                            break;
                    }
                    if (qf.Function_ID == Agencys.Function_ID)
                    {
                        FunctionsTabItem = fTabItem;
                    }

                    if (FunctionsTab == null)
                    {
                        FunctionsTab = new ObservableCollection<TabItem>();
                    }
                    FunctionsTab.Add(fTabItem);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="registrationDate"></param>
        public void RenewUsercontrol(int registrationID, DateTime registrationDate)
        {
            //wifi auto 載入  取掛號資訊清單 Registration
            var queryRegistrations = from qr in dde.Registrations
                                     where qr.Patient_ID == Patients.Patient_ID
                                     orderby qr.Registration_Date descending
                                     select qr;
            RegistrationsCollection = new ObservableCollection<Registrations>(queryRegistrations.ToList());
            SelectedItem = new CusComboBoxItem(registrationDate.ToString("yyyy-MM-dd"), registrationID);
        }
        #endregion

        #region MVVM TabControl
        //private ListFunctionViewModel lfvm;
        //private TemplateFunctionViewModel tfvm;
        //private void LoadFunctions()
        //{
        //    if (dde == null)
        //    {
        //        dde = new DigiDentalEntities();
        //    }
        //    var funcs = from f in dde.Functions
        //                where f.Function_IsEnable == true
        //                select f;
        //    foreach (var func in funcs)
        //    {
        //        switch (func.Function_ID)
        //        {
        //            case 1:
        //                if (lfvm == null)
        //                {
        //                    lfvm = new ListFunctionViewModel(func.Function_Title, patients);
        //                    if (func.Function_ID.Equals(Agencys.Function_ID))
        //                    {
        //                        TabControlSelectedItem = lfvm;
        //                    }
        //                    Tabs.Add(lfvm);
        //                }
        //                break;
        //            case 2:
        //                if (tfvm == null)
        //                {
        //                    tfvm = new TemplateFunctionViewModel { Header = func.Function_Title };
        //                    if (func.Function_ID.Equals(Agencys.Function_ID))
        //                    {
        //                        TabControlSelectedItem = tfvm;
        //                    }
        //                    Tabs.Add(tfvm);
        //                }
        //                break;
        //        }
        //    }
        //}
        #endregion
    }
}
