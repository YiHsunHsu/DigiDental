using DigiDental.Class;
using DigiDental.ViewModels;
using DigiDental.ViewModels.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// PatientCategorySetting.xaml 的互動邏輯
    /// </summary>
    public partial class PatientCategorySetting : Window
    {
        public Patients Patients { get; set; }
        public List<PatientCategoryInfo> PatientCategoryInfo { get { return pcsvm.PatientCategoryInfo; } }
        public string PatientCategory_Title = string.Empty;

        private DigiDentalEntities dde;
        private PatientCategorySettingViewModel pcsvm;
        public PatientCategorySetting(Patients patients)
        {
            InitializeComponent();
            Patients = patients;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
            if (pcsvm == null)
            {
                pcsvm = new PatientCategorySettingViewModel(Patients);
            }
            DataContext = pcsvm;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var qpc = from pc in dde.PatientCategories
                          where pc.PatientCategory_Title.Contains(textBoxCategoryInput.Text)
                          select new PatientCategoryInfo()
                          {
                              PatientCategory_ID = pc.PatientCategory_ID,
                              PatientCategory_Title = pc.PatientCategory_Title,
                              Patient_ID = Patients.Patient_ID,
                              IsChecked = pc.Patients.Where(p => p.Patient_ID == Patients.Patient_ID).Count() > 0 ? true : false
                          };
                pcsvm.PatientCategoryInfo = qpc.ToList();
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private void Button_SearchAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var qpc = from pc in dde.PatientCategories
                          select new PatientCategoryInfo()
                          {
                              PatientCategory_ID = pc.PatientCategory_ID,
                              PatientCategory_Title = pc.PatientCategory_Title,
                              Patient_ID = Patients.Patient_ID,
                              IsChecked = pc.Patients.Where(p => p.Patient_ID == Patients.Patient_ID).Count() > 0 ? true : false
                          };
                pcsvm.PatientCategoryInfo = qpc.ToList();
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }
    }
}
