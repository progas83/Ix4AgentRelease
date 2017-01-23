using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class ImportDataSettings : ICryptor
    {
        public ImportDataSettings()
        {
            ArticleSettings = new ArticleSettings();
            OrderSettings = new OrderSettings();
            DeliverySettings = new DeliverySettings();
        }
        public BaseLicsRequestSettings ArticleSettings { get; set; }
        public BaseLicsRequestSettings OrderSettings { get; set; }
        public BaseLicsRequestSettings DeliverySettings { get; set; }

        public void Decrypt()
        {
            if (ArticleSettings.DataSourceSettings != null)
            {
                ArticleSettings.DataSourceSettings.Decrypt();
            }
            if (OrderSettings.DataSourceSettings != null)
            {
                OrderSettings.DataSourceSettings.Decrypt();
            }
            if (DeliverySettings.DataSourceSettings != null)
            {
                DeliverySettings.DataSourceSettings.Decrypt();
            }

        }

        public void Encrypt()
        {
            if (ArticleSettings.DataSourceSettings != null)
            {
                ArticleSettings.DataSourceSettings.Encrypt();
            }
            if (OrderSettings.DataSourceSettings != null)
            {
                OrderSettings.DataSourceSettings.Encrypt();
            }
            if (DeliverySettings.DataSourceSettings != null)
            {
                DeliverySettings.DataSourceSettings.Encrypt();
            }
        }
    }

    [XmlInclude(typeof(ArticleSettings)), XmlInclude(typeof(OrderSettings)), XmlInclude(typeof(DeliverySettings))]
    [Serializable]
    public class BaseLicsRequestSettings
    {
        public BaseLicsRequestSettings()
        {
            Scheduler = new SchedulerSettings();
        }

        public virtual Ix4ImportDataTypes SettingsName { get; }
        public bool IsActivate { get; set; }

        public bool IncludeArticlesToRequest { get; set; }

        public CustomDataSourceTypes SourceDataType { get; set; }

        public SchedulerSettings Scheduler { get; set; }

        public BaseDataSourceSettings DataSourceSettings { get; set; }

        [XmlIgnore]
        public bool IsNowWorkingTime
        {
            get
            {
                return Scheduler.StartTime.TimeOfDay <= DateTime.Now.TimeOfDay &&
                    DateTime.Now.TimeOfDay <= Scheduler.EndTime.TimeOfDay;
            }
        }
    }
    [Serializable]
    public class ArticleSettings : BaseLicsRequestSettings
    {
        public override Ix4ImportDataTypes SettingsName
        {
            get
            {
                return Ix4ImportDataTypes.Articles;
            }
        }
        public ArticleSettings() : base()
        {

        }
    }

    [Serializable]
    public class DeliverySettings : BaseLicsRequestSettings
    {
        public override Ix4ImportDataTypes SettingsName
        {
            get
            {
                return Ix4ImportDataTypes.Deliveries;
            }
        }
        public DeliverySettings() : base()
        {

        }

    }

    [Serializable]
    public class OrderSettings : BaseLicsRequestSettings
    {
        public override Ix4ImportDataTypes SettingsName
        {
            get
            {
                return Ix4ImportDataTypes.Orders;
            }
        }
        public OrderSettings() : base()
        {

        }

    }
}
