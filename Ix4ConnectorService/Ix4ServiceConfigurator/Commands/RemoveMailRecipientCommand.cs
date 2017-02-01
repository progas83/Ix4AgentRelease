using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ix4Models.SettingsDataModel;

namespace Ix4ServiceConfigurator.Commands
{
    public class RemoveMailRecipientCommand : ICommand
    {
        private ObservableCollection<MailRecipient> recipients;

        public RemoveMailRecipientCommand(ObservableCollection<MailRecipient> recipients)
        {
            this.recipients = recipients;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true; // throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            recipients.Remove(parameter as MailRecipient);
        }
    }
}
