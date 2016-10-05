using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4ServiceConfigurator.Controls;
using Ix4ServiceConfigurator.View.MsSql;
using Ix4ServiceConfigurator.ViewModel.MsSql;
using System.Windows.Controls;

namespace Ix4ServiceConfigurator.ViewModel
{
    public class ImportDataItemViewModel : BaseViewModel
    {
        
        public ImportDataItemViewModel(BaseLicsRequestSettings itemSettings)
        {
            BaseSettings = itemSettings;
            SelectControlForSourceType(BaseSettings.SourceDataType);
        }

        public BaseLicsRequestSettings BaseSettings { get; set; }

        public bool IsActivated
        {
            get { return BaseSettings.IsActivate; }
            set { BaseSettings.IsActivate = value; }
        }

        public bool IncludeArticlesToRequest
        {
            get { return BaseSettings.IncludeArticlesToRequest; }
            set { BaseSettings.IncludeArticlesToRequest = value; }
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
                SelectControlForSourceType(value);
                OnPropertyChanged("PluginControl");
            }
        }

        private UserControl _dataSourceControl;
        public UserControl PluginControl
        {
            get
            {
                return _dataSourceControl;// CustomerDataComposition.Instance.GetDataSettingsControl(BaseSettings);
            }
        }

        private void SelectControlForSourceType(CustomDataSourceTypes t)
        {
            switch(t)
            {
                case CustomDataSourceTypes.MsSql:
                    _dataSourceControl = GetMsSqlControlForSettings();
                    break;
                case CustomDataSourceTypes.Xml:
                    _dataSourceControl = GetXmlControlForSettings();
                    break;
                default:
                    break;
            }
        }

        private UserControl GetMsSqlControlForSettings()
        {
            DBSettingsViewModel dbVM = new DBSettingsViewModel(BaseSettings);
            DBSettingsView dbView = new DBSettingsView();
            dbView.DataContext = dbVM;
            return dbView;
        }

        private XamlFolderSettingsControl _xmlUserControl;
        private XamlFolderSettingsViewModel _xmlUserControlViewModel;
        private UserControl GetXmlControlForSettings()//BaseLicsRequestSettings settings)
        {
            if (_xmlUserControl == null)
            {
                _xmlUserControl = new XamlFolderSettingsControl();
            }
            if (_xmlUserControlViewModel == null)
            {
                _xmlUserControlViewModel = new XamlFolderSettingsViewModel(BaseSettings);
            }
            _xmlUserControl.DataContext = _xmlUserControlViewModel; ;
            return _xmlUserControl;
        }
    }
}
