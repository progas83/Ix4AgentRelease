using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //   public const string MsSqlDatabaseConnectionStringWindowsAuth = @"Data Source ={0};Initial Catalog = {1};Integrated Security=SSPI";
        // public const string MsSqlDatabaseConnectionStringWithServerAuth = @"Server={0};Network Library=DBMSSOCN; Database = {1}; User Id= {2}; Password={3};";

        // public const string MsSqlDatabaseConnectionStringWithServerAuth = @"Data Source={0};Network Library=DBMSSOCN;Initial Catalog={1};User ID={2};Password={3}";
        //return msSqlSettings.UseSqlServerAuth ? string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWithServerAuth, msSqlSettings.ServerAdress,
        //                                                                                               msSqlSettings.DataBaseName,
        //                                                                                               msSqlSettings.DbUserName,
        //                                                                                              msSqlSettings.Password) :
        //                                                          string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, msSqlSettings.ServerAdress,
        //                                                                                               msSqlSettings.DataBaseName);



    }
}
