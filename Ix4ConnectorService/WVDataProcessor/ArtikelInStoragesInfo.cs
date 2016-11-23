using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WVDataProcessor
{
    public class ArtikelInStoragesInfo
    {
        protected static Logger _loger = Logger.GetLogger();
       private Dictionary<string, MSG> _storagePlaces;
        private ArtikelInStoragesInfo()
        {
            _storagePlaces = new Dictionary<string, MSG>();
            _storagePlaces.Add("SP", null);
            _storagePlaces.Add("WE", null);
            _storagePlaces.Add("LP", null);
        }
        public ArtikelInStoragesInfo(MSG artikelInfo):this()
        {
            ArtikelNr = artikelInfo.ItemNo;
            AddInfoToStoragePlace(artikelInfo);
        }

        private string _artikelNr;

        public string ArtikelNr
        {
            get { return _artikelNr; }
            private set { _artikelNr = value; }
        }

        public void AddInfoToStoragePlace(MSG artikelInfo)
        {
            _storagePlaces[artikelInfo.Storageplace]= artikelInfo;
        }

        public IEnumerable<MSG> MakeInventarization()
        {
            List<MSG> inventurenResult = new List<MSG>();
            MSG baseArtikelInfo = _storagePlaces["LP"];
            if (baseArtikelInfo != null)
            {
                if(_storagePlaces["SP"]==null)
                {
                    MSG spArtikelInfo =(MSG) baseArtikelInfo.Clone();
                    spArtikelInfo.Storageplace = "SP";
                    spArtikelInfo.Amount = 0;
                    spArtikelInfo.ResAmount = 0;// baseArtikelInfo.ResAmount;
                 //   spArtikelInfo.ShippingType = 0;// ConvertBackShippingType(baseArtikelInfo.ShippingType);
                    _storagePlaces["SP"] = spArtikelInfo;
                }

                if(_storagePlaces["WE"]==null)
                {
                    MSG weArtikelInfo = (MSG)baseArtikelInfo.Clone();
                    weArtikelInfo.Storageplace = "WE";
                    weArtikelInfo.Amount = 0;
                    weArtikelInfo.ResAmount = 0;
                 //   weArtikelInfo.ShippingType =0;// ConvertBackShippingType(baseArtikelInfo.ShippingType);
                    _storagePlaces["WE"] = weArtikelInfo;
                }

               // baseArtikelInfo.ResAmount = 0;
                inventurenResult.Add(_storagePlaces["SP"]);
                inventurenResult.Add(_storagePlaces["LP"]);
                inventurenResult.Add(_storagePlaces["WE"]);
            }
            else
            {
                _loger.Log(string.Format("There is no base information in \"SP\" for ArtikelNr {0}", this.ArtikelNr));
            }


            return inventurenResult;
        }

        private int ConvertBackShippingType(int shippingType)
        {
            int resultShippingType = 100;
            switch (shippingType)
            {
                case 9:
                    resultShippingType = 100;
                    break;
                case 1:
                    resultShippingType = 900;
                    break;
                case 5:
                    resultShippingType = 200;
                    break;
                case 6:
                    resultShippingType = 800;
                    break;
                case 13:
                    resultShippingType = 130;
                    break;
                case 19:
                    resultShippingType = 0;
                    break;
                default:
                    break;
            }
            return resultShippingType;
        }
    }
}
