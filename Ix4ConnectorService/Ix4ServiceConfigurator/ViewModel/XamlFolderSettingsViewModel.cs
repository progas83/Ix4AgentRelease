using Ix4Models.Enums;
using Ix4Models.SettingsDataModel;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Ix4ServiceConfigurator.ViewModel
{
    public class XamlFolderSettingsViewModel : INotifyPropertyChanged, ICommand
    {
        //   private BaseLicsRequestSettings _xmlPluginSettings;
        public XamlFolderSettingsViewModel(BaseLicsRequestSettings xmlPluginSettings)
        {
            if (xmlPluginSettings.DataSourceSettings is XmlFolderSettingsModel)
            {
                _folderSettingsModel = xmlPluginSettings.DataSourceSettings as XmlFolderSettingsModel;
            }
            else
            {
                _folderSettingsModel = new XmlFolderSettingsModel();
                xmlPluginSettings.DataSourceSettings = _folderSettingsModel;
            }
            //xmlPluginSettings.DataSourceSettings = new XmlFS() { Test2 = "Test3" };
            //CurrentPluginSettings = xmlPluginSettings;
        }
        //public BaseLicsRequestSettings CurrentPluginSettings
        //{
        //    get { return _xmlPluginSettings; }
        //    private set { _xmlPluginSettings = value; }
        //}

        private XmlFolderSettingsModel _folderSettingsModel { get; set; }

        //   private XmlResultHandleActions _actionOnSuccess;

        public XmlResultHandleActions ActionOnSuccess
        {
            get
            {
                //Enum.TryParse<XmlResultHandleActions>(CurrentPluginSettings.ActionOnSuccess,out _actionOnSuccess);
                return _folderSettingsModel.ActionOnSuccess;// _actionOnSuccess;
            }
            set
            {
                _folderSettingsModel.ActionOnSuccess = value;
                //  CurrentPluginSettings.ActionOnSuccess = _actionOnSuccess.ToString();
                OnPropertyChanged("ActionOnSuccess");
            }
        }

        //     private XmlResultHandleActions _actionOnFailure;

        public XmlResultHandleActions ActionOnFailure
        {
            get { return _folderSettingsModel.ActionOnFailure; }
            set { _folderSettingsModel.ActionOnFailure = value; OnPropertyChanged("ActionOnFailure"); }
        }


        //  private bool _activateActionOnSuccess;

        public bool ActivateActionOnSuccess
        {
            get { return _folderSettingsModel.ActivateActionOnSuccess; }
            set { _folderSettingsModel.ActivateActionOnSuccess = value; OnPropertyChanged("ActivateActionOnSuccess"); }
        }


        //  private bool _activateActonOnFailure ;

        public bool ActivateActonOnFailure
        {
            get { return _folderSettingsModel.ActivateActionOnFailure; }
            set { _folderSettingsModel.ActivateActionOnFailure = value; OnPropertyChanged("ActivateActonOnFailure"); }
        }


        // private string _successResultFolder;

        public string SuccessResultFolder
        {
            get { return _folderSettingsModel.SuccessFolder; }
            set { _folderSettingsModel.SuccessFolder = value; OnPropertyChanged("SuccessResultFolder"); }
        }

        //private string _failureResultFolder;

        public string FailureResultFolder
        {
            get { return _folderSettingsModel.FailureFolder; }
            set { _folderSettingsModel.FailureFolder = value; OnPropertyChanged("FailureResultFolder"); }
        }


        //private string _itemSourceFolder;
        public string XmlItemSourceFolder
        {
            get { return _folderSettingsModel.XmlItemSourceFolder; }
            set
            {
                _folderSettingsModel.XmlItemSourceFolder = value;
                OnPropertyChanged("XmlItemSourceFolder");
            }
        }

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            XmlFolderTypes folderType = (XmlFolderTypes)parameter;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            switch (folderType)
            {
                case XmlFolderTypes.XmlSourceFolder:
                    XmlItemSourceFolder = dialog.SelectedPath;
                    break;
                case XmlFolderTypes.XmlSuccessFolder:
                    SuccessResultFolder = dialog.SelectedPath;
                    break;
                case XmlFolderTypes.XmlFailureFolder:
                    FailureResultFolder = dialog.SelectedPath;
                    break;

            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
