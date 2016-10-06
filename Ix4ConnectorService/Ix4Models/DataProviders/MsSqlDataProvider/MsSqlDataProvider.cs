﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ix4Models.SettingsDataModel;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using SimplestLogger;
using Ix4Models.Interfaces;

namespace Ix4Models.DataProviders.MsSqlDataProvider
{
    public class MsSqlDataProvider :IDataProvider
    {

        public MsSqlDataProvider()
        {
        }

        private List<LICSRequestArticle> LoadArticles(IDataReader reader)
        {
            DataTable table = new DataTable();
            List<LICSRequestArticle> items = new List<LICSRequestArticle>();

            table.Load(reader);
            foreach (DataRow row in table.AsEnumerable())
            {
                LICSRequestArticle article = LoadItem<LICSRequestArticle>(row);
                if (article != null)
                {
                    items.Add(article);
                }
            }
            return items;
        }

        private static Logger _loger = Logger.GetLogger();

        private List<T> LoadItems<T>(IDataReader reader) where T : new()
        {
            DataTable table = new DataTable();
            List<T> items = new List<T>();

            table.Load(reader);
            foreach (DataRow row in table.AsEnumerable())
            {
                items.Add(LoadItem<T>(row));
            }
            return items;
        }

        private T LoadItem<T>(DataRow row) where T : new()
        {
            T item = new T();

            foreach (DataColumn column in row.Table.Columns)
            {
                try
                {
                    PropertyInfo propertyInfo = item.GetType().GetProperty(column.ColumnName);
                    if (row[column.ColumnName].GetType().Equals(DBNull.Value.GetType()))
                    {
                        propertyInfo.SetValue(item, Convert.ChangeType(GetDefaultValue(propertyInfo.PropertyType), propertyInfo.PropertyType), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(item, Convert.ChangeType(row[column.ColumnName].ToString().Trim(), propertyInfo.PropertyType), null);
                    }
                }
                catch (Exception ex)
                {
                    _loger.Log("Exception while reflect DataColumn values using Reflection");
                    _loger.Log(ex);
                }

            }
            return item;
        }
   
        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        public List<LICSRequestArticle> GetArticles(BaseDataSourceSettings baseSettings)
        {
            MsSqlArticlesSettings settings = baseSettings as MsSqlArticlesSettings;
            
            List<LICSRequestArticle> articles = null;
           
            try
            {
                if (settings == null)
                {
                    throw new Exception("Empty articles settings");
                }
                using (var connection = new SqlConnection(settings.BuilDBConnection()))
                {
                    connection.Open();
                    var cmdText = settings.ArticlesQuery;
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    articles = LoadItems<LICSRequestArticle>(reader);// LoadArticles(reader);// 
                    _loger.Log(string.Format("Article no in SQL Extractor = {0}", articles.Count));
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            return articles;
        }

        public List<LICSRequestDelivery> GetDeliveries(BaseDataSourceSettings baseSettings)
        {
            MsSqlDeliveriesSettings settings = baseSettings as MsSqlDeliveriesSettings;

            List<LICSRequestDelivery> requestDeliveries = new List<LICSRequestDelivery>();
            try
            {
                if (settings == null)
                {
                    throw new Exception("Empty deliveries settings");
                }
                using (var connection = new SqlConnection(settings.BuilDBConnection()))
                {
                    connection.Open();
                    var cmdText = settings.DeliveriesQuery;
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    DataTable table = new DataTable();
                    //  List<LICSRequestDelivery> deliveries = new List<LICSRequestDelivery>();
                    table.Load(reader);

                    foreach (DataRow row in table.AsEnumerable())
                    {
                        LICSRequestDelivery delivery = LoadItem<LICSRequestDelivery>(row);

                        string getPositionsCommand = string.Format(settings.DeliveryPositionsQuery, delivery.DeliveryNo);
                        SqlCommand cmdPositions = new SqlCommand(getPositionsCommand, connection);
                        SqlDataReader positonsReader = cmdPositions.ExecuteReader();
                       // DataTable tablePositions = new DataTable();
                       // tablePositions.Load(positonsReader);
                        delivery.Positions = LoadItems<LICSRequestDeliveryPosition>(positonsReader).ToArray<LICSRequestDeliveryPosition>();
                        if (delivery.Positions != null && delivery.Positions.Length > 0)
                        {
                            requestDeliveries.Add(delivery);
                        }
                        else
                        {
                            _loger.Log("There aren't positions for deliveryNO = " + delivery.DeliveryNo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            return requestDeliveries;
        }

        public List<LICSRequestOrder> GetOrders(BaseDataSourceSettings baseSettings)
        {
            MsSqlOrdersSettings settings = baseSettings as MsSqlOrdersSettings;
            List<LICSRequestOrder> orders = new List<LICSRequestOrder>();
            try
            {
                if (settings == null)
                {
                    throw new Exception("Empty orders settings");
                }
                string dbConnectStr = settings.BuilDBConnection();
                _loger.Log("connection string = "+ dbConnectStr);
                using (var connection = new SqlConnection(dbConnectStr))
                {
                    connection.Open();
                    var cmdText = settings.OrdersQuery;
                    _loger.Log(string.Format("Order reques sql {0}", cmdText));
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);
                    foreach (DataRow row in table.AsEnumerable())
                    {
                        LICSRequestOrder orderItem = LoadItem<LICSRequestOrder>(row);// new LICSRequestOrder();

                        string getOrderRecipientQuery = string.Format(settings.OrderRecipientQuery, orderItem.OrderNo);
                        _loger.Log(string.Format("Order recipient reques sql {0}", getOrderRecipientQuery));
                        SqlCommand cmdRecipient = new SqlCommand(getOrderRecipientQuery, connection);
                        SqlDataReader readerRecipient = cmdRecipient.ExecuteReader();
                        DataTable tableRecipients = new DataTable();
                        tableRecipients.Load(readerRecipient);
                        orderItem.Recipient = LoadItem<LICSRequestOrderRecipient>(tableRecipients.Rows[0]);

                        _loger.Log(string.Format("Order {0} recipient Name {1} FirstName {2}", orderItem.OrderNo,orderItem.Recipient.Name,orderItem.Recipient.FirstName));


                        string getOrderPositionsQuery = string.Format(settings.OrderPositionsQuery, orderItem.OrderNo);
                        _loger.Log(string.Format("Order positions sql {0}", getOrderPositionsQuery));
                        SqlCommand cmdPositions = new SqlCommand(getOrderPositionsQuery, connection);
                        SqlDataReader readerPositions = cmdPositions.ExecuteReader();
                        //DataTable tablePositions = new DataTable();
                        //tablePositions.Load(readerPositions);
                        orderItem.Positions = LoadItems<LICSRequestOrderPosition>(readerPositions).ToArray<LICSRequestOrderPosition>();
                        _loger.Log(orderItem.Positions, "orderItem.Positions");
                        _loger.Log("Position number = " +orderItem.Positions.Length);
                        if (orderItem.Positions != null && orderItem.Positions.Length > 0)
                        {
                            orders.Add(orderItem);
                        }
                        else
                        {
                            _loger.Log("There isn't positions in order No " + orderItem.OrderNo);
                        }
                    }
                    //  orders = LoadOrders(reader, connection);
                    _loger.Log(string.Format("Orders no in SQL Extractor = {0}", orders.Count));
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            return orders;
        }
    }
}
