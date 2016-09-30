using Ix4Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using XmlDataExtractor.Settings.View;
using XmlDataExtractor.Settings.ViewModel;
using Ix4Models.Converters;
using Ix4Models.SettingsDataModel;
using Ix4Models.Interfaces;
using System.Xml;

namespace XmlDataExtractor
{
    [Export(typeof(ICustomerDataConnector))]
    [ExportMetadata(CurrentServiceInformation.NameForPluginMetadata, CurrentServiceInformation.CustomDataSourceTypeXml)]
    public class XmlDataExtractor : ICustomerDataConnector
    {
        XamlFolderSettingsControl _xamlUserControl;
        XamlFolderSettingsViewModel _viewModel;
       
        public UserControl GetControlForSettings(BaseLicsRequestSettings settings)
        {
            BaseLicsRequestSettings _xmlSettings = settings;
            if(_xamlUserControl == null)
            {
                _xamlUserControl = new XamlFolderSettingsControl();
            }
            if(_viewModel==null)
            {
                _viewModel = new XamlFolderSettingsViewModel   (_xmlSettings);
            }
            _xamlUserControl.DataContext = _viewModel; ;
            return _xamlUserControl;
        }

        //public LICSRequest GetCustomerDataFromXml(string fileName)
        //{

        //    XmlSerializer xS = new XmlSerializer(typeof(OutputPayLoad));
        //    LICSRequest licsRequest = new LICSRequest();
        //    using (FileStream fs = new FileStream(fileName, FileMode.Open))
        //    {
        //        OutputPayLoad customerInfo =(OutputPayLoad) xS.Deserialize(fs);
        //        licsRequest = customerInfo.ConvertToLICSRequest();
        //    }

        //    return licsRequest;
        //}

       public void SaveSettings(PluginsSettings settings)
        {
            if(_viewModel==null)
            {
                return;
            }
           // if(_viewModel.CurrentPluginSettings.IsActivated)
            {
           //     settings.XmlSettings = _viewModel.CurrentPluginSettings;
            }
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
        private readonly int _deliveriesLimit = 20;
        public LICSRequest[] GetRequestsWithArticles(IPluginSettings pluginSettings, Ix4RequestProps ix4Property)
        {

            List<LICSRequest> requests = new List<LICSRequest>();
            XmlPluginSettings xmlSettings = pluginSettings as XmlPluginSettings;
            if (xmlSettings == null)
            {
                return requests.ToArray();
            }

            if (pluginSettings.CheckArticles)
            {
                LICSRequest request = new LICSRequest();
                switch (ix4Property)
                {
                    
                    case Ix4RequestProps.Articles:
                        request.ArticleImport = GetRequestArticles(pluginSettings);
                        requests.Add(request);
                        break;
                    case Ix4RequestProps.Orders:
                        //request.ArticleImport = GetRequestArticles(pluginSettings);
                        //request.OrderImport = GetRequestOrders(pluginSettings);
                        //requests.Add(request);


                        {
                            
                            

                            // string[] xmlSourceFiles = Directory.GetFiles("C:\\Ilya\\TestXmlFolder\\XmlSource");// _customerInfo.PluginSettings.XmlSettings.SourceFolder);
                            // ICustomerDataConnector xmlDataConnector = CustomerDataComposition.Instance.GetDataConnector(CustomDataSourceTypes.Xml);
                            string[] xmlSourceFiles = Directory.GetFiles(xmlSettings.XmlArticleSourceFolder, "*.xml");
                            if (xmlSourceFiles.Length > 0)
                            {
                                foreach (string file in xmlSourceFiles)
                                {

                                    //    _streamWriterFile.WriteLine(string.Format("Filename:  {0}", file));

                                    LICSRequest req = GetCustomerDataFromXml(file);// CustomerDataComposition.Instance.GetCustomerDataFromXml(file);// xmlDataConnector.GetCustomerDataFromXml(file);
                                    requests.Add(req);

                                }
                            }
                        }
                        break;
                    case Ix4RequestProps.Deliveries:
                      //  LICSRequestDelivery[] deliveries = GetRequestDeliveries(pluginSettings);
                      //  List<LICSRequestDelivery> deliveryList = new List<LICSRequestDelivery>();
                      //  if(deliveries.Length>_deliveriesLimit)
                      //  {
                      //      int i = 0;
                      //      foreach(var deliv in deliveries)
                      //      {
                      //          if(i<=_deliveriesLimit)
                      //          {
                      //              deliveryList.Add(deliv);
                      //              i++;
                      //          }
                      //          else
                      //          {
                      //              i = 0;
                      //              request.DeliveryImport = deliveryList.ToArray();
                      //              requests.Add(request);
                      //              deliveryList = new List<LICSRequestDelivery>();
                      //          }
                      //      }
                      //  }
                      //  request.ArticleImport = GetRequestArticles(pluginSettings);
                      ////  request.OrderImport = GetRequestOrders(pluginSettings);
                      //  requests.Add(request);
                        break;
                    default:
                        break;
                }
            }
            
            return requests.ToArray();
        }



        private List<LICSRequest>  CheckXmlOrdersFolder(IPluginSettings pluginSettings)
        {
            List<LICSRequest> requests = new List<LICSRequest>();
            try
            {

                XmlPluginSettings xmlSettings = pluginSettings as XmlPluginSettings;
                if (xmlSettings == null)
                {
                    return requests;
                }

                // string[] xmlSourceFiles = Directory.GetFiles("C:\\Ilya\\TestXmlFolder\\XmlSource");// _customerInfo.PluginSettings.XmlSettings.SourceFolder);
                // ICustomerDataConnector xmlDataConnector = CustomerDataComposition.Instance.GetDataConnector(CustomDataSourceTypes.Xml);
                string[] xmlSourceFiles = Directory.GetFiles(xmlSettings.XmlArticleSourceFolder,"*.xml");
                if (xmlSourceFiles.Length > 0)
                {
                    foreach (string file in xmlSourceFiles)
                    {

                        //    _streamWriterFile.WriteLine(string.Format("Filename:  {0}", file));

                        LICSRequest request = GetCustomerDataFromXml(file);// CustomerDataComposition.Instance.GetCustomerDataFromXml(file);// xmlDataConnector.GetCustomerDataFromXml(file);
                        requests.Add(request);
                     
                    }
                }


                //string mes1 = string.Format("Service Timer has been elapsed at {0} | {1}", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString());
                //string mes2 = string.Format("Count of files in the folder {0} = {1}", _customerInfo.PluginSettings.XmlSettings.SourceFolder, xmlSourceFiles.Length);
                //WrightLog(mes1);
                //WrightLog(mes2);


                //foreach (string file in xmlSourceFiles)
                //{


                //}
            }
            catch (Exception ex)
            {
               // WrightLog(ex.Message);
            }

            return requests;
        }

        public LICSRequest GetCustomerDataFromXml(string fileName)
        {

            XmlSerializer xS = new XmlSerializer(typeof(OutputPayLoad));
            LICSRequest licsRequest = new LICSRequest();
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                OutputPayLoad customerInfo = (OutputPayLoad)xS.Deserialize(fs);
                licsRequest = customerInfo.ConvertToLICSRequest();
            }

            return licsRequest;
        }

        public void ExportDataToCustomerSource(IPluginSettings pluginSettings, string exportDataType, string exportData, string[] exportDataParameters = null)
        {
            throw new NotImplementedException();
        }

        public void ExportDataToCustomerSource(IPluginSettings pluginSettings, XmlNode exportData)
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
