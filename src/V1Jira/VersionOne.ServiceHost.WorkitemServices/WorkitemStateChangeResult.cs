/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.ServiceHost.WorkitemServices {
    public class WorkitemStateChangeResult : WorkitemUpdateResult {
        public WorkitemStateChangeResult(string externalId, string workitemId) {
            throw new NotImplementedException("Don't use this constructor");
        }
        
        public WorkitemStateChangeResult(string externalId, string workitemId, DateTime changeDateUtc) {
            ExternalId = externalId;
            WorkitemId = workitemId;
            ChangeDateUtc = changeDateUtc;
        }

        public string ExternalId { get; private set; }
        public bool ChangesProcessed { get; set; }
        public DateTime ChangeDateUtc { get; private set; }

        public override string ToString() {
            return string.Format("{0}\n\tExternal ID: {1}, ChangeDateUtc: {2}", base.ToString(), ExternalId, ChangeDateUtc);
        }
    }
}