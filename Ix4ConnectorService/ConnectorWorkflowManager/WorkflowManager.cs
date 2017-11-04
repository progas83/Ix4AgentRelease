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
using Ix4Models.Reports;
using ConnectorWorkflowManager.WebStatisticsClient;
using ConnectorWorkflowManager.Mailer;
using System.Text;
using System.Security.Cryptography;

namespace ConnectorWorkflowManager
{
    public class WorkflowManager
    {
        private static WorkflowManager _manager;
        private CustomerInfo _customerSettings;
        protected Timer _timer;
        private static object _padlock = new object();
        private static readonly long RElapsedEvery = 1 * 1 * 1000;


        private static Logger _loger = Logger.GetLogger();
       // private Ix4StatisticClient _ix4StatisticClient = new Ix4StatisticClient("http://93.77.219.73/");
        private MailLogger _mailLoggerAgent;
        private WorkflowManager()
        {
            AssembleCustomerDataComponents();
            _customerSettings = XmlConfigurationManager.Instance.GetCustomerInformation();
            _currentDataProcessor = GetDataProcessor(_customerSettings.UserName + _customerSettings.ClientID);
            _currentDataProcessor.LoadSettings(_customerSettings);
            _currentDataProcessor.OperationReportEvent += OnOperationReportEvent;
            Logger.LoggedExceptionEvent += OnLoggedExceptionEvent;
           
            _mailLoggerAgent = new MailLogger(_customerSettings.UserName, _customerSettings.MailSettings);
        }

        private void OnLoggedExceptionEvent(object sender, Exception e)
        {
            _mailLoggerAgent.LogMail(new ContentDescription("Undescribed exception", e.Message));
        }

        private void OnOperationReportEvent(object sender, DataReportEventArgs e)
        {
            try
            {
                e.Report.LVSClientID = _customerSettings.ClientID;
                e.Report.OperationDate = DateTime.Now;
                _loger.Log(string.Format(" {0} messages has been completed", e.Report.DataTypeName));
                //_ix4StatisticClient.PostReport(e.Report);
                _loger.Log(string.Format("Report has been sent"));
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
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

        private IDataProcessor _currentDataProcessor { get; set; }
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
                EnableTimerPrecisely();
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }
        private bool _isBusy = false;

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!_isBusy && _currentDataProcessor != null)
            {
               if (DateTime.Now.Minute == 30 || DateTime.Now.Minute == 0)
                {
                    _timer.Enabled = false;
                    _isBusy = true;
                    try
                    {

                        _loger.Log("Start Import data");
                        _currentDataProcessor.ImportData();
                        _loger.Log("Finish Import data");
                        _loger.Log("Start Export data");
                        _currentDataProcessor.ExportData();
                        _loger.Log("Finish Export data");

                    }
                    catch (Exception ex)
                    {
                        _loger.Log(ex);
                    }
                    finally
                    {
                        _isBusy = false;
                        _mailLoggerAgent.SendMailReport();
                        EnableTimerPrecisely();
                    }
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
                EnableTimerPrecisely();
                _loger.Log("Service resumed");
            }
        }

        private void EnableTimerPrecisely()
        {
            if (DateTime.Now.Second != 0)
            {
                System.Threading.Thread.Sleep((60 - DateTime.Now.Second) * 1000);
            }
            _timer.Enabled = true;
            _loger.Log("Timer enabled at " + DateTime.Now.ToLongTimeString());
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


        private class Cryptor
        {
            private static readonly string Schlüssel = "je2dGckI8dnMXjdusemdpv8n4D8fm2Jd";
            private static readonly string Brunnen = "pvmJd9L4jdprfHnÜ";
            public string Encrypt(string decrypted)
            {
                if (string.IsNullOrEmpty(decrypted))
                    return string.Empty;
                byte[] textBytes = ASCIIEncoding.ASCII.GetBytes(decrypted);
                AesCryptoServiceProvider encDec = GetCryptProvider();

                ICryptoTransform icrypt = encDec.CreateEncryptor(encDec.Key, encDec.IV);

                byte[] enc = icrypt.TransformFinalBlock(textBytes, 0, textBytes.Length);
                icrypt.Dispose();
                encDec.Dispose();
                return Convert.ToBase64String(enc);

            }
            //   private AesCryptoServiceProvider _cyptProvider;
            private AesCryptoServiceProvider GetCryptProvider()
            {
                return new AesCryptoServiceProvider()
                {
                    BlockSize = 128,
                    KeySize = 256,
                    Key = ASCIIEncoding.ASCII.GetBytes(Schlüssel),
                    IV = ASCIIEncoding.ASCII.GetBytes(Brunnen),
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC
                };
            }

            public string Decrypt(string encrypted)
            {
                byte[] encryptBytes = Convert.FromBase64String(encrypted);
                AesCryptoServiceProvider encDec = GetCryptProvider();
                ICryptoTransform icrypt = encDec.CreateDecryptor(encDec.Key, encDec.IV);
                byte[] decryptBytes = icrypt.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length);
                icrypt.Dispose();
                encDec.Dispose();

                return ASCIIEncoding.ASCII.GetString(decryptBytes);

            }
        }
    }
}
