using Ix4Models.Interfaces;
using System;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class CsvPluginSettings : IPluginSettings
    {
        public bool CheckArticles
        {
            get; set;
        }

        public bool CheckDeliveries
        {
            get; set;
        }

        public bool CheckOrders
        {
            get; set;
        }

        public bool IsActivated
        {
            get; set;
        }

        [XmlIgnore]
        public CustomDataSourceTypes PluginType
        {
            get
            {
                return CustomDataSourceTypes.Csv;
            }
        }
    }
}
