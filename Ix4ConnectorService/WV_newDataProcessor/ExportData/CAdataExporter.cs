using Ix4Connector;
using Ix4Models;
using Ix4Models.Reports;
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

        protected override void ProcessExportedData(XDocument exportedDataDocument)
        {
            Report = new ExportDataReport(ExportDataName);
            List<FailureItem> failureItems = new List<FailureItem>();
            try
            {
                if (exportedDataDocument != null)
                {
                    IEnumerable<XElement> caMessages = exportedDataDocument.Descendants("MSG").ToList();
                    int messagesCount = caMessages.Count();
                    Report.CountOfImportedItems = messagesCount;
                    _loger.Log(string.Format("Have got {0} of {1} items", messagesCount, ExportDataName));
                    if (messagesCount > 0)
                    {
                        foreach (var message in caMessages)
                        {
                            ShippingTypeElementConvert(message.Element("MSGPos_ShippingType"));
                           

                            int recordHeaderNumber = _storageCollaborator.SaveData(message.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList(), "MsgHeader");
                            //   OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save CA MsgHeader record no {0}",recordHeaderNumber));
                            if (recordHeaderNumber > 0)
                            {
                                //  saveMsgHeaderResult.ItemOperationSuccess = true;
                                //   report.Operations.Add(saveMsgHeaderResult);
                                //   OperationResult saveMsgPosResult = new OperationResult(string.Format("Save CA MsgPos WakopfID ={0} ", message.Element("MSGPos_WAKopfID")!=null ? message.Element("MSGPos_WAKopfID").Value : 0.ToString()));

                                XElement headerIdElement = new XElement("MSGPos_HeaderID");
                                headerIdElement.Value = recordHeaderNumber.ToString();
                                message.Add(headerIdElement);

                                List<XElement> msgPosElemens = message.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList<XElement>();
                                if (_storageCollaborator.SaveData(msgPosElemens, "MsgPos") > 0)
                                {
                                    // saveMsgPosResult.ItemOperationSuccess = true;
                                    message.Remove();
                                    exportedDataDocument.Save(FileFullName);
                                    Report.SuccessfullHandledItems++;
                                    _loger.Log(string.Format("CA Msg POS element with MSGPos_WakopfID = {0} succesfully saved", message.Element("MSGPos_WAKopfID") != null ? message.Element("MSGPos_WAKopfID").Value : 0.ToString()));
                                }
                                else
                                {
                                    string resultMessage = string.Format("Can't save MsgPos. MSGPos_WAKopfID = {0}", message.Element("MSGPos_WAKopfID") != null ? message.Element("MSGPos_WAKopfID").Value : "Unknown WakopfID");
                                    FailureItem fi = new FailureItem();
                                    fi.ExceptionMessage = resultMessage;
                                    fi.ItemContent = message.ToString();
                                    failureItems.Add(fi);
                                    Report.FailureHandledItems++;//= groupItem.Count();
                                    _loger.Log(resultMessage);
                                }
                                //  report.Operations.Add(saveMsgPosResult);

                            }
                            else
                            {
                                string resultMessage = string.Format("Can't save MsgHeader. MSGPos_WAKopfID = {0}", message.Element("MSGPos_WAKopfID") != null ? message.Element("MSGPos_WAKopfID").Value : "Unknown WakopfID");
                                FailureItem fi = new FailureItem();
                                fi.ExceptionMessage = resultMessage;
                                fi.ItemContent = message.ToString();
                                failureItems.Add(fi);
                                Report.FailureHandledItems++;//= groupItem.Count();
                                _loger.Log(resultMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Export CA messages error");
                _loger.Log(ex);
                Report.OperationInfo = ex.Message;
                Report.Status = -1;
            }

            Report.FailureItems = failureItems.ToArray();
        }
    }
}
