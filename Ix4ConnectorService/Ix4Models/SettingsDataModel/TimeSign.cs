using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public enum TimeSign
    {
        sec = 0,
        min = 60,
        hour = 3600,
        day = 86400
    }
}
