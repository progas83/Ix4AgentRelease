using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SimplestLogger;

namespace WV_newDataProcessor
{
    public class SqlTableCollaborator : IDataTargetCollaborator
    {
        //  private string _dbConnection = @"Data Source =DESKTOP-PC\SQLEXPRESS2012;Initial Catalog = Inventurdaten;Integrated Security=SSPI";
        private string _dbConnectionString;
        private IEnumerable<IDataMapper> _dataToTableMappers;
        public SqlTableCollaborator(string dbConnectionString, IEnumerable<IDataMapper> dataToTableMappers)
        {
            _dbConnectionString = dbConnectionString;
            _dataToTableMappers = dataToTableMappers;
        }
        protected static Logger _loger = Logger.GetLogger();

        public int SaveData(IEnumerable<XElement> exportedDataElements, string tableName)
        {
            int recordNumber = -1;
            IDataMapper dataMapper = _dataToTableMappers.FirstOrDefault(mapper => mapper.TableName.Equals(tableName));
            if (dataMapper == null)
            {
                _loger.Log(string.Format("There is no mapper for {0} table", tableName));
                return -1;
            }

            StringBuilder colums = new StringBuilder();
            StringBuilder values = new StringBuilder();
            IEnumerable<KeyValuePair<string, string>> mappedData = dataMapper.GetTablesFieldsAndValuesQuery(exportedDataElements);
            foreach (KeyValuePair<string, string> mappedFieldValue in mappedData)
            {
                colums.Append(string.Format("[{0}],", mappedFieldValue.Key));
                values.Append(string.Format("'{0}',", mappedFieldValue.Value));
            }

            try
            {
                using (var connection = new SqlConnection(_dbConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = string.Format("INSERT INTO {0} ({1})  VALUES ({2});SELECT SCOPE_IDENTITY() AS LastItemID;", tableName, colums.ToString().TrimEnd(','), values.ToString().TrimEnd(','));
                    sqlCommand.Connection = connection;
                    connection.Open();
                    SqlDataReader dr = sqlCommand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        bool resd = dr.Read();
                        recordNumber = dr["LastItemID"].GetType().Equals(typeof(DBNull)) ? 0 : Convert.ToInt32(dr["LastItemID"]);// Convert.ToInt32("-1");// dr.GetSchemaTable().Columns["LastItemID"]);
                        _loger.Log(string.Format("New record was insert into {0} with index = {1}", tableName, recordNumber));
                    }
                    else
                    {
                        _loger.Log("Cant get last inserted record ID");
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

            return recordNumber;
        }

        public T GetData<T>(string fromName, Func<DataTable, T> predicate, string cmdText = "") where T : new()
        {
            T result = (T)GetDefaultValue(typeof(T));
            try
            {
                using (var dbConnection = new SqlConnection(_dbConnectionString))
                {
                    string commandText = string.IsNullOrEmpty(cmdText) ? string.Format("Select * from {0}", fromName) : cmdText;
                    SqlCommand cmd = new SqlCommand(commandText, dbConnection);
                    dbConnection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    result = predicate(dt);
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

            return result;
        }

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        private readonly string msgHeaderTableName = "MsgHeader";
        private readonly string msgPosTableName = "MsgPos";


        private int SaveInSpecificTable(SqlCommand sqlCommand, XElement exportedDataElement, string tableName)
        {
            int insertItemNumber = -1;
            IDataMapper dataMapper = _dataToTableMappers.FirstOrDefault(mapper => mapper.TableName.Equals(tableName));
            if (dataMapper == null)
            {
                throw new Exception($"Data mapper for table {tableName} wasn't initialized");
            }

            StringBuilder colums = new StringBuilder();
            StringBuilder values = new StringBuilder();
            IEnumerable<KeyValuePair<string, string>> mappedData = dataMapper.GetTablesFieldsAndValuesQuery(exportedDataElement.Descendants().Where(x => x.Name.LocalName.StartsWith(tableName, true, CultureInfo.CurrentCulture)));

            foreach (KeyValuePair<string, string> mappedFieldValue in mappedData)
            {
                colums.Append(string.Format("[{0}],", mappedFieldValue.Key));
                values.Append(string.Format("'{0}',", mappedFieldValue.Value));
            }

            sqlCommand.CommandText = string.Format("INSERT INTO {0} ({1})  VALUES ({2});SELECT SCOPE_IDENTITY() AS LastItemID;", tableName, colums.ToString().TrimEnd(','), values.ToString().TrimEnd(','));

            SqlDataReader dr = sqlCommand.ExecuteReader();
            dr.Read();
            insertItemNumber = Convert.ToInt32(dr["LastItemID"]); // Convert.ToInt32("-1");// dr.GetSchemaTable().Columns["LastItemID"]);
            _loger.Log($"New record was insert into {tableName} with index = {insertItemNumber}");
            if (insertItemNumber <= 0)
            {
                throw new Exception($"Error record to table {tableName}");
            }
            return insertItemNumber;
        }
        public bool SaveExportDataTransaction(XElement exportedDataElement, bool needFindExistedMsgHeader = false)
        {
            bool transactionResult = false;

            try
            {
                using (var connection = new SqlConnection(_dbConnectionString))
                {
                    connection.Open();
                    SqlCommand sqlCommand = connection.CreateCommand();
                    SqlTransaction sqlTransaction = connection.BeginTransaction();
                    sqlCommand.Connection = connection;
                    sqlCommand.Transaction = sqlTransaction;

                    try
                    {
                        int headerRecordId = this.SaveInSpecificTable(sqlCommand, exportedDataElement, msgHeaderTableName);

                        XElement headerIdElement = new XElement("MSGPos_HeaderID");
                        headerIdElement.Value = headerRecordId.ToString();
                        exportedDataElement.Add(headerIdElement);
                        this.SaveInSpecificTable(sqlCommand, exportedDataElement, msgPosTableName);
                        sqlTransaction.Commit();
                        transactionResult = true;
                    }
                    catch (Exception ex)
                    {
                        _loger.Log("Transaction error");
                        sqlTransaction.Rollback();
                        _loger.Log(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

            return transactionResult;
        }
    }
}
