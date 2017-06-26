﻿using DigiDental.Class;
using DigiDental.DataAccess.DbObject;
using DigiDental.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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
        private Patients pi;

        public Loading()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
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

                    //測試資料
                    pi = new Patients();
                    pi.Patient_ID = "0001";
                    pi.Patient_Number = "E0001";
                    pi.Patient_Name = "Eason";
                    pi.Patient_Gender = true;
                    pi.Patient_Birth = DateTime.Parse("1986-08-11");
                    pi.Patient_IDNumber = "W100399932";
                    
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
                        Clients c = isExistClient.First();
                        VerificationCodeClient = c.Agency_VerificationCode;
                    }
                    else//第一次使用，輸入驗證碼
                    {
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
                        Agencys a = checkAgencyStatus.First();
                        bool? IsVerify = a.Agency_IsVerify;
                        bool? IsTry = a.Agency_IsTry;
                        if ((bool)IsVerify)
                        {
                            if ((bool)IsTry)
                            {
                                if (a.Agency_TrialPeriod < DateTime.Now.Date)
                                {
                                    MessageBoxTips = "試用期限已到，請聯絡資訊廠商";
                                    MessageBox.Show(MessageBoxTips, "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                                else
                                {
                                    MessageBoxTips = "此為試用版本，試用日期至" + ((DateTime)a.Agency_TrialPeriod).ToShortDateString();
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
                    Thread.Sleep(1000);
                }
                else
                {
                    Status.Text = "登入失敗，原因:" + MessageBoxTips;
                    Status.Refresh();
                    Thread.Sleep(1000);
                }
                //回傳結果
                DialogResult = ReturnDialogResult;
            }
            catch(Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                DialogResult = ReturnDialogResult;
            }
        }
        /// <summary>
        /// 新增新病患
        /// </summary>
        private void addNewPatient()
        {
            //pi = (Application.Current as App).p;

            if (pi.Patient_ID != null)
            {
                var isExistPatient = from iep in dde.Patients
                                     where iep.Patient_ID == pi.Patient_ID
                                     select iep;
                if (isExistPatient.Count() == 0)
                {
                    dde.Patients.Add(pi);
                    dde.SaveChanges();
                }
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
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
