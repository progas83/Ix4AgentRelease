using CompositionHelper;
using Ix4Models;
using Ix4Models.SettingsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ix4ServiceConfigurator.ViewModel
{
    public class LicsRequestItemViewModel : BaseViewModel
    {
       private CustomerDataComposition _compositor;
        public LicsRequestItemViewModel(string itemName)
        {
            _itemName = itemName;
            _compositor = new CustomerDataComposition(XmlConfigurationManager.Instance.GetCustomerInformation().PluginSettings);
        }

        private string _itemName;

        public string Ix4PartItemName
        {
            get { return _itemName; }
            set { _itemName = value; OnPropertyChanged("Ix4PartItemName"); }
        }


        private CustomDataSourceTypes _selectedDataSource;
        public CustomDataSourceTypes SelectedDataSource
        {
            get { return _selectedDataSource; }
            set
            {
                _selectedDataSource = value;
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
