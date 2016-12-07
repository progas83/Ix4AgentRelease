using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public class InventurenDataMapper : IDataMapper
    {
        private Dictionary<string, string> _xmlElementNameTableColumnName;

        public InventurenDataMapper()
        {
            TableName = "Inventuren";
            _xmlElementNameTableColumnName = new Dictionary<string, string>();
            _xmlElementNameTableColumnName.Add("MSGHeader_Inventurnummer", "Inventurnummer");
            _xmlElementNameTableColumnName.Add("MSGHeader_Mandant", "Mandant");
            _xmlElementNameTableColumnName.Add("MSGHeader_Bezeichnung", "Bezeichnung");
            _xmlElementNameTableColumnName.Add("MSGHeader_Inventurstart", "Inventurstart");
            _xmlElementNameTableColumnName.Add("MSGHeader_Inventurende", "Inventurende");
            _xmlElementNameTableColumnName.Add("MSGHeader_GenDate", "GenDate");
            _xmlElementNameTableColumnName.Add("MSGHeader_GenUser", "GenUser");
            _xmlElementNameTableColumnName.Add("MSGHeader_ModDate", "ModDate");
            _xmlElementNameTableColumnName.Add("MSGHeader_ModUser", "ModUser");
            _xmlElementNameTableColumnName.Add("MSGHeader_Type", "Type");
            _xmlElementNameTableColumnName.Add("MSGHeader_Status", "Status");
        }
        private string _tableName;
        public string TableName
        {
            get
            {
                return _tableName;
            }

            private set { _tableName = value; }
        }

        public KeyValuePair<string, string> MapToTableField(XElement dataXmlElement)
        {
            string tableFieldName = _xmlElementNameTableColumnName[dataXmlElement.Name.LocalName];
            string datValue = dataXmlElement.Value;
            return new KeyValuePair<string, string>(tableFieldName, datValue);
        }

        public string GetTablesFieldsAndValuesQuery(IEnumerable<XElement>)
    }
}
