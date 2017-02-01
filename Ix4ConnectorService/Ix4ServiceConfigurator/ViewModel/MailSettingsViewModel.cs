using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using Ix4Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Ix4ServiceConfigurator.Commands;

namespace Ix4ServiceConfigurator.ViewModel
{
    public class MailSettingsViewModel : BaseViewModel
    {
        private MailNotificationSettings _mailSettings;

        public event EventHandler CanExecuteChanged;

        public MailSettingsViewModel(MailNotificationSettings mailSettings)
        {
            this._mailSettings = mailSettings;
            
            Recipients = new ObservableCollection<MailRecipient>(_mailSettings.Recipients);// (mailSettings.Recipients);
            Recipients.CollectionChanged += OnRecipientsCollectionChanged;
           // Recipients.Add(new MailRecipient() { RecipientAdress = "test@terst.tu", EnableRecipient = true });
            RemoveRecipient = new RemoveMailRecipientCommand(Recipients);
            AddRecipient = new AddMailRecipientCommand(Recipients);
        }

        private void OnRecipientsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _mailSettings.Recipients = Recipients.ToArray();
        }

        public string Host
        {
            get
            {
                return _mailSettings.Host;
            }
            set
            {
                _mailSettings.Host = value;
                OnPropertyChanged("Host");
            }

        }

        public int Port
        {
            get { return _mailSettings.Port; }
            set
            {
                _mailSettings.Port = value;
                OnPropertyChanged("Port");
            }
        }
        public long TimeOut
        {
            get { return _mailSettings.TimeOut; }
            set
            {
                _mailSettings.TimeOut = value;
                OnPropertyChanged("TimeOut");
            }
        }
        public bool EnableSSL
        {
            get { return _mailSettings.EnableSSL; }
            set
            {
                _mailSettings.EnableSSL = value;
                OnPropertyChanged("EnableSSL");
            }
        }
        public string MailFrom
        {
            get { return _mailSettings.MailFrom; }
            set
            {
                _mailSettings.MailFrom = value;
                OnPropertyChanged("MailFrom");
            }
        }
        public string MailPass
        {
            get { return _mailSettings.MailPass; }
            set
            {
                _mailSettings.MailPass = value;
                OnPropertyChanged("MailPass");
            }
        }
       // public bool IsBodyHtml { get; set; }
        public ObservableCollection<MailRecipient> Recipients { get; set; }

        public ICommand RemoveRecipient { get; set; }

        public ICommand AddRecipient { get; set; }

        //public bool CanExecute(object parameter)
        //{
        //    return true;//  throw new NotImplementedException();
        //}

        //public void Execute(object parameter)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
