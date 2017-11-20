using Ix4Connector;
using Ix4Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Text;

namespace WV_newDataProcessor
{
    public class GRdataExporter : DataExporter
    {
        private readonly DateTime _defaultDateTime = new DateTime(1970, 1, 1);
        private IDataTargetCollaborator _storageCollaborator;
        public GRdataExporter(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService, "GR")
        {
            _storageCollaborator = storageCollaborator;
        }

        protected override void ProcessExportedData(XDocument exportedDataDocument)
        {
            Report = new ExportDataReport(ExportDataName);
         //   List<FailureItem> failureItems = new List<FailureItem>();
            try
            {
                if (exportedDataDocument != null)
                {

                    // Get messages or from storage or from web service
                    IEnumerable<XElement> grMessages = exportedDataDocument.Descendants("MSG").ToList();
                    int messagesCount = grMessages.Count();
                    _loger.Log(string.Format("Have got {0} of {1} items", messagesCount, ExportDataName));
                    if (messagesCount > 0)
                    {
                        string itemsNoRange = string.Empty;
                        List<string> grItemNumbers = grMessages.Select(m => m.Element("MSGPos_ItemNo").Value).Distinct().ToList();

                        foreach (string grItemNo in grItemNumbers)
                        {
                            itemsNoRange = $"{itemsNoRange}'{grItemNo}',";
                        }
                        itemsNoRange = itemsNoRange.Remove(itemsNoRange.Length - 1);

                        _loger.Log($"Unique itemsNo  = {itemsNoRange}");
                        string _dbBeposConnection = @"Data Source=192.168.50.3\sql,1433;Network Library=DBMSSOCN;Initial Catalog=lms10dat; User ID=sa;Password=sa";

                        IDataTargetCollaborator beposTableCollaborator = new SqlTableCollaborator(_dbBeposConnection, null);

                        string getBeposIdQuery = string.Format("SELECT min(BEPosID) as BEPosID, ArtikelNR from BEPos WHERE Status=0 and Datum>getdate()-365 and ArtikelID is not null and ArtikelNR IN ({0}) group by ArtikelNR", itemsNoRange);
                        _loger.Log($"Bepos ids query: {getBeposIdQuery}");
                        Dictionary<string, string> bePosIds = beposTableCollaborator.GetData<Dictionary<string, string>>(string.Empty, HandleDataTableResult, getBeposIdQuery);
                        _loger.Log($"Count of keys = {bePosIds.Count}");

                        foreach (var d in bePosIds)
                        {
                            _loger.Log($" ArtikelNR = {d.Key} BEPosID = {d.Value}");
                        }




                        foreach (string itemNo in grItemNumbers.Except(bePosIds.Keys))
                        {
                            StringBuilder sb = new StringBuilder();
                            string message = $"There is no corresponds record BEPosID for ArtikelNR = {itemNo} {Environment.NewLine}";
                            sb.Append(message);
                            _loger.Log(message);
                            _loger.Log(new Exception(sb.ToString()));
                        }

                        int mark = 0;
                        foreach (var message in grMessages)
                        {
                            //Skip GR message without BEPosId
                            if(!bePosIds.Keys.Contains(message.Element("MSGPos_ItemNo").Value))
                            {
                                mark++;
                                _loger.Log($"Skip MSG WITH ItmNo = {message.Element("MSGPos_ItemNo").Value}");
                                _loger.Log($"Skipped items {mark}");
                                continue;
                            }

                            _loger.Log($"Start handle MSGPos_ItemNo = {message.Element("MSGPos_ItemNo").Value}");
                            if (message.Element("MSGPos_Supplier") == null)
                            {
                                message.Add(new XElement("MSGPos_Supplier"));
                            }

                            // if 1 => supplier should be 145001
                            if (message.Element("MSGPos_OrderType").Value == "0")
                            {
                                message.Element("MSGPos_Supplier").Value = "145001";
                            }


                            if (message.Element("MSGPos_PurchaseOrder") == null)
                            {
                                message.Add(new XElement("MSGPos_PurchaseOrder"));
                            }

                            if(message.Element("MSGPos_OrderType")?.Value!=null && !message.Element("MSGPos_OrderType").Value.Equals("1"))
                            {
                                message.Element("MSGPos_PurchaseOrder").Value = bePosIds[message.Element("MSGPos_ItemNo").Value];
                            }

                            if (message.Element("MSGHeader_LastUpdate") != null && !string.IsNullOrEmpty(message.Element("MSGHeader_LastUpdate").Value))
                            {
                                CheckLastUpdateCorrectData(message.Element("MSGHeader_LastUpdate"));
                            }

                            ShippingTypeElementConvert(message.Element("MSGPos_ShippingType"));

                            ConvertElementValueDoubleToInt(message.Element("MSGPos_Amount"));

                            ConvertElementValueDoubleToInt(message.Element("MSGPos_ResAmount"));


                            bool saveInDbResult = _storageCollaborator.SaveExportDataTransaction(message);

                            if(saveInDbResult)
                            {
                                message.Remove();
                                exportedDataDocument.Save(FileFullName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log("Export GR messages error");
                _loger.Log(ex);
                Report.OperationInfo = ex.Message;
                Report.Status = -1;
            }

            //Report.FailureItems = failureItems.ToArray();
        }

        private Dictionary<string, string> HandleDataTableResult(DataTable arg)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (arg != null)
            {
                foreach (var data in arg.AsEnumerable())
                {
                    result.Add(data["ArtikelNR"].ToString().Trim(), data["BEPosID"].ToString().Trim());
                }
            }
            return result;
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
