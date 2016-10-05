using Ix4Models;
using Ix4Models.SettingsDataModel;
using SqlDataExtractor.Commands;
using SqlDataExtractor.Settings.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SqlDataExtractor.Settings.ViewModel
{
    public class DBSettingsViewModel : BaseViewModel
    {
        private MsSqlSettings _msSqlSettings;
        private readonly string _testConnectionButton = "Test connection";
        public DBSettingsViewModel(BaseLicsRequestSettings sqlSourceSettings)
        {
            if (sqlSourceSettings.DataSourceSettings!=null && sqlSourceSettings.DataSourceSettings is MsSqlSettings)
            {
                switch (sqlSourceSettings.SettingsName)
                {
                    case Ix4RequestProps.Articles:
                        
                        DBImportDataSettings = new MsSqlArticleSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlArticlesSettings);
                        break;
                    case Ix4RequestProps.Deliveries:
                       
                        DBImportDataSettings = new MsSqlDeliveriesSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                        break;
                    case Ix4RequestProps.Orders:
                        
                        DBImportDataSettings = new MsSqlOrdersSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlOrdersSettings);
                        break;
                    default:
                        break;
                }
                ///_folderSettingsModel = xmlPluginSettings.DataSourceSettings as XmlFolderSettingsModel;
            }
            else
            {
                switch (sqlSourceSettings.SettingsName)
                {
                    case Ix4RequestProps.Articles:
                        sqlSourceSettings.DataSourceSettings = new MsSqlArticlesSettings();
                        DBImportDataSettings = new MsSqlArticleSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlArticlesSettings);
                        break;
                    case Ix4RequestProps.Deliveries:
                        sqlSourceSettings.DataSourceSettings = new MsSqlDeliveriesSettings();
                        DBImportDataSettings = new MsSqlDeliveriesSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                        break;
                    case Ix4RequestProps.Orders:
                        sqlSourceSettings.DataSourceSettings = new MsSqlOrdersSettings();
                        DBImportDataSettings = new MsSqlOrdersSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlOrdersSettings);
                        break;
                    default:
                        break;
                }
            }
            _msSqlSettings = sqlSourceSettings.DataSourceSettings as MsSqlSettings;
            _testConnectionCommand = new DbConnectionCommand(this);
            DbNames = new ObservableCollection<string>();
            if(!string.IsNullOrEmpty(_msSqlSettings.ServerAdress) && !string.IsNullOrEmpty(_msSqlSettings.DataBaseName))
            {
                CheckMsSqlConnection();
            }
            
            DbConnectionStatus = _testConnectionButton;
        }
        DbConnectionCommand _testConnectionCommand;
        public ICommand TestConnectionCommand { get { return _testConnectionCommand; } }
        public UserControl DBImportDataSettings { get;set;}

        private string _dbConnectionStatus;

        public string DbConnectionStatus
        {
            get { return _dbConnectionStatus; }
            set { _dbConnectionStatus = value; OnPropertyChanged("DbConnectionStatus"); }
        }

        private string _connectionStatusError;

        public string ConnectionStatusError
        {
            get { return _connectionStatusError; }
            set { _connectionStatusError = value; OnPropertyChanged("ConnectionStatusError"); }
        }


        public string ServerAdress
        {
            get { return _msSqlSettings.ServerAdress; }
            set
            {
                _msSqlSettings.ServerAdress = value;
                OnPropertyChanged("ServerAdress");
              //  CheckMsSqlConnection();
            }
        }

        public void CheckMsSqlConnection()
        {
            string connectionString = _msSqlSettings.BuilDBConnection();
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();
                    DbConnectionStatus = string.Format("Connection Success. Server version is {0}", con.ServerVersion);
                    DataTable databases = con.GetSchema("Databases");
                    DbNames.Clear();
                    foreach (DataRow database in databases.Rows)
                    {
                        DbNames.Add(database.Field<String>("database_name"));
                    }
                }
            }
            catch (Exception ex)
            {
                DbConnectionStatus = "Failed connection";
                ConnectionStatusError = ex.Message;
            }
        }

        public ObservableCollection<string> DbNames { get; set; }

        public string SelectedDbName
        {
            get { return _msSqlSettings.DataBaseName; }
            set { _msSqlSettings.DataBaseName = value; OnPropertyChanged("DbName"); }
        }
        public bool UseSqlServierAuth
        {
            get { return _msSqlSettings.UseSqlServerAuth; }
            set { _msSqlSettings.UseSqlServerAuth = value; OnPropertyChanged("UseSqlServierAuth"); }
        }

        public string DbUserName
        {
            get { return _msSqlSettings.DbUserName; }
            set { _msSqlSettings.DbUserName = value; OnPropertyChanged("DbUserName");}
        }

        public string DbPassword
        {
            get { return _msSqlSettings.Password; }
            set { _msSqlSettings.Password = value; OnPropertyChanged("DbPassword");}
        }
    }
}
