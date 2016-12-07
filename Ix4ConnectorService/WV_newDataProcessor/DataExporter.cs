using Ix4Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WV_newDataProcessor
{
   public abstract class DataExporter
    {
        private static readonly string _archiveFolder = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ArchiveData");
        protected string ExportDataName
        { get; private set; }

        protected IProxyIx4WebService Ix4InterfaceService;
        public DataExporter(IProxyIx4WebService ix4InterfaceService, string exportDataName)
        {
            ExportDataName = exportDataName;
            Ix4InterfaceService = ix4InterfaceService;
        }

        protected string FileFullName { get { return string.Format("{0}\\{1}.xml", _archiveFolder, ExportDataName); } }
        private bool SaveExportedDataToFile(XmlNode exportedData)
        {
            bool result = false;
            if(exportedData!=null)
            {
                if (System.IO.File.Exists(FileFullName))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(FileFullName);
                    XmlDocument exportedDataDoc = new XmlDocument();
                    exportedDataDoc.LoadXml(exportedData.OuterXml);
                    XmlNodeList justExportedMessages = exportedDataDoc.GetElementsByTagName("MSG");
                    foreach (XmlNode node in justExportedMessages)
                    {

                        XmlDocumentFragment xmlDocFragment = doc.CreateDocumentFragment();
                        xmlDocFragment.InnerXml = node.OuterXml;
                        XmlElement rootElement = doc.DocumentElement;
                        rootElement.LastChild.LastChild.AppendChild(xmlDocFragment);
                    }
                    doc.Save(FileFullName);
                }
                else
                {
                    var streamWriter = new StreamWriter(new FileStream(FileFullName, FileMode.Create));
                    streamWriter.Write(exportedData.OuterXml);
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
                result = true;
            }
            return result;
        }


        protected XmlDocument GetStoredExportedData()
        {
            XmlDocument doc = null;
            XmlNode invdbData = Ix4InterfaceService.ExportData(ExportDataName, null);
            

            if(SaveExportedDataToFile(invdbData))
            {
                doc = new XmlDocument();
                doc.Load(FileFullName);
            }
           
            return doc;
        }
    }
}
