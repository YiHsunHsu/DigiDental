namespace DigiDental.ViewModels.Class
{
    using System.Linq;
    public class PatientCategoryInfo : ViewModelBase.ViewModelBase
    {
        public string Patient_ID { get; set; }
        public string PatientCategory_Title { get; set; }
        public int PatientCategory_ID { get; set; }

        private bool isChecked = false;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value != isChecked)
                {
                    isChecked = value;
                    OnPropertyChanged("IsChecked");
                    if (isChecked)
                    {
                        InsertPatients_PatientCategories();
                    }
                    else
                    {
                        DeletePatients_PatientCategories();
                    }
                }
            }
        }

        private DigiDentalEntities dde;
        public PatientCategoryInfo()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
        }
        private void InsertPatients_PatientCategories()
        {
            Patients patients = (from p in dde.Patients
                                 where p.Patient_ID == Patient_ID
                                 select p).First();
            PatientCategories patientCategories = (from pc in dde.PatientCategories
                                                    where pc.PatientCategory_ID == PatientCategory_ID
                                                    select pc).First();
            patients.PatientCategories.Add(patientCategories);
            dde.SaveChanges();
        }
        private void DeletePatients_PatientCategories()
        {
            Patients patients = dde.Patients.First(p => p.Patient_ID == Patient_ID);
            PatientCategories patientCategories = dde.PatientCategories.First(pc => pc.PatientCategory_ID == PatientCategory_ID);
            patients.PatientCategories.Remove(patientCategories);
            dde.SaveChanges();
        }
    }
}
