﻿using Ix4Models.Attributes;
using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using Ix4Connector;
using DataProcessorHelper;
using Ix4Models;
using Ix4Models.DataProviders.MsSqlDataProvider;

namespace WVDataProcessor
{
    [ExportDataProcessor("wwinterface1000090")]
    public class DataProcessor : BaseDataProcessor, IDataProcessor
    {
      
       

       // private IProxyIx4WebService _ix4WebServiceConnector;
       // private UpdateTimeWatcher _updateTimeWatcher;
       // private CustomerInfo _customerSettings;
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
        //    _msSqlDataProvider = new MsSqlDataProvider();
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
                List<LICSRequestDelivery> deliveries = _msSqlDataProvider.GetDeliveries(CustomerSettings.ImportDataSettings.DeliverySettings.DataSourceSettings as MsSqlDeliveriesSettings);
                if (CustomerSettings.ImportDataSettings.DeliverySettings.IncludeArticlesToRequest)
                {
                    if (_cachedArticles == null)
                    {
                        _loger.Log("There is no cheched articles for filling deliveries");
                        List<LICSRequestArticle> articles = _msSqlDataProvider.GetArticles(CustomerSettings.ImportDataSettings.ArticleSettings.DataSourceSettings as MsSqlArticlesSettings);
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
                        delivery.ClientNo = currentClientID;
                    }
                }

                var res = SendLicsRequestToIx4(request, "deliveryFile.xml");
                _loger.Log("Delivery result: " + res);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }

        protected override void CheckOrders()
        {
            try
            {

                int currentClientID = CustomerSettings.ClientID;
                LICSRequest request = new LICSRequest();
                request.ClientId = currentClientID;
                List<LICSRequestOrder> orders = _msSqlDataProvider.GetOrders(CustomerSettings.ImportDataSettings.DeliverySettings.DataSourceSettings as MsSqlDeliveriesSettings);
                if (CustomerSettings.ImportDataSettings.OrderSettings.IncludeArticlesToRequest)
                {
                    if (_cachedArticles == null)
                    {
                        _loger.Log("There is no cheched articles for filling deliveries");
                        List<LICSRequestArticle> articles = _msSqlDataProvider.GetArticles(CustomerSettings.ImportDataSettings.ArticleSettings.DataSourceSettings as MsSqlArticlesSettings);
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
                        order.ClientNo = currentClientID;
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

                    request.ArticleImport = articlesByOrders.ToArray();
                }
                else
                {
                    foreach (LICSRequestOrder order in orders)
                    {
                        order.ClientNo = currentClientID;
                    }
                }

                var res = SendLicsRequestToIx4(request, "ordersFile.xml");
                _loger.Log("Orders result: " + res);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
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
        protected override void ProcessExportedData(ExportDataItemSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
