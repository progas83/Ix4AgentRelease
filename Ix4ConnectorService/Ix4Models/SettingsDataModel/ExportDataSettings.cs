using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class ExportDataSettings
    {
        public ExportDataSettings()
        {
            ExportDataItemSettings = new ExportDataItemSettings[0];
        }

       public ExportDataItemSettings[] ExportDataItemSettings { get; set; }
    }
}
