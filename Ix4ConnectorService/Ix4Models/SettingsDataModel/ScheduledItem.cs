using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
   [Serializable]
    public  class ScheduledItem
    {
        [XmlAttribute]
        public Ix4RequestProps scheduledProperty { get; set; }

        [XmlAttribute]
        public int secValue { get; set; }
        public ScheduledItem()
        {

        }
        public ScheduledItem(Ix4RequestProps key, int value)
        {
            this.scheduledProperty = key;
            this.secValue = value;
        }
    }
}
