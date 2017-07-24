using DigiDental.Class;
using DigiDental.ViewModels;
using DigiDental.Views.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private DigiDentalEntities dde;

        private bool IsStretch = true;
        private string HostName { get; set; }
        private Agencys Agencys { get; set; }
        private Patients Patients { get; set; }
        public ObservableCollection<Registrations> RegistrationsCollection { get; set; }
        public ObservableCollection<Images> ImagesCollection { get; set; }

        private MainWindowViewModel mwvm;
        private ListFunction lf;
        private TemplateFunction tf;
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

                    if(dde == null)
                    {
                        dde = new DigiDentalEntities();
                    }

                    LoadFunctions();
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

        #region MenuItem Functions

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Setting_Click(object sender, RoutedEventArgs e)
        {
            Settings Settings = new Settings(Agencys);
            if (Settings.ShowDialog() == true)
            {
                Agencys = Settings.Agencys;
                mwvm.Agencys = Agencys;
                lf.Agencys = Agencys;
            }
        }

        #endregion

        #region Methods

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
                //建立Tabcontrol Functions 功能頁面
                foreach (var qf in queryFunctions)
                {
                    TabItem tiFunction = new TabItem();
                    switch (qf.Function_ID)
                    {
                        case 1:

                            tiFunction.Header = qf.Function_Title;
                            if (lf == null)
                            {
                                lf = new ListFunction(Agencys, Patients);
                            }
                            tiFunction.Content = lf;
                            break;
                        case 2:
                            tiFunction.Header = qf.Function_Title;
                            if (tf == null)
                            {
                                tf = new TemplateFunction();
                            }
                            tiFunction.Content = tf;
                            break;
                    }
                    if (qf.Function_ID == Agencys.Function_ID)
                    {
                        tiFunction.IsSelected = true;
                    }
                    FunctionsTab.Items.Add(tiFunction);
                }
            }
        }

        #endregion
    }
}
