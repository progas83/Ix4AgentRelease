using System;
using System.Xml.Serialization;

namespace Ix4Models
{
    public enum Ix4ImportDataTypes
    {
        [XmlEnum(Name = "Articles")]
        Articles =0,
        [XmlEnum(Name = "Orders")]
        Orders =1,
        [XmlEnum(Name = "Deliveries")]
        Deliveries =2
    }
}
