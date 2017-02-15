using Ix4Models.SettingsDataModel;
using Ix4Models.SettingsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ix4ServiceConfigurator.Commands
{
    public class SelectMailRecipientCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        //public event EventHandler NeedToUpdate;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MailRecipient recipient = parameter as MailRecipient;
            XmlConfigurationManager.Instance.UpdateMailRecipientStatus(recipient);
            // recipient.EnableRecipient = !recipient.EnableRecipient;
           // NeedToUpdate?.Invoke(this, new EventArgs());
        }
    }
}
