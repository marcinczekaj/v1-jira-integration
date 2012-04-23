/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using VersionOne.ServiceHost.Core;

namespace VersionOne.ServiceHost {
    internal class ServiceMode : ModeBase {
        internal void Start() {
            try {
                Starter.Startup();
                Starter.Logger.Log("ServiceHost running as Service");
            } catch(Exception ex) {
                throw new ApplicationException("Startup Failed", ex);
            }
        }

        internal void Stop() {
            try {
                Starter.Shutdown();
            } catch(Exception ex) {
                throw new ApplicationException("Shutdown Failed", ex);
            }
        }
    }
}