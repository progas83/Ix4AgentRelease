using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [XmlInclude(typeof(XmlFolderSettingsModel)),XmlInclude(typeof(MsSqlSettings)), XmlInclude(typeof(MsSqlArticlesSettings))]
    [Serializable]
    public class BaseDataSourceSettings
    {
      
        public BaseDataSourceSettings()
        {

        }
    }
}
