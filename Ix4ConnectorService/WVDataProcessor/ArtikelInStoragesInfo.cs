using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WVDataProcessor
{
    public class ArtikelInStoragesInfo
    {
        protected static Logger _loger = Logger.GetLogger();
       private Dictionary<string, XmlNode> _storagePlaces;
        private ArtikelInStoragesInfo()
        {
            _storagePlaces = new Dictionary<string, XmlNode>();
            _storagePlaces.Add("SP", null);
            _storagePlaces.Add("WE", null);
            _storagePlaces.Add("LP", null);
        }
        public ArtikelInStoragesInfo(XmlNode artikelInfo):this()
        {
            ArtikelNr = artikelInfo["MSGPos_ItemNo"].LastChild.Value;
            AddInfoToStoragePlace(artikelInfo);
        }

        private string _artikelNr;

        public string ArtikelNr
        {
            get { return _artikelNr; }
            private set { _artikelNr = value; }
        }

        public void AddInfoToStoragePlace(XmlNode artikelInfo)
        {
            if (artikelInfo["MSGPos_Storageplace"]!=null)
            {
                _storagePlaces[artikelInfo["MSGPos_Storageplace"].LastChild.Value] = artikelInfo;
            }
        }

        public XmlNodeList MakeInventarization()
        {
            //  List<MSG> inventurenResult = new List<MSG>();
            XmlDocument doc = new XmlDocument();
            try
            {
                XmlNode baseArtikelInfo = _storagePlaces["LP"];
                if (baseArtikelInfo != null)
                {
                    if (_storagePlaces["SP"] == null)
                    {
                        XmlNode spArtikelInfo = (XmlNode)baseArtikelInfo.Clone();
                        spArtikelInfo["MSGPos_Storageplace"].LastChild.Value = "SP";
                        spArtikelInfo["MSGPos_Amount"].LastChild.Value = "0";
                        spArtikelInfo["MSGPos_ResAmount"].LastChild.Value = "0";// baseArtikelInfo.ResAmount;
                                                                                //   spArtikelInfo.ShippingType = 0;// ConvertBackShippingType(baseArtikelInfo.ShippingType);
                        _storagePlaces["SP"] = spArtikelInfo;
                    }

                    if (_storagePlaces["WE"] == null)
                    {
                        XmlNode weArtikelInfo = (XmlNode)baseArtikelInfo.Clone();
                        weArtikelInfo["MSGPos_Storageplace"].LastChild.Value = "WE";
                        weArtikelInfo["MSGPos_Amount"].LastChild.Value = "0";
                        weArtikelInfo["MSGPos_ResAmount"].LastChild.Value = "0";
                        //   weArtikelInfo.ShippingType =0;// ConvertBackShippingType(baseArtikelInfo.ShippingType);
                        _storagePlaces["WE"] = weArtikelInfo;
                    }
                    //    XmlNodeList nodeList = _storagePlaces["SP"].AppendChild(_storagePlaces["LP"]).AppendChild(_storagePlaces["WE"]).ChildNodes;
                    // baseArtikelInfo.ResAmount = 0;
                    //  inventurenResult.Add(_storagePlaces["SP"]);
                    //  inventurenResult.Add(_storagePlaces["LP"]);
                    // inventurenResult.Add(_storagePlaces["WE"]);
                }
                else
                {
                    _loger.Log(string.Format("There is no base information in \"SP\" for ArtikelNr {0}", this.ArtikelNr));
                }
                
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);
                doc.AppendChild(doc.CreateElement("CONTENT"));
                doc.DocumentElement.AppendChild(doc.ImportNode(_storagePlaces["SP"], true));
                doc.DocumentElement.AppendChild(doc.ImportNode(_storagePlaces["LP"], true));
                doc.DocumentElement.AppendChild(doc.ImportNode(_storagePlaces["WE"], true));
              //  var test = doc.GetElementsByTagName("MSG");
            }
            catch(Exception ex)
            {
                _loger.Log(ex);
            }
          

            //doc.LoadXml(_storagePlaces["SP"].InnerXml);
            //doc.AppendChild(_storagePlaces["LP"]);
            //doc.AppendChild(_storagePlaces["WP"]);
            //var test = doc.GetElementsByTagName("MSG");// _storagePlaces["SP"].AppendChild(_storagePlaces["LP"]).AppendChild(_storagePlaces["WE"]);
            return doc.GetElementsByTagName("MSG"); // doc. _storagePlaces["SP"].AppendChild(_storagePlaces["LP"]).AppendChild(_storagePlaces["WE"]).ChildNodes;// inventurenResult;
        }

        //private int ConvertBackShippingType(int shippingType)
        //{
        //    int resultShippingType = 100;
        //    switch (shippingType)
        //    {
        //        case 9:
        //            resultShippingType = 100;
        //            break;
        //        case 1:
        //            resultShippingType = 900;
        //            break;
        //        case 5:
        //            resultShippingType = 200;
        //            break;
        //        case 6:
        //            resultShippingType = 800;
        //            break;
        //        case 13:
        //            resultShippingType = 130;
        //            break;
        //        case 19:
        //            resultShippingType = 0;
        //            break;
        //        default:
        //            break;
        //    }
        //    return resultShippingType;
        //}
    }
}
