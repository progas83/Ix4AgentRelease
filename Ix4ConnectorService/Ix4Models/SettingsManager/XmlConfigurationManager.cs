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

        public void SaveLocalization(string selectedLanguage)
        {
            if (File.Exists(CurrentServiceInformation.FileName))
            {
                try
                {
                    XmlDocument configDoc = new XmlDocument();
                    configDoc.Load(CurrentServiceInformation.FileName);
                    XmlNode root = configDoc.DocumentElement;//.GetElementsByTagName("LanguageCulture");
                    XmlNode node = root.SelectSingleNode("descendant::LanguageCulture");
                    node.FirstChild.Value = selectedLanguage;
                   // node.Value = selectedLanguage;
                    configDoc.Save(CurrentServiceInformation.FileName);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
