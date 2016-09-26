using Ix4Models;
using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XmlDataExtractor.Settings.ViewModel
{
    public class XamlFolderSettingsViewModel : INotifyPropertyChanged, ICommand
    {
        private XmlPluginSettings _xmlPluginSettings;
        public XamlFolderSettingsViewModel(XmlPluginSettings xmlPluginSettings)
        {
            CurrentPluginSettings = xmlPluginSettings;
        }
        public XmlPluginSettings CurrentPluginSettings
        {
            get { return _xmlPluginSettings; }
            private set { _xmlPluginSettings = value; }
        }

        private XmlResultHandleActions _actionOnSuccess;

        public XmlResultHandleActions ActionOnSuccess
        {
            get { return _actionOnSuccess; }
            set { _actionOnSuccess = value; OnPropertyChanged("ActionOnSuccess"); }
        }

        private XmlResultHandleActions _actionOnFailure;

        public XmlResultHandleActions ActionOnFailure
        {
            get { return _actionOnFailure; }
            set { _actionOnFailure = value; }
        }


        private bool _activateActionOnSuccess;

        public bool ActivateActionOnSuccess
        {
            get { return _activateActionOnSuccess; }
            set { _activateActionOnSuccess = value; OnPropertyChanged("ActivateActionOnSuccess"); }
        }


        private bool _activateActonOnFailure ;

        public bool ActivateActonOnFailure 
        {
            get { return _activateActonOnFailure; }
            set { _activateActonOnFailure = value; OnPropertyChanged("ActivateActonOnFailure"); }
        }


        private string _successResultFolder;

        public string SuccessResultFolder
        {
            get { return _successResultFolder; }
            set { _successResultFolder = value; OnPropertyChanged("SuccessResultFolder"); }
        }

        private string _failureResultFolder;

        public string FailureResultFolder
        {
            get { return _failureResultFolder; }
            set { _failureResultFolder = value; OnPropertyChanged("FailureResultFolder"); }
        }


        private string _itemSourceFolder;
        public string XmlItemSourceFolder
        {
            get { return _itemSourceFolder; }
            set
            {
                _itemSourceFolder = value;
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
            switch(folderType)
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
