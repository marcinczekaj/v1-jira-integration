using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionOne.ServiceHost.Core.Logging;

namespace IntegrationTests.Logger
{
    class LoggerImpl : VersionOne.ServiceHost.Core.Logging.ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
        public void Log(string message, Exception exception)
        {
            Console.WriteLine(message, exception);
        }
        public void Log(LogMessage.SeverityType severity, string message)
        {
            Console.WriteLine(string.Format("[{0}] {1}", severity, message));
        }
        public void Log(LogMessage.SeverityType severity, string message, Exception exception)
        {
            Console.WriteLine(string.Format("[{0}] {1}", severity, message), exception);
        }
    }
}
