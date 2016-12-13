using System;

namespace WV_newDataProcessor
{
    public class DataReportEventArgs : EventArgs
    {
        private DataReport _dataReport;

        public DataReport Report
        {
            get { return _dataReport; }
           private set { _dataReport = value; }
        }

        public DataReportEventArgs(DataReport report)
        {
            Report = report;
        }
    }
}
