using Ix4Models.Reports;
using System;

namespace Ix4Models.Reports
{
    public class DataReportEventArgs : EventArgs
    {
        private ExportDataReport _dataReport;

        public ExportDataReport Report
        {
            get { return _dataReport; }
           private set { _dataReport = value; }
        }

        public DataReportEventArgs(ExportDataReport report)
        {
            Report = report;
        }
    }
}
