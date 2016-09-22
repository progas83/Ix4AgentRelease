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
        //    Schedule = new Dictionary<Ix4RequestProps, int>();
           
          //  ScheduledIssues = new ScheduledItem[] { };
        //   _scheduledIssues = new ScheduledItem[] { };
        }
        // [XmlIgnore]
      //    public Dictionary<Ix4RequestProps, int> Schedule { get; set; }
        private ScheduledItem[] _scheduledIssues;
           public ScheduledItem[] ScheduledIssues
        {
            get { return _scheduledIssues; }
            set { _scheduledIssues = value; }
        }
        //{
        //    get
        //    {
        //        return Schedule.Select(item => new ScheduledItem(item.Key, item.Value)).ToArray();
        //    }
        //    set
        //    {
        //        foreach (var item in value)
        //        {
        //            if (Schedule.ContainsKey(item.scheduledProperty))
        //            {
        //                Schedule[item.scheduledProperty] = item.secValue;
        //            }
        //            else
        //            {
        //                Schedule.Add(item.scheduledProperty, item.secValue);
        //            }
                    
        //        }
        //    }
        //}
    }
}
