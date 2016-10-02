using Ix4Models;
using Ix4Models.SettingsDataModel;
using SqlDataExtractor.Settings.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SqlDataExtractor.Settings.ViewModel
{
    public class DBSettingsViewModel : BaseViewModel
    {
        public DBSettingsViewModel(BaseLicsRequestSettings sqlSourceSettings)
        {
            if (sqlSourceSettings.DataSourceSettings is MsSqlSettings)
            {
                switch(sqlSourceSettings.SettingsName)
                {
                    case Ix4RequestProps.Articles:
                        
                        DBImportDataSettings = new MsSqlArticleSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlArticlesSettings);
                        break;
                    case Ix4RequestProps.Deliveries:
                       
                        DBImportDataSettings = new MsSqlDeliveriesSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                        break;
                    case Ix4RequestProps.Orders:
                        
                        DBImportDataSettings = new MsSqlOrdersSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlOrdersSettings);
                        break;
                    default:
                        break;
                }
                ///_folderSettingsModel = xmlPluginSettings.DataSourceSettings as XmlFolderSettingsModel;
            }
            else
            {
                switch (sqlSourceSettings.SettingsName)
                {
                    case Ix4RequestProps.Articles:
                        sqlSourceSettings.DataSourceSettings = new MsSqlArticlesSettings();
                        DBImportDataSettings = new MsSqlArticleSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlArticlesSettings);
                        break;
                    case Ix4RequestProps.Deliveries:
                        sqlSourceSettings.DataSourceSettings = new MsSqlDeliveriesSettings();
                        DBImportDataSettings = new MsSqlDeliveriesSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlDeliveriesSettings);
                        break;
                    case Ix4RequestProps.Orders:
                        sqlSourceSettings.DataSourceSettings = new MsSqlOrdersSettings();
                        DBImportDataSettings = new MsSqlOrdersSettingsView(sqlSourceSettings.DataSourceSettings as MsSqlOrdersSettings);
                        break;
                    default:
                        break;
                }
            }
        }

        public UserControl DBImportDataSettings { get;set;}
    }
}
