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
        //private CustomerInfo _customerSettings;
        //private IProxyIx4WebService _ix4WebServiceConnector;
        //private UpdateTimeWatcher _updateTimeWatcher;

       
        public DataProcessor()
        {
            //    _ensureData = new DataEnsure(_customerInfo.UserName);
        }
        //private CustomerInfo _CustomerSettings
        //{
        //    get
        //    {
        //        if (_customerSettings == null)
        //            throw (new Exception("Settings was not load"));
        //        return _customerSettings;
        //    }
        //}

        //private MsSqlDataProvider _msSqlDataProvider;
        //public void LoadSettings(CustomerInfo customerSettings)
        //{
        //    _customerSettings = customerSettings;
        //    _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(_CustomerSettings.ClientID, _CustomerSettings.UserName, _CustomerSettings.Password, _CustomerSettings.ServiceEndpoint);
        //    _updateTimeWatcher = new UpdateTimeWatcher(_CustomerSettings.ImportDataSettings, _CustomerSettings.ExportDataSettings);
        //    _msSqlDataProvider = new MsSqlDataProvider();// _CustomerSettings.ImportDataSettings, _CustomerSettings.ExportDataSettings);
        //}
        //public void ExportData()
        //{
        //    //  throw new NotImplementedException();
        //}

        //public override void ImportData()
        //{
        //    base.ImportData();
        //    //if (_customerSettings.ImportDataSettings.ArticleSettings.IsActivate && _updateTimeWatcher.TimeToCheck(Ix4RequestProps.Articles))
        //    //{
        //    //    CheckArticles();
        //    //    _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Articles);
        //    //}

        //    //if (_customerSettings.ImportDataSettings.DeliverySettings.IsActivate && _updateTimeWatcher.TimeToCheck(Ix4RequestProps.Deliveries))
        //    //{
        //    //    CheckDeliveries();
        //    //    _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Deliveries);
        //    //}

        //    //if (_customerSettings.ImportDataSettings.OrderSettings.IsActivate && _updateTimeWatcher.TimeToCheck(Ix4RequestProps.Orders))
        //    //{
        //    //    CheckOrders();
        //    //    _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Orders);
        //    //}

        //    //   _updateTimeWatcher.SaveLastUpdateValues();
        //}
        protected override void CheckArticles()
        {
            int countA = 0;
            try
            {

                int currentClientID = CustomerSettings.ClientID;

                LICSRequest request = new LICSRequest();
                request.ClientId = currentClientID;
                List<LICSRequestArticle> articles = _importDataProvider.GetArticles();// _msSqlDataProvider.GetArticles(CustomerSettings.ImportDataSettings.ArticleSettings.DataSourceSettings as MsSqlArticlesSettings);
                _cachedArticles = articles;
                _loger.Log(string.Format("Got ARTICLES {0}", articles != null ? articles.Count : 0));

                if (articles == null || articles.Count == 0)
                {
                    // _loger.Log("There is no available articles");
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
                        if (resSent.ArticleImport.CountOfFailed == 0)
                        {
                            countA++;
                            _loger.Log(string.Format("Was sent {0} request with {1} articles", countA, tempAtricles.Count));
                        }
                        else
                        {

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
                _loger.Log("Inner excep MESSAGE" + ex.InnerException.Message);
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
                        List<LICSRequestArticle> articles = _importDataProvider.GetArticles();
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

                var res = SendLicsRequestToIx4(request, "deliveryFile.xml");
                _loger.Log("Delivery result: " + res);
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
            if(xmlSettings==null)
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
                        request.ClientId = CustomerSettings.ClientID;

                        if(request.ArticleImport!=null)
                        {
                            foreach(var article in request.ArticleImport)
                            {
                                article.ClientNo = CustomerSettings.ClientID;
                            }
                        }
                        if(request.OrderImport!=null)
                        {
                            foreach(var ord in request.OrderImport)
                            {
                                ord.ClientNo = CustomerSettings.ClientID;
                            }
                        }
                        LICSResponse response = SendLicsRequestToIx4(request, "deliveryFile.xml");
                        if (response.DeliveryImport.CountOfFailed == 0)
                        {
                            string clientsDirectory = "D:\\Transfer\\XML_Archiv";
                            string customDirectory = string.Format("{0}\\Archive", xmlSettings.XmlItemSourceFolder);
                            string successFolder = string.Empty;
                            if (!Directory.Exists(clientsDirectory))
                            {
                                if (!Directory.Exists(customDirectory))
                                {
                                    Directory.CreateDirectory(customDirectory);
                                    successFolder = customDirectory;
                                }
                            }
                            else
                            {
                                successFolder = clientsDirectory;
                            }
                            string fn = Path.GetFileName(file);
                            File.Move(file, string.Format("{0}\\{1}", successFolder, fn));
                            _loger.Log(string.Format("File {0} moved to {1} folder",fn,successFolder));
                        }
                    }
                }
                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Orders);
            }
            catch(Exception ex)
            {
                _loger.Log(ex);
            }
           
        }
        private LICSRequest GetCustomerDataFromXml(string fileName)
        {

            XmlSerializer xS = new XmlSerializer(typeof(OutputPayLoad));
            LICSRequest licsRequest = new LICSRequest();
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                OutputPayLoad customerInfo = (OutputPayLoad)xS.Deserialize(fs);
                licsRequest = customerInfo.ConvertToLICSRequest();
            }

            return licsRequest;
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


        /*   private CustomerInfo _customerInfo;
           private IProxyIx4WebService _ix4WebServiceConnector;
           // private DataEnsure _ensureData;
           private UpdateTimeWatcher _updateTimeWatcher;

           public void ImportData()
           {

               ImportDataSettings dataSettings = _customerInfo.ImportDataSettings;
               WrightLog("Start checking");
               if (dataSettings.ArticleSettings.IsActivate)//_customerInfo.im.PluginSettings.MsSqlSettings.CheckArticles)
               {
                   WrightLog("Start checking articles");
                   CheckArticles();
               }

               if (dataSettings.DeliverySettings.IsActivate)
               {
                   WrightLog("Start checking deliveries");
                   CheckDeliveries();
               }

               if (_customerInfo.PluginSettings.XmlSettings.CheckOrders)
               {
                   WrightLog("Start Check Orders");
                   CheckPreparedRequest(CustomDataSourceTypes.Xml, Ix4RequestProps.Orders);
               }

           }
           private static Logger _loger = Logger.GetLogger();
           bool _isArticlesBusy = false;



           private LICSRequestArticle[] GetRequestArticles()
           {
               MsSqlPluginSettings _customerInfo.ImportDataSettings.ArticleSettings.DataSourceSettings
               LICSRequestArticle[] articles = null;
               try
               {
                   using (var connection = new SqlConnection(DbConnection))
                   {
                       connection.Open();
                       var cmdText = articleCmd;// _pluginSettings.ArticlesQuery;
                       SqlCommand cmd = new SqlCommand(cmdText, connection);
                       SqlDataReader reader = cmd.ExecuteReader();
                       articles = LoadArticles(reader);
                       _loger.Log(string.Format("Article no in SQL Extractor = {0}", articles.Length));
                   }
               }
               catch (Exception ex)
               {
                   _loger.Log(ex);
               }
               return articles;
           }


           public void ExportData()
           {

           }

        */




       

        // private void ExportData()
        // {
        //if (_ix4WebServiceConnector != null && _ensureData != null && _dataCompositor != null)
        //{
        //    // if (UpdateTimeWatcher.TimeToCheck("GP"))
        //    {
        //        try
        //        {
        //            foreach (string mark in new string[] {"SA"})// { "GP", "GS" })
        //            {
        //                //if (!UpdateTimeWatcher.TimeToCheck(mark))
        //                //{
        //                //    continue;
        //                //}
        //                //else
        //                //{

        //                //}
        //                _loger.Log("Starting export data " + mark);
        //                XmlNode nodeResult = _ix4WebServiceConnector.ExportData(mark, null);

        //                XmlDocument xmlDoc = new XmlDocument();
        //                xmlDoc.InnerXml = nodeResult.OuterXml;
        //                var msgNodes = xmlDoc.GetElementsByTagName("MSG");

        //                //  var msgNodes = nodeResult.LastChild.LastChild.SelectNodes("MSG");
        //                _loger.Log(string.Format("Got Exported {0} items count = {1}", mark, msgNodes.Count));
        //                if (msgNodes != null && msgNodes.Count > 0)
        //                {
        //                    EnsureType ensureType = EnsureType.CollectData;
        //                    switch (mark)
        //                    {
        //                        case "SA":
        //                            ensureType = EnsureType.UpdateStoredData;
        //                            break;
        //                        case "GP":
        //                            ensureType = EnsureType.CollectData;
        //                            break;
        //                        case "GS":
        //                            ensureType = EnsureType.CollectData;
        //                            break;
        //                        default:
        //                            ensureType = EnsureType.CollectData;
        //                            break;
        //                    }

        //                    if (!_ensureData.StoreExportedNodeList(msgNodes, mark, ensureType))
        //                    {
        //                        _ensureData.RudeStoreExportedData(nodeResult, mark);
        //                    }
        //                    else
        //                    {
        //                        _ensureData.ProcessingStoredDataToClientStorage(mark, _dataCompositor.GetCustomerDataConnector(CustomDataSourceTypes.MsSql));
        //                    }
        //                    _loger.Log("End export data " + mark);
        //                    System.Threading.Thread.Sleep(30000);
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _loger.Log("Exception while export data");
        //            _loger.Log(ex);
        //        }
        //        //    UpdateTimeWatcher.SetLastUpdateTimeProperty(mark);


        //    }
        // }
        //  }

        //  private static int _errorCount = 0;



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
            throw new NotImplementedException();
        }
    }
}
