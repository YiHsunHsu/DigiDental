﻿using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.ViewModelBase;
using DigiDental.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace DigiDental.ViewModels.UserControlViewModels
{
    public class TemplateFunctionViewModel : ViewModelBase.ViewModelBase
    {
        private Agencys agencys;
        public Agencys Agencys
        {
            get { return agencys; }
            set
            {
                agencys = value;
                OnPropertyChanged("Agencys");
                SetTemplateLayout();
            }
        }

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
                    if (templateItem != null)
                    {
                        AutoImportEnable = true;
                    }
                    else
                    {
                        AutoImportEnable = false;
                    }
                }
            }
        }

        private bool autoImportEnable = false;
        public bool AutoImportEnable
        {
            get { return autoImportEnable; }
            set
            {
                autoImportEnable = value;
                OnPropertyChanged("AutoImportEnable");
            }
        }

        private DateTime templateImportDate = DateTime.Now.Date;

        public DateTime TemplateImportDate
        {
            get { return templateImportDate; }
            set
            {
                if (templateImportDate != value)
                {
                    templateImportDate = value;
                    OnPropertyChanged("TemplateImportDate");
                    SetTemplateContent(TemplateItem);
                    ImportDateString = (from i in ImportDateCollect
                                        where i == templateImportDate.ToString("yyyy-MM-dd")
                                        select i).ToList().Count() > 0 ? templateImportDate.ToString("yyyy-MM-dd") : null;
                }
            }
        }

        private List<string> importDateCollect;

        public List<string> ImportDateCollect
        {
            get { return importDateCollect; }
            set
            {
                importDateCollect = value;
                OnPropertyChanged("ImportDateCollect");
            }
        }


        private string importDateString;

        public string ImportDateString
        {
            get { return importDateString; }
            set
            {
                if (importDateString != value)
                {
                    importDateString = value;
                    OnPropertyChanged("ImportDateString");
                    if (importDateString != null)
                        TemplateImportDate = DateTime.Parse(importDateString);
                    else
                        TemplateImportDate = TemplateImportDate;
                }
            }
        }


        private TBeforeAfter tBA;
        private TIn6s tI6;
        private TInOut9s tIO9;
        private TInOut10s tIO10;
        private TInOut11s tIO11;
        private TXRay6s tXR6;
        private TXRay19s tXR19;
        private TPlasterModel5s tPM15;
        private TFdi52s tF52;
        private TOthers1s tO1;

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
            using (var dde = new DigiDentalEntities())
            {
                var temp = from t in dde.Templates
                           select t;
                Templates = new ObservableCollection<Templates>(temp);
            }
        }

        private void SetTemplateLayout()
        {
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
        }

        private void SetTemplateContent(Templates templateItem)
        {
            switch (templateItem.Template_UserControlName)
            {
                case "TBeforeAfter":
                    //if (tBA == null)
                    //{
                    //    tBA = new TBeforeAfter(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tBA = new TBeforeAfter(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tBA;
                    break;
                case "TIn6s":
                    //if (tI6 == null)
                    //{
                    //    tI6 = new TIn6s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tI6 = new TIn6s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tI6;
                    break;
                case "TInOut9s":
                    //if (tIO9 == null)
                    //{
                    //    tIO9 = new TInOut9s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tIO9 = new TInOut9s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tIO9;
                    break;
                case "TInOut10s":
                    //if (tIO10 == null)
                    //{
                    //    tIO10 = new TInOut10s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tIO10 = new TInOut10s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tIO10;
                    break;
                case "TInOut11s":
                    //if (tIO11 == null)
                    //{
                    //    tIO11 = new TInOut11s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tIO11 = new TInOut11s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tIO11;
                    break;
                case "TXRay6s":
                    //if (tXR6 == null)
                    //{
                    //    tXR6 = new TXRay6s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tXR6 = new TXRay6s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tXR6;
                    break;
                case "TXRay19s":
                    //if (tXR19 == null)
                    //{
                    //    tXR19 = new TXRay19s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tXR19 = new TXRay19s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tXR19;
                    break;
                case "TPlasterModel5s":
                    //if (tPM15 == null)
                    //{
                    //    tPM15 = new TPlasterModel5s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tPM15 = new TPlasterModel5s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tPM15;
                    break;
                case "TFdi52s":
                    //if (tF52 == null)
                    //{
                    //    tF52 = new TFdi52s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tF52 = new TFdi52s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tF52;
                    break;
                case "TOthers1s":
                    //if (tO1 == null)
                    //{
                    //    tO1 = new TOthers1s(Agencys, Patients, templateItem, TemplateImportDate);
                    //}
                    tO1 = new TOthers1s(Agencys, Patients, templateItem, TemplateImportDate);
                    TemplateContent = tO1;
                    break;
            }
            using (var dde = new DigiDentalEntities())
            {
                var queryImportDate = from ti in dde.TemplateImages
                                       where ti.Patient_ID == Patients.Patient_ID &&
                                       ti.Template_ID == TemplateItem.Template_ID
                                       group ti by ti.TemplateImage_ImportDate into tt
                                       select tt.Key.ToString();
                ImportDateCollect = queryImportDate.ToList();
                //ImportDateCollect = new ObservableCollection<string>(queryImportDate);
            }
        }
    }
}
