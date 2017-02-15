using ConnectorWorkflowManager.Properties;
using System;
using System.Text;

namespace ConnectorWorkflowManager.Mailer
{
    internal class MailReportHtml
    {
        private static string _mailReportExceptionsHtml = Resource.MailReportExceptionsHtml;
        private static string _reportContent = Resource.ReportContent;

        private StringBuilder _reportMessages = new StringBuilder();
        private string _clientName;

        public MailReportHtml(string clientName)
        {
            _clientName = clientName;
        }
        private int _msgCount = 0;
        public int MessagesCount { get { return _msgCount; } }
        public void AddMessage(ContentDescription contentDescription)
        {
            _reportMessages.Append(_reportContent.Replace("RN", (++_msgCount).ToString()).Replace("ContentDescription", contentDescription.ToString()));
        }
        public void ResetMailReport()
        {
            _msgCount = 0;
            _reportMessages = new StringBuilder();
        }
        public string GetHTMLReport()
        {
            return _mailReportExceptionsHtml.Replace("ClientName", _clientName).Replace("ReportDateTime", DateTime.Now.ToString()).Replace("ReportContent", _reportMessages.ToString());
        }
    }
}
