﻿using DigiDental.Class;
using DigiDental.ViewModels.Class;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// TInOut10s.xaml 的互動邏輯
    /// </summary>
    public partial class TInOut10s : UserControl
    {
        public Agencys Agencys { get; set; }
        public Patients Patients { get; set; }
        public Templates Templates { get; set; }
                
        private DBTemplateImages dbTI;
        //控制頁面載入所有圖的解析
        private int TemplateImagePixelWidth = 600;
        public TInOut10s(Agencys agencys, Patients patients, Templates templates)
        {
            InitializeComponent();
            Agencys = agencys;

            Patients = patients;

            Templates = templates;

            if (dbTI == null)
            {
                dbTI = new DBTemplateImages();
            }

            dbTI.ShowTemplateImage(Agencys, Patients,Templates, TemplateImagePixelWidth, MainGrid);
            
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Image img = e.Source as Image;

                ImageInfo dragImage = new ImageInfo();
                dragImage = ((ImageInfo)e.Data.GetData(DataFormats.Text));

                LoadBitmapImage lbi = new LoadBitmapImage();
                img.Source = lbi.SettingBitmapImage(dragImage.Image_FullPath, TemplateImagePixelWidth);

                //BEFORE TemplateImage_Number = 0
                dbTI.InsertOrUpdateImage(Patients, Templates, dragImage.Image_ID, dragImage.Image_Path, img.Uid);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("移動圖片發生錯誤，聯絡資訊人員", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
