using Ix4Models.Attributes;
using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using Ix4Connector;

namespace WV_newDataProcessor
{
    [ExportDataProcessor("ilyatest1111")]
    public class WV_DataProcessor : IDataProcessor
    {
        CustomerInfo CustomerSettings;
        public WV_DataProcessor()
        {

        }
        public void ExportData()
        {
           
            ExportDataBuilder exportDataBuilder = new ExportDataBuilder(CustomerSettings.ExportDataSettings, _ix4WebServiceConnector);

            //INVDBdataExporter invDbExporter =(INVDBdataExporter) exportDataBuilder.GetDataExporter("INVDB");
            //invDbExporter.ReportEvent += OnProcessReportResult;
            //invDbExporter.ExportData();
            //invDbExporter.ExportData();

            //SAdataExporter saDataExporter = (SAdataExporter)exportDataBuilder.GetDataExporter("SA");
            //saDataExporter.ReportEvent += OnProcessReportResult;
            //saDataExporter.ExportData();

            CAdataExporter caDataExporter = (CAdataExporter)exportDataBuilder.GetDataExporter("CA");
            caDataExporter.ReportEvent += OnProcessReportResult;
            caDataExporter.SettingAllowToStart = AllowToStart(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("CA")));

            GSdataExporter gsDataExporter = (GSdataExporter)exportDataBuilder.GetDataExporter("GS");
            gsDataExporter.ReportEvent += OnProcessReportResult;
            gsDataExporter.NextExportOperation = new OnCompleteNextOperation(caDataExporter.ExportData);
            gsDataExporter.SettingAllowToStart = AllowToStart(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("GS")));

            GPdataExporter gpDataExporter = (GPdataExporter)exportDataBuilder.GetDataExporter("GP");
            gpDataExporter.ReportEvent += OnProcessReportResult;
            gpDataExporter.NextExportOperation =new OnCompleteNextOperation(gsDataExporter.ExportData);
            gpDataExporter.SettingAllowToStart = AllowToStart(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("GP")));
            gpDataExporter.ExportData();

        }

        private bool AllowToStart(ExportDataItemSettings exportDataItemSettings)
        {
            return true;
        }

        private void OnProcessReportResult(object sender, DataReportEventArgs e)
        {
            
        }

        public void ImportData()
        {
            throw new NotImplementedException();
        }
        IProxyIx4WebService _ix4WebServiceConnector;
        public void LoadSettings(CustomerInfo customerSettings)
        {
            CustomerSettings = customerSettings;
           _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(CustomerSettings.ClientID, CustomerSettings.UserName, CustomerSettings.Password, CustomerSettings.ServiceEndpoint);
        }
    }
}
