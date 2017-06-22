using DigiDental.Class;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// Loading.xaml 的互動邏輯
    /// </summary>
    public partial class Loading : Window
    {
        public Loading()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //判斷app.config
                //Server 欄位
                if (!ConfigManage.ReadSetting("Server"))
                {
                    InputDialog idServerIP = new InputDialog("請輸入伺服器位置:", "IP");
                    if (idServerIP.ShowDialog() == true)
                    {
                        string serverIP = idServerIP.Answer;
                        ConfigManage.AddUpdateAppCongig("Server", serverIP);
                    }
                }

                //連接Server connection

                //判斷client 有無註冊
                //取得本機MAC Address IP
                //第一次使用，輸入驗證碼
                InputDialog idVerify = new InputDialog("請輸入驗證碼:", "Verify");
                if (idVerify.ShowDialog() == true)
                {
                    string verificationCode = idVerify.Answer;
                }

                //判斷有無此病患
                DialogResult = true;
            }
            catch
            {
                DialogResult = false;
            }
        }
    }
}
