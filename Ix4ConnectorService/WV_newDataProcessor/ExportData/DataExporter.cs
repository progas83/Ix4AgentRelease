using Ix4Connector;
using Ix4Models.Reports;
using SimplestLogger;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public abstract class DataExporter
    {
        private static readonly string _archiveFolder = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ArchiveData");
        protected static Logger _loger = Logger.GetLogger();
        protected string FileFullName { get { return string.Format("{0}\\{1}.xml", _archiveFolder, ExportDataName); } }
        private IProxyIx4WebService _ix4InterfaceService;
        public event EventHandler<DataReportEventArgs> ExportOperationReportEvent;
        protected abstract void ProcessExportedData(XDocument exportedData);

        public bool SettingAllowToStart { get; set; }

        protected string ExportDataName
        { get; private set; }

        public Action NextExportOperation { get; set; }

        public DataExporter(IProxyIx4WebService ix4InterfaceService, string exportDataName)
        {
            ExportDataName = exportDataName;
            _ix4InterfaceService = ix4InterfaceService;
            CheckArchiveFolder();

        }
        private void CheckArchiveFolder()
        {
            if (!Directory.Exists(_archiveFolder))
            {
                Directory.CreateDirectory(_archiveFolder);
            }
        }
        protected ExportDataReport Report { get; set; }

        protected void ConvertElementValueDoubleToInt(XElement element)
        {
            if (element != null)
            {
                double d = Double.Parse(element.Value, CultureInfo.InvariantCulture);
                element.Value = Convert.ToInt32(d).ToString();
            }
        }

        protected void ShippingTypeElementConvert(XElement shipingTypeElement)
        {
            if (shipingTypeElement != null)
            {
                int mSGPos_ShippingTypeField = Convert.ToInt32(shipingTypeElement.Value);

                int resultShippingType = 9;
                switch (mSGPos_ShippingTypeField)
                {
                    case 900:
                        resultShippingType = 6;
                        break;

                    case 200:
                        resultShippingType = 5;
                        break;
                    case 800:
                    case 801:
                        resultShippingType = 1;
                        break;
                    case 130:
                        resultShippingType = 13;
                        break;

                    case 0:
                        resultShippingType = 19;
                        break;

                    case 100:
                        resultShippingType = 9;
                        break;
                    case 110:
                        resultShippingType = 20;
                        break;
                    default:
                        resultShippingType = 9;
                        break;

                }
                shipingTypeElement.Value = resultShippingType.ToString();
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
                    using (var streamWriter = new StreamWriter(new FileStream(FileFullName, FileMode.Create)))
                    {
                        streamWriter.Write(exportedData.OuterXml);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                MakeReserveCopyFile(exportedData);
                result = true;
            }
            return result;
        }

        private void MakeReserveCopyFile(XmlNode exportedData)
        {
            try
            {
                string dataFileName = string.Empty;
                int attemptLookForFile = 0;
                do
                {
                    attemptLookForFile++;
                    dataFileName = string.Format("{0}{1}", FileFullName, attemptLookForFile);
                }
                while (File.Exists(dataFileName));
                using (var streamWriter = new StreamWriter(new FileStream(dataFileName, FileMode.Create)))
                {
                    streamWriter.Write(exportedData.OuterXml);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Exception in MakeReserveCopyFile");
                _loger.Log(ex);
            }

        }
        public void ExportData()
        {
            //ExportDataReport report = new ExportDataReport(ExportDataName);
            if (SettingAllowToStart)
            {
                if (HasStoredData())
                {
                    XDocument docc = XDocument.Load(FileFullName);
                    ProcessExportedData(docc);
                    SendReportOnOperationComlete();
                }
                ProcessExportedData(GetStoredExportedData());
                SendReportOnOperationComlete();
                AfterExportDataOperationComplete();
            }

        }

        protected virtual void AfterExportDataOperationComplete()
        {
            if (NextExportOperation == null)
                return;
            if (Report.Status >= 0 && Report.CountOfFailures == 0)//&& Report.SuccessfullHandledItems>0)
            {
                NextExportOperation();
            }
        }

        private void SendReportOnOperationComlete()
        {
            if (Report != null)
            {
                Report.OperationDate = DateTime.Now;
                EventHandler<DataReportEventArgs> reportEvent = ExportOperationReportEvent;
                if (reportEvent != null)
                {
                    reportEvent(this, new DataReportEventArgs(Report));
                }
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
            if (!HasStoredData())
            {
                try
                {
                    XmlNode invdbData = _ix4InterfaceService.ExportData(ExportDataName, null);
                    if (SaveExportedDataToFile(invdbData))
                    {
                        doc = XDocument.Load(FileFullName);
                    }
                }
                catch (Exception ex)
                {
                    _loger.Log(ex);
                }

            }
            return doc;
        }
    }
}
