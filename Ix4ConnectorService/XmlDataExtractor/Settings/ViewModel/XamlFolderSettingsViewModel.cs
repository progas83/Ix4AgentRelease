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


        private string _itemSourceFolder;
        public string XmlItemSourceFolder
        {
            get { return _itemSourceFolder; }
            set
            {
                _itemSourceFolder = value;
                OnPropertyChanged("XmlOrdersSourceFolder");
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
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            XmlItemSourceFolder = dialog.SelectedPath;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
