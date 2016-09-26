using Ix4ServiceConfigurator.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Controls;
using CompositionHelper;
using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4Models.SettingsManager;
using System.Collections.ObjectModel;
using System.Security;
using System.Runtime.InteropServices;

namespace Ix4ServiceConfigurator.ViewModel
{

    internal class CustomerInfoViewModel : BaseViewModel, ICommand, IDisposable
    {
      //  CustomerDataComposition _compositor;
        private CustomerInfoView _view;
        public CustomerInfoViewModel()
        {
            _view = new CustomerInfoView();
            _view.DataContext = this;
         //   _view.Closing += OnCustomerViewClosing;

            Customer = XmlConfigurationManager.Instance.GetCustomerInformation();
            _view.UIUserInfo.UIPwdBox.Password = Customer.Password;

          //  _compositor = new CustomerDataComposition(Customer.PluginSettings);
        }

        //private CustomDataSourceTypes _selectedDataSource;
        //public CustomDataSourceTypes SelectedDataSource
        //{
        //    get { return _selectedDataSource; }
        //    set
        //    {
        //        _selectedDataSource = value;
        //        OnPropertyChanged("PluginControl");
        //    }
        //}


        //private Ix4RequestProps _TabSelectedItem;

        //public Ix4RequestProps TabSelectedItem
        //{
        //    get { return _TabSelectedItem; }
        //    set { _TabSelectedItem = value; OnPropertyChanged("TabSelectedItem"); }
        //}

        //private int _TabSelectedIndex;

        //public int TabSelectedIndex
        //{
        //    get { return _TabSelectedIndex; }
        //    set { _TabSelectedIndex = value; OnPropertyChanged("TabSelectedIndex"); }
        //}

        public UserControl SendLicsRequestSettings { get; set; }

        //public UserControl PluginControl
        //{
        //    get
        //    {
        //        return _compositor.GetDataSettingsControl(SelectedDataSource);
        //    }
        //}
        public bool? ShowCustomerInfoWindow()
        {
            bool? result = false;
            if (_view != null)
            {
                _view.ShowDialog();
                result = _view.DialogResult;
            }
            return result;
        }
        //private void OnCustomerViewClosing(object sender, CancelEventArgs e)
        //{

        //}

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {

            var passwordContainer = parameter as IPwd;
            if (passwordContainer != null)
            {
                var secureString = passwordContainer.PasswordGet;
                Customer.Password = ConvertToUnsecureString(secureString);
            }

            //Customer.PluginSettings = _compositor.SavePluginsSettings();
            //if(Customer.ScheduleSettings.ScheduledIssues==null)
            //{
            //    Customer.ScheduleSettings.ScheduledIssues = new ScheduledItem[] {   new ScheduledItem(Ix4RequestProps.Articles, 0),
            //                                                                        new ScheduledItem(Ix4RequestProps.Orders, 0),
            //                                                                        new ScheduledItem(Ix4RequestProps.Deliveries, 0)};
            //}
            XmlConfigurationManager.Instance.UpdateCustomerInformation(Customer);

            _view.DialogResult = true;
            _view.Close();
        }

        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        public void Dispose()
        {
            _view = null;
            Customer = null;
        }

        private CustomerInfo _customer;

        public event EventHandler CanExecuteChanged;

        public CustomerInfo Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
            }
        }
        //public SchedulerSettings ScheduledSettings
        //{
        //    get
        //    {
        //        return Customer.ScheduleSettings;
        //    }
        //    set
        //    {
        //        Customer.ScheduleSettings = value;
        //        OnPropertyChanged("ScheduledSettings");
        //    }
        //}
    }
}
