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
                      

                        var groups = from c in exportedDataDocument.Descendants("MSG").Where(x=>x.Element("MSGPos_ItemNo")!=null && x.Element("MSGPos_ItemNo").Value != null)
                                     group c by new
                                     {
                                         p1 = c.Element("MSGHeader_Created").Value,
                                         p2 = c.Element("MSGPos_ItemNo").Value,
                                     } into g
                                     select g;

                        foreach (var groupItem in groups)
                        {
                            if(groupItem.Descendants().FirstOrDefault(elem=>elem.Name.LocalName.Equals("MSGPos_ItemNo")).Value==null)
                            {
                                continue;
                            }
                            IEnumerable<XElement> msgHeaderElement = groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();

                            //  XElement storedMsgHeaderElement = msgHeaderElement.FirstOrDefault(x => x.Name.LocalName.Equals("MSGHeader_Inventurnummer"));
                            OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save SA MsgHeader"));
                            int recordHeaderNumber = _storageCollaborator.SaveData(msgHeaderElement, "MsgHeader");

                            if (recordHeaderNumber > 0)
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = true;
                            }
                            else
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = false;
                                saveMsgHeaderResult.ItemContent = msgHeaderElement.GetContent();
                            }



                            foreach (var posItem in groupItem)
                            {

                                List<XElement> msgPosElemens = posItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList<XElement>();
                                XElement headerIdElement = new XElement("MSGPos_HeaderID");
                                headerIdElement.Value = recordHeaderNumber.ToString();
                                XElement amountElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_Amount"));
                                if(amountElement.Value!=null)
                                {
                                    double d = Double.Parse(amountElement.Value, CultureInfo.InvariantCulture);
                                    amountElement.Value = Convert.ToInt32(d).ToString();
                                }

                                amountElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_ResAmount"));
                                if (amountElement.Value != null)
                                {
                                    double d = Double.Parse(amountElement.Value, CultureInfo.InvariantCulture);
                                    amountElement.Value = Convert.ToInt32(d).ToString();
                                }
                                //  string dd = String.Format("{0:F20}", amountElement.Value);
                                //Decimal.Parse("1.2345E-02", System.Globalization.NumberStyles.Float);
                                //var amountValue =   double.Parse(amountElement.Value, System.Globalization.NumberStyles.Float);
                                // double amountValue = Convert.ToDouble(amountElement.Value, System.Globalization.NumberStyles.Float);
                                // amountElement.Value = Convert.ToInt32(amountValue).ToString();
                                msgPosElemens.Add(headerIdElement);
                               
                               // msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_Position")).Value = recordHeaderNumber.ToString();


                               // XElement storedInventurpositionenElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_Position"));
                                OperationResult saveMsgPos = new OperationResult(string.Format("Save SA message MsgPos item {0} ", posItem.Element("MSGPos_ItemNo").Value));

                                if (_storageCollaborator.SaveData(msgPosElemens, "MsgPos") > 0)
                                {
                                    posItem.Remove();
                                    exportedDataDocument.Save(FileFullName);
                                    saveMsgPos.ItemOperationSuccess = true;
                                   // _loger.Log(string.Format("Inventurpositionen element with MSGPos_Position = {0} succesfully saved", storedInventurpositionenElement.Value ?? "Unknown value"));
                                }
                                else
                                {
                                    saveMsgPos.ItemOperationSuccess = false;
                                    saveMsgPos.ItemContent = msgPosElemens.GetContent();
                                }
                                report.Operations.Add(saveMsgPos);
                            }


                            report.Operations.Add(saveMsgHeaderResult);
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
