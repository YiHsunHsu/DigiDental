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
                    p.Patient_ID = !string.IsNullOrEmpty(e.Args[0].ToString()) ? e.Args[0].ToString() : string.Empty;
                    p.Patient_Number = !string.IsNullOrEmpty(e.Args[1].ToString()) ? e.Args[1].ToString() : string.Empty;
                    p.Patient_Name = !string.IsNullOrEmpty(e.Args[2].ToString()) ? e.Args[2].ToString() : string.Empty;
                    p.Patient_Gender = TransGender(e.Args[3].ToString());
                    p.Patient_Birth = DateTime.TryParse(e.Args[4].ToString(), out DateTime patientBirth) ? DateTime.Parse(e.Args[4].ToString()) : default(DateTime);
                    p.Patient_IDNumber = !string.IsNullOrEmpty(e.Args[5].ToString()) ? e.Args[5].ToString() : string.Empty;
                }
                else
                {
                    //測試資料
                    //1. Patient = null
                    //p = new Patients();
                    //2.Patient Testing
                   p = new Patients()
                   {
                       Patient_ID = "0001",
                       Patient_Number = "E0001",
                       Patient_Name = "Eason",
                       Patient_Gender = true,
                       Patient_Birth = DateTime.Parse("1986-08-11"),
                       Patient_IDNumber = "W100399932"
                   };
                    //3. Patient 2 Testing
                    //p = new Patients()
                    //{
                    //    Patient_ID = "0005",
                    //    Patient_Number = "0005J",
                    //    Patient_Name = "JOE",
                    //    Patient_Gender = false,
                    //    Patient_Birth = DateTime.Parse("1984-11-27"),
                    //    Patient_IDNumber = "W100339105"
                    //};
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
        private bool TransGender(string input)
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
