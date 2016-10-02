using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
   public class MsSqlImportDataSettings
    {
        public MsSqlImportDataSettings()
        {
            ArticlesSettings = new MsSqlArticlesSettings();
            OrdersSettings = new MsSqlOrdersSettings();
            DeliveriesSettings = new MsSqlDeliveriesSettings();
        }
        public MsSqlArticlesSettings ArticlesSettings { get; set; }
        public MsSqlOrdersSettings OrdersSettings { get; set; }
        public MsSqlDeliveriesSettings DeliveriesSettings { get; set; }
    }


    [Serializable]
    public class MsSqlArticlesSettings : MsSqlSettings
    {
        public MsSqlArticlesSettings() : base()
        {

        }
        public string ArticlesQuery
        {
            get; set;
        }
    }

    [Serializable]
    public class MsSqlDeliveriesSettings : MsSqlSettings
    {
        public MsSqlDeliveriesSettings() : base()
        {

        }


        public string DeliveriesQuery
        {
            get; set;
        }

        public string DeliveryPositionsQuery
        {
            get; set;
        }
    }

    [Serializable]
    public class MsSqlOrdersSettings : MsSqlSettings
    {
        public MsSqlOrdersSettings() : base()
        {

        }

        public string OrdersQuery { get; set; }
        public string OrderRecipientQuery { get; set; }
        public string OrderPositionsQuery { get; set; }
    }

}
