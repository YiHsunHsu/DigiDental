using DigiDental.ViewModels;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private string HostName { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Loading Loading = new Loading();

            bool? result = Loading.ShowDialog();
            if ((bool)result)
            {
                HostName = Loading.HostName;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
