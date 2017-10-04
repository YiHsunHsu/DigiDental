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
        
        private PatientCategorySettingViewModel pcsvm;
        public PatientCategorySetting(Patients patients)
        {
            InitializeComponent();
            Patients = patients;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (pcsvm == null)
            {
                pcsvm = new PatientCategorySettingViewModel(Patients);
            }
            DataContext = pcsvm;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            using (var dde = new DigiDentalEntities())
            {
                Patients patients = (from p in dde.Patients
                                     where p.Patient_ID == Patients.Patient_ID
                                     select p).First();
                foreach (PatientCategoryInfo pci in PatientCategoryInfo)
                {
                    PatientCategories patientCategories = dde.PatientCategories.First(pc => pc.PatientCategory_ID == pci.PatientCategory_ID);
                    if (pci.IsChecked)
                    {
                        patients.PatientCategories.Add(patientCategories);
                    }
                    else
                    {
                        patients.PatientCategories.Remove(patientCategories);
                    }
                }
                dde.SaveChanges();
            }
            DialogResult = true;
            Close();
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pcsvm.ShowPatientCategoryInfo = PatientCategoryInfo.Where(w => w.PatientCategory_Title.Contains(textBoxCategoryInput.Text)).ToList();
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
                pcsvm.ShowPatientCategoryInfo = PatientCategoryInfo;
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }
    }
}
