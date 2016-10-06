﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class ImportDataSettings
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

    }

    [XmlInclude(typeof(ArticleSettings)), XmlInclude(typeof(OrderSettings)), XmlInclude(typeof(DeliverySettings))]
    [Serializable]
    public class BaseLicsRequestSettings
    {
        public BaseLicsRequestSettings()
        {
            Scheduler = new SchedulerSettings();
        }

        public virtual Ix4RequestProps SettingsName { get; }
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
                return Scheduler.StartTime.Hour <= DateTime.Now.Hour &&
                    Scheduler.StartTime.Minute <= DateTime.Now.Minute &&
                     DateTime.Now.Hour <= Scheduler.EndTime.Hour &&
                     DateTime.Now.Minute <= Scheduler.EndTime.Minute;
            }
        }
    }
    [Serializable]
    public class ArticleSettings : BaseLicsRequestSettings
    {
        public override Ix4RequestProps SettingsName
        {
            get
            {
                return Ix4RequestProps.Articles;
            }
        }
        public ArticleSettings() : base()
        {

        }
    }

    [Serializable]
    public class DeliverySettings : BaseLicsRequestSettings
    {
        public override Ix4RequestProps SettingsName
        {
            get
            {
                return Ix4RequestProps.Deliveries;
            }
        }
        public DeliverySettings() : base()
        {

        }

    }

    [Serializable]
    public class OrderSettings : BaseLicsRequestSettings
    {
        public override Ix4RequestProps SettingsName
        {
            get
            {
                return Ix4RequestProps.Orders;
            }
        }
        public OrderSettings() : base()
        {

        }

    }
}
