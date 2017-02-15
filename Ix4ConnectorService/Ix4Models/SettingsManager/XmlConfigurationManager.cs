using System;
using System.IO;
using Ix4Models.SettingsDataModel;
using System.Xml;
using System.Linq;

namespace Ix4Models.SettingsManager
{
    public class XmlConfigurationManager
    {
       
        private static object _padlock = new object();
        private CustomerInfoSerializer _xmlSerializer;
        private static XmlConfigurationManager _configrator = null;
        private XmlConfigurationManager()
        {
            _xmlSerializer = new CustomerInfoSerializer();
        }
        public static XmlConfigurationManager Instance
        {
            get
            {
                if (_configrator == null)
                {
                    lock (_padlock)
                    {
                        if (_configrator == null)
                        {
                            _configrator = new XmlConfigurationManager();
                        }
                    }
                }

                return _configrator;
            }
        }

        public CustomerInfo GetCustomerInformation()
        {
            CustomerInfo customerInfo = new CustomerInfo();

            if (!File.Exists(CurrentServiceInformation.FileName))
            {
                using (FileStream fs = new FileStream(CurrentServiceInformation.FileName, FileMode.CreateNew))
                {
                    _xmlSerializer.Serialize(fs, customerInfo);
                    fs.Flush(true);
                }
            }
            try
            {
                using (FileStream fs = new FileStream(CurrentServiceInformation.FileName, FileMode.OpenOrCreate))
                {
                    customerInfo = _xmlSerializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                customerInfo = new CustomerInfo();
            }

            return customerInfo;
        }

        public void UpdateCustomerInformation(CustomerInfo customerInfo)
        {
            try
            {
                using (FileStream fs = new FileStream(CurrentServiceInformation.FileName, FileMode.Truncate))
                {
                    _xmlSerializer.Serialize(fs, customerInfo);
                    fs.Flush(true);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void UpdateMailRecipientStatus(MailRecipient recipient)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(CurrentServiceInformation.FileName);
            XmlNodeList recipientsNodes = doc.GetElementsByTagName("MailRecipient");
            foreach(XmlNode recipNode in recipientsNodes)
            {
                if(recipNode.Attributes!=null && recipNode.Attributes["RecipientAdress"]!=null && recipNode.Attributes["RecipientAdress"].Value.Equals(recipient.RecipientAdress))
                {
                    recipNode.Attributes["Enable"].Value = recipient.EnableRecipient.ToString().ToLower();
                    doc.Save(CurrentServiceInformation.FileName);
                    break;
                }
            }
        }
    }
}
