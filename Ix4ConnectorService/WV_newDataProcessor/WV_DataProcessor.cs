using Ix4Models.Attributes;
using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using Ix4Connector;
using SimplestLogger;
using Ix4Models;
using Ix4Models.Reports;

namespace WV_newDataProcessor
{
   // [ExportDataProcessor("ilyatest1111")]
    public class WV_DataProcessor //: IDataProcessor
    {
        public event EventHandler<DataReportEventArgs> OperationReportEvent;
        CustomerInfo CustomerSettings;
        public WV_DataProcessor()
        {

        }

        private static Logger _loger = Logger.GetLogger();
        public void ExportData()
        {
           
            ExportDataBuilder exportDataBuilder = new ExportDataBuilder(CustomerSettings.ExportDataSettings, _ix4WebServiceConnector);

            DataExporter invDbExporter = exportDataBuilder.GetDataExporter("INVDB");
            invDbExporter.ReportEvent += OnProcessReportResult;
            invDbExporter.SettingAllowToStart = AllowToExportData(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("INVDB")));
            invDbExporter.ExportData();
            invDbExporter.ReportEvent -= OnProcessReportResult;

            SAdataExporter saDataExporter = (SAdataExporter)exportDataBuilder.GetDataExporter("SA");
            saDataExporter.ReportEvent += OnProcessReportResult;
            saDataExporter.SettingAllowToStart = AllowToExportData(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("SA")));
            saDataExporter.ExportData();
            saDataExporter.ReportEvent -= OnProcessReportResult;

            CAdataExporter caDataExporter = (CAdataExporter)exportDataBuilder.GetDataExporter("CA");
            caDataExporter.ReportEvent += OnProcessReportResult;
            caDataExporter.SettingAllowToStart = AllowToExportData(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("CA")));

            GSdataExporter gsDataExporter = (GSdataExporter)exportDataBuilder.GetDataExporter("GS");
            gsDataExporter.ReportEvent += OnProcessReportResult;
            gsDataExporter.NextExportOperation = new Action(caDataExporter.ExportData);
            gsDataExporter.SettingAllowToStart = AllowToExportData(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("GS")));

            GPdataExporter gpDataExporter = (GPdataExporter)exportDataBuilder.GetDataExporter("GP");
            gpDataExporter.ReportEvent += OnProcessReportResult;
            gpDataExporter.NextExportOperation = new Action(gsDataExporter.ExportData);
            gpDataExporter.SettingAllowToStart = AllowToExportData(CustomerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("GP")));
            gpDataExporter.ExportData();
            _updateTimeWatcher.SaveLastUpdateValues();

            caDataExporter.ReportEvent -= OnProcessReportResult;
            gsDataExporter.ReportEvent -= OnProcessReportResult;
            gpDataExporter.ReportEvent -= OnProcessReportResult;

        }

        private bool AllowToExportData(ExportDataItemSettings exportDataItemSettings)
        {
            bool timeToCkeck = _updateTimeWatcher.TimeToCheck(exportDataItemSettings.ExportDataTypeName);
            return exportDataItemSettings != null
                && exportDataItemSettings.IsActive
                && exportDataItemSettings.IsNowWorkingTime && timeToCkeck;
        }

        private void OnProcessReportResult(object sender, DataReportEventArgs e)
        {
            _loger.Log(e.Report.ToString());
            _updateTimeWatcher.SetLastUpdateTimeProperty(e.Report.ExportTypeName);
            if(OperationReportEvent!=null)
            {
                OperationReportEvent(sender, e);
            }
        }

        public void ImportData()
        {
            throw new NotImplementedException();
        }
        IProxyIx4WebService _ix4WebServiceConnector;
        UpdateTimeWatcher _updateTimeWatcher;
        public void LoadSettings(CustomerInfo customerSettings)
        {
            CustomerSettings = customerSettings;
           _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(CustomerSettings.ClientID, CustomerSettings.UserName, CustomerSettings.Password, CustomerSettings.ServiceEndpoint);
            _updateTimeWatcher = new UpdateTimeWatcher(CustomerSettings.ImportDataSettings, CustomerSettings.ExportDataSettings);
        }
    }
}
