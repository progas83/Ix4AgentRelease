using Ix4Models;
using Ix4Models.Interfaces;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WWDataProcessor
{
   public class DataEnsure
    {
        private string _customerName;
        private static Logger _loger = Logger.GetLogger();

        private static readonly string _archiveFolder = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ArchiveData");// @"C:\Ilya\ServiceProgram\configuration.xml";// "configuration.xml";

        // private static readonly string _archiveFolder = "ArchiveData";
        public DataEnsure(string customerName)
        {
            _customerName = customerName;
            if (!Directory.Exists(_archiveFolder))
            {
                Directory.CreateDirectory(_archiveFolder);
            }
        }




        public void RudeStoreExportedData(XmlNode nodeResult, string exportedDataType)
        {
            int countOfExceptionFiles = 0;
            string exceptionFileName = string.Empty;
            do
            {
                exceptionFileName = string.Format("{0}\\{1}_{2}_Ex{3}.xml", _archiveFolder, _customerName, exportedDataType, countOfExceptionFiles);
                countOfExceptionFiles++;
            }
            while (File.Exists(exceptionFileName));
            var _streamWriterFile = new StreamWriter(new FileStream(exceptionFileName, System.IO.FileMode.Create));
            _streamWriterFile.Write(nodeResult.OuterXml);
            _streamWriterFile.Flush();
        }
        private Dictionary<string, string> _dataFileNames = new Dictionary<string, string>();

        public bool StoreExportedNodeList(XmlNodeList nodeResult, string exportedDataType, EnsureType ensureType)
        {
            bool storeResult = false;
            string fileName = string.Format("{0}\\{1}_{2}.xml", _archiveFolder, _customerName, exportedDataType);

            try
            {
                if (ensureType == EnsureType.UpdateStoredData && File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                XmlDocument xmlDoc = new XmlDocument();
                if (!File.Exists(fileName))
                {
                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = xmlDoc.DocumentElement;
                    xmlDoc.InsertBefore(xmlDeclaration, root);
                    xmlDoc.AppendChild(xmlDoc.CreateElement("CONTENT"));
                }
                else
                {
                    xmlDoc.Load(fileName);
                }
                foreach (XmlNode node in nodeResult)
                {
                    XmlNode insertedNode = xmlDoc.ImportNode(node, true);
                    xmlDoc.DocumentElement.AppendChild(insertedNode);
                }

                xmlDoc.Save(fileName);
                if (!_dataFileNames.ContainsKey(exportedDataType))
                {
                    _dataFileNames.Add(exportedDataType, fileName);
                }
                storeResult = true;
            }
            catch (Exception ex)
            {
                _loger.Log(string.Format("Can't correctly create and save {0} file", fileName));
                _loger.Log(ex);
            }
            finally
            {

            }

            return storeResult;
        }

        //public void StoreExportedSingleNode(XmlNode nodeResult, string exportedDataType)
        //{
        //    string fileName = string.Format("{0}\\{1}_{2}.xml",_archiveFolder, _customerName, exportedDataType);

        //    try
        //    {
        //        XmlDocument xmlDoc = new XmlDocument();
        //        if (!File.Exists(fileName))
        //        {
        //            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        //            XmlElement root = xmlDoc.DocumentElement;
        //            xmlDoc.InsertBefore(xmlDeclaration, root);
        //            xmlDoc.AppendChild(xmlDoc.CreateElement("CONTENT"));
        //        }
        //        else
        //        {
        //            xmlDoc.Load(fileName);
        //        }
        //        XmlNode insertedNode = xmlDoc.ImportNode(nodeResult, true);
        //        xmlDoc.DocumentElement.AppendChild(insertedNode);

        //        xmlDoc.Save(fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        _loger.Log(string.Format("Can't correctly create and save {0} file", fileName));
        //        _loger.Log(ex);
        //    }
        //    finally
        //    {

        //    }
        //}


        public void ProcessingStoredDataToClientStorage(string exportedDataName, ExportDataToSQL dataConnector)
        {
            if (exportedDataName == "GS" && _hasGPFatalError)
            {
                _loger.Log("Can't process GS messages! Reason: GP has error");
                return;
            }
            if (dataConnector == null)
            {
                _loger.Log(string.Format("Data {0} has not been processed", exportedDataName));// "There is no stored file for data " + exportedDataName);
                _loger.Log(dataConnector, "dataCompositor");
                return;
            }
            string fileName = string.Empty;
            if (_dataFileNames.ContainsKey(exportedDataName))
            {
                fileName = _dataFileNames[exportedDataName];
            }
            else
            {
                _loger.Log("There is no stored file for data " + exportedDataName);
                return;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                XmlDocument docWithBadMsg = new XmlDocument();
                XmlDeclaration xmlDeclaration = docWithBadMsg.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = docWithBadMsg.DocumentElement;
                docWithBadMsg.InsertBefore(xmlDeclaration, root);
                docWithBadMsg.AppendChild(docWithBadMsg.CreateElement("CONTENT"));


                XmlNodeList nodes = doc.GetElementsByTagName("MSG");
                while (nodes.Count != 0)
                {
                    XmlNode currentNode = nodes[0].ParentNode.RemoveChild(nodes[0]);
                    MSG exportedMsg = dataConnector.ExportDataToCustomerSource<MSG>(currentNode);
                    if (exportedMsg == null)
                    {
                        XmlNode insertedNode = docWithBadMsg.ImportNode(currentNode, true);
                        docWithBadMsg.DocumentElement.AppendChild(insertedNode);
                    }
                    else
                    {
                        if (!exportedMsg.Saved)
                        {
                            string xmlContent = exportedMsg.SerializeObjectToString<MSG>();
                            XmlDocument tempDoc = new XmlDocument();
                            tempDoc.LoadXml(xmlContent);
                            XmlNode insertedNode = docWithBadMsg.ImportNode(tempDoc.DocumentElement, true);
                            docWithBadMsg.DocumentElement.AppendChild(insertedNode);
                        }
                        else
                        {
                            _loger.Log(string.Format("MSG {0} WAKopfID = {1} was succesfully saved ", exportedDataName, exportedMsg.WAKopfID));
                        }
                    }

                    doc.Save(fileName);
                }
                XmlNodeList badNodes = docWithBadMsg.GetElementsByTagName("MSG");
                if (exportedDataName == "GP")
                {
                    _hasGPFatalError = badNodes.Count > 0;
                }
                while (badNodes.Count != 0)
                {
                    XmlNode insertedNode = doc.ImportNode(badNodes[0].ParentNode.RemoveChild(badNodes[0]), true);
                    doc.DocumentElement.AppendChild(insertedNode);
                };
                doc.Save(fileName);
            }


            catch (Exception ex)
            {
                _loger.Log(ex);
            }
        }

        public bool _hasGPFatalError;


        private XmlNode RecoveryExceptionedExportData(string exportedDataType)
        {

            XmlSerializer ser = new XmlSerializer(typeof(XmlNode));
            XmlNode respp = (XmlNode)ser.Deserialize(new FileStream("wwinterface_SA_Ex1.xml", FileMode.Open));
            return respp;
        }
        private void TestSaveSA(XmlNode nodeResult)
        {
            string fileName = "TestML.xml";
            XmlReader reader = XmlReader.Create(new FileStream(fileName, FileMode.Open));

            //  XmlSerializer serializator = new XmlSerializer(typeof(MSG));
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                if (!File.Exists(fileName))
                {
                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = xmlDoc.DocumentElement;
                    xmlDoc.InsertBefore(xmlDeclaration, root);
                    var ch = xmlDoc.ImportNode(nodeResult.LastChild, true);
                    xmlDoc.Save(fileName);
                }
                else
                {
                    xmlDoc.Load(fileName);
                }
                XmlNode insertedNode = xmlDoc.ImportNode(nodeResult, true);
                xmlDoc.DocumentElement.AppendChild(insertedNode);
                xmlDoc.Save(fileName);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }

        }
    }

    public enum EnsureType
    {
        CollectData = 0,
        UpdateStoredData = 1

    }
}
