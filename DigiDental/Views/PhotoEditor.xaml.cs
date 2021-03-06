﻿using DigiDental.ViewModels;
using DigiDental.ViewModels.Class;
using DigiDental.Views.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// PhotoEditor.xaml 的互動邏輯
    /// </summary>
    public partial class PhotoEditor : Window
    {
        public ImageInfo ImageInfo { get; set; }
        public ObservableCollection<ImageInfo> ImagesCollection { get; set; }

        private PhotoEditorViewModel pevm;
        private BaseAdjust ba;

        public PhotoEditor(ObservableCollection<ImageInfo> imagesCollection)
        {
            InitializeComponent();

            if (pevm == null)
            {
                pevm = new PhotoEditorViewModel();
            }

            ImagesCollection = imagesCollection;

            ImageInfo = imagesCollection[0];
            
            DataContext = pevm;

            if (ba == null)
            {
                ba = new BaseAdjust(ImagesCollection, ImageInfo);
            }

            EditorContent.Content = ba;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GC.Collect();
            DialogResult = true;

        }
    }
}
