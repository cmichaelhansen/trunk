using System;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;

namespace Utility.Logging.EventLogging
{
    public class EventLogger : IEventLogger
    {
        private readonly string _source;
        private readonly string _log;
        private readonly LoggingLevel _loglevel;

        public EventLogger(string source, string log, LoggingLevel logLevel)
        {
            _source = source;
            _log = log;
            _loglevel = logLevel;

            if(!EventLog.SourceExists(_source))
                EventLog.CreateEventSource(_source, _log);
                
        }

        public void Info(string message)
        {
            try
            {
                if ((int)_loglevel <= (int)LoggingLevel.Info)
                    EventLog.WriteEntry(_source, message, EventLogEntryType.Information);
            }
            catch (Win32Exception)
            {
                HandleLogFull();
            }
        }

        public void Debug(string message)
        {
            try
            {
                if ((int)_loglevel <= (int)LoggingLevel.Debug)
                    EventLog.WriteEntry(_source, message, EventLogEntryType.Information);
            }
            catch (Win32Exception)
            {
                HandleLogFull();
            }
        }

        public void Warn(string message)
        {
            try
            {
                if ((int)_loglevel <= (int)LoggingLevel.Warning)
                    EventLog.WriteEntry(_source, message, EventLogEntryType.Warning);
            }
            catch (Win32Exception)
            {
                HandleLogFull();
            }

        }

        public void Error(string message)
        {
            try
            {
                if ((int)_loglevel <= (int)LoggingLevel.Error)
                    EventLog.WriteEntry(_source, message, EventLogEntryType.Error);
            }
            catch (Win32Exception)
            {
                HandleLogFull();
            }
        }

        public void Error(string message, Exception ex)
        {
            if ((int)_loglevel <= (int)LoggingLevel.Error)
            {
                var eventLogtext = new StringBuilder(message);

                eventLogtext.AppendLine("Exception Message: " + ex.Message);
                eventLogtext.AppendLine("Exception Source: " + ex.Source);
                eventLogtext.AppendLine("Exception StackTrace: " + ex.StackTrace);
                eventLogtext.AppendLine("Exception TargetSite: " + ex.TargetSite.ToString());
                if (ex.InnerException != null)
                    eventLogtext.AppendLine("Inner Exception: " + ex.InnerException.Message);

                Error(eventLogtext.ToString());
            }
        }

        private void HandleLogFull()
        {
            // instantiate currentlog in Eventlog object so we can clear it
            var currLog = new EventLog(_log, Environment.MachineName, _source);
            currLog.Clear();
        }
    }
}
