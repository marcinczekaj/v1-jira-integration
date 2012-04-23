/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Ninject;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;
using System.Reflection;

namespace VersionOne.ServiceHost {
    public sealed class CommonMode {
        public class FlushProfile { }

        private IProfileStore profileStore;
        private IList<ServiceInfo> services;
        private readonly IKernel container;

        public IEventManager EventManager { get; private set; }
        public ILogger Logger { get; private set; }

        public CommonMode(IKernel container) {
            this.container = container;
        }

        public void Initialize() {
            EventManager = new EventManager();
            Logger = new Logger(EventManager);
            services = (IList<ServiceInfo>)ConfigurationManager.GetSection("Services");
            profileStore = new XmlProfileStore("profile.xml");
        }

        public void Startup() {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            foreach(var ss in services) {

                Logger.Log(string.Format("Initializing {0} ver. {1}", ss.Name, ss.GetType().Assembly.GetName().Version));
                ss.Service.Initialize(ss.Config, EventManager, profileStore[ss.Name]);

                if(ss.Service is IComponentProvider) {
                    ((IComponentProvider) ss.Service).RegisterComponents(container);
                }

                ss.Service.Start();
                Logger.Log(string.Format("Initialized {0}", ss.Name));
            }

            EventManager.Publish(ServiceHostState.Validate);
            EventManager.Publish(ServiceHostState.Startup);
            EventManager.Subscribe(typeof(FlushProfile), FlushProfileImpl);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Logger.Log("Service Host Caught Unhandled Exception", (Exception)e.ExceptionObject);
        }

        public void Shutdown() {
            EventManager.Publish(ServiceHostState.Shutdown);
            Thread.Sleep(5 * 1000);
            profileStore.Flush();
        }

        private void FlushProfileImpl(object o) {
            profileStore.Flush();
        }
    }
}