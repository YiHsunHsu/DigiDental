namespace DigiDental.ViewModels.Class
{
    public class PatientCategoryInfo : ViewModelBase.ViewModelBase
    {
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
                }
            }
        }
        public PatientCategoryInfo() { }
    }
}
