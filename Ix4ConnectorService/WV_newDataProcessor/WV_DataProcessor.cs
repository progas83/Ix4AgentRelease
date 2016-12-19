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
using WV_newDataProcessor.ImportData;

namespace WV_newDataProcessor
{
    [ExportDataProcessor("wwinterface1000090")]
    public class WV_DataProcessor : IDataProcessor
    {
        public event EventHandler<DataReportEventArgs> OperationReportEvent;
        private CustomerInfo _customerSettings;
        private DataImporter _dataImporter;
       // private ImportDataSourcesBuilder _importDataProvider;
        public WV_DataProcessor()
        {

        }

        private static Logger _loger = Logger.GetLogger();
        public void ExportData()
        {
           
            ExportDataBuilder exportDataBuilder = new ExportDataBuilder(_customerSettings.ExportDataSettings, _ix4WebServiceConnector);

            DataExporter invDbExporter = exportDataBuilder.GetDataExporter("INVDB");
            invDbExporter.ReportEvent += OnProcessReportResult;
            invDbExporter.SettingAllowToStart = AllowToExportData(_customerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("INVDB")));
            invDbExporter.ExportData();
            invDbExporter.ReportEvent -= OnProcessReportResult;

            SAdataExporter saDataExporter = (SAdataExporter)exportDataBuilder.GetDataExporter("SA");
            saDataExporter.ReportEvent += OnProcessReportResult;
            saDataExporter.SettingAllowToStart = AllowToExportData(_customerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("SA")));
            saDataExporter.ExportData();
            saDataExporter.ReportEvent -= OnProcessReportResult;

            CAdataExporter caDataExporter = (CAdataExporter)exportDataBuilder.GetDataExporter("CA");
            caDataExporter.ReportEvent += OnProcessReportResult;
            caDataExporter.SettingAllowToStart = AllowToExportData(_customerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("CA")));

            GSdataExporter gsDataExporter = (GSdataExporter)exportDataBuilder.GetDataExporter("GS");
            gsDataExporter.ReportEvent += OnProcessReportResult;
            gsDataExporter.NextExportOperation = new Action(caDataExporter.ExportData);
            gsDataExporter.SettingAllowToStart = AllowToExportData(_customerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("GS")));

            GPdataExporter gpDataExporter = (GPdataExporter)exportDataBuilder.GetDataExporter("GP");
            gpDataExporter.ReportEvent += OnProcessReportResult;
            gpDataExporter.NextExportOperation = new Action(gsDataExporter.ExportData);
            gpDataExporter.SettingAllowToStart = AllowToExportData(_customerSettings.ExportDataSettings.ExportDataItemSettings.FirstOrDefault(s => s.ExportDataTypeName.Equals("GP")));
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


        private bool AllowToImportData(Ix4RequestProps importDataType)
        {
            bool result = false;
            if(_updateTimeWatcher.TimeToCheck(importDataType))
            {
                switch (importDataType)
                {
                    case Ix4RequestProps.Articles:
                        result = _customerSettings.ImportDataSettings.ArticleSettings.IsActivate &&
                                    _customerSettings.ImportDataSettings.ArticleSettings.IsNowWorkingTime;
                        break;
                    case Ix4RequestProps.Orders:
                        result = _customerSettings.ImportDataSettings.DeliverySettings.IsActivate &&
                                 _customerSettings.ImportDataSettings.DeliverySettings.IsNowWorkingTime;
                        break;
                    case Ix4RequestProps.Deliveries:
                        result = _customerSettings.ImportDataSettings.OrderSettings.IsActivate &&
                                    _customerSettings.ImportDataSettings.OrderSettings.IsNowWorkingTime;
                        break;
                    default:
                        break;
                }
            }
            return result;
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
            if(_dataImporter != null)
            {
                if (AllowToImportData(Ix4RequestProps.Articles))
                {

                    _dataImporter.ImportArticles();

                }

                if (AllowToImportData(Ix4RequestProps.Deliveries))
                {
                    _dataImporter.ImportDeliveries();

                }

                if (AllowToImportData(Ix4RequestProps.Orders))
                {
                    _dataImporter.ImportOrders();

                }
                _updateTimeWatcher.SaveLastUpdateValues();
            }
            
           
        }
        IProxyIx4WebService _ix4WebServiceConnector;
        UpdateTimeWatcher _updateTimeWatcher;
        public void LoadSettings(CustomerInfo customerSettings)
        {
            _customerSettings = customerSettings;
           _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(_customerSettings.ClientID, _customerSettings.UserName, _customerSettings.Password, _customerSettings.ServiceEndpoint);
            _updateTimeWatcher = new UpdateTimeWatcher(_customerSettings.ImportDataSettings, _customerSettings.ExportDataSettings);
            _dataImporter = new DataImporter(_customerSettings.ImportDataSettings, _ix4WebServiceConnector, _updateTimeWatcher);
        }
    }
}
