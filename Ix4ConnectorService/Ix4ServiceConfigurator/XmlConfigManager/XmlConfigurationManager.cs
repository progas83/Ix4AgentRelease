using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using Ix4Models;
using Ix4Models.SettingsDataModel;

namespace Ix4ServiceConfigurator.XmlConfigManager
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
            if(!File.Exists(XmlFileData.FileName))
            {
                using (FileStream fs = new FileStream(XmlFileData.FileName, FileMode.CreateNew))
                {
                    _xmlSerializer.Serialize(fs, customerInfo);
                    fs.Flush(true);
                }
            }
            try
            {
                using (FileStream fs = new FileStream(XmlFileData.FileName, FileMode.OpenOrCreate))
                {
                    customerInfo = _xmlSerializer.Deserialize(fs);
                }
            }
            catch(Exception ex)
            {
                customerInfo = new CustomerInfo();
            }

            return customerInfo;
        }

        public void UpdateCustomerInformation(CustomerInfo customerInfo)
        {
            try
            {
                using (FileStream fs = new FileStream(XmlFileData.FileName, FileMode.Truncate))
                {
                    _xmlSerializer.Serialize(fs, customerInfo);
                    fs.Flush(true);
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
