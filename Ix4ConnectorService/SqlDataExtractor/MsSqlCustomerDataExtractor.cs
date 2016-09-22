using Ix4Models;
using Ix4Models.Interfaces;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using SqlDataExtractor.DatabaseSettings.View;
using SqlDataExtractor.DatabaseSettings.ViewModel;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SqlDataExtractor
{
    [Export(typeof(ICustomerDataConnector))]
    [ExportMetadata(CurrentServiceInformation.NameForPluginMetadata, CurrentServiceInformation.CustomDataSourceTypeMsSql)]

    public class MsSqlCustomerDataExtractor : ICustomerDataConnector
    {
        MsSqlPluginSettings _pluginSettings;
        MainDBSettingsViewModel _msSqlPluginSettingsViewModel;
        MainDBSettindsView _msSqlPluginSettingsView;

        private static Logger _loger = Logger.GetLogger();

        public UserControl GetControlForSettings(PluginsSettings settings)
        {
            //ManualMaping.View.ManualMappingView view = new ManualMaping.View.ManualMappingView();
            //view.DataContext = new ManualMaping.ViewModel.ManualMapperViewModel();
            if(_msSqlPluginSettingsView==null)
            {
                _msSqlPluginSettingsView = new MainDBSettindsView();
            }
            if(_msSqlPluginSettingsViewModel==null)
            {
                _msSqlPluginSettingsViewModel = new MainDBSettingsViewModel(settings.MsSqlSettings);
            }
           
            _msSqlPluginSettingsView.DataContext = _msSqlPluginSettingsViewModel;
            return _msSqlPluginSettingsView;
        }

        public void SaveSettings(PluginsSettings settings)
        {
            if(_msSqlPluginSettingsViewModel == null)
            {
                return;
            }
            if (_msSqlPluginSettingsViewModel.CurrentPluginSettings.IsActivated)
            {
                settings.MsSqlSettings = _msSqlPluginSettingsViewModel.CurrentPluginSettings;
            }
        }
      
        public LICSRequestArticle[] GetRequestArticles(IPluginSettings pluginSettings)
        {
            MsSqlPluginSettings settings = pluginSettings as MsSqlPluginSettings;
            //      _loger.Log("Pluging settings for SQL:");
            //     _loger.Log(settings.ToString());
            //SqlTableExplorer<LICSRequestArticle> articleExplorer = new SqlTableExplorer<LICSRequestArticle>(pluginSettings);
            SqlTableArticleExplorer articleExplorer = new SqlTableArticleExplorer(pluginSettings);
            return articleExplorer.GetArticles();
        }

        public LICSRequestDelivery[] GetRequestDeliveries(IPluginSettings pluginSettings)
        {
            SqlTableDeliveryExplorer articleExplorer = new SqlTableDeliveryExplorer(pluginSettings);
            return articleExplorer.GetRequestDeliveries();
        }

        public LICSRequestOrder[] GetRequestOrders(IPluginSettings pluginSettings)
        {
            SqlTableOrdersExplorer ordersExplorer = new SqlTableOrdersExplorer(pluginSettings);
            return ordersExplorer.GetRequestOrders();
        }

        public LICSRequest[] GetRequestsWithArticles(IPluginSettings pluginSettings, Ix4RequestProps ix4Property)
        {
            List<LICSRequest> requests = new List<LICSRequest>();
            LICSRequest request = new LICSRequest();
            switch(ix4Property)
            {
                case Ix4RequestProps.Articles:
                    request.ArticleImport = GetRequestArticles(pluginSettings);
                    break;
                case Ix4RequestProps.Deliveries:
                    request.DeliveryImport = GetRequestDeliveries(pluginSettings);
                    break;
                case Ix4RequestProps.Orders:
                    request.OrderImport = GetRequestOrders(pluginSettings);
                    break;
                default:
                    break;

            }
            requests.Add(request);
            return requests.ToArray();
        }

        //public void ExportDataToCustomerSource(IPluginSettings pluginSettings, string exportDataType, string exportData, string[] exportDataParameters = null)
        //{
        //    
        ////    var dataInstance = Activator.CreateInstance(Type.GetType(exportDataType));
        //    sqlExporter.SaveDataToTable(exportDataType, exportData);
        //}

        public void ExportDataToCustomerSource(IPluginSettings pluginSettings, XmlNode exportData)
        {
            try
            {
                ExportDataToSQL sqlExporter = new ExportDataToSQL(pluginSettings);
                sqlExporter.SaveDataToTable(exportData);
            }
            catch(Exception ex)
            {
                _loger.Log(ex);
            }
            

        
        }

        public T ExportDataToCustomerSource<T>(XmlNode exportData) where T : MSG
        {

           if(_exportDataToSql==null)
            {
                _exportDataToSql = new ExportDataToSQL(_pluginSettings);
            }
            return _exportDataToSql.SaveDataToTable<T>(exportData);
        }
        ExportDataToSQL _exportDataToSql;
        public ICustomerDataConnector GetPrepearedDataConnector(IPluginSettings pluginSettings)
        {
            _pluginSettings = pluginSettings as MsSqlPluginSettings;
            _exportDataToSql = new ExportDataToSQL(_pluginSettings);
            return this;
        }
    }
}
