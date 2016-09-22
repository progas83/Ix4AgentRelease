using Ix4Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    public class MailNotificationSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public long TimeOut { get; set; }
        public bool EnableSSL { get; set; }
        public string MailFrom { get; set; }
        public string MailPass { get; set; }
        public bool IsBodyHtml { get; set; }
        public NotificationRecipient[] Recipients { get; set; }

        public MailNotificationSettings()
        {

        }

    }
    [Serializable]
    public class NotificationRecipient
    {
        public NotificationRecipient()
        {

        }
        [XmlAttribute]
        public MailLogLevel LogLevel { get; set; }

        [XmlAttribute]
        public string MailRecipient { get; set; }
    }
}
