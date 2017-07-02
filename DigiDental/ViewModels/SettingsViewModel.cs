namespace DigiDental.ViewModels
{
    public class SettingsViewModel : ViewModelBase.ViewModelBase
    {
        private Agencys agencys;

        public Agencys Agencys
        {
            get { return agencys; }
            set
            {
                agencys = value;
                ImagePath = agencys.Agency_ImagePath;
                WifiCardPath = agencys.Agency_WifiCardPath;
                StartFunction = agencys.Function_ID;
                ViewType = agencys.Agency_ViewType;
            }
        }


        private string imagePath;
        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }

        private string wifiCardPath;
        public string WifiCardPath
        {
            get { return wifiCardPath; }
            set
            {
                wifiCardPath = value;
                OnPropertyChanged("WifiCardPath");
            }
        }

        private int startFunction;
        public int StartFunction
        {
            get { return startFunction; }
            set
            {
                startFunction = value;
                OnPropertyChanged("StartFunction");
            }
        }

        private string viewType;
        public string ViewType
        {
            get { return viewType; }
            set
            {
                viewType = value;
                OnPropertyChanged("ViewType");
            }
        }
        public SettingsViewModel()
        {
        }
    }
}
