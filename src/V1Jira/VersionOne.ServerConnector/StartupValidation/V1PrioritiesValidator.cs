/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Linq;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServerConnector.StartupValidation {
    public class V1PrioritiesValidator : BaseValidator {
        private readonly ICollection<MappingInfo> priorities;

        public V1PrioritiesValidator(ICollection<MappingInfo> priorities) {
            this.priorities = priorities;
        }

        public override bool Validate() {
            Logger.Log(LogMessage.SeverityType.Info, "Checking VersionOne priorities");
            var result = true;
            var v1Priorities = V1Processor.GetWorkitemPriorities();

            foreach(var priority in priorities.Where(priority => !PriorityExists(v1Priorities, priority.Id))) {
                Logger.Log(LogMessage.SeverityType.Error, string.Format("Cannot find VersionOne priority with identifier {0}", priority.Id));
                result = false;
            }

            Logger.Log(LogMessage.SeverityType.Info, "VersionOne priorities are checked");
            return result;
        }

        //TODO move to helper class and combine with StatusExists
        private static bool PriorityExists(IEnumerable<ValueId> v1Priorities, string priorityId) {
            return v1Priorities.Any(x => x.Token.Equals(priorityId));
        }
    }
}