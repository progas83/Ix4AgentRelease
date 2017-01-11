using Ix4Connector;
using Ix4Models;
using Ix4Models.Reports;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using SinplestLogger.Mailer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WV_newDataProcessor.ImportData
{
    public class DataImporter
    {
        private ImportDataSettings _importDataSettings;
        private ImportDataSourcesBuilder _importDataProvider;
        public event EventHandler<DataReportEventArgs> ImportOperationReportEvent;
        // private UpdateTimeWatcher _updateTimeWatcher;
        protected IProxyIx4WebService _ix4WebServiceConnector;
        public DataImporter(ImportDataSettings importDataSettings,IProxyIx4WebService ix4WebServiceConnector)//, UpdateTimeWatcher updateTimeWatcher)
        {
            _importDataSettings = importDataSettings;
            _ix4WebServiceConnector = ix4WebServiceConnector;
            _importDataProvider = new ImportDataSourcesBuilder(importDataSettings);
           // _updateTimeWatcher = updateTimeWatcher;// new UpdateTimeWatcher(CustomerSettings.ImportDataSettings, CustomerSettings.ExportDataSettings);
        }
       
        private void SendReportOnOperationComlete(ExportDataReport report)
        {
            if (report != null)
            {
                report.OperationDate = DateTime.Now;
                EventHandler<DataReportEventArgs> reportEvent = ImportOperationReportEvent;
                if (reportEvent != null)
                {
                    reportEvent(this, new DataReportEventArgs(report));
                }
            }
        }

        private readonly int _articlesPerRequest = 20;
        protected List<LICSRequestArticle> _cachedArticles;
        private static Logger _loger = Logger.GetLogger();
        private static object _o = new object();

        public int ClientID { get; internal set; }

        public void ImportArticles()
        {
            ExportDataReport report = new ExportDataReport(Ix4ImportDataTypes.Articles.ToString());
            int countA = 0;
            try
            {

             //   int currentClientID = CustomerSettings.ClientID;

                LICSRequest request = new LICSRequest();
                request.ClientId = ClientID;
                List<LICSRequestArticle> articles = _importDataProvider.GetArticles();
                _cachedArticles = articles;
                _loger.Log(string.Format("Got ARTICLES {0}", articles != null ? articles.Count : 0));

                if (articles == null || articles.Count == 0)
                {
                    _loger.Log("There is no available articles");
                    return;
                }


                List<LICSRequestArticle> tempAtricles = new List<LICSRequestArticle>();

                for (int i = 0; i < articles.Count; i++)
                {
                    articles[i].ClientNo = ClientID;
                    tempAtricles.Add(articles[i]);
                    if (tempAtricles.Count >= _articlesPerRequest || i == articles.Count - 1)
                    {
                        request.ArticleImport = tempAtricles.ToArray();
                        LICSResponse resSent = SendLicsRequestToIx4(request, "articleFile.xml");
                        if (resSent != null && resSent.ArticleImport != null)
                        {
                            countA++;
                            _loger.Log(string.Format("Was sent {0} request with {1} articles.Count of CountOfSuccessful = {2}.Count of failed = {3}", countA, tempAtricles.Count, resSent.ArticleImport.CountOfSuccessful, resSent.ArticleImport.CountOfFailed));
                        }
                        else
                        {
                            _loger.Log(string.Format("Error import Articles : State = {0} ; Message = {1}", resSent.State, resSent.Message));
                        }
                        tempAtricles = new List<LICSRequestArticle>();
                    }
                }
                SendReportOnOperationComlete(report);
               // _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4ImportDataTypes.Articles);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
                _loger.Log("Inner excep " + ex.InnerException);
            }

        }

        public void ImportOrders()
        {
            try
            {
                ExportDataReport report = new ExportDataReport(Ix4ImportDataTypes.Orders.ToString());
                // int currentClientID = CustomerSettings.ClientID;
                LICSRequest request = new LICSRequest();
                request.ClientId = ClientID;
                List<LICSRequestOrder> orders = _importDataProvider.GetOrders();// _msSqlDataProvider.GetOrders(CustomerSettings.ImportDataSettings.OrderSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                if (!OrdersHasPositions(orders))
                {
                    _loger.Log("There is no orders for importing");
                    SendReportOnOperationComlete(report);
                    //_updateTimeWatcher.SetLastUpdateTimeProperty(Ix4ImportDataTypes.Orders);
                    return;
                }

                if (_importDataSettings.OrderSettings.IncludeArticlesToRequest)
                {
                    if (_cachedArticles == null)
                    {
                        _loger.Log("There is no cheched articles for filling orders");
                        List<LICSRequestArticle> articles = _importDataProvider.GetArticles();// _msSqlDataProvider.GetArticles(CustomerSettings.ImportDataSettings.ArticleSettings.DataSourceSettings);
                        _cachedArticles = articles;
                        if (_cachedArticles == null)
                        {
                            _loger.Log("WE CANNOT GET DELIVERIES WITHOUT ARTICLES");
                            return;
                        }
                    }
                    List<LICSRequestArticle> articlesByOrders = new List<LICSRequestArticle>();
                    foreach (LICSRequestOrder order in orders)
                    {
                        order.ClientNo = ClientID;
                        foreach (var position in order.Positions)
                        {
                            LICSRequestArticle findArticle = GetArticleByNumber(position.ArticleNo);
                            if (findArticle == null)
                            {
                                _loger.Log("Cannot find article with no:  " + position.ArticleNo);
                                _loger.Log("Delivery with wrong article position:  " + order.OrderNo);
                            }
                            else
                            {
                                articlesByOrders.Add(findArticle);
                            }
                        }
                    }
                    request.OrderImport = orders.ToArray<LICSRequestOrder>();
                    request.ArticleImport = articlesByOrders.ToArray<LICSRequestArticle>();
                }
                else
                {
                    foreach (LICSRequestOrder order in orders)
                    {
                        order.ClientNo = ClientID;
                        if (order.OrderNo.Equals("1701047"))
                        {
                          _loger.Log("Have found order with no " + order.OrderNo);
                          order.Recipient.Name = "Württ. Vers. AG M.Kaczamrek/F.Stüt";
                            order.Recipient.AdditionalName = string.Empty;// "Galster und Kollegen";
                        }
                    }
                    request.OrderImport = orders.ToArray<LICSRequestOrder>();
                }

                LICSResponse response = SendLicsRequestToIx4(request, "ordersFile.xml");
                _loger.Log("Orders result: " + response);
                SimplestParcerLicsRequest(response);
                SendReportOnOperationComlete(report);
                //_updateTimeWatcher.SetLastUpdateTimeProperty(Ix4ImportDataTypes.Orders);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }

        public void ImportDeliveries()
        {
            try
            {
                ExportDataReport report = new ExportDataReport(Ix4ImportDataTypes.Deliveries.ToString());
                // int currentClientID = CustomerSettings.ClientID;
                LICSRequest request = new LICSRequest();
                request.ClientId = ClientID;
                List<LICSRequestDelivery> deliveries = _importDataProvider.GetDeliveries();//.GetArticles(); _msSqlDataProvider.GetDeliveries(CustomerSettings.ImportDataSettings.DeliverySettings.DataSourceSettings as MsSqlDeliveriesSettings);
                if (_importDataSettings.DeliverySettings.IncludeArticlesToRequest)
                {
                    if (_cachedArticles == null)
                    {
                        _loger.Log("There is no cheched articles for filling deliveries");
                        List<LICSRequestArticle> articles = _importDataProvider.GetArticles();// _msSqlDataProvider.GetArticles(CustomerSettings.ImportDataSettings.ArticleSettings.DataSourceSettings as MsSqlArticlesSettings);
                        _cachedArticles = articles;
                        if (_cachedArticles == null)
                        {
                            _loger.Log("WE CANNOT GET DELIVERIES WITHOUT ARTICLES");
                            return;
                        }
                    }
                    List<LICSRequestArticle> articlesByDelliveries = new List<LICSRequestArticle>();
                    foreach (LICSRequestDelivery delivery in deliveries)
                    {
                        delivery.ClientNo = ClientID;
                        foreach (var position in delivery.Positions)
                        {
                            LICSRequestArticle findArticle = GetArticleByNumber(position.ArticleNo);
                            if (findArticle == null)
                            {
                                _loger.Log("Cannot find article with no:  " + position.ArticleNo);
                                _loger.Log("Delivery with wrong article position:  " + delivery.DeliveryNo);
                            }
                            else
                            {
                                articlesByDelliveries.Add(findArticle);
                            }
                        }
                    }

                    request.ArticleImport = articlesByDelliveries.ToArray();
                }
                else
                {
                    foreach (LICSRequestDelivery delivery in deliveries)
                    {
                        delivery.ClientNo = ClientID;
                    }
                }

                var res = SendLicsRequestToIx4(request, "deliveryFile.xml");
                _loger.Log("Delivery result: " + res);
                SendReportOnOperationComlete(report);
                // _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4ImportDataTypes.Deliveries);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }

        private void SimplestParcerLicsRequest(LICSResponse objResponse)
        {
            try
            {
                ////TextReader tr = new StringReader(response);

                //XmlSerializer serializer = new XmlSerializer(typeof(LICSResponse));

                //XmlSerializer ResponseSerializer = new XmlSerializer(typeof(LICSResponse));
                //LICSResponse objResponse;
                //using (TextReader tr = new StringReader(response))
                //{
                //    objResponse = (LICSResponse)ResponseSerializer.Deserialize(tr);
                //}

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

                if (objResponse.OrderImport != null)
                    foreach (var ord in objResponse.OrderImport.Order)
                    {
                        int status = 2;
                        if (ord.State == 1)
                        {
                            status = 5;
                            SendToDB(ord.OrderNo, status);
                        }
                        else
                        {
                            status = 3;
                            MailLogger.Instance.LogMail(new ContentDescription(string.Format("Order {0} was not imported", ord.OrderNo), string.Format("Message: {0}", ord.Message)));
                        }
                     //   SendToDB(ord.OrderNo, status);
                        _loger.Log(string.Format("Has updated order with NO = {0}  new status = {1}", ord.OrderNo, status));
                    }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

        }

        private void SendToDB(string no, int status)
        {
            try
            {
                using (var connection = new SqlConnection((_importDataSettings.OrderSettings.DataSourceSettings as MsSqlSettings).BuilDBConnection()))
                {
                    connection.Open();
                    var cmdText = string.Format(@"UPDATE WAKopf SET Status = {0} WHERE ID = {1} ", status, no);
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // _loger.Log(string.Format("Wasnot errors while updating customer DB"));
                }
            }
            catch (Exception ex)
            {
                _loger.Log(string.Format("Exception while order {0} status update to {1}", no, status));
                _loger.Log(ex);
            }
        }

        private bool OrdersHasPositions(List<LICSRequestOrder> orders)
        {
            bool result = false;
            foreach (LICSRequestOrder order in orders)
            {
                if (order.Positions != null && order.Positions.Count() > 0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

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
        private LICSResponse SendLicsRequestToIx4(LICSRequest request, string fileName)
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
                            string response = _ix4WebServiceConnector.ImportXmlRequest(bytesRequest, fileName);
                            XmlSerializer xS = new XmlSerializer(typeof(LICSResponse));
                            using (var sr = new StringReader(response))
                            {
                                result = (LICSResponse)xS.Deserialize(sr);
                            }
                            _loger.Log("Impoort data response : " + response);
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
