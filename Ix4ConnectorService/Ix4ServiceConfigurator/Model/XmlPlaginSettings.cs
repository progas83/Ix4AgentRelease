using Ix4Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4ServiceConfigurator.Model
{
    [Serializable]
    public class XmlPlaginSettings
    {
        private CustomDataSourceTypes _dataSourceType;

        [XmlIgnore]
        public CustomDataSourceTypes MyProperty
        {
            get { return _dataSourceType; }
        }



    }
}
