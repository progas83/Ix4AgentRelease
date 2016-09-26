using System;
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
    [XmlInclude(typeof(ArticleSettings)),XmlInclude(typeof(OrderSettings)),XmlInclude(typeof(DeliverySettings))]
  
    [Serializable]
    public class BaseLicsRequestSettings
    {
        public BaseLicsRequestSettings()
        {
            Scheduler = new SchedulerSettings();
        }


        private string _xmlSuccessFolder;

        public string SuccessFolder
        {
            get { return _xmlSuccessFolder; }
            set { _xmlSuccessFolder = value; }
        }

        private string _xmlFailureFolder;

        public string FailureFolder
        {
            get { return _xmlFailureFolder; }
            set { _xmlFailureFolder = value; }
        }

        private string _xmlItemSourceFolder;

        public string XmlItemSourceFolder
        {
            get { return _xmlItemSourceFolder; }
            set { _xmlItemSourceFolder = value; }
        }


        private string _activateActionOnFailure;

        public string ActivateActionOnFailure
        {
            get { return _activateActionOnFailure; }
            set { _activateActionOnFailure = value; }
        }

        private bool _activateActionOnSuccess;

        public bool ActivateActionOnSuccess
        {
            get { return _activateActionOnSuccess; }
            set { _activateActionOnSuccess = value; }
        }


        private string _actionOnFailure;

        public string ActionOnFailure
        {
            get { return _actionOnFailure; }
            set { _actionOnFailure = value; }
        }

        private string _actionOnSuccess;

        public string ActionOnSuccess
        {
            get { return _actionOnSuccess; }
            set { _actionOnSuccess = value; }
        }

        public bool IsActivate { get; set; }
        public CustomDataSourceTypes SourceDataType { get; set; }
        public SchedulerSettings Scheduler { get; set; }
        public Object DataSourceSettings { get; set; }
    }
    [Serializable]
    public class ArticleSettings : BaseLicsRequestSettings
    {
        public ArticleSettings() : base()
        {
           
        }

    }

    [Serializable]
    public class DeliverySettings : BaseLicsRequestSettings
    {
        public DeliverySettings() : base()
        {

        }
      
    }

    [Serializable]
    public class OrderSettings : BaseLicsRequestSettings
    {
        public OrderSettings() :base()
        {

        }
       
    }
}
