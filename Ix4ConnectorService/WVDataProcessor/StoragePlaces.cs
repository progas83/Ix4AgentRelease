using SimplestLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace WVDataProcessor
{
    public class StoragePlaces
    {
        protected static Logger _loger = Logger.GetLogger();

        private MSG ConvertToMSG(XmlNode msgNode)
        {
            MSG msgInfo = null;
            try
            {
                XmlSerializer sr = new XmlSerializer(typeof(MSG));
                TextReader tr = new StringReader(msgNode.OuterXml);
                msgInfo = (MSG)sr.Deserialize(tr);
            }
            catch (Exception ex)
            {
                _loger.Log(ex);
            }


            return msgInfo;
        }


        public XmlNodeList GetUpdatedStorageInformation(XmlNodeList articlesData)
        {


            List<ArtikelInStoragesInfo> allArtikelsInfo = new List<ArtikelInStoragesInfo>();


            XmlDocument updatedArticlesInfoDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = updatedArticlesInfoDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = updatedArticlesInfoDoc.DocumentElement;
            updatedArticlesInfoDoc.InsertBefore(xmlDeclaration, root);
            updatedArticlesInfoDoc.AppendChild(updatedArticlesInfoDoc.CreateElement("CONTENT"));


            foreach (XmlNode node in articlesData)
            {
                try
                {
                    if (node["MSGPos_ItemNo"] == null)
                    {
                        continue;
                    }
                   
                    ArtikelInStoragesInfo artikelInStoragesInfo =  allArtikelsInfo.FirstOrDefault(artikelInfo => artikelInfo.ArtikelNr.Equals(node["MSGPos_ItemNo"].LastChild.Value));
                    if(artikelInStoragesInfo!=null)
                    {
                        artikelInStoragesInfo.AddInfoToStoragePlace(node);
                    }
                    else
                    {
                        allArtikelsInfo.Add(new ArtikelInStoragesInfo(node));
                    }
                }
                catch(Exception ex)
                {
                    _loger.Log(ex);
                    _loger.Log(string.Format("Error while processing node[MSGPos_ItemNo]={0}", node["MSGPos_ItemNo"]));
                    throw new Exception("Can't handle SA messages for all storageplaces. Can't GetUpdatedStorageInformation");
                }

              
                
            }

            foreach (ArtikelInStoragesInfo artikelInStoragesInfo in allArtikelsInfo)
            {
                foreach (XmlNode artikelInfo in artikelInStoragesInfo.MakeInventarization())
                {
                    updatedArticlesInfoDoc.DocumentElement.AppendChild(updatedArticlesInfoDoc.ImportNode(artikelInfo, true));
                }
            }
            return updatedArticlesInfoDoc.GetElementsByTagName("MSG"); 
        }

    }
}
