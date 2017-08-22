using System;

namespace DigiDental.ViewModels.Class
{
    public class PatientsFolder
    {
        //..\病患資料夾\掛號日期
        public string PatientFolderPath { get; private set; }
        //..\病患資料夾\掛號日期\Original
        public string PatientFolderPathOriginal { get; private set; }
        //..\病患資料夾\掛號日期\Small
        public string PatientFolderPathSmall { get; private set; }
        //Agencys_ImagePath\病患資料夾\掛號日期
        public string PatientFullFolderPath { get; private set; }
        //Agencys_ImagePath\病患資料夾\掛號日期\Original
        public string PatientFullFolderPathOriginal { get; private set; }
        //Agencys_ImagePath\病患資料夾\掛號日期\Small
        public string PatientFullFolderPathSmall { get; private set; }

        public PatientsFolder(Agencys agencys, Patients patients, DateTime registrationDate)
        {
            //..\病患資料夾\掛號日期
            PatientFolderPath = patients.Patient_ID + @"\" + registrationDate.ToString("yyyyMMdd");
            //..\病患資料夾\掛號日期\Original
            PatientFolderPathOriginal = PatientFolderPath + @"\Original";
            //..\病患資料夾\掛號日期\Small
            PatientFolderPathSmall = PatientFolderPath + @"\Small";
            //Agencys_ImagePath\病患資料夾\掛號日期
            PatientFullFolderPath = agencys.Agency_ImagePath + @"\" + PatientFolderPath;
            //Agencys_ImagePath\病患資料夾\掛號日期\Original
            PatientFullFolderPathOriginal = PatientFullFolderPath + @"\Original";
            //Agencys_ImagePath\病患資料夾\掛號日期\Small
            PatientFullFolderPathSmall = PatientFullFolderPath + @"\Small";
        }
    }
}
