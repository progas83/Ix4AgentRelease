using DataProcessorHelper;
using Ix4Connector;
using Ix4Models;
using Ix4Models.Attributes;
using Ix4Models.DataProviders.MsSqlDataProvider;
using Ix4Models.Interfaces;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace WWDataProcessor
{
    [ExportDataProcessor("wsbio1000001")]
    public class DataProcessor : BaseDataProcessor, IDataProcessor
    {
        public DataProcessor()
        {
        }

        protected override void CheckArticles()
        {
            int countA = 0;
            try
            {

                int currentClientID = CustomerSettings.ClientID;

                LICSRequest request = new LICSRequest();
                request.ClientId = currentClientID;
                List<LICSRequestArticle> articles = _importDataProvider.GetArticles();
                _cachedArticles = articles;
                if (articles == null || articles.Count == 0)
                {
                    _loger.Log("There is no available articles");
                    return;
                }

                List<LICSRequestArticle> tempAtricles = new List<LICSRequestArticle>();

                for (int i = 0; i < articles.Count; i++)
                {
                    articles[i].ClientNo = currentClientID;
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

                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Articles);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
                _loger.Log("Inner excep " + ex.InnerException);
            }
        }

        protected override void CheckDeliveries()
        {
            try
            {

                int currentClientID = CustomerSettings.ClientID;
                LICSRequest request = new LICSRequest();
                request.ClientId = currentClientID;
                List<LICSRequestDelivery> deliveries = _importDataProvider.GetDeliveries();
                if (CustomerSettings.ImportDataSettings.DeliverySettings.IncludeArticlesToRequest)
                {
                    if (_cachedArticles == null)
                    {
                        _loger.Log("There is no cheched articles for filling deliveries");
                        _cachedArticles = _importDataProvider.GetArticles();
                        if (_cachedArticles == null)
                        {
                            _loger.Log("WE CANNOT GET DELIVERIES WITHOUT ARTICLES");
                            return;
                        }
                    }
                    List<LICSRequestArticle> articlesByDelliveries = new List<LICSRequestArticle>();
                    foreach (LICSRequestDelivery delivery in deliveries)
                    {
                        delivery.ClientNo = currentClientID;
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
                                findArticle.ClientNo = CustomerSettings.ClientID;
                                articlesByDelliveries.Add(findArticle);
                            }
                        }
                    }
                    request.DeliveryImport = deliveries.ToArray<LICSRequestDelivery>();
                    request.ArticleImport = articlesByDelliveries.ToArray<LICSRequestArticle>();
                }
                else
                {
                    foreach (LICSRequestDelivery delivery in deliveries)
                    {
                        delivery.ClientNo = currentClientID;
                    }
                    request.DeliveryImport = deliveries.ToArray<LICSRequestDelivery>();
                }

                LICSResponse res = SendLicsRequestToIx4(request, "deliveryFile.xml");
                if (res != null && res.DeliveryImport != null)
                {
                    _loger.Log(string.Format("Delivery result: CountOfFailed = {0} ; CountOfSuccess = {1}" + res.DeliveryImport.CountOfFailed, res.DeliveryImport.CountOfSuccessful));
                }

                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Deliveries);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }

        protected override void CheckOrders()
        {
            XmlFolderSettingsModel xmlSettings = CustomerSettings.ImportDataSettings.OrderSettings.DataSourceSettings as XmlFolderSettingsModel;
            if (xmlSettings == null)
            {
                _loger.Log("Wrong settings data for orders");
                return;
            }

            try
            {
                string[] xmlSourceFiles = Directory.GetFiles(xmlSettings.XmlItemSourceFolder, "*.xml");
                if (xmlSourceFiles.Length > 0)
                {
                    foreach (string file in xmlSourceFiles)
                    {
                        LICSRequest request = GetCustomerDataFromXml(file);
                        if (request != null)
                        {
                            request.ClientId = CustomerSettings.ClientID;

                            if (request.ArticleImport != null)
                            {
                                foreach (var article in request.ArticleImport)
                                {
                                    article.ClientNo = CustomerSettings.ClientID;
                                }
                            }
                            if (request.OrderImport != null)
                            {
                                foreach (var ord in request.OrderImport)
                                {
                                    ord.ClientNo = CustomerSettings.ClientID;
                                }
                            }
                            LICSResponse response = SendLicsRequestToIx4(request, "deliveryFile.xml");
                            if (response!=null && response.DeliveryImport!=null && response.DeliveryImport.CountOfFailed == 0)
                            {
                                string customDirectory = string.Format("{0}\\SuccessOrdersArchive", xmlSettings.XmlItemSourceFolder);
                                if (!Directory.Exists(customDirectory))
                                {
                                    Directory.CreateDirectory(customDirectory);
                                }
                                string fn = Path.GetFileName(file);
                                File.Move(file, string.Format("{0}\\{1}", customDirectory, fn));
                                _loger.Log(string.Format("File {0} moved to {1} folder", fn, customDirectory));
                            }
                            else
                            {
                                _loger.Log(string.Format("Error while import order. Response state = {0} Response message = {1}",response.State,response.Message));
                            }
                        }
                    }
                }
                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Orders);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

        }


        private LICSRequest GetCustomerDataFromXml(string fileName)
        {
            LICSRequest licsRequest = null;
            try
            {
                XmlSerializer xS = new XmlSerializer(typeof(OutputPayLoad));

                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    OutputPayLoad customerInfo = (OutputPayLoad)xS.Deserialize(fs);
                    licsRequest = customerInfo.ConvertToLICSRequest();
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            return licsRequest;
        }




        private LICSRequestArticle GetArticleByNumber(string articleNo)
        {
            LICSRequestArticle foundArticle = null;
            if (_cachedArticles != null)
            {
                foundArticle = _cachedArticles.FirstOrDefault(item => item.ArticleNo.Equals(articleNo));
                if (foundArticle == null)
                {
                    _loger.Log(string.Format("We didn't find Article with number {0}", articleNo));
                }
            }
            else
            {
                _loger.Log("There is no CACHED ARTICLES!!!!!!!!!!");
            }
            return foundArticle;
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
                return string.Empty;
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

        protected override void ProcessExportedData(ExportDataItemSettings settings)
        {

        }
    }
}
