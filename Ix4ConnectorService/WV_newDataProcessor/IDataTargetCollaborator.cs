using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public interface IDataTargetCollaborator
    {
        int SaveData(IEnumerable<XElement> exportedDataElements, string targetName);
    }
}
