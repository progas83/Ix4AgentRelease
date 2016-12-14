using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models.Reports
{
   public class ExportDataReport
    {
        private int _clientID;

        public int ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
        }

        public ExportDataReport(string exportTypeName)
        {
            ExportTypeName = exportTypeName;
        }
        private string _exportTypeName;

        public string ExportTypeName
        {
            get { return _exportTypeName; }
            set { _exportTypeName = value; }
        }

        private string _operationInfo;

        public string OperationInfo
        {
            get { return _operationInfo; }
            set { _operationInfo = value; }
        }


        public DateTime LastUpdate
        {
            get;set;
        }
        private int _status;

        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private int _importedCount;

        public int CountOfImportedItems
        {
            get { return _importedCount; }
            set { _importedCount = value; }
        }

        private int _successufulHadledItems;

        public int SuccessfullHandledItems
        {
            get { return _successufulHadledItems; }
            set { _successufulHadledItems = value; }
        }

        private int _failureHandledItems;

        public int FailureHandledItems
        {
            get { return _failureHandledItems; }
            set { _failureHandledItems = value; }
        }

        public FailureItem[] FailureItems { get; set; }

    }


    public class FailureItem
    {
        private string _itemContent;

        public string ItemContent
        {
            get { return _itemContent; }
            set { _itemContent = value; }
        }

        private string _exceptionMessage;

        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set { _exceptionMessage = value; }
        }

    }
    
}
