using Ix4Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;

namespace Ix4ServiceConfigurator.ViewModel
{
     public class ImportDataSettingsViewModel : BaseViewModel
    {
        private ImportDataSettings _importDataSettings;

        public ImportDataSettingsViewModel()
        {
            
        }

        public ImportDataSettingsViewModel(ImportDataSettings importDataSettings)
        {
            this._importDataSettings = importDataSettings;
            ArticlesSettings = new ImportDataItemViewModel(_importDataSettings.ArticleSettings);
            OrdersSettings = new ImportDataItemViewModel(_importDataSettings.OrderSettings);
            DeliveriesSettings = new ImportDataItemViewModel(_importDataSettings.DeliverySettings);
        }

        public ImportDataItemViewModel ArticlesSettings { get; set; }
        public ImportDataItemViewModel OrdersSettings { get; set; }
        public ImportDataItemViewModel DeliveriesSettings { get; set; }
    }
}
