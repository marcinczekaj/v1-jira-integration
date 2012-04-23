/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Linq;
using System.Collections.Generic;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Configuration;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public class JiraPrioritiesValidator : BaseValidator {
        private readonly string username;
        private readonly string password;
        private readonly ICollection<MappingInfo> priorities;

        public JiraPrioritiesValidator(string url, string username, string password, ICollection<MappingInfo> priorities)
            : base(url) {
            this.password = password;
            this.username = username;
            this.priorities = priorities;
        }

        public override bool Validate() {
            var result = true;
            Logger.Log(LogMessage.SeverityType.Info, "Checking JIRA priorities");

            using (var service = GetJiraService()) {
                var token = service.Login(username, password);
                var jiraPriorities = service.GetPriorities(token);

                foreach(var priority in priorities) {
                    if (!jiraPriorities.Any(x => x.Id.Equals(priority.Id))) {
                        Logger.Log(LogMessage.SeverityType.Error, string.Format("Cannot find JIRA priority with identifier {0}", priority.Id));
                        result = false;
                    }
                }
                
                service.Logout(token);
            }

            Logger.Log(LogMessage.SeverityType.Info, "JIRA priorities are checked");
            return result;
        }
    }
}