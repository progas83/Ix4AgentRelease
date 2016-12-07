using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV_newDataProcessor
{
    public class TableFieldMapInfo
    {
        private string _fieldName;

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        private bool _allowDBNull;

        public bool AllowDBNull
        {
            get { return _allowDBNull; }
            set { _allowDBNull = value; }
        }


        private string _fieldTypeFullName;

        public string FieldTypeFullName
        {
            get { return _fieldTypeFullName; }
            set { _fieldTypeFullName = value; }
        }

        private string _xmlElementName;

        public string XmlElementName
        {
            get { return _xmlElementName; }
            set { _xmlElementName = value; }
        }
    }
}
