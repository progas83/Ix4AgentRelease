using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using Ix4Models;

namespace Ix4ServiceConfigurator.ViewModel
{
    public class MailSettingsViewModel : BaseViewModel
    {
        private MailNotificationSettings _mailSettings;

        public MailSettingsViewModel(MailNotificationSettings mailSettings)
        {
            this._mailSettings = mailSettings;
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
        public bool IsBodyHtml { get; set; }
        public NotificationRecipient[] Recipients { get; set; }
    }
}
