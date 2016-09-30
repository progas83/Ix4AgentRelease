using CompositionHelper;
using Ix4Connector;
using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4Models.SettingsManager;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Xml;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Timers;

namespace ConnectorWorkflowManager
{
    public class WorkflowManager
    {
        private static WorkflowManager _manager;
        private CustomerInfo _customerInfo;
        private CustomerDataComposition _dataCompositor;
        private IProxyIx4WebService _ix4ServiceConnector;
        private DataEnsure _ensureData;
        protected Timer _timer;
        private static object _padlock = new object();
        private static readonly long RElapsedEvery = 20 * 1 * 1000;
        private static readonly int _articlesPerRequest = 20;


        bool _isArticlesBusy = false;

        private static Logger _loger = Logger.GetLogger();

        private WorkflowManager()
        {

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

                _customerInfo = XmlConfigurationManager.Instance.GetCustomerInformation();
               // _dataCompositor = new CustomerDataComposition(_customerInfo.PluginSettings);
                _ix4ServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(_customerInfo.ClientID, _customerInfo.UserName, _customerInfo.Password, _customerInfo.ServiceEndpoint);
                _ensureData = new DataEnsure(_customerInfo.UserName);
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
                _loger.Log(_customerInfo, "_customerInfo");
                _loger.Log(_ix4ServiceConnector, "_ix4ServiceConnector");
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Minute == 30 || DateTime.Now.Minute == 0)
            {
                _timer.Enabled = false;
                try
                {
                    WrightLog("Start checking");
                   // if (_customerInfo.PluginSettings.MsSqlSettings.CheckDeliveries)
                    {
                        ExportData();
                    }

                   // if (_customerInfo.PluginSettings.MsSqlSettings.CheckArticles)
                    {

                        if (!_isArticlesBusy)
                            Task.Run(() => CheckArticles());
                    }

                  //  if (_customerInfo.PluginSettings.MsSqlSettings.CheckOrders)
                    {

                        CheckPreparedRequest(CustomDataSourceTypes.MsSql, Ix4RequestProps.Orders);
                        WrightLog("Check Orders finished");
                    }
                    WrightLog("Finish checking");
                }
                catch (Exception ex)
                {
                    _loger.Log(ex.Message);
                }
                finally
                {
                    _timer.Enabled = true;
                }

                SinplestLogger.Mailer.MailLogger.Instance.SendMailReport();
            }
        }

        private void ExportData()
        {
            if (_ix4ServiceConnector != null && _ensureData != null && _dataCompositor != null)
            {
                // if (UpdateTimeWatcher.TimeToCheck("GP"))
                {
                    try
                    {
                        foreach (string mark in new string[] {"SA"})// { "GP", "GS" })
                        {
                            //if (!UpdateTimeWatcher.TimeToCheck(mark))
                            //{
                            //    continue;
                            //}
                            //else
                            //{

                            //}
                            _loger.Log("Starting export data " + mark);
                            XmlNode nodeResult = _ix4ServiceConnector.ExportData(mark, null);

                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.InnerXml = nodeResult.OuterXml;
                            var msgNodes = xmlDoc.GetElementsByTagName("MSG");

                            //  var msgNodes = nodeResult.LastChild.LastChild.SelectNodes("MSG");
                            _loger.Log(string.Format("Got Exported {0} items count = {1}", mark, msgNodes.Count));
                            if (msgNodes != null && msgNodes.Count > 0)
                            {
                                EnsureType ensureType = EnsureType.CollectData;
                                switch (mark)
                                {
                                    case "SA":
                                        ensureType = EnsureType.UpdateStoredData;
                                        break;
                                    case "GP":
                                        ensureType = EnsureType.CollectData;
                                        break;
                                    case "GS":
                                        ensureType = EnsureType.CollectData;
                                        break;
                                    default:
                                        ensureType = EnsureType.CollectData;
                                        break;
                                }

                                if (!_ensureData.StoreExportedNodeList(msgNodes, mark, ensureType))
                                {
                                    _ensureData.RudeStoreExportedData(nodeResult, mark);
                                }
                                else
                                {
                                    _ensureData.ProcessingStoredDataToClientStorage(mark, _dataCompositor.GetCustomerDataConnector(CustomDataSourceTypes.MsSql));
                                }
                                _loger.Log("End export data " + mark);
                                System.Threading.Thread.Sleep(30000);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        _loger.Log("Exception while export data");
                        _loger.Log(ex);
                    }
                    //    UpdateTimeWatcher.SetLastUpdateTimeProperty(mark);


                }
            }
        }

        public void Pause()
        {
            if (_timer != null && _timer.Enabled)
            {
                _timer.Enabled = false;
                WrightLog("Service paused");
            }
        }

        public void Continue()
        {
            if (_timer != null && !_timer.Enabled)
            {
                _timer.Enabled = true;
                WrightLog("Service resumed");
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

            WrightLog("Service has stopped");
        }


        private static object _o = new object();
        //  private static int _errorCount = 0;
        private bool SendLicsRequestToIx4(LICSRequest request, string fileName)
        {
            bool result = false;
            lock (_o)
            {
                try
                {
                    if (_ix4ServiceConnector != null)
                    {
                        bool requestSuccess = true;
                        XmlSerializer serializator = new XmlSerializer(typeof(LICSRequest));
                        using (Stream st = new FileStream(CurrentServiceInformation.TemporaryXmlFileName, FileMode.OpenOrCreate))
                        {
                            _loger.Log("Check customerID = ClientId" + request.ClientId);
                            serializator.Serialize(st, request);
                            byte[] bytesRequest = ReadToEnd(st);
                            string resp = _ix4ServiceConnector.ImportXmlRequest(bytesRequest, fileName);
                            requestSuccess = CheckStateRequest(resp);
                            SimplestParcerLicsRequest(resp);
                            _loger.Log(resp);
                        }
                        // if (!requestSuccess)
                        {
                            //_errorCount++;
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
                        result = requestSuccess;
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



        private bool CheckStateRequest(string response)
        {
            bool result = true;
            try
            {
                // TextReader tr = ;
                //  XElement elem = XElement.Load(tr);

                //   string xml = "<StatusDocumentItem xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><DataUrl/><LastUpdated>2013-02-01T12:35:29.9517061Z</LastUpdated><Message>Job put in queue</Message><State>0</State><StateName>Waiting to be processed</StateName></StatusDocumentItem>";
                XmlSerializer serializer = new XmlSerializer(typeof(LICSResponse));

                LICSResponse resp = (LICSResponse)serializer.Deserialize(new StringReader(response));
                if (resp.State != 0)
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            return result;
        }

        private void SimplestParcerLicsRequest(string response)
        {
            try
            {
                //TextReader tr = new StringReader(response);

                XmlSerializer serializer = new XmlSerializer(typeof(LICSResponse));

                XmlSerializer ResponseSerializer = new XmlSerializer(typeof(LICSResponse));
                LICSResponse objResponse;
                using (TextReader tr = new StringReader(response))
                {
                    objResponse = (LICSResponse)ResponseSerializer.Deserialize(tr);
                }

                //Validate the results
                if (objResponse.ArticleImport != null && objResponse.ArticleImport.CountOfFailed > 0)
                {
                    //Handle ArticleImportErrors
                }

                if (objResponse.DeliveryImport != null && objResponse.DeliveryImport.CountOfFailed > 0)
                {
                    //Handle DeliveryImportErrors
                }

                if (objResponse.OrderImport != null && objResponse.OrderImport.CountOfFailed > 0)
                {
                    //Handle OrderImportErrors
                }






                // LICSResponse resp = (LICSResponse)serializer.Deserialize(tr);
                if (objResponse.OrderImport != null)
                    foreach (var ord in objResponse.OrderImport.Order)
                    {
                        int status = 2;
                        if (ord.State == 1)
                        {
                            status = 5;
                        }
                        else
                        {
                            status = 3;
                        }
                        SendToDB(ord.OrderNo, status);
                        _loger.Log(string.Format("Has updated order with NO = {0}  new status = {1}", ord.ReferenceNo, status));
                    }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

        }
        private string DbConnection
        {
            get
            {
              return  string.Empty;
                //return _customerInfo.PluginSettings.MsSqlSettings.DbSettings.UseSqlServerAuth ? string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWithServerAuth, _customerInfo.PluginSettings.MsSqlSettings.DbSettings.ServerAdress,
                //                                                                                         _customerInfo.PluginSettings.MsSqlSettings.DbSettings.DataBaseName,
                //                                                                                         _customerInfo.PluginSettings.MsSqlSettings.DbSettings.DbUserName,
                //                                                                                         _customerInfo.PluginSettings.MsSqlSettings.DbSettings.Password) :
                //                                                    string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _customerInfo.PluginSettings.MsSqlSettings.DbSettings.ServerAdress,
                //                                                                                         _customerInfo.PluginSettings.MsSqlSettings.DbSettings.DataBaseName);

                //return   string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _pluginSettings.DbSettings.ServerAdress,
                //                                                                                    _pluginSettings.DbSettings.DataBaseName);

            }
        }

        private void SendToDB(string no, int status)
        {
            try
            {
                using (var connection = new SqlConnection(DbConnection))
                {
                    connection.Open();
                    var cmdText = string.Format(@"UPDATE WAKopf SET Status = {0} WHERE ID = {1} ", status, no);
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    _loger.Log(string.Format("Wasnot errors while updating customer DB"));
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Exception in the Sent info to DB");
                _loger.Log(ex);
            }


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

        private void WrightLog(string message)
        {
            _loger.Log(message);
        }



        private bool HasItemsForSending(LICSRequest[] requests, Ix4RequestProps ix4Property)
        {
            bool result = false;
            if (requests != null)
            {
                switch (ix4Property)
                {
                    case Ix4RequestProps.Articles:
                        foreach (LICSRequest request in requests)
                        {
                            _loger.Log(string.Format("Count of available {0} = {1}", ix4Property, request.OrderImport.Length));
                            if (request.ArticleImport.Length > 0)
                            {
                                result = true;
                                break;
                            }
                        }
                        break;
                    case Ix4RequestProps.Orders:
                        foreach (LICSRequest request in requests)
                        {
                            _loger.Log(string.Format("Count of available {0} = {1}", ix4Property, request.OrderImport.Length));
                            if (request.OrderImport.Length > 0)
                            {
                                result = true;
                                break;
                            }
                        }
                        break;
                    case Ix4RequestProps.Deliveries:
                        foreach (LICSRequest request in requests)
                        {
                            _loger.Log(string.Format("Count of available {0} = {1}", ix4Property, request.OrderImport.Length));
                            if (request.DeliveryImport.Length > 0)
                            {
                                result = true;
                                break;
                            }
                        }
                        break;
                    default:
                        break;

                }
            }

            return result;
        }

        private void CheckPreparedRequest(CustomDataSourceTypes dataSourceType, Ix4RequestProps ix4Property)
        {

            try
            {
                // if (UpdateTimeWatcher.TimeToCheck(ix4Property))
                {
                    _loger.Log(string.Format("Start Check {0} using {1} plugin", ix4Property.ToString(), dataSourceType.ToString()));
                    LICSRequest[] requests = _dataCompositor.GetPreparedRequests(dataSourceType, ix4Property);

                    if (requests != null && HasItemsForSending(requests, ix4Property))
                    {
                        foreach (var item in requests)
                        {
                            //_loger.Log(string.Format("Count of available {0} = {1}", ix4Property, item.OrderImport.Length));
                            //_loger.Log("LicsReques orders = " + item.SerializeObjectToString<LICSRequest>());
                            item.ClientId = _customerInfo.ClientID;
                            //_loger.Log("client id = " + _customerInfo.ClientID);
                            foreach (var order in item.OrderImport)
                            {
                                order.ClientNo = _customerInfo.ClientID;
                            }
                            bool res = SendLicsRequestToIx4(item, string.Format("{0}File.xml", ix4Property.ToString()));
                            _loger.Log(string.Format("{0} result: {1}", ix4Property, res));
                            if (res)
                            {
                                UpdateTimeWatcher.SetLastUpdateTimeProperty(ix4Property);
                            }
                        }
                    }
                    _loger.Log(string.Format("Finish Check {0} using {1} plugin", ix4Property.ToString(), dataSourceType.ToString()));
                    System.Threading.Thread.Sleep(30000);
                }

            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }



        private void CheckDeliveries()
        {
            try
            {
                if (UpdateTimeWatcher.TimeToCheck(Ix4RequestProps.Deliveries))
                {
                    if (_cachedArticles == null)
                    {
                        _loger.Log("There is no cheched articles for filling deliveries");
                        CheckArticles();
                        if (_cachedArticles == null)
                        {
                            _loger.Log("WE CANNOT GET DELIVERIES WITHOUT ARTICLES");
                            return;
                        }
                    }
                    int currentClientID = _customerInfo.ClientID;
                    LICSRequest request = new LICSRequest();
                    request.ClientId = currentClientID;
                    LICSRequestDelivery[] deliveries = _dataCompositor.GetRequestDeliveries();
                    List<LICSRequestArticle> articlesByDelliveries = new List<LICSRequestArticle>();
                    _loger.Log(deliveries, "deliveries");
                    if (deliveries.Length == 0)
                    {
                        _loger.Log("There is no deliveries");
                        return;
                    }
                    foreach (LICSRequestDelivery delivery in deliveries)
                    {
                        bool deliveryHasErrors = false;
                        articlesByDelliveries = new List<LICSRequestArticle>();
                        delivery.ClientNo = currentClientID;
                        request.DeliveryImport = new LICSRequestDelivery[] { delivery };
                        foreach (var position in delivery.Positions)
                        {
                            LICSRequestArticle findArticle = GetArticleByNumber(position.ArticleNo);
                            if (findArticle == null)
                            {
                                _loger.Log("Cannot find article with no:  " + position.ArticleNo);
                                _loger.Log("Delivery with wrong article position:  " + delivery);
                                deliveryHasErrors = true;
                            }
                            else
                            {
                                articlesByDelliveries.Add(findArticle);
                            }
                        }
                        if (deliveryHasErrors)
                        {
                            _loger.Log("Delivery " + delivery + "WAS NOT SEND");
                            continue;
                        }
                        else
                        {
                            request.ArticleImport = articlesByDelliveries.ToArray();
                            _loger.Log("Delivery before sending: ");
                            foreach (LICSRequestDelivery item in request.DeliveryImport)
                            {
                                _loger.Log(item.SerializeObjectToString<LICSRequestDelivery>());
                            }

                            var res = SendLicsRequestToIx4(request, "deliveryFile.xml");
                            _loger.Log("Delivery result: " + res);
                        }

                    }
                    UpdateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Deliveries);
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }
        LICSRequestArticle[] _cachedArticles;
        private LICSRequestArticle GetArticleByNumber(string articleNo)
        {
            if (_cachedArticles != null)
                return _cachedArticles.FirstOrDefault(item => item.ArticleNo.Equals(articleNo));
            else
            {
                _loger.Log("There is no CACHED ARTICLES!!!!!!!!!!");
                return null;
            }

        }



        private void CheckArticles()
        {
            int countA = 0;
            if (_isArticlesBusy)
            {
                _loger.Log("Check articles is busy");
                return;
            }
            try
            {
                if (UpdateTimeWatcher.TimeToCheck(Ix4RequestProps.Articles))
                {
                    WrightLog("Check Artikles started");
                    _isArticlesBusy = true;

                    int currentClientID = _customerInfo.ClientID;
                    LICSRequest request = new LICSRequest();
                    request.ClientId = currentClientID;
                    LICSRequestArticle[] articles = _dataCompositor.GetRequestArticles();

                    _loger.Log(string.Format("Got ARTICLES {0}", articles != null ? articles.Length : 0));

                    if (articles == null || articles.Length == 0)
                    {
                        _loger.Log("There is no available articles");
                        return;
                    }

                    List<LICSRequestArticle> tempAtricles = new List<LICSRequestArticle>();

                    for (int i = 0; i < articles.Length; i++)
                    {
                        articles[i].ClientNo = currentClientID;
                        tempAtricles.Add(articles[i]);
                        if (tempAtricles.Count >= _articlesPerRequest || i == articles.Length - 1)
                        {
                            request.ArticleImport = tempAtricles.ToArray();
                            var resSent = SendLicsRequestToIx4(request, "articleFile.xml");
                            if (resSent)
                            {
                                countA++;
                                _loger.Log(string.Format("Was sent {0} request with {1} articles", countA, tempAtricles.Count));
                                tempAtricles = new List<LICSRequestArticle>();
                            }
                        }
                    }
                    UpdateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Articles);
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
                _loger.Log("Inner excep " + ex.InnerException);
                _loger.Log("Inner excep MESSAGE" + ex.InnerException.Message);
            }
            finally
            {
                _isArticlesBusy = false;
            }
        }

        //private void CheckOrders()
        //{
        //    try
        //    {
        //        if (_customerInfo == null)
        //        {
        //            return;
        //        }

        //        string[] xmlSourceFiles = Directory.GetFiles(_customerInfo.PluginSettings.XmlSettings.XmlOrdersSourceFolder);
        //        if (xmlSourceFiles.Length > 0)
        //        {
        //            foreach (string file in xmlSourceFiles)
        //            {
        //                LICSRequest request = new LICSRequest();

        //                LICSRequestOrder[] requestOrders = _dataCompositor.GetRequestOrders();
        //                request.OrderImport = requestOrders;
        //                request.ClientId = _customerInfo.ClientID;
        //                var res = SendLicsRequestToIx4(request, Path.GetFileName(file));
        //            }
        //        }


        //        string mes1 = string.Format("Service Timer has been elapsed at {0} | {1}", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString());
        //        string mes2 = string.Format("Count of files in the folder {0} = {1}", _customerInfo.PluginSettings.XmlSettings.XmlArticleSourceFolder, xmlSourceFiles.Length);
        //        WrightLog(mes1);
        //        WrightLog(mes2);
        //    }
        //    catch (Exception ex)
        //    {
        //        WrightLog(ex.Message);
        //    }
        //}
    }
}
