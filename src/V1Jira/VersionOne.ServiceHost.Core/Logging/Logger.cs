/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using VersionOne.ServiceHost.Eventing;
using Ninject;

namespace VersionOne.ServiceHost.Core.Logging {
    public class Logger : ILogger {
        private readonly IEventManager eventManager;

        [Inject]
        public Logger(IEventManager eventManager) {
            this.eventManager = eventManager;
        }

        #region ILogger Members

        public void Log(string message) {
            Log(LogMessage.SeverityType.Info, message, null);
        }

        public void Log(string message, Exception exception) {
            Log(LogMessage.SeverityType.Error, message, exception);
        }

        public void Log(LogMessage.SeverityType severity, string message) {
            Log(severity, message, null);
        }

        public void Log(LogMessage.SeverityType severity, string message, Exception exception) {
            eventManager.Publish(new LogMessage(severity, message, exception));
        }

        #endregion
    }
}