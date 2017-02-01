using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ix4ServiceConfigurator.Commands
{
    public class AddMailRecipientCommand : ICommand
    {
        private ObservableCollection<MailRecipient> _recipients;

        public AddMailRecipientCommand(ObservableCollection<MailRecipient> recipients)
        {
            this._recipients = recipients;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            TextBox tb = parameter as TextBox;
            if(tb!=null && !string.IsNullOrEmpty(tb.Text))
            {
                MailRecipient addedRecipient = new MailRecipient { RecipientAdress = tb.Text, EnableRecipient = true };
                if (_recipients != null && addedRecipient != null && !_recipients.Contains(addedRecipient))
                {
                    _recipients.Add(addedRecipient);
                }
                tb.Clear();
            }
           
        }
    }
}
