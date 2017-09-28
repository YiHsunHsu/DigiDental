using System.Collections.ObjectModel;

namespace DigiDental.ViewModels
{
    public class PatientCategoryViewModel : ViewModelBase.ViewModelBase
    {
        private ObservableCollection<PatientCategories> patientCategories;

        public ObservableCollection<PatientCategories> PatientCategories
        {
            get { return patientCategories; }
            set
            {
                patientCategories = value;
                OnPropertyChanged("PatientCategories");
            }
        }

        public PatientCategoryViewModel()
        {
            if (PatientCategories == null)
            {
                PatientCategories = new ObservableCollection<PatientCategories>();
            }
        }
    }
}
