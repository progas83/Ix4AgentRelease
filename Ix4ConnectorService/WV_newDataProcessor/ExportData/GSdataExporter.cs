using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ix4Connector;
using System.Data;
using Ix4Models;
using Ix4Models.Reports;

namespace WV_newDataProcessor
{
    public class GSdataExporter : DataExporter
    {
        private IDataTargetCollaborator _storageCollaborator;
        public GSdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "GS")
        {
            _storageCollaborator = storageCollaborator;
        }

        private int SelectHeaderID(DataTable tableData)
        {
            int headerId = -1;
            if (tableData != null)
            {
                foreach (DataRow row in tableData.AsEnumerable())
                {
                    if (row["HeaderID"] != null)
                    {
                        headerId = Convert.ToInt32(row["HeaderID"]);
                        break;
                    }
                }
            }
            return headerId;
        }
        protected override void ProcessExportedData(XDocument exportedData)
        {
            Report = new ExportDataReport(ExportDataName);
            List<FailureItem> failureItems = new List<FailureItem>();
            try
            {
                if (exportedData != null)
                {
                    int messagesCount = exportedData.Descendants("MSG").Count();
                    Report.CountOfImportedItems = messagesCount;
                    _loger.Log(string.Format("Have got {0} of {1} items", exportedData.Descendants("MSG").Count(), ExportDataName));
                    if (messagesCount > 0)
                    {

                        var groupsByWakopfID = from c in exportedData.Descendants("MSG")
                                               group c by new
                                               {
                                                   WakopfID = c.Element("MSGPos_WAKopfID").Value
                                               } into g
                                               select g;

                        foreach (var groupItem in groupsByWakopfID)
                        {
                            //OperationResult saveHeaderOperation = new OperationResult(string.Format("Save or find GS Msg Header"));
                            IEnumerable<XElement> msgHeaderElements = groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();
                            string wakopfID = groupItem.Key.WakopfID;
                            int existedheaderID = _storageCollaborator.GetData<int>(string.Empty, SelectHeaderID, string.Format("SELECT HeaderID FROM MsgPos WHERE HeaderID IN(SELECT ID FROM MsgHeader WHERE TYPE = 'GS')  AND WAKopfID = {0}", wakopfID));
                            if (existedheaderID <= 0)
                            {

                                existedheaderID = _storageCollaborator.SaveData(msgHeaderElements, "MsgHeader");
                            }

                            if (existedheaderID > 0)
                            {
                                // saveHeaderOperation.ItemOperationSuccess = true;
                                XElement HeaderID = new XElement("MSGPos_HeaderID");
                                HeaderID.Value = existedheaderID.ToString();
                                foreach (var msgPosItem in groupItem)
                                {
                                    ShippingTypeElementConvert(msgPosItem.Element("MSGPos_ShippingType"));
                                    ConvertElementValueDoubleToInt(msgPosItem.Element("MSGPos_Amount"));

                                    ConvertElementValueDoubleToInt(msgPosItem.Element("MSGPos_ResAmount"));
                                    //  OperationResult savePosOperation = new OperationResult(string.Format("Save MsgPos with HeaderID = {0} ", existedheaderID));
                                    msgPosItem.Add(HeaderID);
                                    exportedData.Save(FileFullName);
                                    List<XElement> msgPosElements = msgPosItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList();
                                    //msgPosItem.Add(HeaderID);
                                    if (_storageCollaborator.SaveData(msgPosElements, "MsgPos") > 0)
                                    {
                                        // savePosOperation.ItemOperationSuccess = true;
                                        Report.SuccessfullHandledItems++;
                                        msgPosItem.Remove();
                                        exportedData.Save(FileFullName);
                                    }
                                    else
                                    {
                                        string resultMessage = string.Format("Can't save MsgHPos.MSGPos_WAKopfID = {0}, MSGPos_HeaderID = {1}", groupItem.Key.WakopfID, existedheaderID);
                                        FailureItem fi = new FailureItem();
                                        fi.ExceptionMessage = resultMessage;
                                        fi.ItemContent = msgPosItem.ToString();
                                        failureItems.Add(fi);
                                        Report.FailureHandledItems++;//= groupItem.Count();
                                        _loger.Log(resultMessage);
                                    }

                                }

                            }
                            else
                            {
                                string resultMessage = string.Format("Can't save MsgHeader.MSGPos_WAKopfID = {0}", groupItem.Key.WakopfID);
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
                _loger.Log("Export GS messages error");
                _loger.Log(ex);
                Report.OperationInfo = ex.Message;
                Report.Status = -1;
            }
            Report.FailureItems = failureItems.ToArray();
        }

        //protected override void AfterExportDataOperationComplete(DataReport report)
        //{
        //    if (NextExportOperation == null)
        //        return;
        //    //   if (!report.HasErrors && report.Operations.Count > 0)
        //    if (!report.HasErrors)// && report.Operations.Count > 0)
        //    {
        //        NextExportOperation();
        //    }
        //}
    }
}
