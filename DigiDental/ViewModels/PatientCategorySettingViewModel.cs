using DigiDental.ViewModels.Class;
using System.Collections.Generic;
using System.Linq;

namespace DigiDental.ViewModels
{
    public class PatientCategorySettingViewModel : ViewModelBase.ViewModelBase
    {
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

        private List<PatientCategoryInfo> showPatientCategoryInfo;
        public List<PatientCategoryInfo> ShowPatientCategoryInfo
        {
            get { return showPatientCategoryInfo; }
            set
            {
                showPatientCategoryInfo = value;
                OnPropertyChanged("ShowPatientCategoryInfo");
            }
        }

        public PatientCategorySettingViewModel(Patients patients)
        {
            using (var dde = new DigiDentalEntities())
            {
                var qpc = from pc in dde.PatientCategories
                          select new PatientCategoryInfo()
                          {
                              PatientCategory_ID = pc.PatientCategory_ID,
                              PatientCategory_Title = pc.PatientCategory_Title,
                              Patient_ID = patients.Patient_ID,
                              IsChecked = pc.Patients.Where(p => p.Patient_ID == patients.Patient_ID).Count() > 0 ? true : false
                          };
                ShowPatientCategoryInfo = PatientCategoryInfo = qpc.ToList();
            }
        }
    }
}
