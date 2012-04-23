/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.ServiceProcess;
using System.Timers;

namespace VersionOne.ServiceHost {
    public class ServiceTimeRequestor : IDisposable {
        private readonly Timer timer;
        private readonly ServiceBase service;
        private int allowedTimeMilliseconds;
        private readonly int requestIntervalMilliseconds;

        public ServiceTimeRequestor(ServiceBase service, TimeSpan allowedTime, int requestIntervalMilliseconds) {
            this.service = service;
            this.requestIntervalMilliseconds = requestIntervalMilliseconds;

            allowedTimeMilliseconds = Convert.ToInt32(allowedTime.TotalMilliseconds);

            timer = new Timer();
            timer.Elapsed += (sender, e) => RequestAdditionalTime(requestIntervalMilliseconds);
            timer.Interval = requestIntervalMilliseconds;
            timer.Enabled = true;
        }

        private void RequestAdditionalTime(int requestTimeMillis) {
            if (allowedTimeMilliseconds <= 0) {
                return;
            }

            service.RequestAdditionalTime(requestTimeMillis);
            allowedTimeMilliseconds -= requestIntervalMilliseconds;
        }

        public void Dispose() {
            timer.Enabled = false;
        }
    }
}