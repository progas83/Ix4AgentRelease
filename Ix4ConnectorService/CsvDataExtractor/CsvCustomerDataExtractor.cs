using Ix4Models;
using Ix4Models.Interfaces;
using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Xml;

namespace CsvDataExtractor
{
    [Export(typeof(ICustomerDataConnector))]
    [ExportMetadata(CurrentServiceInformation.NameForPluginMetadata, CurrentServiceInformation.CustomDataSourceTypeCsv)]
    public class CsvCustomerDataExtractor : ICustomerDataConnector
    {
        public UserControl GetControlForSettings(PluginsSettings settings)
        {
            return new UserControl() { Content = new Label() { Content = "I am no implemented user control Of CSV DaTA" } };
        }

        public LICSRequestArticle[] GetRequestArticles(IPluginSettings pluginSettings)
        {
            throw new NotImplementedException();
        }

        public LICSRequestDelivery[] GetRequestDeliveries(IPluginSettings pluginSettings)
        {
            throw new NotImplementedException();
        }

        public LICSRequestOrder[] GetRequestOrders(IPluginSettings pluginSettings)
        {
            throw new NotImplementedException();
        }

        public void SaveSettings(PluginsSettings settings)
        {
            
        }

        public LICSRequest[] GetRequestsWithArticles(IPluginSettings pluginSettings, Ix4RequestProps ix4Property)
        {
            List<LICSRequest> requests = new List<LICSRequest>();

            return requests.ToArray();
        }

        public void ExportDataToCustomerSource(IPluginSettings pluginSettings, string exportDataType, string exportData, string[] exportDataParameters = null)
        {
            throw new NotImplementedException();
        }

        //public void ExportDataToCustomerSource(IPluginSettings pluginSettings, XmlNode exportData)
        //{
        //    throw new NotImplementedException();
        //}

        public T ExportDataToCustomerSource<T>(IPluginSettings pluginSettings, XmlNode exportData)
        {
            throw new NotImplementedException();
        }

        public T ExportDataToCustomerSource<T>(XmlNode exportData) where T : MSG
        {
            throw new NotImplementedException();
        }

        public ICustomerDataConnector GetPrepearedDataConnector(IPluginSettings pluginSettings)
        {
            throw new NotImplementedException();
        }
    }
}
