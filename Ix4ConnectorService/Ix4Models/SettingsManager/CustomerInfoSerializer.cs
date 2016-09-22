using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsManager
{
   public class CustomerInfoSerializer : XmlSerializer
    {
       
        public CustomerInfoSerializer() : base(typeof(CustomerInfo))
        {
          
            
        }

        public CustomerInfo Deserialize(FileStream fs)
        {
            CustomerInfo customerInfo = (CustomerInfo)base.Deserialize(fs);
            customerInfo.Decrypt();
            return customerInfo;
        }

        public void Serialize(FileStream fs, CustomerInfo customerInfo)
        {
            customerInfo.Encrypt();
            base.Serialize(fs, customerInfo);
        }
      
    }
}
