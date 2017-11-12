using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace WV_newDataProcessor
{
    public interface IDataTargetCollaborator
    {
        int SaveData(IEnumerable<XElement> exportedDataElements, string targetName);

        T GetData<T>(string fromName, Func<DataTable, T> predicate, string cmdText = "") where T : new();

        /// <summary>
        /// Save data using transaction
        /// </summary>
        /// <param name="exportedDataElements">Data need to save</param>
        /// <param name="needFindExistedMsgHeader">Many MsgPos to one MsgHeader</param>
        /// <returns>Transuction success result</returns>
        bool SaveExportDataTransaction(XElement exportedDataElements, bool needFindExistedMsgHeader = false);
    }
}
