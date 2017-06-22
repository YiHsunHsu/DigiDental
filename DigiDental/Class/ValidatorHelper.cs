using System.Text.RegularExpressions;

namespace DigiDental.Class
{
    public class ValidatorHelper
    {
        /// <summary>
        /// 驗證IP
        /// </summary>
        /// <param name="input">要驗證的字串</param>
        /// <returns>驗證通過返回true</returns>
        public static bool IsIP(string input)
        {
            return Regex.IsMatch(input, RegularExp.IP);
        }
    }

    public struct RegularExp
    {
        public const string IP = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";
    }
}
