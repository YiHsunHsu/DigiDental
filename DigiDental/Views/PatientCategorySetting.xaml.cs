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

        public PatientCategorySetting()
        {
            InitializeComponent();
        }

        public PatientCategorySetting(Patients patients)
        {
            InitializeComponent();
            Patients = patients;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (pcsvm == null)
            {
                if (Patients != null)
                {
                    //更新病患(需要載入已勾選項目)
                    pcsvm = new PatientCategorySettingViewModel(Patients);
                }
                else
                {
                    //新增病患(尚未有Patient資訊)
                    pcsvm = new PatientCategorySettingViewModel();
                }
            }
            DataContext = pcsvm;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
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
