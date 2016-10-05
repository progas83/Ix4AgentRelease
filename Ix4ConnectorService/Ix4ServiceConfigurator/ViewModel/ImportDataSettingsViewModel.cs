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
        public ImportDataSettingsViewModel(ImportDataSettings importDataSettings)
        {
            ArticlesSettings = new ImportDataItemViewModel(importDataSettings.ArticleSettings);
            OrdersSettings = new ImportDataItemViewModel(importDataSettings.OrderSettings);
            DeliveriesSettings = new ImportDataItemViewModel(importDataSettings.DeliverySettings);
        }

        public ImportDataItemViewModel ArticlesSettings { get; set; }
        public ImportDataItemViewModel OrdersSettings { get; set; }
        public ImportDataItemViewModel DeliveriesSettings { get; set; }
    }
}
