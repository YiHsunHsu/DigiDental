using System.Configuration;
namespace DigiDental.Class
{
    public class ConfigManage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings[key] != null)
                {
                    if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                        return true;
                }
                return false;
            }
            catch (ConfigurationErrorsException cex)
            {
                Error_Log.ErrorMessageOutput(cex.ToString());
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddUpdateAppCongig(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException cex)
            {
                Error_Log.ErrorMessageOutput(cex.ToString());
            }
        }
    }
}
