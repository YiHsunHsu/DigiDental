using DigiDental.ViewModels;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isLoaded = false;
        private string HostName { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
            if (!isLoaded)
            {
                Loading loading = new Loading();

                bool? result = loading.ShowDialog();
                if ((bool)result)
                {
                    HostName = loading.HostName;
                    isLoaded = true;
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
