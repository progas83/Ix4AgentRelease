using Ix4Models;
using Ix4Models.Interfaces;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataExtractor
{
    class SqlTableArticleExplorer
    {
        private MsSqlPluginSettings _pluginSettings;
        public SqlTableArticleExplorer(IPluginSettings pluginSettings)
        {
            _pluginSettings = pluginSettings as MsSqlPluginSettings;
        }

        private string DbConnection
        {
            get
            {
                return _pluginSettings.DbSettings.UseSqlServerAuth ? string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWithServerAuth, _pluginSettings.DbSettings.ServerAdress,
                                                                                                         _pluginSettings.DbSettings.DataBaseName,
                                                                                                         _pluginSettings.DbSettings.DbUserName,
                                                                                                         _pluginSettings.DbSettings.Password) :
                                                                    string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _pluginSettings.DbSettings.ServerAdress,
                                                                                                         _pluginSettings.DbSettings.DataBaseName);

                //return   string.Format(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth, _pluginSettings.DbSettings.ServerAdress,
                //                                                                                    _pluginSettings.DbSettings.DataBaseName);

            }
        }
        private static Logger _loger = Logger.GetLogger();
        //    Logger _logger = Logger.GetLogger();
        public LICSRequestArticle[] GetArticles()
        {
            LICSRequestArticle[] articles = null;
            try
            {
                using (var connection = new SqlConnection(DbConnection))
                {
                    connection.Open();
                    var cmdText = _pluginSettings.ArticlesQuery;
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

        private object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        private LICSRequestArticle[] LoadArticles(IDataReader reader)
        {
            DataTable table = new DataTable();
            List<LICSRequestArticle> articles = new List<LICSRequestArticle>();

            table.Load(reader);
            foreach (DataRow row in table.AsEnumerable())
            {
                try
                {
                    LICSRequestArticle articleItem = new LICSRequestArticle();
                 //   articleItem.GetType().GetProperty(propertyName).SetValue.GetValue((car, null);

                    //  var r = table.AsEnumerable();
                    foreach (DataColumn column in  row.Table.Columns)
                    {
                        var res = row[column.ColumnName];
                        PropertyInfo propertyInfo = articleItem.GetType().GetProperty(column.ColumnName);
                        if (row[column.ColumnName].GetType().Equals(DBNull.Value.GetType()))
                        {
                            propertyInfo.SetValue(articleItem, Convert.ChangeType(GetDefaultValue(propertyInfo.PropertyType), propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(articleItem, Convert.ChangeType(row[column.ColumnName].ToString().Trim(), propertyInfo.PropertyType), null);
                        }
                      
                       
                    }
                    articles.Add(articleItem);
                    //double currentArticleGroupFactor = 0;
                    //double currentWeight = 0;
                    //int identityNo = 0;
                    //if (!string.IsNullOrEmpty(Convert.ToString(row["ArticleGroupFactor"])))
                    //{
                    //    currentArticleGroupFactor = double.Parse(Convert.ToString(row["ArticleGroupFactor"]), CultureInfo.InvariantCulture);
                    //}

                    //if (!string.IsNullOrEmpty(Convert.ToString(row["Weight"])))
                    //{
                    //    currentWeight = double.Parse(Convert.ToString(row["Weight"]), CultureInfo.InvariantCulture);
                    //}
                    //if (!string.IsNullOrEmpty(Convert.ToString(row["IdentityNo"])))
                    //{
                    //   identityNo = Int32.Parse(Convert.ToString(row["IdentityNo"]), CultureInfo.InvariantCulture);
                    //}

                    //articles.Add(new LICSRequestArticle
                    //{
                    //    ArticleNo = (row["ArticleNo"] ?? string.Empty).ToString().Trim(),
                    //    ArticleNo2 = (row["ArticleNo2"] ?? string.Empty).ToString().Trim(),
                    //    ArticleDescription = (row["ArticleDescription"] ?? string.Empty).ToString().Trim(),
                    //    ArticleDescription2 = (row["ArticleDescription2"] ?? string.Empty).ToString().Trim(),
                    //    //  IdentityNo = identityNo,
                    //    QuantityUnit = (row["QuantityUnit"] ?? string.Empty).ToString().Trim(),
                    //    // EAN = (row["EAN"] ?? string.Empty).ToString(),
                    //    //  ProductCode = (row["ProductCode"] ?? string.Empty).ToString(),
                    //    //  ArticleGroup = (row["ArticleGroup"] ?? string.Empty).ToString(),
                    //    //  ArticleGroupFactor = currentArticleGroupFactor,
                    //    //   Weight = currentWeight
                    //});

                }
                catch (Exception ex)
                {
                    _loger.Log("Exception while reflect DataColumn values using Reflection");
                    _loger.Log(ex);
                }
            }
            return articles.Where(i => !string.IsNullOrEmpty(i.ArticleNo)).ToArray();
        }
    }
}
