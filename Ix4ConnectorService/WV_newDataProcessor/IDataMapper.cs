using System.Collections.Generic;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public interface IDataMapper
    {
        string TableName { get; }
        IEnumerable<KeyValuePair<string, string>> GetTablesFieldsAndValuesQuery(IEnumerable<XElement> xmlValues);
    }
}
