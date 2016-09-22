using Ix4Models;
using Ix4Models.Interfaces;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataExtractor
{
    class SqlTableWorker
    {
        protected MsSqlPluginSettings _pluginSettings;
        protected static Logger _loger = Logger.GetLogger();
        public SqlTableWorker(IPluginSettings pluginSettings)
        {
            this._pluginSettings = pluginSettings as MsSqlPluginSettings;
        }
        protected string DbConnection
        {
            get
            {
                return _pluginSettings.DbSettings.UseSqlServerAuth ? string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWithServerAuth, _pluginSettings.DbSettings.ServerAdress,
                                                                                                         _pluginSettings.DbSettings.DataBaseName,
                                                                                                         _pluginSettings.DbSettings.DbUserName,
                                                                                                         _pluginSettings.DbSettings.Password) :
                                                                    string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _pluginSettings.DbSettings.ServerAdress,
                                                                                                         _pluginSettings.DbSettings.DataBaseName);

                //return   string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _pluginSettings.DbSettings.ServerAdress,
                //                                                                                    _pluginSettings.DbSettings.DataBaseName);

            }
        }
    }
}
