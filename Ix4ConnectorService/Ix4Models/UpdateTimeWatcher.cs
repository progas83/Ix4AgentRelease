using Ix4Models.SettingsDataModel;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models
{
    public class UpdateTimeWatcher
    {
        private static Logger _loger = Logger.GetLogger();
        ImportDataSettings _importTimerSettings;
        ExportDataSettings _exportTimeSettings;
        public UpdateTimeWatcher(ImportDataSettings importTimerSettings, ExportDataSettings exportTimeSettings)
        {
            _importTimerSettings = importTimerSettings;
            _exportTimeSettings = exportTimeSettings;

            if (_articlesLastUpdate == 0)
                _articlesLastUpdate = Properties.LastUpdate.Default.Articles;
            if (_ordersLastUpdate == 0)
                _ordersLastUpdate = Properties.LastUpdate.Default.Orders;
            if (_deliveriesLastUpdate == 0)
                _deliveriesLastUpdate = Properties.LastUpdate.Default.Deliveries;

            if (_invdbLastUpdate == 0)
                _invdbLastUpdate = Properties.LastUpdate.Default.INVDB;
            if (_saLastUpdate == 0)
                _saLastUpdate = Properties.LastUpdate.Default.SA;
            if (_gpLastUpdate == 0)
                _gpLastUpdate = Properties.LastUpdate.Default.GP;
            if (_gsLastUpdate == 0)
                _gsLastUpdate = Properties.LastUpdate.Default.GS;
            if (_caLastUpdate == 0)
                _caLastUpdate = Properties.LastUpdate.Default.CA;
            if (_grLastUpdate == 0)
                _grLastUpdate = Properties.LastUpdate.Default.GR;
            //if (_deliveriesLastUpdate == 0)
            //    _deliveriesLastUpdate = Properties.LastUpdate.Default.Deliveries;
            //if (_exportDataListUpdateInfo == null)
            //{
            //    if (Properties.LastUpdate.Default.INVDB != null)
            //    {
            //        _exportDataListUpdateInfo = Properties.LastUpdate.Default.INVDB;
            //    }
            //    else
            //    {
            //        _exportDataListUpdateInfo = new EDLastUpdate(exportTimeSettings);
            //    }
            //}

            // _loger.Log(string.Format("Articles lust update TS = {0}; Orders lust update = {1}; Deliveries lust update = {2}", GetTimeFromTS(Properties.LastUpdate.Default.Articles), GetTimeFromTS(Properties.LastUpdate.Default.Orders), GetTimeFromTS(Properties.LastUpdate.Default.Deliveries)));
            _loger.Log(string.Format("Articles were updated at = {0}; Orders were updated at = {1}; Deliveries were updated at = {2}", GetTimeFromTS(Properties.LastUpdate.Default.Articles), GetTimeFromTS(Properties.LastUpdate.Default.Orders), GetTimeFromTS(Properties.LastUpdate.Default.Deliveries)));
        }

        public void SaveLastUpdateValues()
        {
            Properties.LastUpdate.Default.Articles = _articlesLastUpdate;
            Properties.LastUpdate.Default.Orders = _ordersLastUpdate;
            Properties.LastUpdate.Default.Deliveries = _deliveriesLastUpdate;
            Properties.LastUpdate.Default.INVDB = _invdbLastUpdate;
            Properties.LastUpdate.Default.SA = _saLastUpdate;
            Properties.LastUpdate.Default.GP = _gpLastUpdate;
            Properties.LastUpdate.Default.GS = _gsLastUpdate;
            Properties.LastUpdate.Default.CA = _caLastUpdate;
            Properties.LastUpdate.Default.GR = _grLastUpdate;

            Properties.LastUpdate.Default.Save();
        }
        private static long _articlesLastUpdate;// = 0;
        private static long _ordersLastUpdate;
        private static long _deliveriesLastUpdate;
        private static long _saLastUpdate;
        private static long _invdbLastUpdate;
        private static long _gpLastUpdate;
        private static long _gsLastUpdate;
        private static long _caLastUpdate;
        private static long _grLastUpdate;
        //  private static Ix4Models.EDLastUpdate _exportDataListUpdateInfo;



        //   private static long _exportGPLastUpdate = 0;
        //   private static long _exportGSLastUpdate = 0;
        //   private static long _exportSALastUpdate = 1;


        private string GetTimeFromTS(long seconds)
        {
            DateTime result = _startDT.AddSeconds(seconds);
            return result.ToString();
        }
        private static DateTime _startDT = new DateTime(1970, 1, 1);
        private static long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow.Subtract(_startDT)).TotalSeconds;
        }

        public bool TimeToCheck(string exportDataType)
        {
            bool isItTimeToCheck = false;
            ExportDataItemSettings item = null;
            foreach (ExportDataItemSettings ds in _exportTimeSettings.ExportDataItemSettings)
            {
                if (ds.ExportDataTypeName.Equals(exportDataType))
                {
                    item = ds;
                    break;
                }
            }

            if (item != null)
            {
                long currentTypeLastUpdate = 0;
                switch (exportDataType)
                {

                    case "INVDB":
                        currentTypeLastUpdate = _invdbLastUpdate;
                        break;
                    case "SA":
                        currentTypeLastUpdate = _saLastUpdate;
                        break;
                    case "GP":
                        currentTypeLastUpdate = _gpLastUpdate;
                        break;
                    case "GS":
                        currentTypeLastUpdate = _gsLastUpdate;
                        break;
                    case "CA":
                        currentTypeLastUpdate = _caLastUpdate;
                        break;
                    case "GR":
                        currentTypeLastUpdate = _grLastUpdate;
                        break;
                    default:
                        break;
                }
                if (item.IsNowWorkingTime && (currentTypeLastUpdate == 0 || (GetTimeStamp() - currentTypeLastUpdate) >= item.Scheduler.TimeGap * (int)item.Scheduler.GapSign))
                {
                    isItTimeToCheck = true;
                }
            }

            return isItTimeToCheck;
        }

        public bool TimeToCheck(Ix4RequestProps ix4Property)
        {
            // var t = GetTimeStamp();

            bool result = false;
            switch (ix4Property)
            {
                case Ix4RequestProps.Articles:
                    if (_articlesLastUpdate == 0 || (GetTimeStamp() - _articlesLastUpdate) >= _importTimerSettings.ArticleSettings.Scheduler.TimeGap * (int)_importTimerSettings.ArticleSettings.Scheduler.GapSign)
                    {
                        result = true;
                    }
                    break;
                case Ix4RequestProps.Deliveries:
                    if (_deliveriesLastUpdate == 0 || (GetTimeStamp() - _deliveriesLastUpdate) > _importTimerSettings.DeliverySettings.Scheduler.TimeGap * (int)_importTimerSettings.DeliverySettings.Scheduler.GapSign)
                    {
                        result = true;
                    }
                    break;
                case Ix4RequestProps.Orders:
                    if (_ordersLastUpdate == 0 || (GetTimeStamp() - _ordersLastUpdate) > _importTimerSettings.OrderSettings.Scheduler.TimeGap * (int)_importTimerSettings.OrderSettings.Scheduler.GapSign)
                    {
                        result = true;
                    }
                    break;
                default:
                    break;

            }
            return result;
        }

        public void SetLastUpdateTimeProperty(Ix4RequestProps ix4Property)
        {
            switch (ix4Property)
            {
                case Ix4RequestProps.Articles:
                    _articlesLastUpdate = GetTimeStamp();
                    break;
                case Ix4RequestProps.Deliveries:
                    _deliveriesLastUpdate = GetTimeStamp();
                    break;
                case Ix4RequestProps.Orders:
                    _ordersLastUpdate = GetTimeStamp();
                    break;
                default:
                    break;
            }
        }

        public void SetLastUpdateTimeProperty(string exportDataType)
        {
            try
            {
                switch (exportDataType)
                {
                    case "INVDB":
                        _invdbLastUpdate = GetTimeStamp();
                        break;
                    case "SA":
                        _saLastUpdate = GetTimeStamp();
                        break;
                    case "GP":
                        _gpLastUpdate = GetTimeStamp();
                        break;
                    case "GS":
                        _gsLastUpdate = GetTimeStamp();
                        break;
                    case "CA":
                        _caLastUpdate = GetTimeStamp();
                        break;
                    case "GR":
                        _grLastUpdate = GetTimeStamp();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
