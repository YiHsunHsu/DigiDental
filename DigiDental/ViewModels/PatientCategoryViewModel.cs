using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DigiDental.ViewModels
{
    public class PatientCategoryViewModel : ViewModelBase.ViewModelBase
    {
        private List<PatientCategories> patientCategories;

        public List<PatientCategories> PatientCategories
        {
            get { return patientCategories; }
            set
            {
                patientCategories = value;
                OnPropertyChanged("PatientCategories");
            }
        }

        private DigiDentalEntities dde;
        public PatientCategoryViewModel()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
            PatientCategories = dde.PatientCategories.ToList();
        }
    }
}
