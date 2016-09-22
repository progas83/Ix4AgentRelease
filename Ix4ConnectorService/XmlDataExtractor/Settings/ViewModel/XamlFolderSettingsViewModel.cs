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
        public bool IsActivated
        {
            get { return _xmlPluginSettings.IsActivated; }
            set
            {
                _xmlPluginSettings.IsActivated = value;
                OnPropertyChanged("IsActivated");
            }
        }

        public bool CheckArticles
        {
            get { return _xmlPluginSettings.CheckArticles; }
            set
            {
                _xmlPluginSettings.CheckArticles = value;
                OnPropertyChanged("CheckArticles");
            }
        }

        public bool CheckOrders
        {
            get { return _xmlPluginSettings.CheckOrders; }
            set
            {
                _xmlPluginSettings.CheckOrders = value;
                OnPropertyChanged("CheckOrders");
            }
        }

        public bool CheckDeliveries
        {
            get { return _xmlPluginSettings.CheckDeliveries; }
            set
            {
                _xmlPluginSettings.CheckDeliveries = value;
                OnPropertyChanged("CheckDeliveries");
            }
        }

        public string XmlArticleSourceFolder
        {
            get { return _xmlPluginSettings.XmlArticleSourceFolder; }
            set
            {
                _xmlPluginSettings.XmlArticleSourceFolder = value;
                OnPropertyChanged("XmlArticleSourceFolder");
            }
        }

        public string XmlOrdersSourceFolder
        {
            get { return _xmlPluginSettings.XmlOrdersSourceFolder; }
            set
            {
                _xmlPluginSettings.XmlOrdersSourceFolder = value;
                OnPropertyChanged("XmlOrdersSourceFolder");
            }
        }

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public string XmlDeliveriesSourceFolder
        {
            get { return _xmlPluginSettings.XmlDeliveriesSourceFolder; }
            set { _xmlPluginSettings.XmlDeliveriesSourceFolder = value;
                OnPropertyChanged("XmlDeliveriesSourceFolder");
            }
        }

        public void Execute(object parameter)
        {
            Ix4RequestProps folderName = (Ix4RequestProps)parameter;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            switch (folderName)
            {
                case Ix4RequestProps.Articles:
                    XmlArticleSourceFolder = dialog.SelectedPath;
                    break;
                case Ix4RequestProps.Orders:
                    XmlOrdersSourceFolder = dialog.SelectedPath;
                    break;
                case Ix4RequestProps.Deliveries:
                    XmlDeliveriesSourceFolder = dialog.SelectedPath;
                    break;


                default:
                    break;
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
