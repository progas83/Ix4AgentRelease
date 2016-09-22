using Ix4Models;
using Ix4Models.SettingsDataModel;
using SqlDataExtractor.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SqlDataExtractor.DatabaseSettings.ViewModel
{
   public class MainDBSettingsViewModel : BaseViewModel
    {
        MsSqlPluginSettings _msSqlPluginSettings;
        DbConnectionCommand _testConnectionCommand;
        private readonly string _testConnectionButton = "Test connection";
        public MainDBSettingsViewModel(MsSqlPluginSettings msSqlPluginSettings)
        {
            CurrentPluginSettings = msSqlPluginSettings;
            _testConnectionCommand = new DbConnectionCommand(this);
            DbConnectionStatus = _testConnectionButton;
        }

        public MsSqlPluginSettings CurrentPluginSettings
        {
            get { return _msSqlPluginSettings; }
            private set { _msSqlPluginSettings = value; }
        }

        public ICommand TestConnectionCommand { get { return _testConnectionCommand; } }
        public bool IsActivated
        {
            get
            {
                return _msSqlPluginSettings.IsActivated;
            }
            set
            {
                _msSqlPluginSettings.IsActivated = value;
                OnPropertyChanged("IsActivated");
            }
        }

        private void CheckTestConnectionAvailable()
        {
            if (_testConnectionCommand != null)
            {
                _testConnectionCommand.CheckTestAvailable();
            }
            DbConnectionStatus = _testConnectionButton;
        }

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
            get { return _msSqlPluginSettings.DbSettings.ServerAdress; }
            set
            {
                _msSqlPluginSettings.DbSettings.ServerAdress = value;
                OnPropertyChanged("ServerAdress");
                CheckTestConnectionAvailable();
            }
        }

        public string DbName
        {
            get { return _msSqlPluginSettings.DbSettings.DataBaseName; }
            set { _msSqlPluginSettings.DbSettings.DataBaseName = value; OnPropertyChanged("DbName"); CheckTestConnectionAvailable(); }
        }
        public bool UseSqlServierAuth
        {
            get { return _msSqlPluginSettings.DbSettings.UseSqlServerAuth; }
            set { _msSqlPluginSettings.DbSettings.UseSqlServerAuth = value; OnPropertyChanged("UseSqlServierAuth"); CheckTestConnectionAvailable(); }
        }

        public string DbUserName
        {
            get { return _msSqlPluginSettings.DbSettings.DbUserName; }
            set { _msSqlPluginSettings.DbSettings.DbUserName = value; OnPropertyChanged("DbUserName"); CheckTestConnectionAvailable(); }
        }

        public string DbPassword
        {
            get { return _msSqlPluginSettings.DbSettings.Password; }
            set { _msSqlPluginSettings.DbSettings.Password = value; OnPropertyChanged("DbPassword"); CheckTestConnectionAvailable(); }
        }

        public bool CheckArticles
        {
            get { return _msSqlPluginSettings.CheckArticles; }
            set { _msSqlPluginSettings.CheckArticles = value; OnPropertyChanged("CheckArticles"); }
        }

        public string ArticlesQuery
        {
            get { return _msSqlPluginSettings.ArticlesQuery; }
            set { _msSqlPluginSettings.ArticlesQuery = value; OnPropertyChanged("ArticlesQuery"); }
        }

        public bool CheckDeliveries
        {
            get { return _msSqlPluginSettings.CheckDeliveries; }
            set { _msSqlPluginSettings.CheckDeliveries = value; OnPropertyChanged("CheckDeliveries"); }
        }

        public string DeliveriesQuery
        {
            get { return _msSqlPluginSettings.DeliveriesQuery; }
            set { _msSqlPluginSettings.DeliveriesQuery = value; OnPropertyChanged("DeliveriesQuery"); }
        }

        public string DeliveryPositionsQuery
        {
            get { return _msSqlPluginSettings.DeliveryPositionsQuery; }
            set { _msSqlPluginSettings.DeliveryPositionsQuery = value; OnPropertyChanged("DeliveryPositionsQuery"); }
        }

        public bool CheckOrders
        {
            get { return _msSqlPluginSettings.CheckOrders; }
            set { _msSqlPluginSettings.CheckOrders = value;  OnPropertyChanged("CheckOrders"); }
        }

        public string OrdersQuery
        {
            get { return _msSqlPluginSettings.OrdersQuery; }
            set { _msSqlPluginSettings.OrdersQuery = value; OnPropertyChanged("OrdersQuery"); }
        }

        public string OrderRecipientQuery
        {
            get { return _msSqlPluginSettings.OrderRecipientQuery; }
            set { _msSqlPluginSettings.OrderRecipientQuery = value; OnPropertyChanged("OrderRecipientQuery"); }
        }

        public string OrderPositionsQuery
        {
            get { return _msSqlPluginSettings.OrderPositionsQuery; }
            set { _msSqlPluginSettings.OrderPositionsQuery = value; OnPropertyChanged("OrderPositionsQuery"); }
        }

    }
}
