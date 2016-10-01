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
            //  _itemName = itemName;
            //BaseSettings.
            // InitSettings(itemName);
            _compositor = new CustomerDataComposition(XmlConfigurationManager.Instance.GetCustomerInformation().ImportDataSettings.ArticleSettings);
            // Scheduler = BaseSettings.Scheduler;// new SchedulerSettings();
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
        // private bool  _isActivate;

        public bool IsActivated
        {
            get { return BaseSettings.IsActivate; }
            set { BaseSettings.IsActivate = value; }
        }

        private SchedulerSettings _scheduler;

        public SchedulerSettings Scheduler
        {
            get { return BaseSettings.Scheduler; }
            set { BaseSettings.Scheduler = value; OnPropertyChanged("Scheduler"); }
        }


        //  private Ix4RequestProps _itemName;

        public Ix4RequestProps Ix4PartItemName
        {
            get { return BaseSettings.SettingsName; }
            //set { _itemName = value; OnPropertyChanged("Ix4PartItemName"); }
        }


        //  private CustomDataSourceTypes _selectedDataSource;
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
