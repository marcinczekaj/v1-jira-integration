/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public class JiraConnectionValidator : BaseValidator {
        private readonly string username;
        private readonly string password;

        public JiraConnectionValidator(string url, string username, string password) : base(url) {
            this.password = password;
            this.username = username;
        }

        public override bool Validate() {
            using (var service = GetJiraService()) {
                try {
                    var token = service.Login(username, password);

                    if (string.IsNullOrEmpty(token)) {
                        Logger.Log(LogMessage.SeverityType.Error, "Incorrect credentials or JIRA URL.");
                        return false;
                    }
                    service.Logout(token);
                } catch (Exception ex) {                    
                    Logger.Log(LogMessage.SeverityType.Error, "Incorrect credentials or JIRA URL. " + ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}