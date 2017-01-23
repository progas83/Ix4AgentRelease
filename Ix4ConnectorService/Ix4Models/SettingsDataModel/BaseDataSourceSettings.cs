using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [XmlInclude(typeof(XmlFolderSettingsModel)),
        XmlInclude(typeof(MsSqlSettings)), 
        XmlInclude(typeof(MsSqlArticlesSettings)), 
        XmlInclude(typeof(MsSqlOrdersSettings)),
        XmlInclude(typeof(MsSqlDeliveriesSettings))]
    [Serializable]
    public abstract class BaseDataSourceSettings: ICryptor
    {
      
        public BaseDataSourceSettings()
        {

        }

        public abstract void Decrypt();

        public abstract void Encrypt();
    }
}
