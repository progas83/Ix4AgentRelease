using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.Interfaces;
using Ix4Models.SettingsDataModel;
using System.Data.SqlClient;
using Ix4Models;
using System.Data;
using System.Reflection;
using SimplestLogger;
using SinplestLogger.Mailer;
using Ix4Models.Enums;

namespace SqlDataExtractor
{
    class SqlTableOrdersExplorer : SqlTableWorker
    {
       // private MsSqlPluginSettings _pluginSettings;
      //  private static Logger _loger = Logger.GetLogger();

        public SqlTableOrdersExplorer(IPluginSettings pluginSettings) : base(pluginSettings)
        {
        
        }
        //private string DbConnection
        //{
        //    get
        //    {
        //        return _pluginSettings.DbSettings.UseSqlServerAuth ? string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWithServerAuth, _pluginSettings.DbSettings.ServerAdress,
        //                                                                                                 _pluginSettings.DbSettings.DataBaseName,
        //                                                                                                 _pluginSettings.DbSettings.DbUserName,
        //                                                                                                 _pluginSettings.DbSettings.Password) :
        //                                                            string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _pluginSettings.DbSettings.ServerAdress,
        //                                                                                                 _pluginSettings.DbSettings.DataBaseName);

        //        //return   string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _pluginSettings.DbSettings.ServerAdress,
        //        //                                                                                    _pluginSettings.DbSettings.DataBaseName);

        //    }
        //}

        internal LICSRequestOrder[] GetRequestOrders()
        {
            LICSRequestOrder[] orders = null;
            try
            {
                using (var connection = new SqlConnection(DbConnection))
                {
                    connection.Open();
                    var cmdText = _pluginSettings.OrdersQuery;
                    _loger.Log(string.Format("Order reques sql {0}", cmdText));
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    orders = LoadOrders(reader, connection);
                    _loger.Log(string.Format("Orders no in SQL Extractor = {0}", orders.Length));
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            return orders;
        }

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        private LICSRequestOrder[] LoadOrders(IDataReader reader, SqlConnection connection)
        {
            DataTable table = new DataTable();
            List<LICSRequestOrder> orders = new List<LICSRequestOrder>();

            table.Load(reader);
            foreach (DataRow row in table.AsEnumerable())
            {
                LICSRequestOrder orderItem = new LICSRequestOrder();
                PropertyInfo propertyInfo = null;
                try
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                       // _loger.Log("LICSRequestOrderTest");
                     //   var res = row[column.ColumnName];
                        propertyInfo = orderItem.GetType().GetProperty(column.ColumnName);
                        if (propertyInfo == null)
                        {
                           // _loger.Log("LICSRequestOrder");
                            _loger.Log(propertyInfo, "propertyInfo");
                            _loger.Log(string.Format("{0} = {1}", column.ColumnName, Convert.ToString(row[column.ColumnName])));
                            continue;
                        }
                      //  _loger.Log(row[column.ColumnName], "row[column.ColumnName]");
                        bool columnValueEqualsNull = row[column.ColumnName].GetType().Equals(DBNull.Value.GetType());
                        //_loger.Log(columnValueEqualsNull, "columnValueEqualsNull");
                        if (columnValueEqualsNull)
                        {
                            _loger.Log(string.Format("{0} = {1}", column.ColumnName, "Is DB Null"));
                            propertyInfo.SetValue(orderItem, Convert.ChangeType(GetDefaultValue(propertyInfo.PropertyType), propertyInfo.PropertyType), null);
                        }
                        else
                        {
                          //  _loger.Log(string.Format("{0} = {1}", column.ColumnName, Convert.ToString(row[column.ColumnName])));
                            propertyInfo.SetValue(orderItem, Convert.ChangeType(Convert.ToString(row[column.ColumnName]).Trim(), propertyInfo.PropertyType), null);
                        }


                    }
                    orderItem.Recipient = GetOrderRecipient(connection, orderItem.OrderNo);
                    orderItem.Positions = GetRequestOrderPositions(connection, orderItem.OrderNo);
                    if(orderItem.Positions.Length>0)
                    {
                        orders.Add(orderItem);
                    }
                    else
                    {
                        string errorDescription = "There aren't positions in Order number " + orderItem.OrderNo;
                        _loger.Log(errorDescription);
                        MailLogger.Instance.LogMail(new ContentDescription(errorDescription));
                    }
                    
                }
                catch(FormatException formatEx)
                {
                    _loger.Log("FormatException while reflect DataColumn values using Reflection in LoadOrders");
                    _loger.Log(formatEx);
                    string mailMessage = string.Format("There was FormatException with Order number = {0}, problem field name = {1}", orderItem.OrderNo, propertyInfo.Name);
                    MailLogger.Instance.LogMail(new ContentDescription(mailMessage, formatEx.Message));
                }
                catch (Exception ex)
                {
                    _loger.Log("Exception while reflect DataColumn values using Reflection in LoadOrders");
                    _loger.Log(ex);
                }
            }
            return orders.ToArray();
        }

        private LICSRequestOrderPosition[] GetRequestOrderPositions(SqlConnection connection, string orderId)
        {
            List<LICSRequestOrderPosition> orderPositions = new List<LICSRequestOrderPosition>();

            string getOrderPositionsQuery = string.Format(_pluginSettings.OrderPositionsQuery, orderId);
            _loger.Log(string.Format("getOrderPositionsQuery reques sql {0}", getOrderPositionsQuery));
            SqlCommand cmd = new SqlCommand(getOrderPositionsQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            foreach (DataRow row in table.AsEnumerable())
            {

                try
                {
                    LICSRequestOrderPosition orderPosition = new LICSRequestOrderPosition();

                    foreach (DataColumn column in row.Table.Columns)
                    {
                        var res = row[column.ColumnName];
                      //  _loger.Log(string.Format("column.ColumnName ={0} value = {1}", (string)column.ColumnName, res.ToString()));
                        PropertyInfo propertyInfo = orderPosition.GetType().GetProperty(column.ColumnName);
                        if (propertyInfo == null)
                        {
                            _loger.Log("LICSRequestOrderPosition");
                            _loger.Log(string.Format("{0} = {1}", column.ColumnName, (string)res));
                            _loger.Log(propertyInfo, "propertyInfo");
                            continue;
                        }

                        if (row[column.ColumnName].GetType().Equals(DBNull.Value.GetType()))
                        {
                            propertyInfo.SetValue(orderPosition, Convert.ChangeType(GetDefaultValue(propertyInfo.PropertyType), propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(orderPosition, Convert.ChangeType(Convert.ToString(row[column.ColumnName]).Trim(), propertyInfo.PropertyType), null);
                        }
                    }
                    orderPositions.Add(orderPosition);
                }
                catch (Exception ex)
                {
                    _loger.Log("Exception while reflect DataColumn values using Reflection in GetOrderPositions");
                    _loger.Log(ex);
                }

            }
            return orderPositions.ToArray();
        }

        private LICSRequestOrderRecipient GetOrderRecipient(SqlConnection connection, string referenceNo)
        {
            string getOrderRecipientQuery = string.Format(_pluginSettings.OrderRecipientQuery, referenceNo);
            _loger.Log(string.Format("OrderRecipient reques sql {0}", getOrderRecipientQuery));
            SqlCommand cmd = new SqlCommand(getOrderRecipientQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();

            table.Load(reader);

            LICSRequestOrderRecipient orderRecipient = new LICSRequestOrderRecipient();
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        var res = row[column.ColumnName];
                        PropertyInfo propertyInfo = orderRecipient.GetType().GetProperty(column.ColumnName);
                        if (propertyInfo == null)
                        {
                            _loger.Log("LICSRequestOrderRecipient");
                            _loger.Log(string.Format("{0} = {1}", column.ColumnName, Convert.ToString(res)));
                            _loger.Log(propertyInfo, "propertyInfo");
                            continue;
                        }


                        if (row[column.ColumnName].GetType().Equals(DBNull.Value.GetType()))
                        {
                            propertyInfo.SetValue(orderRecipient, Convert.ChangeType(GetDefaultValue(propertyInfo.PropertyType), propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(orderRecipient, Convert.ChangeType(Convert.ToString(row[column.ColumnName]).Trim(), propertyInfo.PropertyType), null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _loger.Log("Exception while reflect DataColumn values using Reflection in GetOrderRecipient");
                    _loger.Log(ex);
                }
            }
            return orderRecipient;
        }
    }
}
