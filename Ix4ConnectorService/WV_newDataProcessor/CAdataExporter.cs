using Ix4Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
   public class CAdataExporter : DataExporter
    {
        private IDataTargetCollaborator _storageCollaborator;
        public CAdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "CA")
        {
            _storageCollaborator = storageCollaborator;
        }

        protected override DataReport ProcessExportedData(XDocument exportedData)
        {
            throw new NotImplementedException();
        }
    }
}
