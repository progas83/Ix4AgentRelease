using SimplestLogger;
using System;
using System.Collections.Generic;
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
            IDataMapper dataMapper = _dataToTableMappers.FirstOrDefault(mapper=>mapper.TableName.Equals(tableName));
            int recordNumber = -1;
            StringBuilder colums= new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach(XElement element in exportedDataElements)
            {
                KeyValuePair<string,string> mappedData = dataMapper.MapToTableField(element);
                colums.Append(string.Format("\"{0}\",", mappedData.Key));
                values.Append(string.Format("\"{0}\",", mappedData.Value));
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
                        dr.Read();
                        recordNumber = Convert.ToInt32(dr["LastItemID"]);
                        _loger.Log(string.Format("New record was insert into {0} with index = {1}", tableName, recordNumber));
                    }
                    else
                    {
                        _loger.Log("Cant get last inserted headerID");
                    }
                    var resQuery = sqlCommand.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {

            }
           
            return recordNumber;
        }
    }
}
