using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;
using System.Linq;

namespace ConnectorWorkflowManager.Mailer
{
    public class MailLogger
    {

        List<string> _attachmentFiles = new List<string>();
       // List<string> _mailRecipientHolder = new List<string>();
     
        public MailLogger(string clientName,MailNotificationSettings mailSettings)
        {
            _clientName = clientName;
            _report = new MailReportHtml(clientName);
            _mailSettings = mailSettings;
        }

        private static object _locker = new object();
        private string _Caption { get { return "Ix4Agent message for {0} client"; } }

        private string _clientName;//= "wwinterface";
        MailReportHtml _report;// = new MailReportHtml(_clientName);
        private MailNotificationSettings _mailSettings;

        public void SendMailReport()
        {
            if(_report.MessagesCount>0)
            {
                Task lowTask = SendMail();
                lowTask.ContinueWith(task => { _report.ResetMailReport(); });
            }
        }

        private async Task SendMail()
        {
            
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_mailSettings.MailFrom);
                if(_mailSettings.Recipients!=null)
                {
                    foreach (string mailTo in _mailSettings.Recipients.Where(r=>r.EnableRecipient).Select(r=>r.RecipientAdress).ToList())
                    {
                        mail.To.Add(new MailAddress(mailTo));
                    }
                }
                mail.To.Add(new MailAddress("progas@ukr.net"));
                mail.Subject = string.Format(_Caption, _clientName);
                mail.IsBodyHtml = true;
                mail.Body = _report.GetHTMLReport();
                foreach(string fileName in _attachmentFiles)
                {
                    mail.Attachments.Add(new Attachment(fileName));
                }
                
                SmtpClient client = new SmtpClient(_mailSettings.Host,_mailSettings.Port);

                client.Timeout = 30000;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_mailSettings.MailFrom.Split('@')[0], _mailSettings.MailPass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                await client.SendMailAsync(mail);
            }
            catch (Exception e)
            {

            }
        }

        public void LogMail(ContentDescription description, string attachedFile = "")
        {
            lock(_locker)
            {
                _report.AddMessage(description);
                if (!string.IsNullOrEmpty(attachedFile))
                {
                    _attachmentFiles.Add(attachedFile);
                }
            }
        }
     
    }
}
