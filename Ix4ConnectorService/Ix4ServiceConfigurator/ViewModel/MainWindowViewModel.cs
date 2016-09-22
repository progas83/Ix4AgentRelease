using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4Models.SettingsManager;
using Ix4ServiceConfigurator.Commands;
using System;
using System.ServiceProcess;
using System.Windows.Input;
using RichTextContainer = SimplestLogger.VisualLogging.LogInfoArgs;
using SimplestLogger.VisualLogging;
using System.Windows.Data;
using System.Globalization;

namespace Ix4ServiceConfigurator.ViewModel
{
    public class MainWindowViewModel : BaseViewModel, IDisposable
    {
        System.Timers.Timer _checkServiceStatusTimer;
        public MainWindowViewModel()
        {
            InstallationCommand InstallationCommand = new InstallationCommand();
            InstallationCommand.ServiceInfoNeedToUpdate += OnServiceInfoNeedToUpdate;
            InstallServiceCommand = InstallationCommand;
            UpdateCustomerInfo();
            MakeChangesCommad MakeChangesCommand = new MakeChangesCommad();
            MakeChangesCommand.CustomInformationSaved += OnCustomerInfoNeedToUpdate;
            MakeChangesCmd = MakeChangesCommand;
            _checkServiceStatusTimer = new System.Timers.Timer(1000);
            _checkServiceStatusTimer.AutoReset = true;
            _checkServiceStatusTimer.Elapsed += OnCheckStatusTimerElapsed;
            _checkServiceStatusTimer.Enabled = true;

            SimplestLogger.VisualLogging.VisualLogger.Instance.LogEvent += OnLoggingEvent;

            
        }

        

        private void OnLoggingEvent(object sender, LogInfoArgs e)
        {
            TextStatusContainer = e;
        }
        private RichTextContainer _textContainer;
        public RichTextContainer TextStatusContainer
        {
            get
            {
                return _textContainer;
            }
            private set
            {
                _textContainer = value;
                OnPropertyChanged("TextStatusContainer");
            }
        }
        ServiceControllerStatus _previousServiceStatus = ServiceControllerStatus.Stopped;

        private void OnCheckStatusTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(_previousServiceStatus!=ServiceStatus)
            {
                OnPropertyChanged("ServiceStatus");
                _previousServiceStatus = ServiceStatus;
            }
        }
         

        private void OnCustomerInfoNeedToUpdate(object sender, EventArgs e)
        {
            UpdateCustomerInfo();

        }

        private void UpdateCustomerInfo()
        {
            Customer = XmlConfigurationManager.Instance.GetCustomerInformation();
            
        }

        private void OnServiceInfoNeedToUpdate(object sender, EventArgs e)
        {
            OnPropertyChanged("ServiceExist");
            OnPropertyChanged("ServiceStatus");
        }

        public void Dispose()
        {
            if(_checkServiceStatusTimer!=null)
            {
                _checkServiceStatusTimer.Enabled = false;
                _checkServiceStatusTimer.Dispose();
                _checkServiceStatusTimer = null;
            }
        }

        CustomerInfo _customer;
        public CustomerInfo Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
            }
        }

        public ICommand InstallServiceCommand { get; private set; }

        public ICommand MakeChangesCmd { get; set; }

        public string ServiceName
        {
            get
            {
                return CurrentServiceInformation.ServiceName;
            }
        }

        public ServiceControllerStatus ServiceStatus
        {
            get
            {
                
                return ServiceInfoWrapper.Instance.ServiceStatus;// ServiceExist ?_serviceStatus : ServiceControllerStatus.Stopped;
            }
        }

        public bool ServiceExist
        {
            get { return ServiceInfoWrapper.Instance.ServiceExist; }
        }
    }
}
