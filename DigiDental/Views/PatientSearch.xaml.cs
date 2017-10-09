using DigiDental.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views
{
    /// <summary>
    /// PatientSearch.xaml 的互動邏輯
    /// </summary>
    public partial class PatientSearch : Window
    {
        public Patients Patients { get; set; }

        private PatientSearchViewModel psvm;
        public PatientSearch()
        {
            InitializeComponent();
            if (psvm == null)
            {
                psvm = new PatientSearchViewModel();
            }
            DataContext = psvm;
        }

        private void Button_PatientSearch_Click(object sender, RoutedEventArgs e)
        {
            using (var dde = new DigiDentalEntities())
            {
                psvm.ListPatients = (from lp in dde.Patients.AsEnumerable()
                                     where lp.Patient_Number.Contains(textPatientKeyword.Text) || 
                                     lp.Patient_Name.Contains(textPatientKeyword.Text) || 
                                     lp.Patient_IDNumber.Contains(textPatientKeyword.Text) || 
                                     lp.Patient_Birth.ToString("yyyyMMdd").Contains(textPatientKeyword.Text) || 
                                     lp.Patient_Birth.ToString("yyyy-MM-dd").Contains(textPatientKeyword.Text) ||
                                     lp.Patient_Birth.ToString("yyyy/MM/dd").Contains(textPatientKeyword.Text)
                                     select lp).ToList();
            }
        }
        private void Button_PatientCategorySearch_Click(object sender, RoutedEventArgs e)
        {
            using (var dde = new DigiDentalEntities())
            {
                psvm.ListPatientCategories = dde.PatientCategories.Where(pc => pc.PatientCategory_Title.Contains(textPatientCategoryKeyword.Text)).ToList();
            }
        }
        private void Button_PatientCategorySearchAll_Click(object sender, RoutedEventArgs e)
        {
            using (var dde = new DigiDentalEntities())
            {
                psvm.ListPatientCategories = dde.PatientCategories.ToList();
            }
        }
        private void Button_PatientCategory_Click(object sender, RoutedEventArgs e)
        {
            PatientCategories patientCategories = ((FrameworkElement)sender).DataContext as PatientCategories;
            using (var dde = new DigiDentalEntities())
            {
                psvm.ListPatients = (from pc in dde.Patients
                                     where pc.PatientCategories.Any(w => w.PatientCategory_ID == patientCategories.PatientCategory_ID)
                                     select pc).ToList();
            }
        }

        private void DataGrid_PatientSelected_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgPatients.SelectedItem != null)
            {
                Patients patients = dgPatients.SelectedItem as Patients;
                Patients = patients;
                DialogResult = true;
                Close();
            }
        }
    }
}
