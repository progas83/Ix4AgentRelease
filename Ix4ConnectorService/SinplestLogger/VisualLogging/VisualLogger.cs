using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplestLogger.VisualLogging
{
    public class VisualLogger

    {
        private static VisualLogger _logger;
        private static object _padlock = new object();

        public event EventHandler<LogInfoArgs> LogEvent;

        private VisualLogger()
        {

        }
        public static VisualLogger Instance
        {
            get
            {
                if (_logger == null)
                {
                    lock (_padlock)
                    {
                        if (_logger == null)
                            _logger = new VisualLogger();
                    }
                }
                return _logger;
            }
        }

        public void Logging(LogStatus status, string logMessage)
        {
            EventHandler<LogInfoArgs> handler = LogEvent;
            if (handler != null)
            {
                handler(this, new LogInfoArgs(status, logMessage));
            }
        }
    }
}
