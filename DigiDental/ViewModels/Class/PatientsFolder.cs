using System;

namespace DigiDental.ViewModels.Class
{
    public class PatientsFolder
    {
        //..\病患資料夾\PatientPhoto
        public string PatientPhotoPath { get; private set; }
        //Agencys_ImagePath\病患資料夾
        public string PatientFullBasePath { get; private set; }
        //Agencys_ImagePath\病患資料夾\PatientPhoto
        public string PatientFullPatientPhotoPath { get; private set; }
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

        /// <summary>
        /// 病患大頭貼路徑
        /// </summary>
        /// <param name="agencys"></param>
        /// <param name="patients"></param>
        public PatientsFolder(Agencys agencys, Patients patients)
        {
            //..\病患資料夾\PatientPhoto
            PatientPhotoPath = @"\" + patients.Patient_ID + @"\PatientPhoto";
            //Agencys_ImagePath\病患資料夾
            PatientFullBasePath = agencys.Agency_ImagePath + @"\" + patients.Patient_ID;
            //Agencys_ImagePath\病患資料夾\PatientPhoto
            PatientFullPatientPhotoPath = PatientFullBasePath + @"\PatientPhoto";
        }
        /// <summary>
        /// 病患影像資料夾(含掛號日底下)
        /// </summary>
        /// <param name="agencys">機構資訊</param>
        /// <param name="patients">病患資訊</param>
        /// <param name="registrationDate">掛號日</param>
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
