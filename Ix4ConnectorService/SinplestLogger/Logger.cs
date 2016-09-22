using Ix4Models;
using Ix4Models.Enums;
using SinplestLogger.Mailer;
using System;
using System.IO;

namespace SimplestLogger
{
    public class Logger
    {
        private static Logger _logger;
        private static object _padlock = new object();
        private static StreamWriter _streamWriterFile;
        private const string _newLine = "      -Date: {0} | Time: {1}----";
        string _logFileName = string.Empty;
        string _currentDate = string.Empty;

       
        private Logger()
        {
        }

        public static Logger GetLogger()
        {

            if (_logger == null)
            {
                lock (_padlock)
                {
                    if (_logger == null)
                    {
                        _logger = new Logger();
                    }
                }
            }

            return _logger;
        }

      
        private static object _streamLock = new object();

        public void Log(string message)
        {
            lock (_streamLock)
            {
                if (!_currentDate.Equals(DateTime.Now.ToShortDateString()))
                {
                    InitCurrentLogFilePath();
                }
                try
                {
                    _streamWriterFile = new StreamWriter(new FileStream(_logFileName, System.IO.FileMode.Append));
                    _streamWriterFile.WriteLine(string.Format("{0}      {1}", message, string.Format(_newLine, _currentDate, DateTime.Now.ToShortTimeString())));
                    _streamWriterFile.Flush();

                }
                finally
                {
                    if (_streamWriterFile != null)
                    {
                        _streamWriterFile.Close();
                        _streamWriterFile.Dispose();
                        _streamWriterFile = null;
                    }
                }
            }
        }


        private void InitCurrentLogFilePath()
        {
            int i = 0;
            string newLogFileName = string.Empty;

            do
            {
                i++;
                newLogFileName = string.Format(CurrentServiceInformation.LoggerFileName, i);
            }
            while (File.Exists(newLogFileName));
            _logFileName = newLogFileName;
            _currentDate = DateTime.Now.ToShortDateString();
        }

        public void Log(Exception exception)
        {
            Log(exception.ToString());
       //     Log("Exception message");
       //     Log(exception.Message);
            MailLogger.Instance.LogMail(new ContentDescription("Undescribed exception", exception.ToString()));
        }

        public void Log(object o, string propertyName)
        {
            if (o == null)
            {
                string res = string.Format("This property {0} is null", propertyName);
                Log(res);
            }
        }
    }
}
