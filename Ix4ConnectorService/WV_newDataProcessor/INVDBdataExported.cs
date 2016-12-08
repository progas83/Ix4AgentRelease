using Ix4Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public class INVDBdataExported : DataExporter
    {
        IDataTargetCollaborator _storageCollaborator;
        public INVDBdataExported(IProxyIx4WebService ix4InterfaceService, IDataTargetCollaborator storageCollaborator) : base(ix4InterfaceService,"INVDB")
        {
            _storageCollaborator = storageCollaborator;
        }
        public ExportedDataReport ExportData()
        {
            //  XmlNode invdbData =  Ix4InterfaceService.ExportData(ExportDataName, null);
            //  base.SaveExportedDataToFile(invdbData);
            //  base.SaveExportedDataToFile(invdbData);
            //  base.SaveExportedDataToFile(invdbData);
            XmlDocument exportedDataDocument = GetStoredExportedData();



            if(exportedDataDocument!=null)
            {
                XDocument doc = XDocument.Parse(exportedDataDocument.InnerXml);


                var groups = from c in doc.Descendants("MSG")
                          group c by new {  p1 = c.Element("MSGHeader_Inventurnummer").Value,
                                            p2 = c.Element("MSGHeader_Mandant").Value,
                                            p3 = c.Element("MSGHeader_Bezeichnung").Value,
                                            p4 = c.Element("MSGHeader_GenDate").Value,
                                            p5 = c.Element("MSGHeader_ModDate").Value
                } into g select g;
                foreach(var groupItem in groups)
                {
                    IEnumerable<XElement> msgHeaderElemens =  groupItem.FirstOrDefault().Descendants().Where(x => x.Name.LocalName.StartsWith("MSGHeader")).ToList();
                    if (_storageCollaborator.SaveData(msgHeaderElemens, "Inventuren") > -1)
                    {
                        foreach (var posItem in groupItem)
                        {
                            IEnumerable<XElement> msgPosElemens = posItem.Descendants().Where(x => x.Name.LocalName.StartsWith("MSGPos")).ToList();
                            if (_storageCollaborator.SaveData(msgPosElemens, "Inventurpositionen") > -1)
                            {

                            }
                        }
                    }
                   
                }
                //var headerGroups = doc.Descendants("MSG").GroupBy(x=>(string)x.Element("MSGHeader_Inventurnummer")).GroupBy(x => (string)x.
                //Element("MSGHeader_Inventurnummer")), (string)x.Element("MSGHeader_Mandant"))


                XmlNodeList nodes = exportedDataDocument.GetElementsByTagName("MSG");
                while(nodes.Count!=0)
                {
                    XmlNode currentNode = nodes[0].ParentNode.RemoveChild(nodes[0]);
                }

            }
            return new ExportedDataReport();
        }
    }
}
