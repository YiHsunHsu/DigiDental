using DigiDental.Class;
using DigiDental.ViewModels;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsStretch = true;
        private string HostName { get; set; }
        private Agencys Agencys { get; set; }
        private Patients Patients { get; set; }

        private MainWindowViewModel mwvm;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Loading Loading = new Loading();
                bool? result = Loading.ShowDialog();
                if ((bool)result)
                {
                    if (mwvm == null)
                    {
                        //Client 電腦名稱
                        HostName = Loading.HostName;
                        //Agencys 載入的機構設定
                        Agencys = Loading.Agencys;
                        //Patients載入的病患 或 沒有
                        Patients = Loading.Patients;
                        mwvm = new MainWindowViewModel(HostName, Agencys, Patients);
                    }
                    DataContext = mwvm;
                    
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

        private void Button_Stretch_Panel(object sender, RoutedEventArgs e)
        {
            if (IsStretch)
            {
                GridLife.Width = new GridLength(0, GridUnitType.Pixel);
                IsStretch = false;
            }
            else
            {
                GridLife.Width = new GridLength(150, GridUnitType.Pixel);
                IsStretch = true;
            }
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

        private void MenuItem_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
