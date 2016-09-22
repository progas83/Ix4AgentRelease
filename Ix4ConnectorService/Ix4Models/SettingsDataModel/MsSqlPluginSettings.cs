using Ix4Models.Interfaces;
using System;
using System.Text;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class MsSqlPluginSettings : IPluginSettings
    {
        public MsSqlPluginSettings()
        {
            DbSettings = new MsSqlSettings();
        }
        public MsSqlSettings DbSettings { get; set; }



        public bool IsActivated
        {
            get; set;
        }

        public bool CheckArticles
        {
            get; set;
        }

        public string ArticlesQuery
        {
            get; set;
        }

        public bool CheckDeliveries
        {
            get; set;
        }

        public string DeliveriesQuery
        {
            get; set;
        }

        public string DeliveryPositionsQuery
        {
            get; set;
        }

        [XmlIgnore]
        public CustomDataSourceTypes PluginType
        {
            get
            {
                return CustomDataSourceTypes.MsSql;
            }
        }

        public bool CheckOrders
        {
            get; set;
        }
        public string OrderRecipientQuery { get; set; }
        public string OrderPositionsQuery { get; set; }
        public string OrdersQuery { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("IsActivated = {0}", IsActivated));
            sb.Append(string.Format("CheckArticles = {0}", CheckArticles));
            sb.Append(string.Format("ArticlesQuery = {0}", ArticlesQuery));
            sb.Append(string.Format(" CheckDeliveries= {0}", CheckDeliveries)); sb.Append(string.Format("DeliveriesQuery = {0}", DeliveriesQuery));
            sb.Append(string.Format("DelivarePositionsQuery = {0}", DeliveryPositionsQuery));
            sb.Append(string.Format("CheckOrders = {0}", CheckOrders));
            sb.Append(string.Format("DbSettings  = {0}", DbSettings.ToString()));


            return sb.ToString();// base.ToString();
        }
    }
}
