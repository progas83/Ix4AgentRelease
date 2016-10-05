using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using Ix4Models;
using Ix4Models.Interfaces;
using Ix4Models.DataProviders.MsSqlDataProvider;
using System.Windows.Data;
using Ix4Models.DataProviders.XmlDataProvider;

namespace DataProcessorHelper
{
     public class ImportDataSourcesBuilder 
    {
        private ImportDataSettings _importDataSettings;
        private IDataProvider _sqlSourceDataProvider;
        private IDataProvider _xmlSourceDataProvider;
        public ImportDataSourcesBuilder(ImportDataSettings importDataSettings)
        {
            _importDataSettings = importDataSettings;
           _sqlSourceDataProvider = new MsSqlDataProvider();
            _xmlSourceDataProvider = new XmlDataProvider();
        }

        public List<LICSRequestArticle> GetArticles()
        {
            List<LICSRequestArticle> articles = null;
           switch (_importDataSettings.ArticleSettings.SourceDataType)
            {
                case CustomDataSourceTypes.MsSql:
                   
                        articles = _sqlSourceDataProvider.GetArticles(_importDataSettings.ArticleSettings.DataSourceSettings);
                        break;
                   
                    
                    
                case CustomDataSourceTypes.Xml:
                   articles = _xmlSourceDataProvider.GetArticles(_importDataSettings.ArticleSettings.DataSourceSettings);
                    break;
                default:
                    break;
            }
            return articles;
            
        }

        public List<LICSRequestDelivery> GetDeliveries()
        {
            List<LICSRequestDelivery> deliveries = null;
            switch (_importDataSettings.ArticleSettings.SourceDataType)
            {
                case CustomDataSourceTypes.MsSql:

                    deliveries = _sqlSourceDataProvider.GetDeliveries(_importDataSettings.ArticleSettings.DataSourceSettings);
                    break;



                case CustomDataSourceTypes.Xml:
                    deliveries = _xmlSourceDataProvider.GetDeliveries(_importDataSettings.ArticleSettings.DataSourceSettings);
                    break;
                default:
                    break;
            }
            return deliveries;
        }

        public List<LICSRequestOrder> GetOrders()
        {
            List<LICSRequestOrder> orders = null;
            switch (_importDataSettings.ArticleSettings.SourceDataType)
            {
                case CustomDataSourceTypes.MsSql:

                    orders = _sqlSourceDataProvider.GetOrders(_importDataSettings.ArticleSettings.DataSourceSettings);
                    break;



                case CustomDataSourceTypes.Xml:
                    orders = _xmlSourceDataProvider.GetOrders(_importDataSettings.ArticleSettings.DataSourceSettings);
                    break;
                default:
                    break;
            }
            return orders;
        }
    }
}
