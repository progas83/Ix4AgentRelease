using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public enum TimeSign
    {
        [XmlEnum(Name ="Unassigned")]
        Unassigned = 0,
        [XmlEnum(Name= "Min")]
        Min = 1,
        [XmlEnum(Name = "Hour")]
        Hour = 60,
        [XmlEnum(Name = "Day")]
        Day = 1440
    }
}
