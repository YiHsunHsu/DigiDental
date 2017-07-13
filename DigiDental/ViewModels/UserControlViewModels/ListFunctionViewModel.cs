using DigiDental.ViewModels.Class;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

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

                if (registrationsCollection.Count() > 0)
                {
                    ////載圖
                    //SetImagesCollectionByDate(SelectedDate);
                    //建立ComboBox選項
                    //RegistrationsCollection 項目變動時 更動
                    ComboBoxItems = new ObservableCollection<ComboBoxItem>();
                    foreach (Registrations r in RegistrationsCollection)
                    {
                        ComboBoxItems.Add(new ComboBoxItem(r.Registration_Date.ToString("yyyy-MM-dd"), r.Registration_ID));
                    }

                    SelectedDate = DateTime.Now.Date;
                }
            }
        }
        /// <summary>
        /// 載入所有影像 (只有大圖 List 不用載小圖)
        /// </summary>
        private ObservableCollection<Images> imagesCollection;
        public ObservableCollection<Images> ImagesCollection
        {
            get { return imagesCollection; }
            set
            {
                imagesCollection = value;
                OnPropertyChanged("ImagesCollection");

                GC.Collect();

                if (imagesCollection.Count > 0)
                {
                    ShowImages = new ObservableCollection<ImageInfo>();

                    foreach (Images imgs in imagesCollection)
                    {
                        FileStream fs = new FileStream(imgs.Image_Path, FileMode.Open);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = fs;
                        bi.DecodePixelWidth = 800;
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.EndInit();
                        fs.Close();

                        ShowImages.Add(new ImageInfo() { ImagesCollection = imgs, BitmapImageSet = bi });
                    }
                }
            }
        }
        /// <summary>
        /// 用來Binding Image
        /// </summary>
        private ObservableCollection<ImageInfo> showImages;
        public ObservableCollection<ImageInfo> ShowImages
        {
            get { return showImages; }
            set
            {
                showImages = value;
                OnPropertyChanged("ShowImages");
            }
        }
        /// <summary>
        /// 用來Binding List SelectedImage
        /// </summary>
        private ImageInfo selectedImage;
        public ImageInfo SelectedImage
        {
            get { return selectedImage; }
            set
            {
                selectedImage = value;
                OnPropertyChanged("SelectedImage");
            }
        }

        /// <summary>
        /// 用來Binding ComboBox 的ItemsSource
        /// </summary>
        private ObservableCollection<ComboBoxItem> comboBoxItems;
        public ObservableCollection<ComboBoxItem> ComboBoxItems
        {
            get { return comboBoxItems; }
            set
            {
                comboBoxItems = value;
                OnPropertyChanged("ComboBoxItems");
            }
        }
        /// <summary>
        /// ComboBox 的SelectionChanged
        /// </summary>
        private ComboBoxItem selectedValue;
        public ComboBoxItem SelectedValue
        {
            get { return selectedValue; }
            set
            {
                if (selectedValue != value)
                {
                    selectedValue = value;
                    OnPropertyChanged("SelectedValue");
                    if(!string.IsNullOrEmpty(selectedValue.DisplayName))
                    {
                        SelectedDate = DateTime.Parse(selectedValue.DisplayName);
                    }
                }
            }
        }
        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged("SelectedDate");
                var queryComboBoxItems = from qcbi in ComboBoxItems
                                         where qcbi.DisplayName == selectedDate.ToString("yyyy-MM-dd")
                                         select qcbi;
                if (queryComboBoxItems.Count() > 0)
                {
                    SelectedValue = queryComboBoxItems.First();
                }
                else
                {
                    SelectedValue = new ComboBoxItem();
                }
                SetImagesCollectionByDate(selectedDate);

            }
        }

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
        private DigiDentalEntities dde;
        public ListFunctionViewModel()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
        }
        #region METHOD
        private void SetImagesCollectionByDate(DateTime date)
        {
            //載入Images
            //取圖片清單 Images
            var queryImages = from r in registrationsCollection
                              where r.Registration_Date.Date == date.Date
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
            ImagesCollection = new ObservableCollection<Images>(queryImages);
        }
        private void SetImagesCollectionAll()
        {
        }
        #endregion
        public class ComboBoxItem
        {
            public string DisplayName { get; set; }
            public int SelectedValue { get; set; }

            public ComboBoxItem() { }
            public ComboBoxItem(string displayName, int selectedValue)
            {
                DisplayName = displayName;
                SelectedValue = selectedValue;
            }
        }
    }
}
