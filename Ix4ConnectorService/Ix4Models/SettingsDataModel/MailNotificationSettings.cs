using Ix4Models.CryptoModule;
using Ix4Models.Enums;
using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class MailNotificationSettings : ICryptor
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

        public void Decrypt()
        {
            using (var cryptor = new Cryptor())
            {
                if (!string.IsNullOrEmpty(MailPass))
                    MailPass = cryptor.Decrypt(MailPass);
                if (!string.IsNullOrEmpty(MailPass))
                    MailPass = cryptor.Decrypt(MailPass);

            }
        }

        public void Encrypt()
        {
            using (var cryptor = new Cryptor())
            {
                if (!string.IsNullOrEmpty(MailPass))
                    MailPass = cryptor.Encrypt(MailPass);
                if (!string.IsNullOrEmpty(MailPass))
                    MailPass = cryptor.Encrypt(MailPass);
            }

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

        [XmlAttribute]
        public bool Enable { get; set; }
    }
}
