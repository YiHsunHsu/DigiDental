using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.ViewModelBase;
using DigiDental.Views.UserControls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace DigiDental.ViewModels.UserControlViewModels
{
    public class TemplateFunctionViewModel : ViewModelBase.ViewModelBase
    {
        public Agencys Agencys { get; set; }

        public Patients Patients { get; set; }

        private int columnSpan;
        public int ColumnSpan
        {
            get { return columnSpan; }
            set
            {
                columnSpan = value;
                OnPropertyChanged("ColumnSpan");
            }
        }

        private int rowSpan;
        public int RowSpan
        {
            get { return rowSpan; }
            set
            {
                rowSpan = value;
                OnPropertyChanged("RowSpan");
            }
        }

        private int stretchWidth = 270;
        public int StretchWidth
        {
            get { return stretchWidth; }
            set
            {
                stretchWidth = value;
                OnPropertyChanged("StretchWidth");
            }
        }

        private int stretchHeight = 205;
        public int StretchHeight
        {
            get { return stretchHeight; }
            set
            {
                stretchHeight = value;
                OnPropertyChanged("StretchHeight");
            }
        }

        private string buttonStretchContent;
        public string ButtonStretchContent
        {
            get { return buttonStretchContent; }
            set
            {
                buttonStretchContent = value;
                OnPropertyChanged("ButtonStretchContent");
            }
        }

        private int buttonStretchColumn;
        public int ButtonStretchColumn
        {
            get { return buttonStretchColumn; }
            set
            {
                buttonStretchColumn = value;
                OnPropertyChanged("ButtonStretchColumn");
            }
        }

        private int buttonStretchRow;
        public int ButtonStretchRow
        {
            get { return buttonStretchRow; }
            set
            {
                buttonStretchRow = value;
                OnPropertyChanged("ButtonStretchRow");
            }
        }

        private int buttonStretchWidth;
        public int ButtonStretchWidth
        {
            get { return buttonStretchWidth; }
            set
            {
                buttonStretchWidth = value;
                OnPropertyChanged("ButtonStretchWidth");
            }
        }

        private int buttonStretchHeight;
        public int ButtonStretchHeight
        {
            get { return buttonStretchHeight; }
            set
            {
                buttonStretchHeight = value;
                OnPropertyChanged("ButtonStretchHeight");
            }
        }

        private int listColumn;
        public int ListColumn
        {
            get { return listColumn; }
            set
            {
                listColumn = value;
                OnPropertyChanged("ListColumn");
            }
        }

        private int listRow;
        public int ListRow
        {
            get { return listRow; }
            set
            {
                listRow = value;
                OnPropertyChanged("ListRow");
            }
        }

        private int listItemColumn;
        public int ListItemColumn
        {
            get { return listItemColumn; }
            set
            {
                listItemColumn = value;
                OnPropertyChanged("ListItemColumn");
            }
        }

        private int listItemRow;
        public int ListItemRow
        {
            get { return listItemRow; }
            set
            {
                listItemRow = value;
                OnPropertyChanged("ListItemRow");
            }
        }

        private ScrollBarVisibility listHSBV;

        public ScrollBarVisibility ListHSBV
        {
            get { return listHSBV; }
            set
            {
                listHSBV = value;
                OnPropertyChanged("ListHSBV");
            }
        }

        private ScrollBarVisibility listVSBV;

        public ScrollBarVisibility ListVSBV
        {
            get { return listVSBV; }
            set
            {
                listVSBV = value;
                OnPropertyChanged("ListVSBV");
            }
        }

        private Orientation wrapOrientation;
        public Orientation WrapOrientation
        {
            get { return wrapOrientation; }
            set
            {
                wrapOrientation = value;
                OnPropertyChanged("WrapOrientation");
            }
        }

        private ObservableCollection<Templates> templates;

        public ObservableCollection<Templates> Templates
        {
            get { return templates; }
            set
            {
                templates = value;
                OnPropertyChanged("Templates");

            }
        }
        private Templates templateItem;

        public Templates TemplateItem
        {
            get { return templateItem; }
            set
            {
                if (templateItem != value)
                {
                    templateItem = value;
                    OnPropertyChanged("TemplateItem");
                    SetTemplateContent(templateItem);
                }
            }
        }

        private TBeforeAfter tBA;
        private TIn6s tI6;

        private UserControl templateContent;
        public UserControl TemplateContent
        {
            get { return templateContent; }
            set
            {
                templateContent = value;
                OnPropertyChanged("TemplateContent");
            }
        }

        //改至MainWindowViewModel
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
        //                        bi.DecodePixelWidth = 300;
        //                        bi.CacheOption = BitmapCacheOption.OnLoad;
        //                        bi.EndInit();
        //                        bi.Freeze();
        //                        fs.Close();
        //                    }
        //                    ShowImages.Add(new ImageInfo()
        //                    {
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
                if (showImages != null)
                    CountImages = ShowImages.Count;
            }
        }

        /// <summary>
        /// 用來Binding Image Count
        /// </summary>
        private int countImages;
        public int CountImages
        {
            get { return countImages; }
            set
            {
                countImages = value;
                OnPropertyChanged("CountImages");
            }
        }

        private DigiDentalEntities dde;

        public TemplateFunctionViewModel()
        {
        }

        public TemplateFunctionViewModel(Agencys agencys, Patients patients, MTObservableCollection<ImageInfo> showImages)
        {
            Agencys = agencys;
            Patients = patients;
            ShowImages = showImages;
            //版面樣式初始化
            //設定Grid 橫向或直向
            //0:橫幅 1:直幅
            switch (Agencys.Agency_ViewType)
            {
                case "0":
                    ColumnSpan = 1;
                    RowSpan = 3;
                    ButtonStretchContent = ">";
                    ButtonStretchColumn = 1;
                    ButtonStretchRow = 0;
                    ButtonStretchWidth = 15;
                    ButtonStretchHeight = 60;
                    ListColumn = 2;
                    ListRow = 0;
                    ListItemColumn = 1;
                    ListHSBV = ScrollBarVisibility.Hidden;
                    ListVSBV = ScrollBarVisibility.Visible;
                    WrapOrientation = Orientation.Vertical;
                    break;
                case "1":
                    ColumnSpan = 3;
                    RowSpan = 1;
                    ButtonStretchContent = "﹀";
                    ButtonStretchColumn = 0;
                    ButtonStretchRow = 1;
                    ButtonStretchWidth = 60;
                    ButtonStretchHeight = 15;
                    ListColumn = 0;
                    ListRow = 2;
                    ListItemRow = 1;
                    ListHSBV = ScrollBarVisibility.Visible;
                    ListVSBV = ScrollBarVisibility.Hidden;
                    WrapOrientation = Orientation.Horizontal;
                    break;
            }
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }

            var temp = from t in dde.Templates
                        select t;
            Templates = new ObservableCollection<Templates>(temp);
        }

        private void SetTemplateContent(Templates templateItem)
        {
            switch (templateItem.Template_UserControlName)
            {
                case "TBeforeAfter":
                    if (tBA == null)
                    {
                        tBA = new TBeforeAfter(Agencys, Patients, templateItem);
                    }
                    TemplateContent = tBA;
                    break;
                case "TIn6s":
                    if (tI6 == null)
                    {
                        tI6 = new TIn6s();
                    }
                    TemplateContent = tI6;
                    break;
            }
        }
    }
}
