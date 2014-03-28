using System;

namespace Utility.Logging.EventLogging
{
    public interface IEventLogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error(string message, Exception ex);
    }

    public enum LoggingLevel
    {
        All = 1,
        Debug = 2,
        Info = 3,
        Warning = 4,
        Error = 5
    }
}