using Ix4Models;
using SimplestLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorWorkflowManager
{
    static class UpdateTimeWatcher
    {
        private static Logger _loger = Logger.GetLogger();
        static UpdateTimeWatcher()
        {
            DateTime yesturdayHalfPastSeventeen = (new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 17, 30, 00)).AddDays(-1);
            _articlesLastUpdate = (long)yesturdayHalfPastSeventeen.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            _loger.Log("Articles last update total seconds = " + _articlesLastUpdate);

            DateTime yesturdayTenPM = (new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 7, 00, 00)).AddDays(-1);
            _exportSALastUpdate = (long)yesturdayTenPM.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        private static long _articlesLastUpdate = 1;
        private static long _ordersLastUpdate = 0;
        private static long _deliveriesLastUpdate = 0;

        private static long _exportGPLastUpdate = 0;
        private static long _exportGSLastUpdate = 0;
        private static long _exportSALastUpdate = 1;




        private static long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static bool TimeToCheck(string exportDataType)
        {
            bool isItTimeToCheck = false;
            switch (exportDataType)
            {
                case "GP":
                    if (_exportGPLastUpdate == 0 || (GetTimeStamp() - _exportGPLastUpdate) > 1700)
                    {
                        isItTimeToCheck = true;
                    }
                    else
                    {
                        isItTimeToCheck = true;
                    }
                    break;
                case "GS":
                    if (_exportGSLastUpdate == 0 || (GetTimeStamp() - _exportGSLastUpdate) > 1700)
                    {
                        isItTimeToCheck = true;
                    }
                    else
                    {
                        isItTimeToCheck = true;
                    }
                    break;
                case "SA":
                    if (_exportSALastUpdate == 0 || GetTimeStamp() - _exportSALastUpdate > 86400)
                    {
                        isItTimeToCheck = true;
                    }
                    break;
                default:
                    break;
            }
            return isItTimeToCheck;
        }

        public static bool TimeToCheck(Ix4RequestProps ix4Property)
        {
            bool result = false;
            switch (ix4Property)
            {
                case Ix4RequestProps.Articles:
                    if (_articlesLastUpdate == 0 || GetTimeStamp() - _articlesLastUpdate > 86400)
                    {
                        result = true;
                    }
                    break;
                case Ix4RequestProps.Deliveries:
                    if (_deliveriesLastUpdate == 0 || (GetTimeStamp() - _deliveriesLastUpdate) > 7200)
                    {
                        result = true;
                    }
                    break;
                case Ix4RequestProps.Orders:
                    if (_ordersLastUpdate == 0 || (GetTimeStamp() - _ordersLastUpdate) > 1800)
                    {
                        result = true;
                    }
                    break;
                default:
                    break;

            }
            return result;
        }

        public static void SetLastUpdateTimeProperty(Ix4RequestProps ix4Property)
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

        public static void SetLastUpdateTimeProperty(string exportDataType)
        {
            switch (exportDataType)
            {
                case "GP":
                    _exportGPLastUpdate = GetTimeStamp();
                    break;
                case "GS":
                    _exportGSLastUpdate = GetTimeStamp();
                    break;
                case "SA":
                    _exportSALastUpdate = GetTimeStamp();
                    break;
                default:
                    break;
            }
        }
    }
}
