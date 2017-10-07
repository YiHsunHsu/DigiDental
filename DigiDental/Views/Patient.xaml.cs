using DigiDental.Class;
using DigiDental.ViewModels;
using DigiDental.ViewModels.Class;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace DigiDental.Views
{
    /// <summary>
    /// PatientAdd.xaml 的互動邏輯
    /// </summary>
    public partial class Patient : Window
    {
        public Agencys Agencys { get; set; }
        public Patients Patients { get; set; }

        private PatientViewModel pvm;
        /// <summary>
        /// 新增
        /// </summary>
        public Patient(Agencys agencys)
        {
            InitializeComponent();
            Agencys = agencys;
        }
        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="patients"></param>
        public Patient(Agencys agencys, Patients patients)
        {
            InitializeComponent();
            Agencys = agencys;
            Patients = patients;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (pvm == null)
            {
                if (Patients != null)
                {
                    pvm = new PatientViewModel(Agencys, Patients);
                }
                else
                {
                    pvm = new PatientViewModel();
                }
            }
            DataContext = pvm;
        }

        private void Button_PatientCategorySetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PatientCategorySetting patientCategorySetting;
                if (Patients != null)//編輯
                {
                    patientCategorySetting = new PatientCategorySetting(Patients);
                }
                else//新增
                {
                    patientCategorySetting = new PatientCategorySetting();
                }                 
                patientCategorySetting.ShowDialog();
                if (patientCategorySetting.DialogResult == true)
                {
                    pvm.PatientCategoryInfo = patientCategorySetting.PatientCategoryInfo.FindAll(pcs => pcs.IsChecked == true);
                }
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Patients != null)//編輯
                {
                    if (MessageBox.Show("確定修改病患資料<" + pvm.PatientNumber + ">?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using (var dde = new DigiDentalEntities())
                        {
                            DateTime updateTime = DateTime.Now;
                            Patients patients = (from p in dde.Patients
                                                 where p.Patient_ID == Patients.Patient_ID
                                                 select p).First();
                            patients.Patient_Number = pvm.PatientNumber;
                            patients.Patient_Name = pvm.PatientName;
                            patients.Patient_IDNumber = pvm.PatientIDNumber;
                            patients.Patient_Birth = pvm.Birth;
                            patients.Patient_Gender = pvm.Gender;
                            patients.UpdateDate = updateTime;
                            if (IsRemove)
                            {
                                patients.Patient_Photo = null;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ImportPatientPhotoPath))
                                {
                                    PatientsFolder pf = new PatientsFolder(Agencys, patients);
                                    if (!Directory.Exists(pf.PatientFullPatientPhotoPath))
                                    {
                                        Directory.CreateDirectory(pf.PatientFullPatientPhotoPath);
                                    }
                                    string extension = Path.GetExtension(ImportPatientPhotoPath).ToUpper();
                                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                    File.Copy(ImportPatientPhotoPath, pf.PatientFullPatientPhotoPath + @"\" + newFileName + extension);
                                    patients.Patient_Photo = pf.PatientPhotoPath + @"\" + newFileName + extension;
                                }
                            }
                            //寫入分類
                            List<PatientCategories> PatientCategories = dde.PatientCategories.ToList();
                            List<PatientCategoryInfo> PatientCategoryInfo = pvm.PatientCategoryInfo.FindAll(pci => pci.IsChecked == true);
                            foreach (PatientCategories pc in PatientCategories)
                            {
                                var queryCheck = PatientCategoryInfo.FindAll(pci => pci.PatientCategory_ID == pc.PatientCategory_ID);
                                if (queryCheck.Count() > 0)
                                {
                                    patients.PatientCategories.Remove(pc);
                                    patients.PatientCategories.Add(pc);
                                }
                                else
                                {
                                    patients.PatientCategories.Remove(pc);
                                }
                            }
                            dde.SaveChanges();
                            MessageBox.Show("修改完成", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                            Close();
                        }
                    }
                }
                else//新增
                {
                    if (MessageBox.Show("確定新增病患<" + pvm.PatientNumber + ">?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using (var dde = new DigiDentalEntities())
                        {
                            string newPatientID = GetPatientID();
                            DateTime newPatientDate = DateTime.Now;
                            //新增病患
                            Patients patients = new Patients()
                            {
                                Patient_ID = newPatientID,
                                Patient_Number = pvm.PatientNumber,
                                Patient_Name = pvm.PatientName,
                                Patient_IDNumber = pvm.PatientIDNumber,
                                Patient_Birth = pvm.Birth,
                                Patient_Gender = pvm.Gender,
                                CreateDate = newPatientDate,
                                UpdateDate = newPatientDate
                            };
                            if (!string.IsNullOrEmpty(ImportPatientPhotoPath))
                            {
                                PatientsFolder pf = new PatientsFolder(Agencys, patients);
                                if (!Directory.Exists(pf.PatientFullPatientPhotoPath))
                                {
                                    Directory.CreateDirectory(pf.PatientFullPatientPhotoPath);
                                }
                                string extension = Path.GetExtension(ImportPatientPhotoPath).ToUpper();
                                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                File.Copy(ImportPatientPhotoPath, pf.PatientFullPatientPhotoPath + @"\" + newFileName + extension);
                                patients.Patient_Photo = pf.PatientPhotoPath + @"\" + newFileName + extension;
                            }
                            //寫入分類
                            List<PatientCategoryInfo> PatientCategoryInfo = pvm.PatientCategoryInfo.FindAll(pcs => pcs.IsChecked == true);
                            if (PatientCategoryInfo.Count > 0)
                            {
                                foreach (PatientCategoryInfo pci in PatientCategoryInfo)
                                {
                                    PatientCategories patientCategories = (from pc in dde.PatientCategories
                                                                           where pc.PatientCategory_ID == pci.PatientCategory_ID
                                                                           select pc).First();
                                    patients.PatientCategories.Add(patientCategories);
                                }
                            }
                            dde.Patients.Add(patients);
                            dde.SaveChanges();
                            MessageBox.Show("新增完成", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_PatientCategoryRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //取出選定的分類
                PatientCategoryInfo patientCategoryInfo = ((FrameworkElement)e.Source).DataContext as PatientCategoryInfo;
                patientCategoryInfo.IsChecked = false;
                pvm.PatientCategoryInfo = pvm.PatientCategoryInfo.FindAll(spcs => spcs.IsChecked == true);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private string GetPatientID()
        {
            byte[] newPatientIDByte = Guid.NewGuid().ToByteArray();
            string newPatientID = BitConverter.ToInt64(newPatientIDByte, 0).ToString();
            using (var dde = new DigiDentalEntities())
            {
                var checkUnique = from p in dde.Patients
                                  where p.Patient_ID == newPatientID
                                  select p;
                if (checkUnique.Count() > 0)
                {
                    Thread.Sleep(1000);
                    return GetPatientID();
                }
                else
                {
                    return newPatientID;
                }
            }
        }

        private bool IsRemove = false;
        private string ImportPatientPhotoPath = string.Empty;
        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                DefaultExt = ".png",
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };
            bool? ofdResult = ofd.ShowDialog();
            if (ofdResult.HasValue && ofdResult.Value)//OpenFileDialog 選確定
            {
                IsRemove = false;
                ImportPatientPhotoPath = ofd.FileName;
                pvm.PatientPhoto = new LoadBitmapImage().SettingBitmapImage(ImportPatientPhotoPath, 400);
            }
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            IsRemove = true;
            ImportPatientPhotoPath = string.Empty;
            pvm.PatientPhoto = new BitmapImage(new Uri(@"C:\Users\Eason_Hsu\Desktop\icon\user.png"));
        }
    }
}
