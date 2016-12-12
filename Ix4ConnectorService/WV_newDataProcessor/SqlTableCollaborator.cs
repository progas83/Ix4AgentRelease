using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public int SaveData(IEnumerable<XElement> exportedDataElements,string tableName)
        {
            int recordNumber = -1;
            IDataMapper dataMapper = _dataToTableMappers.FirstOrDefault(mapper=>mapper.TableName.Equals(tableName));
            if(dataMapper==null)
            {
                _loger.Log(string.Format("There is no mapper for {0} table",tableName));
                return -1;
            }
            
            StringBuilder colums= new StringBuilder();
            StringBuilder values = new StringBuilder();
            IEnumerable<KeyValuePair<string, string>> mappedData = dataMapper.GetTablesFieldsAndValuesQuery(exportedDataElements);
            foreach(KeyValuePair<string,string> mappedFieldValue in mappedData)
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
            catch(Exception ex)
            {
                _loger.Log(ex);
            }
           
            return recordNumber;
        }

        public T GetData<T>(string fromName, Func<DataTable, T> predicate, string cmdText = "") where T : new()
        {
            T result = (T) GetDefaultValue(typeof(T));
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
            catch(Exception ex)
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
    }
}
