/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Timers;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Core.Services {
    /// <summary>
    /// A service that raises an event on an interval basis.
    /// Although the Timer could cause reentrancy, our timer is protected
    /// against that.
    /// </summary>
    public class TimePublisherService : IHostedService {
        private double interval;
        private Type publishtype;
        private IEventManager eventmanager;
        private ILogger logger;
        private Timer timer;

        public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
            if(!double.TryParse(config["Interval"].InnerText, out interval)) {
                interval = -1;
            }

            publishtype = Type.GetType(config["PublishClass"].InnerText);
            eventmanager = eventManager;
            eventmanager.Subscribe(typeof(ServiceHostState), HostStateChanged);
            logger = new Logger(eventManager);
        }

        public void Start() {
            timer = new Timer(interval) {Enabled = false};
            timer.Elapsed += Timer_Elapsed;
        }

        private void HostStateChanged(object pubobj) {
            var state = (ServiceHostState)pubobj;

            if(state == ServiceHostState.Startup) {
                timer.Enabled = true;
            } else if(state == ServiceHostState.Shutdown) {
                timer.Enabled = false;
            }
        }

        // Prevents reentrancy into event handlers.
        private bool busy;

        /// <summary>
        /// Timer event that publishes the configured event.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">The timer event.</param>
        /// <remarks>If the previous event has not returned, we skip the current interval.</remarks>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            // The check-and-set of _busy is not thread-safe, but that shouldn't matter unless the inverval is set unreasonably small.
            if(busy) {
                return;
            }

            busy = true;
            var pub = Activator.CreateInstance(publishtype);
            logger.Log(LogMessage.SeverityType.Debug, string.Format("Timer Elapsed {0} {1} {2}", interval, publishtype.Name, e.SignalTime));
            eventmanager.Publish(pub);
            busy = false;
        }
    }
}