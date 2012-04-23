/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.ServiceProcess;

namespace VersionOne.ServiceHost {
    internal class NtService : ServiceBase {
        private readonly ServiceMode serviceMode = new ServiceMode();

        public NtService(string shortname) {
            ServiceName = shortname;
        }

        protected override void OnStart(string[] args) {
            base.OnStart(args);

            using(new ServiceTimeRequestor(this, TimeSpan.FromMinutes(3), 15000)) {
                serviceMode.Start();
            }
        }

        protected override void OnStop() {
            serviceMode.Stop();
            base.OnStop();
        }
    }
}