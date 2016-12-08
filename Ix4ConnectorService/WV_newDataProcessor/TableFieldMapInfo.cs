using System;

namespace WV_newDataProcessor
{
    public class TableFieldMapInfo
    {
        public TableFieldMapInfo()
        {

        }
        public TableFieldMapInfo(string fieldName, string fieldType, string xmlElementName, bool allowDBNull = false)
        {
            FieldName = fieldName;
            FieldTypeFullName = fieldType;
            XmlElementName = xmlElementName;
            AllowDBNull = allowDBNull;
        }
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

        public string GetDefaultValue()
        {
            Type fieldType = Type.GetType(this._fieldTypeFullName);

            if (fieldType.IsValueType)
            {
                if (fieldType.Equals(typeof(DateTime)))
                {
                    return new DateTime(1970, 1, 1).ToString();
                }
                else
                {
                    return Activator.CreateInstance(fieldType).ToString();
                }
            }


            return string.Empty;
        }
    }
}
