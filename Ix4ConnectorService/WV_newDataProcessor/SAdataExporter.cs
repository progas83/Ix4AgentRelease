using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ix4Connector;
using Ix4Models;
using System.Globalization;

namespace WV_newDataProcessor
{
    public class SAdataExporter : DataExporter
    {
        IDataTargetCollaborator _storageCollaborator;
        public SAdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "SA")
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
                    int messagesCount = exportedDataDocument.Descendants("MSG").Count();
                    if (messagesCount > 0)
                    {
                        _loger.Log(string.Format("Have got {0} of {1} items", messagesCount, ExportDataName));

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
                            if (groupItem.Descendants().FirstOrDefault(elem => elem.Name.LocalName.Equals("MSGPos_ItemNo")).Value == null)
                            {
                                continue;
                            }
                            IEnumerable<XElement> msgHeaderElement = groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();

                            OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save SA MsgHeader"));
                            int recordHeaderNumber = _storageCollaborator.SaveData(msgHeaderElement, "MsgHeader");

                            if (recordHeaderNumber > 0)
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = true;
                                report.Operations.Add(saveMsgHeaderResult);

                                foreach (var posItem in groupItem)
                                {

                                    XElement headerIdElement = new XElement("MSGPos_HeaderID");
                                    headerIdElement.Value = recordHeaderNumber.ToString();
                                    posItem.Add(headerIdElement);
                                    List<XElement> msgPosElemens = posItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList<XElement>();


                                    XElement amountElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_Amount"));
                                    if (amountElement.Value != null)
                                    {
                                        double d = Double.Parse(amountElement.Value, CultureInfo.InvariantCulture);
                                        amountElement.Value = Convert.ToInt32(d).ToString();
                                    }

                                    XElement resAmountElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_ResAmount"));
                                    if (amountElement.Value != null)
                                    {
                                        double d = Double.Parse(amountElement.Value, CultureInfo.InvariantCulture);
                                        amountElement.Value = Convert.ToInt32(d).ToString();
                                    }

                                    OperationResult saveMsgPos = new OperationResult(string.Format("Save SA message MsgPos item {0} ", posItem.Element("MSGPos_ItemNo").Value));

                                    if (_storageCollaborator.SaveData(msgPosElemens, "MsgPos") > 0)
                                    {
                                        posItem.Remove();
                                        exportedDataDocument.Save(FileFullName);
                                        saveMsgPos.ItemOperationSuccess = true;
                                        _loger.Log(string.Format("SA Msg POS element with MSGPos_ItemNo = {0} succesfully saved", posItem.Element("MSGPos_ItemNo").Value ?? "Unknown value"));
                                    }
                                    else
                                    {
                                        saveMsgPos.ItemOperationSuccess = false;
                                        saveMsgPos.ItemContent = msgPosElemens.GetContent();
                                    }
                                    report.Operations.Add(saveMsgPos);
                                }
                            }
                            else
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = false;
                                saveMsgHeaderResult.ItemContent = msgHeaderElement.GetContent();
                                report.Operations.Add(saveMsgHeaderResult);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Export SA messages error");
                _loger.Log(ex);
            }

            return report;
        }
    }
}
