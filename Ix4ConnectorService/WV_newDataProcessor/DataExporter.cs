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
   public delegate void OnCompleteNextOperation();
    public abstract class DataExporter
    {
        private static readonly string _archiveFolder = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ArchiveData");
        protected static Logger _loger = Logger.GetLogger();
        protected string FileFullName { get { return string.Format("{0}\\{1}.xml", _archiveFolder, ExportDataName); } }
        protected IProxyIx4WebService Ix4InterfaceService;
        public event EventHandler<DataReportEventArgs> ReportEvent;
        protected abstract DataReport ProcessExportedData(XDocument exportedData);

        public bool SettingAllowToStart { get; set; }

        protected string ExportDataName
        { get; private set; }

        public OnCompleteNextOperation NextExportOperation { get; set; }
       // protected DataExporter NextExporter { get; private set; }
        public DataExporter(IProxyIx4WebService ix4InterfaceService, string exportDataName)//, DataExporter nextExporter = null)
        {
            ExportDataName = exportDataName;
            Ix4InterfaceService = ix4InterfaceService;
            //NextExporter = nextExporter;
        }

        protected void ShippingTypeElementConvert(XElement shipingTypeElement)
        {
            try
            {
                int mSGPos_ShippingTypeField = Convert.ToInt32(shipingTypeElement.Value);

                int resultShippingType = 9;
                switch (mSGPos_ShippingTypeField)
                {
                    case 100:
                        resultShippingType = 9;
                        break;
                    case 900:
                        resultShippingType = 1;
                        break;
                    case 200:
                        resultShippingType = 5;
                        break;
                    case 800:
                        resultShippingType = 6;
                        break;
                    case 130:
                        resultShippingType = 13;
                        break;
                    case 0:
                        resultShippingType = 19;
                        break;
                    default:
                        break;

                }
                shipingTypeElement.Value = resultShippingType.ToString();
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

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
            DataReport report = new DataReport(string.Format("Start exporting {0} messages", ExportDataName));
            if(SettingAllowToStart)
            {
                if (HasStoredData())
                {
                    XDocument docc = XDocument.Load(FileFullName);
                    report = ProcessExportedData(docc);
                    OnOperationComplete(new DataReportEventArgs(report));
                }
                report = ProcessExportedData(GetStoredExportedData());
            }
            OnOperationComplete(new DataReportEventArgs(report));
            AfterExportDataOperationComplete(report);
        }

        protected virtual void AfterExportDataOperationComplete(DataReport report)
        {
            if (NextExportOperation == null)
                return;
            if(!report.HasErrors)
            {
                NextExportOperation();
            }
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
            XDocument doc = new XDocument();
            if (!HasStoredData() )
            {
                XmlNode invdbData = null;// Ix4InterfaceService.ExportData(ExportDataName, null);
                if (SaveExportedDataToFile(invdbData))
                {
                    doc = XDocument.Load(FileFullName);
                }
            }
            return doc;
        }
    }
}
