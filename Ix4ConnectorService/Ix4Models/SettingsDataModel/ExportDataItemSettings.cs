using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class ExportDataItemSettings
    {
        public ExportDataItemSettings()
        {

        }
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        private string _exportDataTypeName;

        public string ExportDataTypeName
        {
            get { return _exportDataTypeName; }
            set { _exportDataTypeName = value; }
        }

        private string[] _additionalParameters;

        public string[] AdditionalParameters
        {
            get { return _additionalParameters; }
            set { _additionalParameters = value; }
        }


        private SchedulerSettings _scheduler;

        public SchedulerSettings Scheduler
        {
            get { return _scheduler; }
            set { _scheduler = value; }
        }

        [XmlIgnore]
        public bool IsNowWorkingTime
        {
            get
            {
                return true;
            }
        }

        private static long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        private static long GetTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
