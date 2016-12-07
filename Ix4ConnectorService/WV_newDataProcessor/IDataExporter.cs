using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV_newDataProcessor
{
    public interface IDataExporter
    {
        ExportedDataReport ExportData();
    }
}
