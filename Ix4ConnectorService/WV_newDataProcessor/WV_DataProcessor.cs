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
        private ExportDataBuilder _exportDataBuilder;
        private static Logger _loger = Logger.GetLogger();
        IProxyIx4WebService _ix4WebServiceConnector;
        UpdateTimeWatcher _updateTimeWatcher;
        // private ImportDataSourcesBuilder _importDataProvider;
        public WV_DataProcessor()
        {

        }

        
        public void ExportData()
        {
            if(_exportDataBuilder!=null)
            {
                DataExporter invDbExporter = _exportDataBuilder.GetDataExporter("INVDB");
                invDbExporter.ExportOperationReportEvent += OnProcessReportResult;
                invDbExporter.SettingAllowToStart = _updateTimeWatcher.TimeToCheck("INVDB");
                invDbExporter.ExportData();
                invDbExporter.ExportOperationReportEvent -= OnProcessReportResult;

               
               
                
                DataExporter saDataExporter = _exportDataBuilder.GetDataExporter("SA");
                saDataExporter.ExportOperationReportEvent += OnProcessReportResult;
                saDataExporter.SettingAllowToStart = _updateTimeWatcher.TimeToCheck("SA"); ;
                saDataExporter.ExportData();
                saDataExporter.ExportOperationReportEvent -= OnProcessReportResult;

               
              //  caDataExporter.ReportEvent -= OnProcessReportResult;

                DataExporter gsDataExporter = _exportDataBuilder.GetDataExporter("GS");
                gsDataExporter.ExportOperationReportEvent += OnProcessReportResult;
               // gsDataExporter.NextExportOperation = new Action(caDataExporter.ExportData);
                gsDataExporter.SettingAllowToStart = _updateTimeWatcher.TimeToCheck("GS");

                DataExporter gpDataExporter = _exportDataBuilder.GetDataExporter("GP");
                gpDataExporter.ExportOperationReportEvent += OnProcessReportResult;
                gpDataExporter.NextExportOperation = new Action(gsDataExporter.ExportData);
                gpDataExporter.SettingAllowToStart = _updateTimeWatcher.TimeToCheck("GP");
                gpDataExporter.ExportData();


                DataExporter caDataExporter = _exportDataBuilder.GetDataExporter("CA");
                caDataExporter.ExportOperationReportEvent += OnProcessReportResult;
                caDataExporter.SettingAllowToStart = _updateTimeWatcher.TimeToCheck("CA");
                caDataExporter.ExportData();
                caDataExporter.ExportOperationReportEvent -= OnProcessReportResult;
                _updateTimeWatcher.SaveLastUpdateValues();

               
                gsDataExporter.ExportOperationReportEvent -= OnProcessReportResult;
                gpDataExporter.ExportOperationReportEvent -= OnProcessReportResult;

                DataExporter grDataExporter = _exportDataBuilder.GetDataExporter("GR");
                grDataExporter.ExportOperationReportEvent += OnProcessReportResult;
                grDataExporter.SettingAllowToStart = _updateTimeWatcher.TimeToCheck("GR");
                grDataExporter.ExportData();
                grDataExporter.ExportOperationReportEvent -= OnProcessReportResult;
            }
        }

        private void OnProcessReportResult(object sender, DataReportEventArgs e)
        {
            _loger.Log(e.Report.ToString());
            _updateTimeWatcher.SetLastUpdateTimeProperty(e.Report.DataTypeName);
            if(OperationReportEvent!=null)
            {
                OperationReportEvent(sender, e);
            }
        }

        public void ImportData()
        {
            if(_dataImporter != null)
            {
                if (_updateTimeWatcher.TimeToCheck(Ix4ImportDataTypes.Articles.ToString()))
                {

                    _dataImporter.ImportArticles();

                }

                if (_updateTimeWatcher.TimeToCheck(Ix4ImportDataTypes.Deliveries.ToString()))
                {
                   // _dataImporter.ImportDeliveries();

                }

                if (_updateTimeWatcher.TimeToCheck(Ix4ImportDataTypes.Orders.ToString())) 
                {
                    _dataImporter.ImportOrders();

                }
                _updateTimeWatcher.SaveLastUpdateValues();
            }
            
           
        }

        public void LoadSettings(CustomerInfo customerSettings)
        {
            _customerSettings = customerSettings;
           _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(_customerSettings.ClientID, _customerSettings.UserName, _customerSettings.Password, _customerSettings.ServiceEndpoint);
           _updateTimeWatcher = new UpdateTimeWatcher(_customerSettings.ImportDataSettings, _customerSettings.ExportDataSettings);
            _dataImporter = new DataImporter(_customerSettings.ImportDataSettings, _ix4WebServiceConnector);
            _dataImporter.ClientID = _customerSettings.ClientID;
            _dataImporter.ImportOperationReportEvent += OnProcessReportResult;
            _exportDataBuilder = new ExportDataBuilder(_customerSettings.ExportDataSettings, _ix4WebServiceConnector);
        }
    }
}
