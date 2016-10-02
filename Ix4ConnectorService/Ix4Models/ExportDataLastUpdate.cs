using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models
{
    public class ExportDataLastUpdate : Dictionary<string,long>
    {
        private ExportDataSettings _exportDataSettings;
        public ExportDataLastUpdate(ExportDataSettings exportDataSettings)
        {
            _exportDataSettings = exportDataSettings;
            foreach(var item in exportDataSettings.ExportDataItemSettings)
            {
                this.Add(item.ExportDataTypeName, 0);
            }
        }
    }
}
