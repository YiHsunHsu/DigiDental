using DigiDental.Class;
using DigiDental.ViewModels.Class;
using DigiDental.ViewModels.UserControlViewModels;
using DigiDental.ViewModels.ViewModelBase;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views.UserControls
{
    /// <summary>
    /// TemplateFunction.xaml 的互動邏輯
    /// </summary>
    public partial class TemplateFunction : UserControl
    {
        public Agencys Agencys
        {
            get { return tfvm.Agencys; }
            set
            {
                if (tfvm == null)
                    tfvm = new TemplateFunctionViewModel(tmpA, tmpP, tmpI);
                tfvm.Agencys = value;
            }
        }

        public MTObservableCollection<ImageInfo> ShowImages
        {
            get { return tfvm.ShowImages; }
            set
            {
                if (tfvm == null)
                    tfvm = new TemplateFunctionViewModel(tmpA, tmpP, tmpI);
                tfvm.ShowImages = value;
            }
        }

        private TemplateFunctionViewModel tfvm;

        private Agencys tmpA;
        private Patients tmpP;
        private MTObservableCollection<ImageInfo> tmpI;

        public TemplateFunction(Agencys agencys, Patients patients, MTObservableCollection<ImageInfo> showImages)
        {
            InitializeComponent();

            tmpA = agencys;
            tmpP = patients;
            tmpI = showImages;
        }

        //private TBeforeAfter tBA;

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tfvm == null)
            {
                tfvm = new TemplateFunctionViewModel(tmpA, tmpP, tmpI);
            }

            DataContext = tfvm;

            //if (tBA == null)
            //{
            //    tBA = new TBeforeAfter();
            //}
            //Main.NavigationService.Navigate(tBA);
        }
        
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                ImageInfo dragImage = (ImageInfo)((Image)e.Source).DataContext;
                DataObject data = new DataObject(DataFormats.Text, dragImage);

                DragDrop.DoDragDrop((DependencyObject)e.Source, data, DragDropEffects.Copy);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("移動圖片發生錯誤，聯絡資訊人員", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Image img = e.Source as Image;

                ImageInfo dragImage = new ImageInfo();
                dragImage = ((ImageInfo)e.Data.GetData(DataFormats.Text));

                LoadBitmapImage lbi = new LoadBitmapImage();
                img.Source = lbi.SettingBitmapImage(dragImage.Image_FullPath, 0);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("移動圖片發生錯誤，聯絡資訊人員", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 紀錄是否展開
        /// </summary>
        private bool IsStretch = true;
        private void Button_Stretch_Click(object sender, RoutedEventArgs e)
        {
            if (IsStretch)
            {
                switch(Agencys.Agency_ViewType)
                {
                    case "0":
                        tfvm.StretchWidth = 0;
                        ButtonStretch.Content = "<";
                        break;
                    case "1":
                        tfvm.StretchHeight = 0;
                        ButtonStretch.Content = "︿";
                        break;
                }
                IsStretch = false;
            }
            else
            {
                switch (Agencys.Agency_ViewType)
                {
                    case "0":
                        tfvm.StretchWidth = 270;
                        ButtonStretch.Content = ">";
                        break;
                    case "1":
                        tfvm.StretchHeight = 205;
                        ButtonStretch.Content = "﹀";
                        break;
                }
                IsStretch = true;
            }
        }
    }
}
