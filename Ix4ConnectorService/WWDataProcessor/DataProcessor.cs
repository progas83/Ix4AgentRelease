using Ix4Connector;
using Ix4Models;
using Ix4Models.Interfaces;
using Ix4Models.SettingsDataModel;
using Ix4Models.SettingsManager;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWDataProcessor
{
    public class DataProcessor : IDataProcessor
    {
        /*   private CustomerInfo _customerInfo;
           private IProxyIx4WebService _ix4WebServiceConnector;
           // private DataEnsure _ensureData;
           private UpdateTimeWatcher _updateTimeWatcher;
           public DataProcessor()
           {
               _customerInfo = XmlConfigurationManager.Instance.GetCustomerInformation();
               // _dataCompositor = new CustomerDataComposition(_customerInfo.PluginSettings);
               _ix4WebServiceConnector = Ix4ConnectorManager.Instance.GetRegisteredIx4WebServiceInterface(_customerInfo.ClientID, _customerInfo.UserName, _customerInfo.Password, _customerInfo.ServiceEndpoint);

               _updateTimeWatcher = new UpdateTimeWatcher(_customerInfo.ImportDataSettings,_customerInfo.ExportDataSettings);
             //  _ensureData = new DataEnsure(_customerInfo.UserName);
           }
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
                   if (_updateTimeWatcher.TimeToCheck(Ix4RequestProps.Articles))
                   {
                       WrightLog("Check Artikles started");
                       _isArticlesBusy = true;

                       int currentClientID = _customerInfo.ClientID;
                       LICSRequest request = new LICSRequest();
                       request.ClientId = currentClientID;
                       LICSRequestArticle[] articles = _dataCompositor.GetRequestArticles();
                       _cachedArticles = articles;
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
           public void ExportData()
           {

           }

       */
        public void ExportData()
        {
            throw new NotImplementedException();
        }

        public void ImportData()
        {
            throw new NotImplementedException();
        }
    }
}
