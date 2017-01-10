using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4ServiceConfigurator.Commands;
using Ix4ServiceConfigurator.View.MsSql;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ix4ServiceConfigurator.ViewModel.MsSql
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
                    case Ix4ImportDataTypes.Articles:
                        
                        DBImportDataSettings = new MsSqlArticleSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlArticlesSettings);
                        break;
                    case Ix4ImportDataTypes.Deliveries:
                       
                        DBImportDataSettings = new MsSqlDeliveriesSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                        break;
                    case Ix4ImportDataTypes.Orders:
                        
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
                    case Ix4ImportDataTypes.Articles:
                        sqlSourceSettings.DataSourceSettings = new MsSqlArticlesSettings();
                        DBImportDataSettings = new MsSqlArticleSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlArticlesSettings);
                        break;
                    case Ix4ImportDataTypes.Deliveries:
                        sqlSourceSettings.DataSourceSettings = new MsSqlDeliveriesSettings();
                        DBImportDataSettings = new MsSqlDeliveriesSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                        break;
                    case Ix4ImportDataTypes.Orders:
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
            if(!string.IsNullOrEmpty(_msSqlSettings.ServerAdress))
            {
                CheckMsSqlConnection();
            }
            
            DbConnectionStatus = _testConnectionButton;
            DbSettingsView = new DBSettingsView();
            DbSettingsView.DataContext = this;
        }

        public UserControl DbSettingsView { get; private set; }

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
                _msSqlSettings.DataBaseName = string.Empty;
            }
        }

        public ObservableCollection<string> DbNames { get; set; }

        public string SelectedDbName
        {
            get { return _msSqlSettings.DataBaseName; }
            set { _msSqlSettings.DataBaseName = value; OnPropertyChanged("SelectedDbName"); }
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
