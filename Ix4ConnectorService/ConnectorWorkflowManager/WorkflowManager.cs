using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4Models.SettingsManager;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.ComponentModel.Composition;
using Ix4Models.Interfaces;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;

namespace ConnectorWorkflowManager
{
    public class WorkflowManager
    {
        private static WorkflowManager _manager;
        private CustomerInfo _customerSettings;
       // private CustomerDataComposition _dataCompositor;
    //    private IProxyIx4WebService _ix4WebServiceConnector;
        //private DataEnsure _ensureData;
        protected Timer _timer;
        private static object _padlock = new object();
        private static readonly long RElapsedEvery = 5 * 1 * 1000;
        private static readonly int _articlesPerRequest = 20;


        //bool _isArticlesBusy = false;

        private static Logger _loger = Logger.GetLogger();

        private WorkflowManager()
        {
            AssembleCustomerDataComponents();
            _customerSettings = XmlConfigurationManager.Instance.GetCustomerInformation();
            _CurrentDataProcessor = GetDataProcessor(_customerSettings.UserName + _customerSettings.ClientID);
            _CurrentDataProcessor.LoadSettings(_customerSettings);
        }

        public static WorkflowManager Instance
        {
            get
            {
                if (_manager == null)
                {
                    lock (_padlock)
                    {
                        if (_manager == null)
                        {
                            _manager = new WorkflowManager();
                        }
                    }
                }

                return _manager;
            }
        }
        [ImportMany]
        private IEnumerable<Lazy<IDataProcessor, INameMetadata>> DataProcessors { get; set; }


        private void AssembleCustomerDataComponents()
        {
            try
            {
                var directoryPath = string.Format("{0}\\{1}", string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), CurrentServiceInformation.ClientsSubdirectory);
                var directoryCatalog = new DirectoryCatalog(directoryPath, "*.dll");

                var aggregateCatalog = new AggregateCatalog();
                aggregateCatalog.Catalogs.Add(directoryCatalog);

                var container = new CompositionContainer(aggregateCatalog);

                container.ComposeParts(this);


            }
            catch (Exception ex)
            {
               _loger.Log(ex);
            }
        }

        private IDataProcessor _CurrentDataProcessor { get; set; }
        private IDataProcessor GetDataProcessor(string name)
        {
            return DataProcessors.Where(l => l.Metadata.Name.Equals(name)).Select(l => l.Value).FirstOrDefault();
        }
        public void Start()
        {

            try
            {
                if (_timer == null)
                {
                    _timer = new System.Timers.Timer(RElapsedEvery);
                    _timer.AutoReset = true;

                    _timer.Elapsed += OnTimedEvent;
                }

                _loger.Log("Service has been started at");

               // _customerInfo = XmlConfigurationManager.Instance.GetCustomerInformation();
               //// _dataCompositor = new CustomerDataComposition(_customerInfo.PluginSettings);
               // _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(_customerInfo.ClientID, _customerInfo.UserName, _customerInfo.Password, _customerInfo.ServiceEndpoint);
               // _ensureData = new DataEnsure(_customerInfo.UserName);
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
              //  _loger.Log(_customerInfo, "_customerInfo");
              //  _loger.Log(_ix4WebServiceConnector, "_ix4WebServiceConnector");
            }
        }
        private bool _isBusy = false;
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if(!_isBusy && _CurrentDataProcessor != null)
            {
                _timer.Enabled = false;
                _isBusy = true;
                try
                {
                    _CurrentDataProcessor.ImportData();
                    _CurrentDataProcessor.ExportData();
                }
                catch(Exception ex)
                {
                    _loger.Log(ex);
                }
                finally
                {
                    _isBusy = false;
                    _timer.Enabled = true;
                }
            }
        }

        public void Pause()
        {
            if (_timer != null && _timer.Enabled)
            {
                _timer.Enabled = false;
               _loger.Log("Service paused");
            }
        }

        public void Continue()
        {
            if (_timer != null && !_timer.Enabled)
            {
                _timer.Enabled = true;
                _loger.Log("Service resumed");
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= OnTimedEvent;
                _timer.Stop();
                _timer.Enabled = false;
                _timer.Dispose();
                _timer = null;
            }

            _loger.Log("Service has stopped");
        }


    }
}
