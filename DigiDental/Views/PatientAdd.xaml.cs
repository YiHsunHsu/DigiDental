using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// PatientAdd.xaml 的互動邏輯
    /// </summary>
    public partial class PatientAdd : Window
    {
        public PatientAdd()
        {
            InitializeComponent();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
