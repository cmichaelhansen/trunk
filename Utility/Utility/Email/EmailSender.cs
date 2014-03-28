using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Utility.Email
{
    public class EmailSender : IDisposable 
    {
        private readonly Dictionary<string, string> _configurationSettings;
        private const string DefaultTimeOut = "100000";
        private readonly SmtpClient _smtpClient;
        private bool _disposed;

        public EmailSender(string smtpHost, string smtpPort, string smtpAuthUserName, string smtpAuthPassword, string smtpFromAddress, string smtpDisplayName, string smtpClientTimeout = DefaultTimeOut)
        {
            _configurationSettings = new Dictionary<string, string>
                {
                    {"SmtpHost", smtpHost},
                    {"SmtpPort", smtpPort},
                    {"SmtpAuthUsername", smtpAuthUserName},
                    {"SmtpAuthPassword", smtpAuthPassword},
                    {"SmtpFromAddress", smtpFromAddress},
                    {"SmtpFromDisplayName", smtpDisplayName},
                    {"SmtpClientTimeout", smtpClientTimeout}
                };

            _smtpClient = GetSmtpClient();
        }

        
        public void Send(EmailMessage emailMessage)
        {
            Send(emailMessage, _smtpClient);
        }
        
        private void Send(EmailMessage emailMessage, SmtpClient smtpClient)
        {
            MailMessage mailMessage = CreateMailMessage(emailMessage);
            smtpClient.Send(mailMessage);
        }

        private SmtpClient GetSmtpClient()
        {
            // Get the credentials
            NetworkCredential credentials = GetCredentials();

            string host, port, timeout;
            int clientTimeout;

            _configurationSettings.TryGetValue("SmtpHost", out host);
            _configurationSettings.TryGetValue("SmtpPort", out port);
            _configurationSettings.TryGetValue("SmtpClientTimeout", out timeout);

            if(int.TryParse(timeout, out clientTimeout) == false)
                clientTimeout = int.Parse(DefaultTimeOut);

            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(host, "SMTP Host cannot be null. Make sure it is set in config.");

            if (string.IsNullOrEmpty(port))
                port = "10";

            var smtpClient = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Credentials = credentials,
                    Host = host,
                    Port = int.Parse(port),
                    Timeout = clientTimeout
                };

            return smtpClient;
        }

        private NetworkCredential GetCredentials()
        {
            string username;
            string domain;
            string password;

            _configurationSettings.TryGetValue("SmtpAuthUsername", out username);
            _configurationSettings.TryGetValue("SmtpAuthPassword", out password);
            _configurationSettings.TryGetValue("SmtpAuthDomain", out domain);

            return new NetworkCredential(username, password, domain);
        }

        private MailMessage CreateMailMessage(EmailMessage emailMessage)
        {
            string fromAddress;
            string fromDisplayName;

            _configurationSettings.TryGetValue("SmtpFromAddress", out fromAddress);
            _configurationSettings.TryGetValue("SmtpFromDisplayName", out fromDisplayName);
            
            var mailMessage = new MailMessage {From = new MailAddress(fromAddress, fromDisplayName)};

            mailMessage.To.Add(emailMessage.Recipients);
            mailMessage.Subject = emailMessage.Subject;

            mailMessage.Body = emailMessage.Body;
            mailMessage.IsBodyHtml = false;

            if (emailMessage.AttachmentCollection.Any())
            {
                foreach (var attachmentFileSpec in emailMessage.AttachmentCollection)
                {
                    var attachment = new Attachment(attachmentFileSpec);
                    mailMessage.Attachments.Add(attachment);
                }
            }

            return mailMessage;
        }

        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);           
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return; 

            if (disposing) {
                _smtpClient.Dispose();
            }

            _disposed = true;
        }

        ~EmailSender()
        {
            Dispose(false);
        }

    }

    public class EmailMessage
    {
        private string _recipients;
        public string Recipients
        {
            get { return _recipients; }
            set { _recipients = value.Replace(';', ','); }
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> AttachmentCollection { get; set; } 
    }
}