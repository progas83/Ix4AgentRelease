using Ix4Connector;
using Ix4Models;
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


        protected override DataReport ProcessExportedData(XDocument exportedDataDocument)
        {
            DataReport report = new DataReport(string.Format("Export {0} items", ExportDataName));
            try
            {
                if (exportedDataDocument != null)
                {
                    if (exportedDataDocument.Descendants("MSG").Count() > 0)
                    {
                        _loger.Log(string.Format("Have got {0} of {1} items", exportedDataDocument.Descendants("MSG").Count(), ExportDataName));
                        var groups = from c in exportedDataDocument.Descendants("MSG")
                                     group c by new
                                     {
                                         p1 = c.Element("MSGHeader_Inventurnummer").Value,
                                         p2 = c.Element("MSGHeader_Mandant").Value,
                                         p3 = c.Element("MSGHeader_Bezeichnung").Value,
                                         p4 = c.Element("MSGHeader_GenDate").Value,
                                         p5 = c.Element("MSGHeader_ModDate").Value
                                     } into g
                                     select g;

                        foreach (var groupItem in groups)
                        {
                            IEnumerable<XElement> msgHeaderElement = groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();
                            foreach (var posItem in groupItem)
                            {

                                IEnumerable<XElement> msgPosElemens = posItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos"));
                                XElement storedInventurpositionenElement = msgPosElemens.FirstOrDefault(x => x.Name.LocalName.Equals("MSGPos_Position"));
                                OperationResult operationResult = new OperationResult(string.Format("Save MSG to MsgPos MSGPos_Position = {0}", storedInventurpositionenElement.Value));

                                if (_storageCollaborator.SaveData(msgPosElemens, "Inventurpositionen") > -1)
                                {
                                    posItem.Remove();
                                    exportedDataDocument.Save(FileFullName);
                                    operationResult.ItemOperationSuccess = true;
                                    _loger.Log(string.Format("Inventurpositionen element with MSGPos_Position = {0} succesfully saved", storedInventurpositionenElement.Value ?? "Unknown value"));
                                }
                                else
                                {
                                    operationResult.ItemOperationSuccess = false;
                                    operationResult.ItemContent = msgPosElemens.GetContent();
                                }
                                report.Operations.Add(operationResult);
                            }

                            XElement storedInventurHeaderElement = msgHeaderElement.FirstOrDefault(x => x.Name.LocalName.Equals("MSGHeader_Inventurnummer"));
                            OperationResult saveMsgHeaderResult = new OperationResult(string.Format("Save MSG to MsgHeader MSGHeader_Inventurnummer = {0}", storedInventurHeaderElement.Value));
                            if (_storageCollaborator.SaveData(msgHeaderElement, "Inventuren") > -1)
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = true;
                            }
                            else
                            {
                                saveMsgHeaderResult.ItemOperationSuccess = false;
                                saveMsgHeaderResult.ItemContent = msgHeaderElement.GetContent();
                            }

                            report.Operations.Add(saveMsgHeaderResult);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

            return report;
        }
    }
}
