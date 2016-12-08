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

        private DataTableMapper LoadInventurenDataMapper()
        {
            List<TableFieldMapInfo> listOfFields = new List<TableFieldMapInfo>();
            listOfFields.Add(new TableFieldMapInfo("Inventurnummer","System.Int32",   "MSGHeader_Inventurnummer"));
            listOfFields.Add(new TableFieldMapInfo("Mandant",       "System.String",  "MSGHeader_Mandant"));
            listOfFields.Add(new TableFieldMapInfo("Bezeichnung",   "System.String",  "MSGHeader_Bezeichnung"));
            listOfFields.Add(new TableFieldMapInfo("Inventurstart", "System.DateTime","MSGHeader_Inventurstart"));
            listOfFields.Add(new TableFieldMapInfo("Inventurende",  "System.DateTime","MSGHeader_Inventurende"));
            listOfFields.Add(new TableFieldMapInfo("GenDate",       "System.DateTime","MSGHeader_GenDate"));
            listOfFields.Add(new TableFieldMapInfo("GenUser",       "System.String",  "MSGHeader_GenUser"  ));
            listOfFields.Add(new TableFieldMapInfo("ModDate",       "System.DateTime","MSGHeader_ModDate" ));
            listOfFields.Add(new TableFieldMapInfo("ModUser",       "System.String",  "MSGHeader_ModUser" ));
            listOfFields.Add(new TableFieldMapInfo("Type",          "System.String",  "MSGHeader_Type"   ));
            listOfFields.Add(new TableFieldMapInfo("Status",        "System.String",  "MSGHeader_Status"  ));
            //listOfFields.Add(new TableFieldMapInfo("", "", ""));
            return new DataTableMapper("Inventuren", listOfFields);
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
                    dataExporter = new INVDBdataExported(_ix4WebServiceConnector, new SqlTableCollaborator(_dbConnection, new DataTableMapper[] { LoadInventurenDataMapper() }));
                        break;
                default:
                    throw new NotImplementedException("Wasn't implement dataExporter for " + exportDataName);

            }
            return dataExporter;// new INVDBdataExported(_ix4WebServiceConnector);
        }
    }
}
