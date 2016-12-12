using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ix4Connector;
using Ix4Models;

namespace WV_newDataProcessor
{
    public class GPdataExporter : DataExporter
    {
        private readonly DateTime _defaultDateTime = new DateTime(1970, 1, 1);
        private IDataTargetCollaborator _storageCollaborator;
        public GPdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "GP")
        {
            _storageCollaborator = storageCollaborator;
        }

        protected override void AfterExportDataOperationComplete(DataReport report)
        {
            if (NextExportOperation == null)
                return;
            if (!report.HasErrors)
            {
                NextExportOperation();
            }
        }

        protected override DataReport ProcessExportedData(XDocument exportedDataDocument)
        {
            DataReport report = new DataReport(string.Format("Export {0} items", ExportDataName));
            try
            {
                if (exportedDataDocument != null)
                {
                    IEnumerable<XElement> gpMessages = exportedDataDocument.Descendants("MSG").ToList();
                    int messagesCount = gpMessages.Count();
                    _loger.Log(string.Format("Have got {0} of {1} items", messagesCount, ExportDataName));
                    if (messagesCount > 0)
                    {


                        foreach (var message in gpMessages)
                        {
                            if (message.Element("MSGHeader_LastUpdate") != null && !string.IsNullOrEmpty(message.Element("MSGHeader_LastUpdate").Value))
                            {
                                CheckLastUpdateCorrectData(message.Element("MSGHeader_LastUpdate"));
                            }

                            if (message.Element("MSGPos_ShippingType") != null && !string.IsNullOrEmpty(message.Element("MSGPos_ShippingType").Value))
                            {
                                ShippingTypeElementConvert(message.Element("MSGPos_ShippingType"));
                            }
                            OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save GP MsgHeader"));
                            int recordHeaderNumber = _storageCollaborator.SaveData(message.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList(), "MsgHeader");
                            if (recordHeaderNumber > 0)
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = true;
                                report.Operations.Add(saveMsgHeaderResult);
                                OperationResult saveMsgPosResult = new OperationResult(string.Format("Save GP MsgPos"));

                                XElement headerIdElement = new XElement("MSGPos_HeaderID");
                                headerIdElement.Value = recordHeaderNumber.ToString();
                                message.Add(headerIdElement);

                                List<XElement> msgPosElemens = message.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList<XElement>();
                                if (_storageCollaborator.SaveData(msgPosElemens, "MsgPos") > 0)
                                {
                                    saveMsgPosResult.ItemOperationSuccess = true;
                                    message.Remove();
                                    exportedDataDocument.Save(FileFullName);

                                    _loger.Log(string.Format("GP Msg POS element with MSGPos_ItemNo = {0} succesfully saved", message.Element("MSGPos_ItemNo").Value ?? "Unknown value"));
                                }
                                else
                                {
                                    saveMsgPosResult.ItemOperationSuccess = false;
                                    saveMsgPosResult.ItemContent = msgPosElemens.GetContent();
                                }
                                report.Operations.Add(saveMsgPosResult);

                            }
                            else
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = false;
                                saveMsgHeaderResult.ItemContent = message.Descendants().GetContent();
                                report.Operations.Add(saveMsgHeaderResult);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Export GP messages error");
                _loger.Log(ex);
            }

            return report;
        }

       

        private void CheckLastUpdateCorrectData(XElement lastUpdateElement)
        {
            try
            {
                DateTime elementDT = Convert.ToDateTime(lastUpdateElement.Value);
                if (elementDT < _defaultDateTime)
                {
                    lastUpdateElement.Value = _defaultDateTime.ToString();
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }
    }
}
