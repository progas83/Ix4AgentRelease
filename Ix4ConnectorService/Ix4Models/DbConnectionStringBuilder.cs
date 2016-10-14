using Ix4Models.SettingsDataModel;
using System.Text;

namespace Ix4Models
{
    public static class DbConnectionStringBuilder
    {
        public static string BuilDBConnection(this MsSqlSettings msSqlSettings)
        {
            return GetConnectionString(msSqlSettings);
        }

        private static string GetConnectionString(MsSqlSettings msSqlSettings)
        {
            StringBuilder dbConnection = new StringBuilder();
            if (!string.IsNullOrEmpty(msSqlSettings.ServerAdress))
            {
                dbConnection.Append(string.Format("Data Source ={0};", msSqlSettings.ServerAdress));
            }

            if (!string.IsNullOrEmpty(msSqlSettings.DataBaseName))
            {
                dbConnection.Append(string.Format("Initial Catalog = {0};", msSqlSettings.DataBaseName));
            }
            if (msSqlSettings.UseSqlServerAuth)
            {
                dbConnection.Append("Network Library = DBMSSOCN;");
                dbConnection.Append(string.Format("User ID={0};", msSqlSettings.DbUserName));
                dbConnection.Append(string.Format("Password={0}", msSqlSettings.Password));
            }
            else
            {
                dbConnection.Append("Integrated Security=SSPI");
            }

            return dbConnection.ToString();

        }
    }
}
