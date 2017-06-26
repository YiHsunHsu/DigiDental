using DigiDental.Class;
using System;
using System.Windows;

namespace DigiDental
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public Patients p = new Patients();
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                if (e.Args.Length > 0)
                {
                    DateTime patientBirth;
                    
                    p.Patient_ID = !string.IsNullOrEmpty(e.Args[0].ToString()) ? e.Args[0].ToString() : string.Empty;
                    p.Patient_Number = !string.IsNullOrEmpty(e.Args[1].ToString()) ? e.Args[1].ToString() : string.Empty;
                    p.Patient_Name = !string.IsNullOrEmpty(e.Args[2].ToString()) ? e.Args[2].ToString() : string.Empty;
                    p.Patient_Gender = transGender(e.Args[3].ToString());
                    p.Patient_Birth = DateTime.TryParse(e.Args[4].ToString(), out patientBirth) ? DateTime.Parse(e.Args[4].ToString()) : default(DateTime);
                    p.Patient_IDNumber = !string.IsNullOrEmpty(e.Args[5].ToString()) ? e.Args[5].ToString() : string.Empty;
                }
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                MessageBox.Show("帶入的參數有誤，DigiDental無法啟動", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
        private bool transGender(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.ToUpper().Equals("M"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
