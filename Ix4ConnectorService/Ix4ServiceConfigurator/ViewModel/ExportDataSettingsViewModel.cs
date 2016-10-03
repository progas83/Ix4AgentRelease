using Ix4Models;
using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ix4ServiceConfigurator.ViewModel
{
   public class ExportDataSettingsViewModel : BaseViewModel, ICommand
    {
        ExportDataSettings _exportDataSettings;
        public ExportDataSettingsViewModel(ExportDataSettings exportDataSettings)
        {
            _exportDataSettings = exportDataSettings;
            
            
            //ExportDataItems = exportDataSettings.ExportDataItemSettings;// new ObservableCollection<ExportDataItemSettings>(exportDataSettings.ExportDataItemSettings);
        }

       // public ObservableCollection<ExportDataItemSettings> ExportDataItems { get; set; }

        private ExportDataItemSettings[] _exportDataItems;

        public ExportDataItemSettings[] ExportDataItems
        {
            get { return _exportDataSettings.ExportDataItemSettings; }
            set { _exportDataSettings.ExportDataItemSettings = value; OnPropertyChanged("ExportDataItems"); }
        }


        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExportDataItemSettings[] tempArr = new ExportDataItemSettings[ExportDataItems.Length + 1];
            ExportDataItemSettings s = new ExportDataItemSettings();
            s.IsActive = true;
            s.ExportDataTypeName="New data type.....";
            tempArr[0] = s;
            ExportDataItems.CopyTo(tempArr, 1);
            ExportDataItems = tempArr;
        }
    }
}
