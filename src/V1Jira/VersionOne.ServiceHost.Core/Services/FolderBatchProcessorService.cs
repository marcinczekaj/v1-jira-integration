/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Xml;
using Ninject;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core {
    public abstract class FolderBatchProcessorService : IHostedService {
        private IEventManager eventManager;
        private string folderFilterPattern;
        private RecyclingFileMonitor monitor;
        private string suiteName;

        protected IEventManager EventManager {
            get { return eventManager; }
        }

        protected abstract Type EventSinkType { get; }

        public virtual void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            folderFilterPattern = config["Filter"].InnerText;
            suiteName = config["SuiteName"].InnerText;
            monitor = new RecyclingFileMonitor(profile, config["Watch"].InnerText, folderFilterPattern, Process);
            this.eventManager = eventManager;
            this.eventManager.Subscribe(EventSinkType, monitor.ProcessFolder);
        }

        public void Start() {
            // TODO move subscriptions to timer events, etc. here
        }

        private void Process(string[] files) {
            ILogger logger = new Logger(EventManager);

            try {
                logger.Log(LogMessage.SeverityType.Debug, string.Format("Starting Processing {0} files: {1}", files.Length, string.Join(",", files)));
                InternalProcess(files, suiteName);
                logger.Log(LogMessage.SeverityType.Debug, string.Format("Finished Processing {0} files: {1}", files.Length, string.Join(",", files)));
            } catch(Exception ex) {
                string message = string.Format("Failed Processing {0} files: {1}\n{2}", files.Length, string.Join(",", files), ex);
                logger.Log(LogMessage.SeverityType.Error, message);
            }
        }

        protected abstract void InternalProcess(string[] files, string suiteName);
    }
}