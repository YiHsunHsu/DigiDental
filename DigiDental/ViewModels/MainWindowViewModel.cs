using DigiDental.Models;
using DigiDental.ViewModels.UserControlViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace DigiDental.ViewModels
{
    public class MainWindowViewModel : ViewModelBase.ViewModelBase
    {
        public string HostName { get; set; }
        private Agencys agencys;
        public Agencys Agencys
        {
            get { return agencys; }
            set
            {
                agencys = value;
                if (!string.IsNullOrEmpty(agencys.Agency_ImagePath))
                    ShowImportFunction = true;
                else
                    ShowImportFunction = false;
            }
        }
        private bool showImportFunction = false;
        public bool ShowImportFunction
        {
            get { return showImportFunction; }
            set
            {
                showImportFunction = value;
                OnPropertyChanged("ShowImportFunction");
            }
        }
        private Patients patients;
        public Patients Patients
        {
            get { return patients; }
            set { patients = value; }
        }

        private Functions Functions { get; set; }
        private DigiDentalEntities dde;

        private ObservableCollection<TabItemModel> tabs;
        public ObservableCollection<TabItemModel> Tabs
        {
            get
            {
                if (tabs == null)
                {
                    tabs = new ObservableCollection<TabItemModel>();
                }
                return tabs;
            }
        }
        //SelectedItem Binding 
        private object tabControlSelectedItem;
        public object TabControlSelectedItem
        {
            get { return tabControlSelectedItem; }
            set
            {
                if (tabControlSelectedItem != value)
                {
                    tabControlSelectedItem = value;
                    //OnPropertyChanged("TabControlSelectedItem");
                }
            }
        }
        public MainWindowViewModel(string hostName, Agencys agencys, Patients patients)
        {
            HostName = hostName;
            Agencys = agencys;
            Patients = patients;

            LoadFunctions();
        }

        private ListFunctionViewModel lfvm;
        private TemplateFunctionViewModel tfvm;
        private void LoadFunctions()
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }
            var funcs = from f in dde.Functions
                        where f.Function_IsEnable == true
                        select f;
            foreach (var func in funcs)
            {
                switch (func.Function_ID)
                {
                    case 1:
                        if (lfvm == null)
                        {
                            lfvm = new ListFunctionViewModel { Header = func.Function_Title };
                            if (func.Function_ID.Equals(Agencys.Function_ID))
                            {
                                TabControlSelectedItem = lfvm;
                            }
                            Tabs.Add(lfvm);
                        }
                        break;
                    case 2:
                        if (tfvm == null)
                        {
                            tfvm = new TemplateFunctionViewModel { Header = func.Function_Title };
                            if (func.Function_ID.Equals(Agencys.Function_ID))
                            {
                                TabControlSelectedItem = tfvm;
                            }
                            Tabs.Add(tfvm);
                        }
                        break;
                }
            }
        }
    }
}
