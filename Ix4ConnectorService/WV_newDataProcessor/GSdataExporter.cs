using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ix4Connector;
using System.Data;
using Ix4Models;

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
        protected override DataReport ProcessExportedData(XDocument exportedData)
        {
            DataReport report = new DataReport(string.Format("Export {0} items", ExportDataName));
            try
            {
                if (exportedData != null)
                {
                    if (exportedData.Descendants("MSG").Count() > 0)
                    {
                        _loger.Log(string.Format("Have got {0} of {1} items", exportedData.Descendants("MSG").Count(), ExportDataName));

                        var groupsByWakopfID = from c in exportedData.Descendants("MSG")
                                               group c by new
                                               {
                                                   WakopfID = c.Element("MSGPos_WAKopfID").Value
                                               } into g
                                               select g;

                        foreach (var groupItem in groupsByWakopfID)
                        {
                            OperationResult saveHeaderOperation = new OperationResult(string.Format("Save or find GS Msg Header"));
                            IEnumerable<XElement> msgHeaderElements = groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();
                            string wakopfID = groupItem.Key.WakopfID;
                            int existedheaderID = _storageCollaborator.GetData<int>(string.Empty, SelectHeaderID, string.Format("SELECT HeaderID FROM MsgPos WHERE HeaderID IN(SELECT ID FROM MsgHeader WHERE TYPE = 'GS')  AND WAKopfID = {0}", wakopfID));
                            if (existedheaderID <= 0)
                            {

                                existedheaderID = _storageCollaborator.SaveData(msgHeaderElements, "MsgHeader");
                            }

                            if (existedheaderID > 0)
                            {
                                saveHeaderOperation.ItemOperationSuccess = true;
                                XElement HeaderID = new XElement("MSGPos_HeaderID");
                                HeaderID.Value = existedheaderID.ToString();
                                foreach (var msgPosItem in groupItem)
                                {
                                    if (msgPosItem.Element("MSGPos_ShippingType") != null && !string.IsNullOrEmpty(msgPosItem.Element("MSGPos_ShippingType").Value))
                                    {
                                        ShippingTypeElementConvert(msgPosItem.Element("MSGPos_ShippingType"));
                                    }

                                    OperationResult savePosOperation = new OperationResult(string.Format("Save MsgPos with HeaderID = {0} ", existedheaderID));
                                    List<XElement> msgPosElements = msgPosItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList();
                                    msgPosElements.Add(HeaderID);
                                    if (_storageCollaborator.SaveData(msgPosElements, "MsgPos") > 0)
                                    {
                                        savePosOperation.ItemOperationSuccess = true;
                                        msgPosItem.Remove();
                                        exportedData.Save(FileFullName);
                                    }
                                    else
                                    {
                                        savePosOperation.ItemOperationSuccess = false;
                                        savePosOperation.ItemContent = msgPosItem.Elements().GetContent();
                                    }
                                    report.Operations.Add(savePosOperation);

                                }

                            }
                            else
                            {
                                saveHeaderOperation.ItemOperationSuccess = false;
                                saveHeaderOperation.ItemContent = groupItem.Descendants().GetContent();
                            }
                            report.Operations.Add(saveHeaderOperation);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _loger.Log("Export GS messages error");
                _loger.Log(ex);
            }

            return report;
        }

        protected override void AfterExportDataOperationComplete(DataReport report)
        {
            if (NextExportOperation == null)
                return;
            if (!report.HasErrors && report.Operations.Count > 0)
            {
                NextExportOperation();
            }
        }
    }
}
