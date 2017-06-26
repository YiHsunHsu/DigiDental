using DigiDental.Class;
using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace DigiDental.DataAccess.DbObject
{
    public class ConnectionString
    {
        /// <summary>
        /// App.config Server位置
        /// </summary>
        private string ServerIP = ConfigurationManager.AppSettings["Server"];
        /// <summary>
        /// 回傳EFConnectionString
        /// </summary>
        /// <returns></returns>
        public string EFConnectionString()
        {
            // Specify the provider name, server and database.
            string providerName = "System.Data.SqlClient";
            string serverName = ServerIP + @"\DigiDental";
            string databaseName = "DigiDental";
            string userID = "sa";
            string password = "0939566880";

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.UserID = userID;
            sqlBuilder.Password = password;
            sqlBuilder.IntegratedSecurity = true;
            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl";

            return entityBuilder.ToString();
        }

        /// <summary>
        /// 判斷是否能連線
        /// </summary>
        /// <returns></returns>
        public bool CheckConnection()
        {
            try
            {
                using (EntityConnection conn = new EntityConnection(EFConnectionString()))
                {
                    conn.Open();
                    conn.Close();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Error_Log.ErrorMessageOutput(ex.ToString());
                return false;
            }
        }
    }
}
