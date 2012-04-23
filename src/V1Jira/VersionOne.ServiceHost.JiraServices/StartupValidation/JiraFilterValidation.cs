/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public class JiraFilterValidation : BaseValidator {
        private readonly JiraFilter filter;
        private readonly string username;
        private readonly string password;

        public JiraFilterValidation(string url, string username, string password, JiraFilter filter) : base(url) {
            this.filter = filter;
            this.password = password;
            this.username = username;
        }

        public override bool Validate() {
            Logger.Log(LogMessage.SeverityType.Info, "Checking JIRA filter.");

            if(filter == null) {
                Logger.Log(LogMessage.SeverityType.Error, "Filter is null.");
                return false;
            }
            if (!filter.Enabled) {
                Logger.Log(LogMessage.SeverityType.Debug, string.Format("Filter {0} disabled.", filter.Id));
                return true;
            }

            using(var service = GetJiraService()) {
                try {
                    var token = service.Login(username, password);
                    service.GetIssuesFromFilter(token, filter.Id);
                    service.Logout(token);
                } catch(Exception) {
                    Logger.Log(LogMessage.SeverityType.Error, string.Format("Can't find {0} filter.", filter.Id));
                    return false;
                }
            }

            Logger.Log(LogMessage.SeverityType.Info, "JIRA filter is checked.");

            return true;
        }
    }
}