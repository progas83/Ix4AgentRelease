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
using SqlDataExtractor.Settings.ViewModel;

namespace SqlDataExtractor.Commands
{
    public class DbConnectionCommand : ICommand
    {
        private DBSettingsViewModel _viewModel;

        public DbConnectionCommand(DBSettingsViewModel mainDBSettingsViewModel)
        {
            this._viewModel = mainDBSettingsViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            //bool allDataFilled =    _viewModel!=null && 
            //                        !string.IsNullOrEmpty(_viewModel.ServerAdress) && 
            //                        !string.IsNullOrEmpty(_viewModel.DbName) &&
            //                        (!_viewModel.UseSqlServierAuth || (!string.IsNullOrEmpty(_viewModel.DbName) && !string.IsNullOrEmpty(_viewModel.DbPassword)));
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.CheckMsSqlConnection();
        }
    }
}
