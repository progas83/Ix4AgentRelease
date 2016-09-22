using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4ServiceConfigurator.Model
{
    [Serializable]
    public class PluginsSettings
    {
        public XmlPlaginSettings XmlSettings { get; set; }

        public CsvPluginSettings CsvSettings { get; set; }

        public MsSqlPlaginSettings MsSqlSettings { get; set; }
    }
}
