using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
   public class DataTableMapper : IDataMapper
    {
        public DataTableMapper(string tableName, IEnumerable<TableFieldMapInfo> fieldsMapInfo)
        {
            TableName = tableName;
            TableFieldsMapInfo = fieldsMapInfo;
        }
        private string _tableName;

        public string TableName
        {
            get { return _tableName; }
           private set { _tableName = value; }
        }

        private IEnumerable<TableFieldMapInfo> _tableFieldsMapInfo;

        private IEnumerable<TableFieldMapInfo> TableFieldsMapInfo
        {
            get { return _tableFieldsMapInfo; }
            set { _tableFieldsMapInfo = value; }
        }
        protected static Logger _loger = Logger.GetLogger();
        public IEnumerable<KeyValuePair<string,string>> GetTablesFieldsAndValuesQuery(IEnumerable<XElement> xmlValues)
        {
            Dictionary<string, string> dict1 = new Dictionary<string, string>();
            foreach(TableFieldMapInfo field in TableFieldsMapInfo)
            {
                string tableFieldValue = string.Empty;
                if (!string.IsNullOrEmpty(field.XmlElementName))
                {
                    XElement source = xmlValues.FirstOrDefault(x => x.Name.LocalName.Equals(field.XmlElementName));
                    if (source != null && !string.IsNullOrEmpty(source.Value))
                    {
                        tableFieldValue = source.Value;
                    }
                    else
                    {
                        if (field.AllowDBNull)
                        {
                            continue;
                        }
                        else
                        {
                            tableFieldValue = field.GetDefaultValue();
                        }
                    }
                }
                else
                {
                    _loger.Log(string.Format("There isn't mapped data for {0} field in {1} table",field.FieldName,this.TableName));
                    if(field.AllowDBNull)
                    {
                        continue;
                    }
                    else
                    {
                        tableFieldValue = field.GetDefaultValue();
                    }
                }

                dict1.Add(field.FieldName, tableFieldValue);
            }
       return dict1.ToArray();

        }
    }
}
