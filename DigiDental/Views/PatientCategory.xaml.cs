using DigiDental.Class;
using DigiDental.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DigiDental.Views
{
    /// <summary>
    /// PatientCategory.xaml 的互動邏輯
    /// </summary>
    public partial class PatientCategory : Window
    {
        private DigiDentalEntities dde;
        private PatientCategoryViewModel pcvm;

        public PatientCategory()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dde == null)
            {
                dde = new DigiDentalEntities();
            }

            if (pcvm == null)
            {
                pcvm = new PatientCategoryViewModel();
            }
            DataContext = pcvm;
        }

        private void Button_CategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pc = new PatientCategories()
                {
                    PatientCategory_Title = textBoxCategoryInput.Text
                };
                dde.PatientCategories.Add(pc);
                dde.SaveChanges();
                pcvm.PatientCategories = dde.PatientCategories.ToList();
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private void Button_CategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PatientCategories patientCategories = ((FrameworkElement)sender).DataContext as PatientCategories;
                if (MessageBox.Show("確定刪除<" + patientCategories.PatientCategory_Title + ">分類?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var deleteItem = dde.PatientCategories.Where(w => w.PatientCategory_ID == patientCategories.PatientCategory_ID).First();
                    dde.PatientCategories.Remove(deleteItem);
                    dde.SaveChanges();
                    pcvm.PatientCategories = dde.PatientCategories.ToList();
                }
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private void DataGrid_Update_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            try
            {
                string editTex = ((TextBox)e.EditingElement).Text;
                PatientCategories patientCategories = ((FrameworkElement)e.Row).DataContext as PatientCategories;
                if (!patientCategories.PatientCategory_Title.Equals(editTex))
                {
                    if (MessageBox.Show("確定將\r\n<" + patientCategories.PatientCategory_Title + ">\r\n修改為\r\n<" + editTex + ">?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        PatientCategories updateItem = (from pc in dde.PatientCategories
                                                        where pc.PatientCategory_ID == patientCategories.PatientCategory_ID
                                                        select pc).First();
                        updateItem.PatientCategory_Title = editTex;
                        dde.SaveChanges();
                    }
                    else
                    {
                        pcvm.PatientCategories = dde.PatientCategories.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
            }
        }

        private void Button_EditEnd_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
