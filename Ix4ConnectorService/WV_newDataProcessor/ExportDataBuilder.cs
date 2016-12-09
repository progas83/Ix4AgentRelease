using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Connector;
using SimplestLogger;

namespace WV_newDataProcessor
{
   public class ExportDataBuilder
    {
        private static Logger _loger = Logger.GetLogger();
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

        private DataTableMapper LoadInventurpositionenDataMapper()
        {
            List<TableFieldMapInfo> listOfFields = new List<TableFieldMapInfo>();
            listOfFields.Add(new TableFieldMapInfo("Position",          "System.Int32",     "MSGPos_Position"));
            listOfFields.Add(new TableFieldMapInfo("Inventurnummer",    "System.Int32",     "MSGPos_Inventurnummer"));
            listOfFields.Add(new TableFieldMapInfo("Mandant",           "System.String",    "MSGPos_Mandant"));
            listOfFields.Add(new TableFieldMapInfo("ArtikelNr",         "System.String",    "MSGPos_ArtikelNr"));
            listOfFields.Add(new TableFieldMapInfo("Merkmal1",          "System.Int32",     "MSGPos_Merkmal1"));
            listOfFields.Add(new TableFieldMapInfo("Merkmal2",          "System.Int32",     "MSGPos_Merkmal2"));
            listOfFields.Add(new TableFieldMapInfo("Merkmal3",          "System.Int32",     "MSGPos_Merkmal3"));
            listOfFields.Add(new TableFieldMapInfo("Lager",             "System.String",    "MSGPos_Lager"));
            listOfFields.Add(new TableFieldMapInfo("Lagerort",          "System.String",    "MSGPos_Lagerort"));
            listOfFields.Add(new TableFieldMapInfo("Lagerplatz",        "System.String",    "MSGPos_Lagerplatz"));
            listOfFields.Add(new TableFieldMapInfo("MengeSoll",         "System.Double",    "MSGPos_MengeSoll"));
            listOfFields.Add(new TableFieldMapInfo("MengeIst",          "System.Double",    "MSGPos_MengeIst"));
            listOfFields.Add(new TableFieldMapInfo("Serialnummer",      "System.String",    "MSGPos_Serialnummer"));
            listOfFields.Add(new TableFieldMapInfo("Chargennummer",     "System.String",    "MSGPos_Chargennummer"));
            listOfFields.Add(new TableFieldMapInfo("Teilezustand",      "System.String",    "MSGPos_Teilezustand"));
            listOfFields.Add(new TableFieldMapInfo("LHMRef",            "System.Int32",     "MSGPos_LHMRef"));
            listOfFields.Add(new TableFieldMapInfo("Einlagerdatum",     "System.DateTime",  "MSGPos_Einlagerdatum"));
            listOfFields.Add(new TableFieldMapInfo("Differenz",         "System.Double",    "MSGPos_Differenz"));
            listOfFields.Add(new TableFieldMapInfo("Zustand",           "System.String",    "MSGPos_Zustand"));
            listOfFields.Add(new TableFieldMapInfo("LfdNrLagerbelegung","System.Int32",     "MSGPos_LfdNrLagerbelegung"));
            listOfFields.Add(new TableFieldMapInfo("Status",            "System.String",    "MSGPos_Status"));
            listOfFields.Add(new TableFieldMapInfo("GenDate",           "System.DateTime",  "MSGPos_GenDate"));
            listOfFields.Add(new TableFieldMapInfo("GenUser",           "System.String",    "MSGPos_GenUser"));
            listOfFields.Add(new TableFieldMapInfo("ModDate",           "System.DateTime",  "MSGPos_ModDate"));
            listOfFields.Add(new TableFieldMapInfo("ModUser",           "System.String",    "MSGPos_ModUser"));
            listOfFields.Add(new TableFieldMapInfo("Zaehlliste",        "System.Int32",     "MSGPos_Zaehlliste"));
            listOfFields.Add(new TableFieldMapInfo("Einheit",           "System.String",    "MSGPos_Einheit"));
            listOfFields.Add(new TableFieldMapInfo("BearbeitungsNr",    "System.Int32",     "MSGPos_BearbeitungsNr"));
            listOfFields.Add(new TableFieldMapInfo("Jahresverbrauch",   "System.Double",    "MSGPos_Differenz",true));
            listOfFields.Add(new TableFieldMapInfo("geprueft",          "System.String",    "MSGPos_geprueft",true));
           
            return new DataTableMapper("Inventurpositionen", listOfFields);
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
                    dataExporter = new INVDBdataExported(_ix4WebServiceConnector, new SqlTableCollaborator(_dbConnection, new DataTableMapper[] { LoadInventurenDataMapper(), LoadInventurpositionenDataMapper() }));
                        break;
                default:
                    throw new NotImplementedException("Wasn't implement dataExporter for " + exportDataName);

            }
            return dataExporter;
        }
    }
}
