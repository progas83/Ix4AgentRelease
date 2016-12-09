using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV_newDataProcessor
{
    public class DataReport
    {
        public DataReport()
        {
            Operations = new List<OperationResult>();
        }

        public DataReport(string operationName):this()
        {
            OperationName = operationName;
        }
        private string _operationName;

        public string OperationName
        {
            get { return _operationName; }
            set { _operationName = value; }
        }

        public bool HasErrors
        {
            get
            {
                return Operations.FirstOrDefault(o => !o.ItemOperationSuccess) != null;
            }
        }
        private List<OperationResult> _operations;

        public List<OperationResult> Operations
        {
            get { return _operations; }
            set { _operations = value; }
        }


    }

    public class OperationResult
    {

        public OperationResult()
        {

        }
        public OperationResult(string operationItem):this()
        {
            OperationItem = operationItem;
        }
        private string _operationItem;

        public string OperationItem
        {
            get { return _operationItem; }
            set { _operationItem = value; }
        }

        private bool _operationResult;

        public bool ItemOperationSuccess
        {
            get { return _operationResult; }
            set { _operationResult = value; }
        }

        private string _itemContent;

        public string ItemContent
        {
            get { return _itemContent; }
            set { _itemContent = value; }
        }



    }
}
