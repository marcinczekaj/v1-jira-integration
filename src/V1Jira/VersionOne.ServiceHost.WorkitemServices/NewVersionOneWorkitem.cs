/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;

namespace VersionOne.ServiceHost.WorkitemServices {
    public class NewVersionOneWorkitem : Workitem {

        public NewVersionOneWorkitem(string queryScopeId)
        {
            QueryScopeId = queryScopeId;
            Warnings = new List<string>();
            Messages = new List<string>();
        }

        public string QueryScopeId { get; set; }
        public bool ChangesProcessed { get; set; }

        public List<string> Warnings { get; set; }
        public List<string> Messages { get; set; }

        public override string Type { get { return "NewVersionOneWorkitem"; } }

        public override string ToString() {
            return string.Format("{0}\n\tExternal ID: {1}, Created: {2}", base.ToString(), ExternalId, Created);
        }
    }
}