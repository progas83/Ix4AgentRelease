using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ix4Connector.Args;
using Ix4Connector.Ix4WebService;
using System.Net;

namespace Ix4Connector
{
    internal class ProxyIx4WebService : IProxyIx4WebService
    {
        private LBSoapAuthenticationHeader _header;
        private string _endPoint;
        public ProxyIx4WebService(LBSoapAuthenticationHeader header,string endpoint)
        {
             System.Net.ServicePointManager.Expect100Continue = false;
            this._header = header;
            this._endPoint = endpoint;
        }

        public event ExportDataAsyncCompletedEventHandler ExportDataAsyncCompleted;

        public XmlNode ExportData(string exportDataType, string[] additionalParameters)
        {
            return GetWebPublicInterface().ExportData(exportDataType, additionalParameters);
        }

        public void ExportDataAsync(string exportDataType, string[] additionalParameters)
        {
            Ix4WebService.ix4PublicInterface ws = GetWebPublicInterface();
            ws.ExportDataCompleted += OnExportDataAsyncCompleted;
            ws.ExportDataAsync(exportDataType, additionalParameters);
        }

        private void OnExportDataAsyncCompleted(object sender, ExportDataCompletedEventArgs e)
        {
            if(ExportDataAsyncCompleted!=null)
            {
                ExportDataAsyncCompleted(sender, new ExportDataAsyncCompletedEventArgs(e));
            }
        }

        public string ImportXmlRequest(byte[] xmlFileContent, string fileName)
        {
            return GetWebPublicInterface().LICSImportXMLRequest(xmlFileContent, fileName);
        }

        private Ix4WebService.ix4PublicInterface GetWebPublicInterface()
        {
            Ix4WebService.ix4PublicInterface ws = new Ix4WebService.ix4PublicInterface();
            ws.LBSoapAuthenticationHeaderValue = _header;
            ws.Proxy = System.Net.HttpWebRequest.GetSystemWebProxy();
            ws.Proxy.Credentials = CredentialCache.DefaultCredentials;
            ws.Url = _endPoint;
            return ws;
        }
    }
}
