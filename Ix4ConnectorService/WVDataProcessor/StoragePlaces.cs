using Ix4Models;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WVDataProcessor
{
    public class StoragePlaces
    {
        protected static Logger _loger = Logger.GetLogger();
        private Dictionary<string, List<MSG>> _storages = new Dictionary<string, List<MSG>>();
        public StoragePlaces(params string[] storagesName)
        {
            foreach(string storageName in storagesName)
            {
                _storages.Add(storageName, new List<MSG>());
            }
        }

        private MSG ConvertToMSG(XmlNode msgNode)
        {
            MSG msgInfo = null;
            try
            {
                XmlSerializer sr = new XmlSerializer(typeof(MSG));
                TextReader tr = new StringReader(msgNode.OuterXml);
                msgInfo = (MSG)sr.Deserialize(tr);
            }
            catch(Exception ex)
            {
                _loger.Log(ex);
            }


            return msgInfo;
        }
    
        public XmlNodeList GetUpdatedStorageInformation(XmlNodeList articlesData)
        {

            XmlDocument updatedArticlesInfoDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = updatedArticlesInfoDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = updatedArticlesInfoDoc.DocumentElement;
            updatedArticlesInfoDoc.InsertBefore(xmlDeclaration, root);
            updatedArticlesInfoDoc.AppendChild(updatedArticlesInfoDoc.CreateElement("CONTENT"));


            foreach (XmlNode node in articlesData)
            {
                try
                {
                    MSG articleData = ConvertToMSG(node);

                    if (articlesData != null)
                    {
                        foreach (string storagePlace in _storages.Keys)
                        {
                            if (storagePlace.Equals(articleData.Storageplace))
                            {
                                MSG existedItemInCurrentStorage = _storages[storagePlace].FirstOrDefault(it => it.ItemNo.Equals(articleData.ItemNo));

                                if (existedItemInCurrentStorage != null)
                                {
                                    _storages[storagePlace].Remove(existedItemInCurrentStorage);
                                }
                                _storages[storagePlace].Add(articleData);
                            }
                            else
                            {
                                MSG existedItemInCurrentStorage = _storages[storagePlace].FirstOrDefault(it => it.ItemNo.Equals(articleData.ItemNo));
                                if (existedItemInCurrentStorage == null)
                                {
                                    MSG articleDataStub = GetCopyAllProperties(articleData);// (MSG)  articleData.Clone();
                                    articleDataStub.Amount = 0;
                                    articleDataStub.Storageplace = storagePlace;
                                    articleDataStub.ShippingType = 0;// articleData.ShippingType;
                                    _storages[storagePlace].Add(articleDataStub);

                                    string xmlContent = articleDataStub.SerializeObjectToString<MSG>();
                                    XmlDocument tempDoc = new XmlDocument();
                                    tempDoc.LoadXml(xmlContent);
                                    updatedArticlesInfoDoc.DocumentElement.AppendChild(updatedArticlesInfoDoc.ImportNode(tempDoc.DocumentElement, true));
                                }
                            }
                        }

                    }

                    XmlNode insertedNode = updatedArticlesInfoDoc.ImportNode(node, true);
                    updatedArticlesInfoDoc.DocumentElement.AppendChild(insertedNode);
                }
                catch(Exception ex)
                {
                    _loger.Log(ex);
                    _loger.Log("Need to handle SA messges from start");
                    throw new Exception("Can't handle SA messages for all storageplaces. Can't GetUpdatedStorageInformation");
                }
                
            }
            return updatedArticlesInfoDoc.GetElementsByTagName("MSG"); 
        }

        private MSG GetCopyAllProperties(MSG source)
        {
            MSG result = new MSG();
            PropertyInfo[] posProperties = source.GetType().GetProperties();
            try
            {
                foreach (PropertyInfo propInfo in posProperties)
                {
                    object propertyValue = propInfo.GetValue(source);
                    if (propertyValue != null)
                    {
                        propInfo.SetValue(result, propertyValue);
                    }
                }
            }
            catch(Exception ex)
            {

            }
           

            return result;
        }
    }
}
