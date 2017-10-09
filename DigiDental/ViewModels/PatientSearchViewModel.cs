using System.Collections.Generic;
using System.Linq;

namespace DigiDental.ViewModels
{
    public class PatientSearchViewModel : ViewModelBase.ViewModelBase
    {
        private List<PatientCategories> listPatientCategories;
        public List<PatientCategories> ListPatientCategories
        {
            get { return listPatientCategories; }
            set
            {
                listPatientCategories = value;
                OnPropertyChanged("ListPatientCategories");
            }
        }

        private List<Patients> listPatients;

        public List<Patients> ListPatients
        {
            get { return listPatients; }
            set
            {
                listPatients = value;
                OnPropertyChanged("ListPatients");
            }
        }



        public PatientSearchViewModel()
        {
            using (var dde = new DigiDentalEntities())
            {
                ListPatientCategories = dde.PatientCategories.ToList();
                ListPatients = dde.Patients.ToList();
            }
        }
    }
}
