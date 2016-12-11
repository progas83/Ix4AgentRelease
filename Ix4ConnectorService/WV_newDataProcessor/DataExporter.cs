using Ix4Connector;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public abstract class DataExporter
    {
        private static readonly string _archiveFolder = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ArchiveData");
        protected static Logger _loger = Logger.GetLogger();
        protected string FileFullName { get { return string.Format("{0}\\{1}.xml", _archiveFolder, ExportDataName); } }
        protected IProxyIx4WebService Ix4InterfaceService;
        public event EventHandler<DataReportEventArgs> ReportEvent;
        protected abstract DataReport ProcessExportedData(XDocument exportedData);


        protected string ExportDataName
        { get; private set; }


        public DataExporter(IProxyIx4WebService ix4InterfaceService, string exportDataName)
        {
            ExportDataName = exportDataName;
            Ix4InterfaceService = ix4InterfaceService;
        }


        private bool SaveExportedDataToFile(XmlNode exportedData)
        {
            bool result = false;
            if (exportedData != null)
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
        public void ExportData()
        {
            DataReport report = new DataReport();
            if (HasStoredData())
            {
                XDocument docc = XDocument.Load(FileFullName);
                //XmlDocument doc = new XmlDocument();
               // doc.Load(FileFullName);
                report = ProcessExportedData(docc);
                OnOperationComplete(new DataReportEventArgs(report));
            }
            report = ProcessExportedData(GetStoredExportedData());
            OnOperationComplete(new DataReportEventArgs(report));

        }

        protected virtual void OnOperationComplete(DataReportEventArgs e)
        {
            EventHandler<DataReportEventArgs> reportEvent = ReportEvent;
            if (reportEvent != null)
            {
                reportEvent(this, e);
            }
        }

       

        private bool HasStoredData()
        {
            bool result = false;
            if (System.IO.File.Exists(FileFullName))
            {
                XDocument doc = XDocument.Load(FileFullName);
                
                if (doc.Descendants("MSG").Count() > 0)
                {
                    result = true;
                }
                else
                {
                    File.Delete(FileFullName);
                }
            }
            return result;
        }

        private XDocument GetStoredExportedData()
        {
            XDocument doc = new XDocument();// null;
            if (!HasStoredData() )//(&& !ExportDataName.Equals("GP")))
            {
                XmlNode invdbData = Ix4InterfaceService.ExportData(ExportDataName, null);
                if (SaveExportedDataToFile(invdbData))
                {
                    doc = XDocument.Load(FileFullName);
                }
            }
            return doc;
        }
    }
}
