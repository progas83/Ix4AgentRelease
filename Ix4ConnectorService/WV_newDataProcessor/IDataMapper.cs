using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
  public  interface IDataMapper
    {
        string TableName { get; }
        KeyValuePair<string,string> MapToTableField(XElement dataXmlElement);
    }
}
