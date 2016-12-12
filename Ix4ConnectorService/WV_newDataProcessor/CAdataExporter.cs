using Ix4Connector;
using Ix4Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public class CAdataExporter : DataExporter
    {
        private IDataTargetCollaborator _storageCollaborator;
        public CAdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "CA")
        {
            _storageCollaborator = storageCollaborator;
        }

        protected override DataReport ProcessExportedData(XDocument exportedDataDocument)
        {
            DataReport report = new DataReport(string.Format("Export {0} items", ExportDataName));
            try
            {
                if (exportedDataDocument != null)
                {
                    IEnumerable<XElement> caMessages = exportedDataDocument.Descendants("MSG").ToList();
                    int messagesCount = caMessages.Count();
                    _loger.Log(string.Format("Have got {0} of {1} items", messagesCount, ExportDataName));
                    if (messagesCount > 0)
                    {
                       foreach (var message in caMessages)
                        {
                            if (message.Element("MSGPos_ShippingType") != null && !string.IsNullOrEmpty(message.Element("MSGPos_ShippingType").Value))
                            {
                                ShippingTypeElementConvert(message.Element("MSGPos_ShippingType"));
                            }
                            OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save CA MsgHeader"));
                            int recordHeaderNumber = _storageCollaborator.SaveData(message.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList(), "MsgHeader");
                            if (recordHeaderNumber > 0)
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = true;
                                report.Operations.Add(saveMsgHeaderResult);
                                OperationResult saveMsgPosResult = new OperationResult(string.Format("Save CA MsgPos"));

                                XElement headerIdElement = new XElement("MSGPos_HeaderID");
                                headerIdElement.Value = recordHeaderNumber.ToString();
                                message.Add(headerIdElement);

                                List<XElement> msgPosElemens = message.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList<XElement>();
                                if (_storageCollaborator.SaveData(msgPosElemens, "MsgPos") > 0)
                                {
                                    saveMsgPosResult.ItemOperationSuccess = true;
                                    message.Remove();
                                    exportedDataDocument.Save(FileFullName);

                                    _loger.Log(string.Format("CA Msg POS element with MSGPos_WakopfID = {0} succesfully saved", message.Element("MSGPos_WAKopfID").Value ?? "Unknown value"));
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
                _loger.Log("Export CA messages error");
                _loger.Log(ex);
            }

            return report;
        }
    }
}
