using DigiDental.Class;
using DigiDental.ViewModels.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DigiDental.ViewModels
{
    public class PatientViewModel : ViewModelBase.ViewModelBase
    {
        private Agencys agencys;
        public Agencys Agencys
        {
            get { return agencys; }
            set
            {
                agencys = value;
                OnPropertyChanged("Agencys");
            }
        }

        private Patients patients;
        public Patients Patients
        {
            get { return patients; }
            set
            {
                patients = value;
                OnPropertyChanged("Patients");
            }
        }

        private List<PatientCategoryInfo> patientCategoryInfo;
        public List<PatientCategoryInfo> PatientCategoryInfo
        {
            get { return patientCategoryInfo; }
            set
            {
                patientCategoryInfo = value;
                OnPropertyChanged("PatientCategoryInfo");
            }
        }

        private string patientNumber;
        public string PatientNumber
        {
            get { return patientNumber; }
            set
            {
                patientNumber = value;
                OnPropertyChanged("PatientNumber");
                if (Patients != null && value == Patients.Patient_Number)
                {
                    TipsVisibility = Visibility.Hidden;
                    SaveIsEnable = true;
                }
                else
                {
                    using (var dde = new DigiDentalEntities())
                    {
                        var queryPatient = from p in dde.Patients
                                           where p.Patient_Number == patientNumber
                                           select p;

                        if (queryPatient.Count() > 0)
                        {
                            TipsVisibility = Visibility.Visible;
                            SaveIsEnable = false;
                        }
                        else
                        {
                            TipsVisibility = Visibility.Hidden;
                            SaveIsEnable = true;
                        }
                    }
                }
            }
        }

        private string patientName;

        public string PatientName
        {
            get { return patientName; }
            set
            {
                patientName = value;
                OnPropertyChanged("PatientName");
            }
        }

        private string patientIDNumber;

        public string PatientIDNumber
        {
            get { return patientIDNumber; }
            set
            {
                patientIDNumber = value;
                OnPropertyChanged("PatientIDNumber");
            }
        }

        private bool gender = true;
        public bool Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                OnPropertyChanged("Gender");
            }
        }

        private DateTime birth = DateTime.Now.Date;
        public DateTime Birth
        {
            get { return birth; }
            set
            {
                birth = value;
                OnPropertyChanged("Birth");
            }
        }
        private BitmapImage patientPhoto = new BitmapImage(new Uri(@"C:\Users\Eason_Hsu\Desktop\icon\user.png"));
        public BitmapImage PatientPhoto
        {
            get { return patientPhoto; }
            set
            {
                patientPhoto = value;
                OnPropertyChanged("PatientPhoto");
            }
        }

        private Visibility tipsVisibility = Visibility.Hidden;
        public Visibility TipsVisibility
        {
            get { return tipsVisibility; }
            set
            {
                tipsVisibility = value;
                OnPropertyChanged("TipsVisibility");
            }
        }

        private bool saveIsEnable = true;
        public bool SaveIsEnable
        {
            get { return saveIsEnable; }
            set
            {
                saveIsEnable = value;
                OnPropertyChanged("SaveIsEnable");
            }
        }
        //新增
        public PatientViewModel()
        {
        }
        //編輯
        public PatientViewModel(Agencys agencys, Patients patients)
        {
            Agencys = agencys;
            Patients = patients;
            PatientNumber = Patients.Patient_Number;
            PatientName = Patients.Patient_Name;
            PatientIDNumber = Patients.Patient_IDNumber;
            Gender = Patients.Patient_Gender;
            Birth = Patients.Patient_Birth.Date;
            if (!string.IsNullOrEmpty(Patients.Patient_Photo))
            {
                PatientsFolder patientsFolder = new PatientsFolder(Agencys, Patients);
                if (!Directory.Exists(patientsFolder.PatientFullPatientPhotoPath))
                {
                    Directory.CreateDirectory(patientsFolder.PatientFullPatientPhotoPath);
                }
                PatientPhoto = new LoadBitmapImage().SettingBitmapImage(Agencys.Agency_ImagePath + @"\" + Patients.Patient_Photo, 400);
            }
            using (var dde = new DigiDentalEntities())
            {
                var qpc = from pc in dde.PatientCategories
                          select new PatientCategoryInfo()
                          {
                              PatientCategory_ID = pc.PatientCategory_ID,
                              PatientCategory_Title = pc.PatientCategory_Title,
                              IsChecked = pc.Patients.Where(p => p.Patient_ID == patients.Patient_ID).Count() > 0 ? true : false
                          };
                PatientCategoryInfo = qpc.ToList().FindAll(pci=>pci.IsChecked == true);
            }
        }
    }
}
