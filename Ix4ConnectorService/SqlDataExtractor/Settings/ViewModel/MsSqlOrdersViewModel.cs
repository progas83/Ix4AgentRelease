using Ix4Models;
using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataExtractor.Settings.ViewModel
{
    public class MsSqlOrdersViewModel : BaseViewModel
    {
        MsSqlOrdersSettings _ordersSettings;
        public MsSqlOrdersViewModel(MsSqlOrdersSettings ordersSettings)
        {
            _ordersSettings = ordersSettings;
        }

        public string OrdersQuery
        {
            get { return _ordersSettings.OrdersQuery; }
            set { _ordersSettings.OrdersQuery = value; OnPropertyChanged("OrdersQuery"); }
        }

        public string OrderRecipientQuery
        {
            get { return _ordersSettings.OrderRecipientQuery; }
            set { _ordersSettings.OrderRecipientQuery = value; OnPropertyChanged("OrderRecipientQuery"); }
        }

        public string OrderPositionsQuery
        {
            get { return _ordersSettings.OrderPositionsQuery; }
            set { _ordersSettings.OrderPositionsQuery = value; OnPropertyChanged("OrderPositionsQuery"); }
        }
    }
}
