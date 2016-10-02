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
          return msSqlSettings.UseSqlServerAuth ? string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWithServerAuth, msSqlSettings.ServerAdress,
                                                                                                         msSqlSettings.DataBaseName,
                                                                                                         msSqlSettings.DbUserName,
                                                                                                        msSqlSettings.Password) :
                                                                    string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, msSqlSettings.ServerAdress,
                                                                                                         msSqlSettings.DataBaseName);

        }
    }
}
