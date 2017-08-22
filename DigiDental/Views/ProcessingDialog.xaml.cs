using DigiDental.ViewModels;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// ProcessingDialog.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessingDialog : Window
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
        public bool PIsIndeterminate
        {
            get { return pdvm.PIsIndeterminate; }
            set { pdvm.PIsIndeterminate = value; }
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

        public string ButtonContent
        {
            get { return pdvm.ButtonContent; }
            set { pdvm.ButtonContent = value; }
        }

        /// <summary>
        /// 委派回傳ProcessingDialog 
        /// </summary>
        /// <param name="isDetecting">true:停止/falae:跳過</param>
        public delegate void ReturnValueDelegate(bool isDetecting);

        public event ReturnValueDelegate ReturnValueCallback;

        private ProcessingDialogViewModel pdvm;
        
        public ProcessingDialog()
        {
            InitializeComponent();

            if (pdvm == null)
            {
                pdvm = new ProcessingDialogViewModel();
            }

            DataContext = pdvm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReturnValueCallback(false);
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            ReturnValueCallback(true);
        }
    }
}
