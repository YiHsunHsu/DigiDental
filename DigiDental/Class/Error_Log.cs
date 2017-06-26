using System;
using System.IO;

namespace DigiDental.Class
{
    public class Error_Log
    {
        private static string errorLogFolder = AppDomain.CurrentDomain.BaseDirectory + @"\ErrorLog";

        public static void ErrorMessageOutput(string ErrMsg)
        {
            if (!Directory.Exists(errorLogFolder))
            {
                Directory.CreateDirectory(errorLogFolder);
            }
            string fileName = DateTime.Now.ToString("yyyyMMdd") + @".txt";
            string OutputMsg = "*****" + DateTime.Now + "*****\r\n" + ErrMsg + "\r\n";
            File.AppendAllText(errorLogFolder + @"\" + fileName, OutputMsg);
        }
    }
}