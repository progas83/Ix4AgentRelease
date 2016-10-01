using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using Ix4Models;
using Ix4Models.SettingsDataModel;
using Ix4Models.Interfaces;
using System.Xml;
using Ix4Models.SettingsManager;
using SimplestLogger;

namespace CompositionHelper
{
    public class CustomerDataComposition
    {

        [ImportMany]
        public System.Lazy<ICustomerDataConnector, IDictionary<string, object>>[] CustomerDataPlugins { get; set; }

        private static CustomerDataComposition _dataCompositor = null;
        private static object _padlock = new object();
        private Logger _logger = Logger.GetLogger();

        public static CustomerDataComposition Instance
        {
            get
            {
                if (_dataCompositor == null)
                {
                    lock (_padlock)
                    {
                        if (_dataCompositor == null)
                        {
                            _dataCompositor = new CustomerDataComposition();
                        }
                    }

                }
                return _dataCompositor;
            }
        }

        private CustomerInfo _customerSettings;
        private CustomerDataComposition()
        {
            AssembleCustomerDataComponents();
            _customerSettings = XmlConfigurationManager.Instance.GetCustomerInformation();
        }

        private void AssembleCustomerDataComponents()
        {
            try
            {
                var directoryPath = string.Format("{0}\\{1}", string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), CurrentServiceInformation.PluginsSubdirectory);
                var directoryCatalog = new DirectoryCatalog(directoryPath, "*.dll");

                var aggregateCatalog = new AggregateCatalog();
                aggregateCatalog.Catalogs.Add(directoryCatalog);

                var container = new CompositionContainer(aggregateCatalog);

                container.ComposeParts(this);


            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }

        public UserControl GetDataSettingsControl(BaseLicsRequestSettings settings)
        {
            UserControl uc = null;

            try
            {
                foreach (var plugin in CustomerDataPlugins)
                {
                    if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes),settings.SourceDataType)))
                    {
                        uc = plugin.Value.GetControlForSettings(settings);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
               _logger.Log(ex);
            }
            return uc;
        }




        private PluginsSettings _pluginSettings;

        //BaseLicsRequestSettings _settings;
        //public CustomerDataComposition(BaseLicsRequestSettings settings) : this()
        //{
        //    _settings = settings;
        //}

      
      
     

        public PluginsSettings SavePluginsSettings()
        {
            PluginsSettings ps = new PluginsSettings();
            try
            {
                foreach (var plugin in CustomerDataPlugins)
                {
                    plugin.Value.SaveSettings(_pluginSettings);
                }
            }
            catch (Exception ex)
            {
                //       _logger.Log(ex);
            }


            return _pluginSettings;
        }

     

        public LICSRequestArticle[] GetRequestArticles()
        {
            LICSRequestArticle[] articles = new LICSRequestArticle[] { };
            try
            {
                IPluginSettings plugingSettings = _pluginSettings.AllAvailablePluginSettings().FirstOrDefault(pl => pl.CheckArticles);

                if (plugingSettings == null)
                {
                    //_logger.Log("There was not adjusted any article setting");
                    return articles;
                }
                foreach (var plugin in CustomerDataPlugins)
                {
                    if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes),
                        plugingSettings.PluginType)))
                    {
                        articles = plugin.Value.GetRequestArticles(plugingSettings);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //         _logger.Log(ex);
            }
            return articles;

        }

        public LICSRequestDelivery[] GetRequestDeliveries()
        {
            LICSRequestDelivery[] deliveries = new LICSRequestDelivery[] { };
            IPluginSettings plugingSettings = _pluginSettings.AllAvailablePluginSettings().FirstOrDefault(pl => pl.CheckDeliveries);

            if (plugingSettings == null)
            {
                //_logger.Log("There was not adjusted any article setting");
                return deliveries;
            }

            try
            {
                foreach (var plugin in CustomerDataPlugins)
                {
                    if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes), plugingSettings.PluginType)))
                    {
                        deliveries = plugin.Value.GetRequestDeliveries(plugingSettings);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //         _logger.Log(ex);
            }
            return deliveries;

        }

        public LICSRequestOrder[] GetRequestOrders()
        {
            LICSRequestOrder[] requestOrders = new LICSRequestOrder[] { };
            try
            {
                IPluginSettings plugingSettings = _pluginSettings.AllAvailablePluginSettings().FirstOrDefault(pl => pl.CheckOrders);
                if (plugingSettings == null)
                {
                    //_logger.Log("There was not adjusted any orders setting");
                    return requestOrders;
                }
                foreach (var plugin in CustomerDataPlugins)
                {
                    if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes), plugingSettings.CheckOrders)))
                    {
                        requestOrders = plugin.Value.GetRequestOrders(plugingSettings);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //         _logger.Log(ex);
            }
            return requestOrders;
        }

        public LICSRequest[] GetPreparedRequests(CustomDataSourceTypes dataSourceType, Ix4RequestProps ix4Property)
        {
            LICSRequest[] requests = new LICSRequest[] { };
            var plugingSettings = _pluginSettings.AllAvailablePluginSettings().FirstOrDefault(pl => pl.PluginType == dataSourceType);
            if (plugingSettings == null)
            {
                return requests;
            }

            foreach (var plugin in CustomerDataPlugins)
            {
                if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes), dataSourceType)))
                {
                    requests = plugin.Value.GetRequestsWithArticles(plugingSettings, ix4Property);
                    break;
                }
            }
            return requests;
        }

        public ICustomerDataConnector GetCustomerDataConnector(CustomDataSourceTypes dataSourceType)
        {
            ICustomerDataConnector connector = null;
            var plugingSettings = _pluginSettings.AllAvailablePluginSettings().FirstOrDefault(pl => pl.PluginType == dataSourceType);
            if (plugingSettings == null)
            {
                return connector;
            }

            foreach (var plugin in CustomerDataPlugins)
            {
                if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes), dataSourceType)))
                {
                    connector = plugin.Value.GetPrepearedDataConnector(plugingSettings);//
                    break;
                }
            }
            return connector;
        }

        public void ExportData(CustomDataSourceTypes dataSourceType, XmlNode exportData)
        {
            //var plugingSettings = _pluginSettings.AllAvailablePluginSettings().FirstOrDefault(pl => pl.PluginType == dataSourceType);
            //if (plugingSettings == null)
            //{
            //    return ;
            //}

            //foreach (var plugin in CustomerDataPlugins)
            //{
            //    if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes), dataSourceType)))
            //    {
            //        plugin.Value.ExportDataToCustomerSource(plugingSettings,exportData);
            //        break;
            //    }
            //}
        }

        //public LICSRequestArticle GetArticleByNumber(string articleNumber)
        //{
        //    LICSRequestArticle article = new LICSRequestArticle();
        //    try
        //    {
        //        IPluginSettings plugingSettings = _pluginSettings.AllAvailablePluginSettings().FirstOrDefault(pl => pl.CheckArticles);

        //        if (plugingSettings == null)
        //        {
        //            //_logger.Log("There was not adjusted any article setting");
        //            return null;// articles;
        //        }
        //        foreach (var plugin in CustomerDataPlugins)
        //        {
        //            if (((string)plugin.Metadata[CurrentServiceInformation.NameForPluginMetadata]).Equals(Enum.GetName(typeof(CustomDataSourceTypes),
        //                plugingSettings.PluginType)))
        //            {
        //                article = plugin.Value.GetRequestArticles(plugingSettings);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //         _logger.Log(ex);
        //    }
        //    return article;

        //}


    }
}
