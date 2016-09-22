using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ix4ServiceConfigurator.Commands
{
    public class InstallationCommand : ICommand
    {
        public event EventHandler ServiceInfoNeedToUpdate;
        public event EventHandler CanExecuteChanged;
        private bool _canExecute = true;
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _canExecute = false;
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }

            bool isExist = (bool)parameter;
            try
            {
                if (!isExist)
                {
                    AutomaticalServiceInstaller.InstallService();
                }
                else
                {
                    AutomaticalServiceInstaller.UninstallService();
                }

            }
            finally
            {
                if (ServiceInfoNeedToUpdate != null)
                {
                    ServiceInfoNeedToUpdate(this, new EventArgs());
                }
                _canExecute = true;
                if(CanExecuteChanged!=null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }
            
        }
    }
}
