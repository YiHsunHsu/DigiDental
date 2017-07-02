using DigiDental.Class;
using DigiDental.DataAccess.DbObject;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;

namespace DigiDental.Views
{
    /// <summary>
    /// Loading.xaml 的互動邏輯
    /// </summary>
    public partial class Loading : Window
    {
        public string HostName { get; set; }

        private string MessageBoxTips = string.Empty;
        private bool ReturnDialogResult = false;
        
        private DigiDentalEntities dde;
        public Agencys Agencys { get; set; }
        public Patients Patients { get; set; }

        public Loading()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                Status.Text = "伺服器位置確認中...";
                Status.Refresh();

                //判斷app.config
                if (!ConfigManage.ReadSetting("Server"))//尚未設置
                {
                    InputDialog idServerIP = new InputDialog("請輸入伺服器位置:", "IP");
                    if (idServerIP.ShowDialog() == true)
                    {
                        //寫入config Server 欄位
                        string serverIP = idServerIP.Answer;
                        ConfigManage.AddUpdateAppCongig("Server", serverIP);
                    }
                }

                Status.Text = "嘗試連接伺服器...";
                Status.Refresh();

                //連接Server connection
                if (new ConnectionString().CheckConnection())
                {
                    dde = new DigiDentalEntities();
                    
                    Status.Text = "取得本機資訊...";
                    Status.Refresh();

                    //取得本機訊息
                    //HostName IP
                    string LocalIP = string.Empty;
                    HostName = Dns.GetHostName();
                    IPHostEntry ipHostEntry = Dns.GetHostEntry(HostName);
                    foreach (IPAddress ip in ipHostEntry.AddressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            LocalIP = ip.ToString();
                        }
                    }

                    Status.Text = "確認本機註冊資訊...";
                    Status.Refresh();

                    //判斷client 有無資料
                    var isExistClient = from iec in dde.Clients
                                        where iec.Client_HostName == HostName
                                        select iec;
                    string VerificationCodeClient = string.Empty;
                    if (isExistClient.Count() > 0)//已註冊//判斷VerificationCode 與Server的狀態
                    {
                        Status.Text = "本機已註冊...";
                        Status.Refresh();
                        
                        VerificationCodeClient = isExistClient.First().Agency_VerificationCode;
                    }
                    else//第一次使用，輸入驗證碼
                    {
                        Status.Text = "本機尚未註冊...";
                        Status.Refresh();

                        InputDialog idVerify = new InputDialog("第一次登入，請輸入產品驗證碼:", "Verify");
                        if (idVerify.ShowDialog() == true)
                        {
                            VerificationCodeClient = idVerify.Answer;
                            dde.Clients.Add(new Clients
                            {
                                Client_HostName = HostName,
                                Client_IP = LocalIP,
                                Agency_VerificationCode = VerificationCodeClient
                            }
                                            );
                            dde.SaveChanges();
                        }
                    }

                    Status.Text = "取得伺服器認證資訊...";
                    Status.Refresh();

                    //用驗證碼(VerificationCodeClient)與Agencys確認目前狀態
                    var checkAgencyStatus = from cas in dde.Agencys
                                            where cas.Agency_VerificationCode == VerificationCodeClient
                                            select cas;
                    if (checkAgencyStatus.Count() > 0)
                    {
                        Agencys = checkAgencyStatus.First();
                        bool? IsVerify = Agencys.Agency_IsVerify;
                        bool? IsTry = Agencys.Agency_IsTry;
                        if ((bool)IsVerify)
                        {
                            if ((bool)IsTry)
                            {
                                if (Agencys.Agency_TrialPeriod < DateTime.Now.Date)
                                {
                                    MessageBoxTips = "試用期限已到，請聯絡資訊廠商";
                                    MessageBox.Show(MessageBoxTips, "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                                else
                                {
                                    MessageBoxTips = "此為試用版本，試用日期至" + ((DateTime)Agencys.Agency_TrialPeriod).ToShortDateString();
                                    MessageBox.Show(MessageBoxTips, "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                                    Status.Text = "病患資訊確認中...";
                                    Status.Refresh();

                                    //判斷病患
                                    addNewPatient();
                                    ReturnDialogResult = true;
                                }
                            }
                            else
                            {
                                Status.Text = "病患資訊確認中...";
                                Status.Refresh();

                                //判斷此病患
                                addNewPatient();
                                ReturnDialogResult = true;
                            }
                        }
                        else
                        {
                            MessageBoxTips = "此驗證碼已停用，如欲使用請聯絡資訊廠商";
                            MessageBox.Show(MessageBoxTips, "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        MessageBoxTips = "伺服器尚未建立認證";
                        MessageBox.Show(MessageBoxTips, "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    //寫入ConnectingLog資訊
                    addConnectingLog(HostName, LocalIP, MessageBoxTips, ReturnDialogResult);
                }
                else
                {
                    MessageBoxTips = "伺服器連接失敗";
                    MessageBox.Show(MessageBoxTips, "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    ConfigManage.AddUpdateAppCongig("Server", "");
                }

                if (ReturnDialogResult)
                {
                    Status.Text = "成功登入，歡迎使用DigiDental...";
                    Status.Refresh();
                }
                else
                {
                    Status.Text = "登入失敗，原因:" + MessageBoxTips;
                    Status.Refresh();
                }
                //回傳結果
                DialogResult = ReturnDialogResult;
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                DialogResult = ReturnDialogResult;
            }
            Thread.Sleep(1000);
            Close();
        }
        /// <summary>
        /// 新增新病患
        /// </summary>
        private void addNewPatient()
        {

            //Data from AppStartup

            if (((Application.Current as App).p).Patient_ID != null)
            {
                Patients = (Application.Current as App).p;
                var isExistPatient = from iep in dde.Patients
                                     where iep.Patient_ID == Patients.Patient_ID
                                     select iep;
                if (isExistPatient.Count() == 0)
                {
                    dde.Patients.Add(Patients);
                    dde.SaveChanges();
                }
                else
                {
                    Patients = isExistPatient.First();
                }
            }
            else
            {
                //測試資料
                Patients = new Patients()
                {
                    Patient_ID = "0001",
                    Patient_Number = "E0001",
                    Patient_Name = "Eason",
                    Patient_Gender = true,
                    Patient_Birth = DateTime.Parse("1986-08-11"),
                    Patient_IDNumber = "W100399932"
                };
            }
        }
        /// <summary>
        /// 寫入連線資訊
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="ip"></param>
        /// <param name="isPermit"></param>
        private void addConnectingLog(string hostName, string ip, string errorMsg, bool isPermit)
        {
            dde.Connecting_Logs.Add(new Connecting_Logs { Connecting_Log_HostName = hostName,
                                                        Connecting_Log_IP = ip,
                                                        Connecting_Log_Error = errorMsg,
                                                        Connecting_Log_IsPermit = isPermit }
                                    );
            dde.SaveChanges();
        }
    }
}
