using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Connector;

namespace WV_newDataProcessor
{
   public class ExportDataBuilder
    {
        private ExportDataSettings _exportDataSettings;
        private IProxyIx4WebService _ix4WebServiceConnector;

        public ExportDataBuilder(ExportDataSettings exportDataSettings)
        {
            _exportDataSettings = exportDataSettings;
        }

        public ExportDataBuilder(ExportDataSettings exportDataSettings, IProxyIx4WebService _ix4WebServiceConnector) : this(exportDataSettings)
        {
            this._ix4WebServiceConnector = _ix4WebServiceConnector;
        }
        private static string _dbConnection = @"Data Source =DESKTOP-PC\SQLEXPRESS2012;Initial Catalog = Inventurdaten; Integrated Security=SSPI";
        public DataExporter GetDataExporter(string exportDataName)
        {
            DataExporter dataExporter = null;
            switch (exportDataName)
                {
                case "INVDB":
                    dataExporter = new INVDBdataExported(_ix4WebServiceConnector, new SqlTableCollaborator(_dbConnection, new InventurenDataMapper[] { new InventurenDataMapper() }));
                        break;
                default:
                    throw new NotImplementedException("Wasn't implement dataExporter for " + exportDataName);

            }
            return dataExporter;// new INVDBdataExported(_ix4WebServiceConnector);
        }
    }
}
