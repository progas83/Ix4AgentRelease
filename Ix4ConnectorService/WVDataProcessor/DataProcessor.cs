﻿using Ix4Models.Attributes;
using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Ix4Models.SettingsDataModel;
using DataProcessorHelper;
using Ix4Models;
using System.Data.SqlClient;
using System.Xml;
using SinplestLogger.Mailer;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace WVDataProcessor
{
    [ExportDataProcessor("wwinterface1000090")]
    public class DataProcessor : BaseDataProcessor, IDataProcessor
    {

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
                _loger.Log(string.Format("Got ARTICLES {0}", articles != null ? articles.Count : 0));

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
                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Deliveries);
            }
            catch (Exception ex)
            {
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
        protected override void CheckOrders()
        {
            try
            {

                int currentClientID = CustomerSettings.ClientID;
                LICSRequest request = new LICSRequest();
                request.ClientId = currentClientID;
                List<LICSRequestOrder> orders = _importDataProvider.GetOrders();// _msSqlDataProvider.GetOrders(CustomerSettings.ImportDataSettings.OrderSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                if (!OrdersHasPositions(orders))
                {
                    _loger.Log("There is no orders for importing");
                    _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Orders);
                    return;
                }

                if (CustomerSettings.ImportDataSettings.OrderSettings.IncludeArticlesToRequest)
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
                    request.OrderImport = orders.ToArray<LICSRequestOrder>();
                    request.ArticleImport = articlesByOrders.ToArray<LICSRequestArticle>();
                }
                else
                {
                    foreach (LICSRequestOrder order in orders)
                    {
                        order.ClientNo = currentClientID;
                    }
                    request.OrderImport = orders.ToArray<LICSRequestOrder>();
                }

                LICSResponse response = SendLicsRequestToIx4(request, "ordersFile.xml");
                _loger.Log("Orders result: " + response);
                SimplestParcerLicsRequest(response);
                _updateTimeWatcher.SetLastUpdateTimeProperty(Ix4RequestProps.Orders);
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
                            MailLogger.Instance.LogMail(new ContentDescription("Order was not imported", string.Format("Message: {0}",ord.Message)));
                        }
                        SendToDB(ord.OrderNo, status);
                        _loger.Log(string.Format("Has updated order with NO = {0}  new status = {1}", ord.OrderNo, status));
                    }
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

        ExportDataToSQL _dataExportetToSql;
        protected override void ProcessExportedData(ExportDataItemSettings settings)
        {
            if (_dataExportetToSql == null)
                _dataExportetToSql = new ExportDataToSQL(CustomerSettings.ExportDataSettings);
            switch (settings.ExportDataTypeName)
            {
                case "GP":
                    ProcessGPData();
                    break;
                case "SA":
                    ProcessSAData();
                    break;
                case "BO":
                    ProcessBOData();
                    break;
                case "GR":
                    ProcessGRData();
                    break;
                case "CA":
                    ProcessCAData();
                    break;
                default: break;
            }
        }

        private void ProcessSAData()
        {
            StoragePlaces storages = new StoragePlaces();// "SP", "WE", "LP");
            try
            {
                foreach (string mark in new string[] { "SA" })
                {
                    _loger.Log("Starting export data " + mark);
                    XmlNode nodeResult = _ix4WebServiceConnector.ExportData(mark, null);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.InnerXml = nodeResult.OuterXml;
                   // XmlNodeList msgNodes1 = xmlDoc.GetElementsByTagName("MSG");
                    _ensureData.StoreExportedNodeList(xmlDoc.GetElementsByTagName("MSG"), mark, EnsureType.CollectData);
                    XmlNodeList msgNodes = storages.GetUpdatedStorageInformation(xmlDoc.GetElementsByTagName("MSG"));
 
                    if (msgNodes != null && msgNodes.Count > 0)
                    {
                        _loger.Log(string.Format("Got Exported {0} items count = {1}", mark, msgNodes.Count));
                        EnsureType ensureType = EnsureType.UpdateStoredData;
                       
                        if (!_ensureData.StoreExportedNodeList(msgNodes, mark, ensureType))
                        {
                            _ensureData.RudeStoreExportedData(nodeResult, mark);
                        }
                        else
                        {
                            _ensureData.ProcessingStoredDataToClientStorage(mark, _dataExportetToSql.SaveDataToTable<MSG>);
                        }
                        _loger.Log("End export data " + mark);
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Exception while export data");
                _loger.Log(ex);
            }
        }

        private void ProcessGPData()
        {

            try
            {
                foreach (string mark in new string[] { "GP", "GS", "CA" })
                {
                    _loger.Log("Starting export data " + mark);
                    XmlNode nodeResult = _ix4WebServiceConnector.ExportData(mark, null);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.InnerXml = nodeResult.OuterXml;
                    var msgNodes = xmlDoc.GetElementsByTagName("MSG");

                    //  var msgNodes = nodeResult.LastChild.LastChild.SelectNodes("MSG");
                    _loger.Log(string.Format("Got Exported {0} items count = {1}", mark, msgNodes.Count));
                    if (msgNodes != null && msgNodes.Count > 0)
                    {
                        EnsureType ensureType = EnsureType.CollectData;

                        if (!_ensureData.StoreExportedNodeList(msgNodes, mark, ensureType))
                        {
                            _ensureData.RudeStoreExportedData(nodeResult, mark);
                        }
                        else
                        {
                            _ensureData.ProcessingStoredDataToClientStorage(mark, _dataExportetToSql.SaveDataToTable<MSG>);//.SaveDataToTable(. _dataCompositor.GetCustomerDataConnector(CustomDataSourceTypes.MsSql));
                        }
                        _loger.Log("End export data " + mark);
                        //  System.Threading.Thread.Sleep(30000);
                    }

                }
            }
            catch (Exception ex)
            {
                _loger.Log("Exception while export data");
                _loger.Log(ex);
            }
        }

        private void ProcessBOData()
        {
            try
            {
                foreach (string mark in new string[] { "BO" })
                {
                    _loger.Log("Starting export data " + mark);
                    XmlNode nodeResult = _ix4WebServiceConnector.ExportData(mark, null);

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
                            case "GR":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "CA":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "BO":
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
                            _ensureData.ProcessingStoredDataToClientStorage(mark, _dataExportetToSql.SaveDataToTable<MSG>);//.SaveDataToTable(. _dataCompositor.GetCustomerDataConnector(CustomDataSourceTypes.MsSql));
                        }
                        _loger.Log("End export data " + mark);
                        //System.Threading.Thread.Sleep(30000);
                    }

                }
            }
            catch (Exception ex)
            {
                _loger.Log("Exception while export data");
                _loger.Log(ex);
            }
        }

        private void ProcessCAData()
        {
            try
            {
                foreach (string mark in new string[] { "CA" })
                {
                    _loger.Log("Starting export data " + mark);
                    XmlNode nodeResult = _ix4WebServiceConnector.ExportData(mark, null);

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
                            case "GR":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "CA":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "BO":
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
                            _ensureData.ProcessingStoredDataToClientStorage(mark, _dataExportetToSql.SaveDataToTable<MSG>);//.SaveDataToTable(. _dataCompositor.GetCustomerDataConnector(CustomDataSourceTypes.MsSql));
                        }
                        _loger.Log("End export data " + mark);
                        //    System.Threading.Thread.Sleep(30000);
                    }

                }
            }
            catch (Exception ex)
            {
                _loger.Log("Exception while export data");
                _loger.Log(ex);
            }
        }

        private void ProcessGRData()
        {
            try
            {
                foreach (string mark in new string[] { "GR" })
                {
                    _loger.Log("Starting export data " + mark);
                    XmlNode nodeResult = _ix4WebServiceConnector.ExportData(mark, null);

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
                                ensureType = EnsureType.CollectData;
                                break;
                            case "GP":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "GS":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "GR":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "CA":
                                ensureType = EnsureType.CollectData;
                                break;
                            case "BO":
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
                            _ensureData.ProcessingStoredDataToClientStorage(mark, _dataExportetToSql.SaveDataToTable<MSG>);//.SaveDataToTable(. _dataCompositor.GetCustomerDataConnector(CustomDataSourceTypes.MsSql));
                        }
                        _loger.Log("End export data " + mark);
                        //System.Threading.Thread.Sleep(30000);
                    }

                }
            }
            catch (Exception ex)
            {
                _loger.Log("Exception while export data");
                _loger.Log(ex);
            }
        }

        private void SendToDB(string no, int status)
        {
            try
            {
                using (var connection = new SqlConnection((CustomerSettings.ImportDataSettings.OrderSettings.DataSourceSettings as MsSqlSettings).BuilDBConnection()))
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
    }
}
