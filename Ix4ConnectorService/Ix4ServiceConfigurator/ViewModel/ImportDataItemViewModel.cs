using CompositionHelper;
using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4Models.SettingsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ix4ServiceConfigurator.ViewModel
{
   public class ImportDataItemViewModel : BaseViewModel
    {
        
        private CustomerDataComposition _compositor;
        public ImportDataItemViewModel(BaseLicsRequestSettings itemSettings)
        {
            BaseSettings = itemSettings;
            _compositor = new CustomerDataComposition(XmlConfigurationManager.Instance.GetCustomerInformation().ImportDataSettings.ArticleSettings);
        }

        private void InitSettings(Ix4RequestProps itemName)
        {
            switch (itemName)
            {
                case Ix4RequestProps.Articles:
                    BaseSettings = XmlConfigurationManager.Instance.GetCustomerInformation().ImportDataSettings.ArticleSettings;
                    break;
                case Ix4RequestProps.Orders:
                    BaseSettings = XmlConfigurationManager.Instance.GetCustomerInformation().ImportDataSettings.OrderSettings;
                    break;
                case Ix4RequestProps.Deliveries:
                    BaseSettings = XmlConfigurationManager.Instance.GetCustomerInformation().ImportDataSettings.DeliverySettings;
                    break;
            }
        }

        public BaseLicsRequestSettings BaseSettings { get; set; }

        public bool IsActivated
        {
            get { return BaseSettings.IsActivate; }
            set { BaseSettings.IsActivate = value; }
        }

        public SchedulerSettings Scheduler
        {
            get { return BaseSettings.Scheduler; }
            set { BaseSettings.Scheduler = value; OnPropertyChanged("Scheduler"); }
        }


        public Ix4RequestProps Ix4PartItemName
        {
            get { return BaseSettings.SettingsName; }
        }

        public CustomDataSourceTypes SelectedDataSource
        {
            get { return BaseSettings.SourceDataType; }
            set
            {
                BaseSettings.SourceDataType = value;
                OnPropertyChanged("PluginControl");
            }
        }

        public UserControl PluginControl
        {
            get
            {
                return _compositor.GetDataSettingsControl(SelectedDataSource);
            }
        }


    }
}
