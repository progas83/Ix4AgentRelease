﻿using System;
using System.IO;
using System.Threading;

namespace SimplestLogger
{
    public class Logger
    {
        private static Logger _logger;
        private static object _padlock = new object();

        private const string _newLine = "      -Date: {0} | Time: {1}----";
        string _logFileName = string.Empty;
        string _currentDate = string.Empty;

        private static readonly string _loggerFileName = string.Format("{0}{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "\\logger{0}.log");// @"C:\Ilya\ServiceProgram\configuration.xml";// "configuration.xml";
        private static string LoggerFileName { get { return _loggerFileName; } }
        public static event EventHandler<Exception> LoggedExceptionEvent;

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

        /// <summary>
        /// Thread locker
        /// </summary>
        ///private ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        public void Log(string message)
        {
           /// _cacheLock.EnterWriteLock();

            lock(_streamLock)
            {
                try
                {
                    if (!_currentDate.Equals(DateTime.Now.ToShortDateString()))
                    {
                        InitCurrentLogFilePath();
                    }
                    using (var fileStream = new FileStream(_logFileName, System.IO.FileMode.Append))
                    using (var _streamWriterFile = new StreamWriter(fileStream))
                    {
                        _streamWriterFile.WriteLine(string.Format("{0}      {1}", message, string.Format(_newLine, _currentDate, DateTime.Now.ToLongTimeString())));
                        _streamWriterFile.Flush();
                    }

                }
                catch
                {

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
                newLogFileName = string.Format(LoggerFileName, i);
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
            if (LoggedExceptionEvent != null)
            {
                LoggedExceptionEvent(this, exception);
            }
            //   MailLogger.Instance.LogMail(new ContentDescription("Undescribed exception", exception.ToString()));
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
