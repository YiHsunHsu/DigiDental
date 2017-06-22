using DigiDental.ViewModels;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// InputDialog.xaml 的互動邏輯
    /// </summary>
    public partial class InputDialog : Window
    {
        /// <summary>
        /// 回傳字串
        /// </summary>
        private string answer;
        public string Answer
        {
            get
            {
                return answer;
            }
            set
            {
                answer = value;
            }
        }

        private InputDialogViewModel idvm;

        public InputDialog(string question, string mode)
        {
            InitializeComponent();

            if (idvm == null)
            {
                idvm = new InputDialogViewModel(mode) { Answer = txtAnswer.Text, IsValid = false };
            }

            DataContext = idvm;

            Question.Content = question;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Answer = idvm.Answer;
            DialogResult = true;
        }
    }
}
