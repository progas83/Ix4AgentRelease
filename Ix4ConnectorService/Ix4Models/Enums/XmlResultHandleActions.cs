using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.Enums
{
    public enum XmlResultHandleActions
    {
        [XmlEnum(Name = "CopySourceTo")]
        CopySourceTo = 0,
        [XmlEnum(Name = "RemoveSource")]
        RemoveSource = 1,
        [XmlEnum(Name = "ReplaceSourceTo")]
        ReplaceSourceTo = 2
    }
}
