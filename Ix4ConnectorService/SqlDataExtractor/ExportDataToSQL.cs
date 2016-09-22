using Ix4Models.Interfaces;
using Ix4Models.Schema.WV;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SqlDataExtractor
{
    class ExportDataToSQL : SqlTableWorker
    {
      //  private string _dbConnection = @"Data Source=192.168.50.3\sql,1433;Network Library=DBMSSOCN;Initial Catalog=InterfaceDilosLMS; User ID=sa;Password=sa";
        //    private string _dbConnectionlms10dat = @"Data Source=192.168.50.3\sql,1433;Network Library=DBMSSOCN;Initial Catalog=lms10dat; User ID=sa;Password=sa";
            private string _dbConnection = @"Data Source =DESKTOP-PC\SQLEXPRESS2012;Initial Catalog = InterfaceDilosLMS;Integrated Security=SSPI";

        public ExportDataToSQL(IPluginSettings pluginSettings) : base(pluginSettings)
        {

        }

       // int itemsCount = 0;

        internal T SaveDataToTable<T>(XmlNode nodeToSave) where T:MSG
        {
            MSG red = null;
                    try
                    {
                        XmlSerializer sr = new XmlSerializer(typeof(MSG));
                        TextReader tr = new StringReader(nodeToSave.OuterXml);
                        red = (MSG)sr.Deserialize(tr);

                        InsertIntoTable(red);
                        //itemsCount++;
                        //_loger.Log("Succefully exported MSG");
                        //_loger.Log(nodeToSave.OuterXml);
                    }
                    catch (Exception ex)
                    {
                        _loger.Log(ex);
                    }
            return (T)red;
        }


        internal void SaveDataToTable(XmlNode exportData)
        {
            XmlNodeList msgNodes = exportData.LastChild.LastChild.SelectNodes("MSG");
            if (msgNodes.Count > 0)
            {
                _loger.Log(string.Format("Got xmlNodes count = {0}", msgNodes.Count));
                XmlSerializer sr = new XmlSerializer(typeof(MSG));
                foreach (XmlNode node in msgNodes)
                {
                    try
                    {
                        TextReader tr = new StringReader(node.OuterXml);
                        MSG red = (MSG)sr.Deserialize(tr);

                        InsertIntoTable(red);
                        //itemsCount++;
                        _loger.Log("Succefully exported MSG");
                        _loger.Log(node.OuterXml);
                    }
                    catch (Exception ex)
                    {
                        _loger.Log(ex);
                    }
                }
            }
        }

        private void InsertIntoTable(MSG message)
        {
            try
            {
                using (var connection = new SqlConnection(_dbConnection))
                {
                    if(message.HeaderId <= 0)
                    {
                        message.HeaderId = InsertHeader(message, connection);
                    }
                    if (message.HeaderId > 0)
                    {
                        SqlCommand cmd = InsertPosition("MsgPos", message);
                        cmd.Connection = connection;
                        connection.Open();
                        _loger.Log("Connection opened to InterfaceDilosLMS for InsertPosition");
                        var result = cmd.ExecuteNonQuery();
                        _loger.Log(string.Format("SQL commang affected {0} rows", result));
                        if (connection.State == System.Data.ConnectionState.Open)
                            connection.Close();
                        message.Saved = true;
                    }
                    else
                    {
                        _loger.Log("There was no correct header number for MSG.WAKopfID = " + message.WAKopfID);
                    }
                }
            }
            catch(Exception ex)
            {
                _loger.Log(ex);
            }
          
        }

        private SqlCommand InsertPosition(string tabelName, MSG message)
        {
            SqlCommand sqlCommand = new SqlCommand();
            StringBuilder tableColumnsNames = new StringBuilder();
            StringBuilder commandParametersNames = new StringBuilder();
            try
            {
                tableColumnsNames.Append("HeaderID");
                commandParametersNames.Append("@posHeaderID");
                sqlCommand.Parameters.AddWithValue("@posHeaderID", message.HeaderId);

                PropertyInfo[] posProperties = message.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MSGPosAttribute))).ToArray();

                for (int i = 0; i < posProperties.Length; i++)
                {
                    if (posProperties[i].GetValue(message) != null)
                    {
                        tableColumnsNames.Append(",");
                        tableColumnsNames.Append(posProperties[i].Name);

                        commandParametersNames.Append(",");
                        string posItem = string.Format("@pos{0}", posProperties[i].Name);
                        commandParametersNames.Append(posItem);
                        sqlCommand.Parameters.AddWithValue(posItem, posProperties[i].GetValue(message));
                    }
                }

                string commandText = string.Format(@"INSERT INTO {0} ({1}) VALUES({2})", tabelName, tableColumnsNames.ToString(), commandParametersNames.ToString());
                _loger.Log(string.Format("Insert position command = {0}", commandText));
                sqlCommand.CommandText = commandText;
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            return sqlCommand;
        }

        private int InsertHeader(MSG message, SqlConnection con)
        {
            int modified = -1;
            if (message.Type.Equals("GS"))
            {
                int existedHeaderID = FindExistedGSHeader(message, con);
                if (existedHeaderID != -1)
                {
                    modified = existedHeaderID;
                }
                else
                {
                    modified = InsertNewHeaderRecord(message, con);
                }

            }
            else
            {
                modified = InsertNewHeaderRecord(message, con);
            }
            return modified;
        }

        private int FindExistedGSHeader(MSG message, SqlConnection con)
        {
            int existedHeaderID = -1;
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT HeaderID FROM MsgPos WHERE HeaderID IN (SELECT ID FROM MsgHeader WHERE TYPE = 'GS')  AND WAKopfID = @pCurrentWAKopfID", con))
                {
                    cmd.Parameters.AddWithValue("@pCurrentWAKopfID", message.WAKopfID);
                    con.Open();
                    var existedItem = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);//.ExecuteScalar();
                    if (existedItem != null)
                    {
                        existedHeaderID = Convert.ToInt32(existedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
            return existedHeaderID;
        }

        private int InsertNewHeaderRecord(MSG message, SqlConnection con)
        {
            int modified = -1;

            using (SqlCommand cmd = new SqlCommand("INSERT INTO MsgHeader (Type,Status,[User], Created,LastUpdate,ErrorText) VALUES (@hType,@hStatus,@hUser,@hCreated,@hLastUpdate,@hErrorText);SELECT SCOPE_IDENTITY() AS LastItemID;", con))
            {
                cmd.Parameters.AddWithValue("@hType", message.Type);
                cmd.Parameters.AddWithValue("@hStatus", message.Status);
                cmd.Parameters.AddWithValue("@hUser", message.User);
                cmd.Parameters.AddWithValue("@hCreated", message.Created);
                cmd.Parameters.AddWithValue("@hLastUpdate", DateTime.Now);
                cmd.Parameters.AddWithValue("@hErrorText", string.IsNullOrEmpty(message.ErrorText) ? string.Empty : message.ErrorText);

                con.Open();
                try
                {

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        modified = Convert.ToInt32(dr["LastItemID"]);
                        _loger.Log(string.Format("New header was inserted to DB for WakopfID = {0}. As result got HeaderId = {1}", message.WAKopfID, modified));
                    }
                    else
                    {
                        _loger.Log("Cant get last inserted headerID");
                    }
                }
                catch (Exception ex)
                {
                    _loger.Log(string.Format("Could not create new record MsgHeader for ", message.WAKopfID));
                    _loger.Log(ex);

                }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                }

            }
            return modified;
        }

        private string GetTableName(string msgType)
        {
            string result = string.Empty;
            switch (msgType)
            {
                case "GR":
                    break;
                case "GP":
                    break;
                case "GS":
                    break;
                case "CA":
                    break;
                case "BO":
                    break;
                case "SA":
                    break;

                default:
                    break;
            }
            return result;
        }
        // „InterfaceDilosLMS“ – the tables „Msgheader“ and „Msgpos“.
        //         “GR” Goods Received
        //-          “GP” Goods Picked
        //-          “GS” Goods Shipped
        //-          “CA” Cancel Order
        //-          “BO” Stock corrections/adjustmets
        //-          “SA” Stock

    }
}
