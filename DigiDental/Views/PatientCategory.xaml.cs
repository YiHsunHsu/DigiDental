using DigiDental.ViewModels;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// PatientCategory.xaml 的互動邏輯
    /// </summary>
    public partial class PatientCategory : Window
    {
        private PatientCategoryViewModel pcvm;

        public PatientCategory()
        {
            InitializeComponent();

            if (pcvm == null)
            {
                pcvm = new PatientCategoryViewModel();
            }
            DataContext = pcvm;
        }


        private void Button_CategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            var pc = new PatientCategories()
            {
                PatientCategory_Title = textBoxCategoryInput.Text
            };
            pcvm.PatientCategories.Add(pc);
        }

        private void Button_EditEnd_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
