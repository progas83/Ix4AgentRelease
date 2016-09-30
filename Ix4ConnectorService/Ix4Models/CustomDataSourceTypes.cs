using System.Xml.Serialization;

namespace Ix4Models
{

    public enum CustomDataSourceTypes
    {
        Unavailable = 0,
        [XmlEnum(Name = "Csv")]
        Csv = 1,
        [XmlEnum(Name = "MsSql")]
        MsSql = 2,
        [XmlEnum(Name = "Xml")]
        Xml = 3
    }
}
