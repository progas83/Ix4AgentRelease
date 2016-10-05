using Ix4Connector;
using Ix4Models;
using Ix4Models.DataProviders.MsSqlDataProvider;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataProcessorHelper
{
    public abstract class BaseDataProcessor
    {
        private CustomerInfo _customerSettings;
        protected IProxyIx4WebService _ix4WebServiceConnector;
        protected UpdateTimeWatcher _updateTimeWatcher;
        protected MsSqlDataProvider _msSqlDataProvider;
        protected CustomerInfo CustomerSettings
        {
            get
            {
                if (_customerSettings == null)
                    throw (new Exception("Settings was not load"));
                return _customerSettings;
            }
        }
        protected ImportDataSourcesBuilder _importDataProvider;
        protected List<LICSRequestArticle> _cachedArticles;
        protected DataEnsure _ensureData;
        public static Logger _loger = Logger.GetLogger();
        private static object _o = new object();

        protected readonly int _articlesPerRequest = 20;

        public void LoadSettings(CustomerInfo customerSettings)
        {
            _customerSettings = customerSettings;
            _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(CustomerSettings.ClientID, CustomerSettings.UserName, CustomerSettings.Password, CustomerSettings.ServiceEndpoint);
            _updateTimeWatcher = new UpdateTimeWatcher(CustomerSettings.ImportDataSettings, CustomerSettings.ExportDataSettings);
            _msSqlDataProvider = new MsSqlDataProvider();// _CustomerSettings.ImportDataSettings, _CustomerSettings.ExportDataSettings);
             _importDataProvider = new ImportDataSourcesBuilder(CustomerSettings.ImportDataSettings);
            _ensureData = new DataEnsure(CustomerSettings.UserName);
        }

        protected abstract void CheckArticles();
        protected abstract void CheckDeliveries();
        protected abstract void CheckOrders();
        protected abstract void ProcessExportedData(ExportDataItemSettings settings);
        public void ImportData()
        {
            if (_customerSettings.ImportDataSettings.ArticleSettings.IsActivate && _updateTimeWatcher.TimeToCheck(Ix4RequestProps.Articles))
            {
                CheckArticles();
                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Articles);
            }

            if (_customerSettings.ImportDataSettings.DeliverySettings.IsActivate && _updateTimeWatcher.TimeToCheck(Ix4RequestProps.Deliveries))
            {
                CheckDeliveries();
                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Deliveries);
            }

            if (_customerSettings.ImportDataSettings.OrderSettings.IsActivate && _updateTimeWatcher.TimeToCheck(Ix4RequestProps.Orders))
            {
                CheckOrders();
                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Orders);
            }
            _updateTimeWatcher.SaveLastUpdateValues();
        }

        public void ExportData()
        {
            foreach(ExportDataItemSettings itemExported in CustomerSettings.ExportDataSettings.ExportDataItemSettings)
            {
                if(itemExported.IsActive && _updateTimeWatcher.TimeToCheck(itemExported.ExportDataTypeName))
                {
                    ProcessExportedData(itemExported);
                    _updateTimeWatcher.SetLastUpdateTimeProperty(itemExported.ExportDataTypeName);
                }
            }
        }
      
        protected LICSResponse SendLicsRequestToIx4(LICSRequest request, string fileName)
        {
            LICSResponse result = new LICSResponse();
            lock (_o)
            {
                try
                {
                    if (_ix4WebServiceConnector != null)
                    {
                        XmlSerializer serializator = new XmlSerializer(typeof(LICSRequest));
                        using (Stream st = new FileStream(CurrentServiceInformation.TemporaryXmlFileName, FileMode.OpenOrCreate))
                        {
                            serializator.Serialize(st, request);
                            byte[] bytesRequest = ReadToEnd(st);
                            //string response = _ix4WebServiceConnector.ImportXmlRequest(bytesRequest, fileName);
                            //XmlSerializer xS = new XmlSerializer(typeof(LICSResponse));
                            //using (var sr = new StringReader(response))
                            //{
                            //    result = (LICSResponse)xS.Deserialize(sr);
                            //}
                            //_loger.Log("Impoort data response : " + response);
                        }
                        {
                            string dataFileName = string.Empty;
                            int attemptLookForFile = 0;
                            do
                            {
                                attemptLookForFile++;
                                dataFileName = string.Format(CurrentServiceInformation.FloatTemporaryXmlFileName, attemptLookForFile);
                            }
                            while (File.Exists(dataFileName));
                            File.Copy(CurrentServiceInformation.TemporaryXmlFileName, dataFileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _loger.Log(ex);
                }
                finally
                {
                    File.Delete(CurrentServiceInformation.TemporaryXmlFileName);
                }
            }
            return result;
        }


        private byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
