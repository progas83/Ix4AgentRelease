using CompositionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorWorkflowManager
{
    class ExportedDataClient
    {
        CustomerDataComposition _dataCompositor;
        public ExportedDataClient(CustomerDataComposition dataCompositor)
        {
            _dataCompositor = dataCompositor;
        }
    }
}
