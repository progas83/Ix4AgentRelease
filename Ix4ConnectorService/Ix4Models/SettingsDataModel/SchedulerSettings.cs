using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class SchedulerSettings
    {
        public SchedulerSettings()
        {
      
        }
       
        private DateTime _startTime;

        public DateTime gg StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        private DateTime _endTime;

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        private bool _useSat;

        public bool UseSaturday
        {
            get { return _useSat; }
            set { _useSat = value; }
        }

        private bool  _useSun;

        public bool  UseSunday
        {
            get { return _useSun; }
            set { _useSun = value; }
        }

        private int _timeGap;

        public int TimeGap
        {
            get { return _timeGap; }
            set { _timeGap = value; }
        }

        private TimeSign _gapSign;

        public TimeSign GapSign
        {
            get { return _gapSign; }
            set { _gapSign = value; }
        }


    }
}
