using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.ViewModelBase;
using System.Linq;
using System.Windows;

namespace DigiDental.ViewModels.UserControlViewModels
{
    public class ListFunctionViewModel : ViewModelBase.ViewModelBase
    {
        private Agencys agencys;
        public Agencys Agencys
        {
            get { return agencys; }
            set { agencys = value; }
        }

        //改至MainWindowViewModel 載入
        ///// <summary>
        ///// 載入的所有掛號日
        ///// </summary>
        //private ObservableCollection<Registrations> registrationsCollection;
        //public ObservableCollection<Registrations> RegistrationsCollection
        //{
        //    get { return registrationsCollection; }
        //    set
        //    {
        //        registrationsCollection = value;

        //        if (registrationsCollection.Count() > 0)
        //        {
        //            //建立ComboBox選項
        //            //RegistrationsCollection 項目變動時 更動
        //            ComboBoxItems = new ObservableCollection<ComboBoxItem>();
        //            foreach (Registrations r in registrationsCollection)
        //            {
        //                ComboBoxItems.Add(new ComboBoxItem(r.Registration_Date.ToString("yyyy-MM-dd"), r.Registration_ID));
        //            }
        //        }
        //    }
        //}

        //改至MainWindowViewModel 載入
        ///// <summary>
        ///// 載入所有影像
        ///// </summary>
        //private ObservableCollection<ImageInfo> imagesInfo;
        //public ObservableCollection<ImageInfo> ImagesInfo
        //{
        //    get { return imagesInfo; }
        //    set
        //    {
        //        imagesInfo = value;
        //        ShowImages = new MTObservableCollection<ImageInfo>();
        //        LoadedImages = 0;

        //        if (imagesInfo != null)
        //        {
        //            CountImages = imagesInfo.Count;
        //            //multi-thread
        //            Task.Factory.StartNew(() =>
        //            {
        //                Parallel.ForEach(imagesInfo, imgs =>
        //                {
        //                    BitmapImage bi = new BitmapImage();
        //                    if (File.Exists(imgs.Image_Path))
        //                    {
        //                        FileStream fs = new FileStream(imgs.Image_Path, FileMode.Open);
        //                        bi.BeginInit();
        //                        bi.StreamSource = fs;
        //                        bi.DecodePixelWidth = 800;
        //                        bi.CacheOption = BitmapCacheOption.OnLoad;
        //                        bi.EndInit();
        //                        bi.Freeze();
        //                        fs.Close();
        //                    }
        //                    ShowImages.Add(new ImageInfo() {
        //                        Registration_Date = imgs.Registration_Date,
        //                        Image_ID = imgs.Image_ID,
        //                        Image_Path = imgs.Image_Path,
        //                        Image_FileName = imgs.Image_FileName,
        //                        Image_Extension = imgs.Image_Extension,
        //                        Registration_ID = imgs.Registration_ID,
        //                        CreateDate = imgs.CreateDate,
        //                        BitmapImageSet = bi
        //                    });

        //                    LoadedImages++;
        //                    LoadImagesInfo = "圖片載入中" + LoadedImages + " / " + CountImages;
        //                });
        //            }).ContinueWith(t =>
        //            {
        //                LoadImagesInfo = "載入完成";
        //                GC.Collect();
        //            });
        //        }
        //    }
        //}

        /// <summary>
        /// 用來Binding Image
        /// </summary>
        private MTObservableCollection<ImageInfo> showImages;
        public MTObservableCollection<ImageInfo> ShowImages
        {
            get { return showImages; }
            set
            {
                showImages = value;
                OnPropertyChanged("ShowImages");
                CountImages = ShowImages.Count();
            }
        }
        
        private int imageSelectedCount = 0;
        public int ImageSelectedCount
        {
            get
            {
                if (imageSelectedCount > 0)
                {
                    IsEditMode = true;
                    SelectedAll = false;
                    SelectedList = true;
                }
                else
                {
                    IsEditMode = false;
                    SelectedAll = true;
                    SelectedList = false;
                }
                return imageSelectedCount;
            }
            set
            {
                imageSelectedCount = value;
                OnPropertyChanged("ImageSelectedCount");
                OnPropertyChanged("TextBlockTips");
            }
        }

        private int countImages = 0;
        public int CountImages
        {
            get { return countImages; }
            set
            {
                countImages = value;
                OnPropertyChanged("CountImages");
                OnPropertyChanged("TextBlockTips");
            }
        }

        private bool selectedAll;
        public bool SelectedAll
        {
            get { return selectedAll; }
            set
            {
                selectedAll = value;
                OnPropertyChanged("SelectedAll");
            }
        }

        private bool selectedList;
        public bool SelectedList
        {
            get { return selectedList; }
            set
            {
                selectedList = value;
                OnPropertyChanged("SelectedList");
            }
        }

        public string TextBlockTips
        {
            get { return "以選取圖片 " + ImageSelectedCount + " 張，共 " + CountImages + " 張"; }
        }

        private bool isEditMode;
        public bool IsEditMode
        {
            get { return isEditMode; }
            set
            {
                isEditMode = value;
                OnPropertyChanged("IsEditMode");
            }
        }

        public ListFunctionViewModel()
        {
        }

        public ListFunctionViewModel(Agencys agencys, MTObservableCollection<ImageInfo> showImages)
        {
            Agencys = agencys;
            ShowImages = showImages;
        }

        ///// <summary>
        ///// 用來Binding ComboBox 的ItemsSource
        ///// </summary>
        //private ObservableCollection<ComboBoxItem> comboBoxItems;
        //public ObservableCollection<ComboBoxItem> ComboBoxItems
        //{
        //    get { return comboBoxItems; }
        //    set
        //    {
        //        comboBoxItems = value;
        //        OnPropertyChanged("ComboBoxItems");
        //    }
        //}

        ///// <summary>
        ///// ComboBox 的SelectionChanged
        ///// </summary>
        //private ComboBoxItem selectedValue;
        //public ComboBoxItem SelectedValue
        //{
        //    get { return selectedValue; }
        //    set
        //    {
        //        if (selectedValue != value)
        //        {
        //            selectedValue = value;
        //            OnPropertyChanged("SelectedValue");
        //            if (selectedValue != null)
        //            {
        //                if (!string.IsNullOrEmpty(selectedValue.DisplayName))
        //                {
        //                    SetImagesCollectionByDate(DateTime.Parse(selectedValue.DisplayName));
        //                }
        //            }
        //        }
        //    }
        //}

        //private DateTime selectedDate;

        //public DateTime SelectedDate
        //{
        //    get { return selectedDate; }
        //    set
        //    {
        //        selectedDate = value;
        //        OnPropertyChanged("SelectedDate");
        //        SetImagesCollectionByDate(selectedDate);
        //    }
        //}

        /// <summary>
        /// 用來 Binding Slider 預設3
        /// </summary>
        private int columnCount = 3;
        public int ColumnCount
        {
            get { return columnCount; }
            set
            {
                columnCount = value;
                OnPropertyChanged("ColumnCount");
            }
        }

        #region METHOD
        //private void SetImagesCollectionByDate(DateTime date)
        //{
        //    //載入Images
        //    //取圖片清單 Images
        //    var queryImages = from r in RegistrationsCollection
        //                      where r.Registration_Date.Date == date.Date
        //                      join i in dde.Images
        //                      on r.Registration_ID equals i.Registration_ID into ri
        //                      from qri in ri.DefaultIfEmpty()
        //                      where qri.Image_Size.Equals("Original")
        //                            && qri.Image_IsEnable == true
        //                      select new ImageInfo()
        //                      {
        //                          Registration_Date = qri.Registrations.Registration_Date,
        //                          Image_ID = qri.Image_ID,
        //                          Image_Path = Agencys.Agency_ImagePath + qri.Image_Path,
        //                          Image_FileName = qri.Image_FileName,
        //                          Image_Extension = qri.Image_Extension,
        //                          Registration_ID = qri.Registration_ID,
        //                          CreateDate = qri.CreateDate
        //                      };
        //    ImagesInfo = new ObservableCollection<ImageInfo>(queryImages);
        //}
        #endregion
    }
}
