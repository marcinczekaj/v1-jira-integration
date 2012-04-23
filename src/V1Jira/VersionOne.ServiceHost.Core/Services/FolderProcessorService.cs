/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Core {
    public abstract class FileProcessorService : IHostedService {
        private IEventManager eventManager;
        private FileMonitor monitor;

        protected IEventManager EventManager {
            get { return eventManager; }
        }

        public void Initialize(XmlElement config, IEventManager manager, IProfile profile) {
            monitor = new FileMonitor(profile, config["Watch"].InnerText, config["Filter"].InnerText, Process);
            eventManager = manager;
            eventManager.Subscribe(EventSinkType, monitor.ProcessFolder);
        }

        public void Start() {
            // TODO move subscriptions to timer events, etc. here
        }

        private void Process(string file) {
            ILogger logger = new Logger(EventManager);

            try {
                logger.Log(string.Format("Starting Processing File: {0}", file));
                InternalProcess(file);
                logger.Log(string.Format("Finished Processing File: {0}", file));
            } catch(Exception ex) {
                logger.Log(string.Format("Failed Processing File: {0}", file), ex);
            }
        }

        protected abstract void InternalProcess(string file);
        protected abstract Type EventSinkType { get; }
    }
}