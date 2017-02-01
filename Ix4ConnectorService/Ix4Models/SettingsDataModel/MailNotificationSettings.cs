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
        public MailRecipient[] Recipients { get; set; }
        public MailNotificationSettings()
        {
            Recipients = new MailRecipient[0];
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
    public class MailRecipient
    {
        public MailRecipient()
        {

        }
        [XmlAttribute]
        public MailLogLevel LogLevel { get; set; }

        [XmlAttribute]
        public string RecipientAdress { get; set; }

        [XmlAttribute(AttributeName = "Enable")]
        public bool EnableRecipient { get; set; }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //        return false;
        //    MailRecipient recip = obj as MailRecipient;

        //    if (recip == null)
        //        return false;

        //    return RecipientAdress.Equals(recip.RecipientAdress);
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }
}
