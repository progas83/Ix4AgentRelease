using Ix4Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using SimplestLogger;
using System.IO;

namespace Ix4Models.DataProviders.XmlDataProvider
{
    public class XmlDataProvider : IDataProvider
    {
        public static Logger _loger = Logger.GetLogger();

        public List<LICSRequestArticle> GetArticles(BaseDataSourceSettings settings)
        {
            throw new NotImplementedException();
        }

        //public LICSRequest GetData(BaseDataSourceSettings settings)
        //{
        //    LICSRequest request = null;
        //    XmlFolderSettingsModel xmlSettings = settings as XmlFolderSettingsModel;
        //    if (xmlSettings == null)
        //    {
        //        _loger.Log("Wrong settings data for orders");
        //        return request;
        //    }
        //    //if (xmlSettings.XmlItemSourceFolder)
        //    //{

        //    //}
        // //   List<LICSRequestOrder> orders = null;
           
        //    string[] xmlSourceFiles = Directory.GetFiles(xmlSettings.XmlItemSourceFolder, "*.xml");
        //    if (xmlSourceFiles.Length > 0)
        //    {
        //        foreach (string file in xmlSourceFiles)
        //        {
        //            LICSRequest request = GetCustomerDataFromXml(file);
        //            LICSResponse response = SendLicsRequestToIx4(request, "deliveryFile.xml");
        //            if (response.DeliveryImport.CountOfFailed == 0)
        //            {
        //                string successFolder = string.Format("{0}\\Archive", xmlSettings.XmlItemSourceFolder);
        //                if (!Directory.Exists(successFolder))
        //                {
        //                    Directory.CreateDirectory(successFolder);
        //                }
        //                File.Move(file, string.Format("{0}\\{1}", successFolder, Path.GetFileName(file)));
        //            }
        //        }
        //    }
        //}

        public List<LICSRequestDelivery> GetDeliveries(BaseDataSourceSettings settings)
        {
            throw new NotImplementedException();
        }

        public List<LICSRequestOrder> GetOrders(BaseDataSourceSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
