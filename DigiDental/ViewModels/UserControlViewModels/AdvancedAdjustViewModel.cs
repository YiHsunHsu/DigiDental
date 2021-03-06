﻿using DigiDental.ViewModels.Class;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels.UserControlViewModels
{
    public class AdvancedAdjustViewModel : ViewModelBase.ViewModelBase
    {
        private ImageInfo imageInfo;
        public ImageInfo ImageInfo
        {
            get { return imageInfo; }
            set
            {
                imageInfo = value;
                BitmapSource = SetNewBitmapImage(imageInfo.Image_FullPath);
                OnPropertyChanged("ImageInfo");
            }
        }

        private BitmapSource bitmapSource;
        public BitmapSource BitmapSource
        {
            get { return bitmapSource; }
            set
            {
                bitmapSource = value;
                OnPropertyChanged("BitmapSource");
            }
        }

        private BitmapImage SetNewBitmapImage(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = fs;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            fs.Close();
            return bi;
        }

        private bool isRotateChecked = false;
        public bool IsRotateChecked
        {
            get
            {
                if (isRotateChecked)
                {
                    ColumnRotateDefinition = new GridLength(45);
                }
                else
                {
                    ColumnRotateDefinition = new GridLength(0);
                }
                return isRotateChecked;
            }
            set
            {
                isRotateChecked = value;
                OnPropertyChanged("IsRotateChecked");
            }
        }

        private GridLength columnRotateDefinition;
        public GridLength ColumnRotateDefinition
        {
            get { return columnRotateDefinition; }
            set
            {
                columnRotateDefinition = value;
                OnPropertyChanged("ColumnRotateDefinition");
            }
        }


        private double angleValue;
        public double AngleValue
        {
            get { return angleValue; }
            set
            {
                angleValue = double.Parse(value.ToString("0.0"));
                OnPropertyChanged("AngleValue");
            }
        }
    }
}
