using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ix4Connector;
using Ix4Models;
using System.Globalization;
using Ix4Models.Reports;

namespace WV_newDataProcessor
{
    public class SAdataExporter : DataExporter
    {
        IDataTargetCollaborator _storageCollaborator;
        public SAdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "SA")
        {
            _storageCollaborator = storageCollaborator;
        }

        protected override void ProcessExportedData(XDocument exportedDataDocument)
        {
            //  DataReport report = new DataReport(ExportDataName));
            Report = new ExportDataReport(ExportDataName);
            List<FailureItem> failureItems = new List<FailureItem>();
            try
            {
                if (exportedDataDocument != null)
                {
                    int messagesCount = exportedDataDocument.Descendants("MSG").Count();
                    Report.CountOfImportedItems = messagesCount;
                    _loger.Log(string.Format("Have got {0} of {1} items", messagesCount, ExportDataName));
                    if (messagesCount > 0)
                    {
                        IEnumerable<XElement> elementsWIthErrors = exportedDataDocument.Descendants("MSG").Where(x => x.Element("MSGPos_ItemNo") == null || x.Element("MSGPos_ItemNo").Value == null);
                        elementsWIthErrors.Remove();
                        exportedDataDocument.Save(FileFullName);


                        var groups = from c in exportedDataDocument.Descendants("MSG").Where(x => x.Element("MSGPos_ItemNo") != null && x.Element("MSGPos_ItemNo").Value != null)
                                     group c by new
                                     {
                                         MSGHeaderCreated = c.Element("MSGHeader_Created").Value,
                                         MSGPosItemNo = c.Element("MSGPos_ItemNo").Value,
                                     } into g
                                     select g;

                        foreach (var groupItem in groups)
                        {
                            //if (groupItem.Descendants().FirstOrDefault(elem => elem.Name.LocalName.Equals("MSGPos_ItemNo")).Value == null)
                            //{
                            //    continue;
                            //}
                            IEnumerable<XElement> msgHeaderElement = groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();

                            // OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save SA MsgHeader"));
                            int recordHeaderNumber = _storageCollaborator.SaveData(msgHeaderElement, "MsgHeader");

                            if (recordHeaderNumber > 0)
                            {
                                //  saveMsgHeaderResult.ItemOperationSuccess = true;
                                //  report.Operations.Add(saveMsgHeaderResult);

                                foreach (var posItem in groupItem)
                                {

                                    XElement headerIdElement = new XElement("MSGPos_HeaderID");
                                    headerIdElement.Value = recordHeaderNumber.ToString();
                                    posItem.Add(headerIdElement);
                                    List<XElement> msgPosElemens = posItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList<XElement>();


                                    XElement amountElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_Amount"));
                                    ConvertElementValueDoubleToInt(amountElement);
                                    //if (amountElement.Value != null)
                                    //{
                                    //    double d = Double.Parse(amountElement.Value, CultureInfo.InvariantCulture);
                                    //    amountElement.Value = Convert.ToInt32(d).ToString();
                                    //}

                                    XElement resAmountElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_ResAmount"));
                                    ConvertElementValueDoubleToInt(resAmountElement);
                                    //if (resAmountElement.Value != null)
                                    //{
                                    //    double d = Double.Parse(resAmountElement.Value, CultureInfo.InvariantCulture);
                                    //    resAmountElement.Value = Convert.ToInt32(d).ToString();
                                    //}

                                    // OperationResult saveMsgPos = new OperationResult(string.Format("Save SA message MsgPos item {0} ", posItem.Element("MSGPos_ItemNo").Value));

                                    if (_storageCollaborator.SaveData(msgPosElemens, "MsgPos") > 0)
                                    {
                                        posItem.Remove();
                                        exportedDataDocument.Save(FileFullName);
                                        Report.SuccessfullHandledItems++;
                                        _loger.Log(string.Format("SA Msg POS element with MSGPos_ItemNo = {0} succesfully saved", posItem.Element("MSGPos_ItemNo").Value ?? "Unknown value"));
                                    }
                                    else
                                    {
                                        FailureItem fi = new FailureItem();
                                        string resultMessage = string.Format("Can't save MsgPos. MSGPos_ItemNo = {0}. MSGPos_Storageplace = {1}", posItem.Element("MSGPos_ItemNo").Value ?? "Unknown value", posItem.Element("MSGPos_Storageplace").Value ?? "Unknown value");
                                        Report.FailureHandledItems++;
                                        fi.ExceptionMessage = resultMessage;
                                        fi.ItemContent = posItem.ToString();
                                        failureItems.Add(fi);
                                        _loger.Log(resultMessage);
                                    }
                                }
                            }
                            else
                            {
                                string resultMessage = string.Format("Can't save MsgHeader.MSGHeaderCreated = {0}, MSGPosItemNo = {1} ", groupItem.Key.MSGHeaderCreated, groupItem.Key.MSGPosItemNo);
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
                _loger.Log("Export SA messages error");
                _loger.Log(ex);
                Report.OperationInfo = ex.Message;
                Report.Status = -1;
            }
            Report.FailureItems = failureItems.ToArray();
        }
    }
}
