using DigiDental.Class;
using System;
using System.Windows;
using System.Linq;
using DigiDental.ViewModels;

namespace DigiDental.Views
{
    /// <summary>
    /// Settings.xaml 的互動邏輯
    /// </summary>
    public partial class Settings : Window
    {
        public Agencys Agencys { get; set; }        

        private SettingsViewModel svm;
        private DigiDentalEntities dde;
        public Settings(Agencys agencys)
        {
            InitializeComponent();
            if (svm == null)
            {
                svm = new SettingsViewModel();
                svm.Agencys = agencys;
            }
            DataContext = svm;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dde == null)
                {
                    dde = new DigiDentalEntities();
                }
                Agencys a = (from q in dde.Agencys
                            where q.Agency_Code == svm.Agencys.Agency_Code
                            select q).First();
                a.Agency_ViewType = svm.ViewType;
                a.Agency_ImagePath = svm.ImagePath;
                a.Agency_WifiCardPath = svm.WifiCardPath;
                a.Function_ID = svm.StartFunction;
                dde.SaveChanges();

                Agencys = a;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                DialogResult = false;
            }
            Close();
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
