/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Log4netLogger = log4net.Repository.Hierarchy.Logger;

namespace VersionOne.ServiceHost.Core.Logging {
    public class LogService : IHostedService {
        private const string LogPattern = "[%level] %date{dd-MM-yyyy HH:mm:ss} %message%n";
        private const string EnabledAttribute = "enabled";
        private const LogMessage.SeverityType DefaultSeverity = LogMessage.SeverityType.Info;

        private ILog Logger {
            get { return LogManager.GetLogger(typeof(LogService)); }
        }

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            eventManager.Subscribe(typeof(LogMessage), HandleLogMessage);
            eventManager.Subscribe(typeof(ServiceHostState), HandleServiceHostStateMessage);

            ConfigureAppenders(config);
        }

        public void Start() { }

        private void HandleLogMessage(object pubobj) {
            Log((LogMessage)pubobj);
        }

        private void HandleServiceHostStateMessage(object pubobj) {
            var state = (ServiceHostState)pubobj;

            switch(state) {
                case ServiceHostState.Startup:
                    Logger.Info("[Startup]");
                    break;
                case ServiceHostState.Shutdown:
                    Logger.Info("[Shutdown]");
                    break;
            }
        }

        private void Log(LogMessage message) {
            switch(message.Severity) {
                case LogMessage.SeverityType.Debug:
                    Logger.Debug(message.Message, message.Exception);
                    return;

                case LogMessage.SeverityType.Info:
                    Logger.Info(message.Message, message.Exception);
                    return;

                case LogMessage.SeverityType.Error:
                    Logger.Error(message.Message, message.Exception);
                    return;

                default:
                    Logger.FatalFormat("Log level {0} is not supported", message.Severity);
                    return;
            }
        }

        private void ConfigureAppenders(XmlElement config) {
            ICollection<IAppender> appenders = new List<IAppender>();
            var consoleNode = config.SelectSingleNode("//Console");

            if(IsEnabled(consoleNode)) {
                var severityString = GetValue(consoleNode, "LogLevel");
                var severity = ParseSeverity(severityString);
                appenders.Add(CreateConsoleAppender(severity));
            }

            var fileNode = config.SelectSingleNode("//File");

            if(IsEnabled(fileNode)) {
                var severityString = GetValue(fileNode, "LogLevel");
                var severity = ParseSeverity(severityString);
                var filename = GetValue(fileNode, "Filename");
                var maximumFileSize = GetValue(fileNode, "MaximumFileSize");
                appenders.Add(CreateRollingFileAppender(severity, filename, maximumFileSize));
            }

            ConfigureLogger(appenders);
        }

        /// <summary>
        ///   Decide whether current appender is enabled, true by default in case of existing node.
        /// </summary>
        private bool IsEnabled(XmlNode node) {
            if(node == null) {
                return false;
            }

            var enabledAttribute = node.Attributes[EnabledAttribute];

            int enabled;
            return enabledAttribute == null || (int.TryParse(enabledAttribute.Value, out enabled) && enabled != 0);
        }

        private LogMessage.SeverityType ParseSeverity(string severity) {
            if(string.IsNullOrEmpty(severity) || !Enum.IsDefined(typeof(LogMessage.SeverityType), severity)) {
                return DefaultSeverity;
            }

            return (LogMessage.SeverityType)Enum.Parse(typeof(LogMessage.SeverityType), severity);
        }

        private static string GetValue(XmlNode node, string childNodeName) {
            var childNode = node.SelectSingleNode(childNodeName);
            return childNode != null ? childNode.InnerText : null;
        }

        private static void ConfigureLogger(IEnumerable<IAppender> appenders) {
            var root = ((Hierarchy)LogManager.GetRepository()).Root;

            foreach(var appender in appenders) {
                root.AddAppender(appender);
            }
            DisableHibernateLogging();

            root.Repository.Configured = true;
        }

        private static void SetLoggerLevel(string loggerName, string levelName) {
            var log = LogManager.GetLogger(loggerName);
            var logger = (Log4netLogger) log.Logger;
            logger.Level = logger.Hierarchy.LevelMap[levelName];
        }

        private static void DisableHibernateLogging() {
            SetLoggerLevel("NHibernate", "Error");
            SetLoggerLevel("NHibernate.SQL", "Error");
        }

        private static IAppender CreateConsoleAppender(LogMessage.SeverityType severity) {
            var appender = new ColoredConsoleAppender();
            appender.AddMapping(new ColoredConsoleAppender.LevelColors {
                ForeColor = ColoredConsoleAppender.Colors.Red,
                Level = Level.Error,
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors {
                ForeColor = ColoredConsoleAppender.Colors.Green,
                Level = Level.Debug,
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors {
                ForeColor = ColoredConsoleAppender.Colors.White,
                Level = Level.Info,
            });

            appender.Layout = new PatternLayout(LogPattern);
            appender.Name = "Console";
            appender.Threshold = TranslateLevel(severity);
            appender.ActivateOptions();

            var filter = new LoggerMatchFilter {AcceptOnMatch = false, LoggerToMatch = "NHibernate"};
            appender.AddFilter(filter);
            filter = new LoggerMatchFilter {AcceptOnMatch = false, LoggerToMatch = "NHibernate.SQL"};
            appender.AddFilter(filter);

            return appender;
        }

        private IAppender CreateRollingFileAppender(LogMessage.SeverityType severity, string filename, string maxFileSize) {
            var appender = new RollingFileAppender {
                Layout = new PatternLayout(LogPattern),
                Name = "File",
                Threshold = TranslateLevel(severity),
                AppendToFile = true,
                File = filename,
                MaximumFileSize = maxFileSize
            };
            appender.ActivateOptions();

            return appender;
        }

        private static Level TranslateLevel(LogMessage.SeverityType severity) {
            switch(severity) {
                case LogMessage.SeverityType.Debug:
                    return Level.Debug;
                case LogMessage.SeverityType.Info:
                    return Level.Info;
                case LogMessage.SeverityType.Error:
                    return Level.Error;
                default:
                    return Level.Info;
            }
        }
    }
}