/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Configuration;
using VersionOne.ServiceHost.Core;

namespace VersionOne.ServiceExecutor {
    internal class BatchMode : ModeBase {
        internal void Run() {
            try {
                InternalRun();
            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        private void InternalRun() {
            Starter.Startup();

            var startUpClass = ConfigurationSettings.AppSettings["StartUpEvent"];
            
            if(!string.IsNullOrEmpty(startUpClass)) {
                var pub = Activator.CreateInstance(Type.GetType(startUpClass));
                Starter.EventManager.Publish(pub);
            }

            Starter.Shutdown();
        }
    }
}