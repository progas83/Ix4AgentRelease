using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models
{
    public class DataSourceInfo
    {
        public DataSourceInfo(string propertyName, Type propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
        }

        public string PropertyName { get; private set; }
        public Type PropertyType { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", PropertyName, PropertyType.ToString());
        }
    }
}
