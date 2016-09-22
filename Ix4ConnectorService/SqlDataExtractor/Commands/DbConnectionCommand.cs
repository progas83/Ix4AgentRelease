using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SqlDataExtractor.DatabaseSettings.ViewModel;
using System.Data.SqlClient;
using Ix4Models;
using System.Data;

namespace SqlDataExtractor.Commands
{
    public class DbConnectionCommand : ICommand
    {
        private MainDBSettingsViewModel _viewModel;

        public DbConnectionCommand(MainDBSettingsViewModel mainDBSettingsViewModel)
        {
            this._viewModel = mainDBSettingsViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool allDataFilled =    _viewModel!=null && 
                                    !string.IsNullOrEmpty(_viewModel.ServerAdress) && 
                                    !string.IsNullOrEmpty(_viewModel.DbName) &&
                                    (!_viewModel.UseSqlServierAuth || (!string.IsNullOrEmpty(_viewModel.DbName) && !string.IsNullOrEmpty(_viewModel.DbPassword)));
            return allDataFilled;
        }

        public void Execute(object parameter)
        {
            try
            {
                using (var connection = new SqlConnection(DbConnection))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        _viewModel.DbConnectionStatus =string.Format( "Connection Success. Server version is {0}",connection.ServerVersion);
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                _viewModel.ConnectionStatusError = ex.Message;
                _viewModel.DbConnectionStatus = "Connection Failed";
            }
            
        }

        private string DbConnection
        {
            get
            {

                if (_viewModel == null)
                    return string.Empty;

                return _viewModel.UseSqlServierAuth ? string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWithServerAuth, _viewModel.ServerAdress,
                                                                                                        _viewModel.DbName,
                                                                                                        _viewModel.DbUserName,
                                                                                                        _viewModel.DbPassword) :
                                                                    string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _viewModel.ServerAdress,
                                                                                                         _viewModel.DbName);

            }
        }

        internal void CheckTestAvailable()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
