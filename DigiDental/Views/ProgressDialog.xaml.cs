using DigiDental.ViewModels;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// ProgressDialog.xaml 的互動邏輯
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public string DTitle
        {
            get { return pdvm.DTitle; }
            set { pdvm.DTitle = value; }
        }
        public string PText
        {
            get { return pdvm.PText; }
            set { pdvm.PText = value; }
        } 
        public int PMinimum
        {
            get { return pdvm.PMinimum; }
            set { pdvm.PMinimum = value; }
        }
        
        public int PMaximum
        {
            get { return pdvm.PMaximum; }
            set { pdvm.PMaximum = value; }
        }
        
        public int PValue
        {
            get { return pdvm.PValue; }
            set { pdvm.PValue = value; }
        }

        private ProgressDialogViewModel pdvm;
        public ProgressDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (pdvm == null)
            {
                pdvm = new ProgressDialogViewModel();
            }

            DataContext = pdvm;
        }
    }
}
