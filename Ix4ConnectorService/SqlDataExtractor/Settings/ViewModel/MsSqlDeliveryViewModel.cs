using Ix4Models;
using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataExtractor.Settings.ViewModel
{
    public class MsSqlDeliveryViewModel : BaseViewModel
    {
        MsSqlDeliveriesSettings _deliverySettings;
        public MsSqlDeliveryViewModel(MsSqlDeliveriesSettings deliverySettings)
        {
            _deliverySettings = deliverySettings;
        }

        public string DeliveriesQuery
        {
            get { return _deliverySettings.DeliveriesQuery; }
            set { _deliverySettings.DeliveriesQuery = value; OnPropertyChanged("DeliveriesQuery"); }
        }

        public string DeliveryPositionsQuery
        {
            get { return _deliverySettings.DeliveryPositionsQuery; }
            set { _deliverySettings.DeliveryPositionsQuery = value; OnPropertyChanged("DeliveryPositionsQuery "); }
        }
    }
}
