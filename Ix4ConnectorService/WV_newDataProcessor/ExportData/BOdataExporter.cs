using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ix4Connector;

namespace WV_newDataProcessor.ExportData
{
    public class BOdataExporter : DataExporter
    {
        public BOdataExporter(IProxyIx4WebService ix4InterfaceService, string exportDataName) : base(ix4InterfaceService, exportDataName)
        {
        }

        protected override void ProcessExportedData(XDocument exportedData)
        {
            throw new NotImplementedException();
        }
    }
}
