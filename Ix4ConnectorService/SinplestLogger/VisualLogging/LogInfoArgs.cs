using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplestLogger.VisualLogging
{
    public class LogInfoArgs : EventArgs
    {
        public LogStatus Status { get; private set; }
        public string Message { get; private set; }
        public LogInfoArgs(LogStatus status, string text)
        {
            Status = status;
            Message = text;
        }
    }
}
