using Ix4Connector;
using Ix4Models;
using Ix4Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public class INVDBdataExporter : DataExporter
    {
        IDataTargetCollaborator _storageCollaborator;
        public INVDBdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "INVDB")
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
                    int messagesCount = exportedDataDocument.Descendants("MSG").Count();
                    Report.CountOfImportedItems = messagesCount;
                    _loger.Log(string.Format("Have got {0} of {1} items", exportedDataDocument.Descendants("MSG").Count(), ExportDataName));
                    if (messagesCount > 0)
                    {
                        var groups = from c in exportedDataDocument.Descendants("MSG")
                                     group c by new
                                     {
                                         Inventurnummer = c.Element("MSGHeader_Inventurnummer").Value,
                                         p2 = c.Element("MSGHeader_Mandant").Value,
                                         p3 = c.Element("MSGHeader_Bezeichnung").Value,
                                         p4 = c.Element("MSGHeader_GenDate").Value,
                                         p5 = c.Element("MSGHeader_ModDate").Value
                                     } into g
                                     select g;


                        foreach (var groupItem in groups)
                        {
                            IEnumerable<XElement> msgHeaderElement = groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();
                            //  XElement storedInventurHeaderElement = msgHeaderElement.FirstOrDefault(x => x.Name.LocalName.Equals("MSGHeader_Inventurnummer"));
                            //   OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save MSG to MsgHeader MSGHeader_Inventurnummer = {0}", storedInventurHeaderElement.Value));
                            if (_storageCollaborator.SaveData(msgHeaderElement, "Inventuren") > -1)
                            {
                                //  saveMsgHeaderResult.ItemOperationSuccess = true;
                                foreach (var posItem in groupItem)
                                {

                                    IEnumerable<XElement> msgPosElemens = posItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos"));
                                    XElement storedInventurpositionenElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_Position"));
                                    //  OperationResult operationResult = new OperationResult(string.Format("Save MSG to MsgPos MSGPos_Position = {0}", storedInventurpositionenElement.Value));

                                    if (_storageCollaborator.SaveData(msgPosElemens, "Inventurpositionen") > -1)
                                    {
                                        posItem.Remove();
                                        exportedDataDocument.Save(FileFullName);
                                        //  operationResult.ItemOperationSuccess = true;
                                        Report.SuccessfullHandledItems++;
                                        _loger.Log(string.Format("Inventurpositionen element with MSGPos_Position = {0} succesfully saved", storedInventurpositionenElement.Value ?? "Unknown value"));
                                    }
                                    else
                                    {
                                        FailureItem fi = new FailureItem();
                                        string resultMessage = string.Format("Can't save MsgPos. MSGPos_Position = {0}. MSGPos_Inventurnummer = {1}", posItem.Element("MSGPos_Position").Value ?? "Unknown value", posItem.Element("MSGPos_Inventurnummer").Value ?? "MSGPos_Storageplace");
                                        Report.FailureHandledItems++;
                                        fi.ExceptionMessage = resultMessage;
                                        fi.ItemContent = posItem.ToString();
                                        failureItems.Add(fi);
                                        _loger.Log(resultMessage);
                                    }
                                    //  report.Operations.Add(operationResult);
                                }

                            }
                            else
                            {
                                string resultMessage = string.Format("Can't save MsgHeader.MSGHeader_Inventurnummer = {0}", groupItem.Key.Inventurnummer);
                                FailureItem fi = new FailureItem();
                                fi.ExceptionMessage = resultMessage;
                                fi.ItemContent = groupItem.ToString();
                                failureItems.Add(fi);
                                Report.FailureHandledItems += groupItem.Count();
                                _loger.Log(resultMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Export INVDB messages error");
                _loger.Log(ex);
                Report.OperationInfo = ex.Message;
                Report.Status = -1;
            }
            Report.FailureItems = failureItems.ToArray();
        }
    }
}
