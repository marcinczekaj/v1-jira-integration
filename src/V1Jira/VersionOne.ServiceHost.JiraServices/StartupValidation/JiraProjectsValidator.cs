/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using System.Linq;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.JiraServices.StartupValidation {
    public class JiraProjectsValidator : BaseValidator {
        private readonly string username;
        private readonly string password;
        private readonly ICollection<MappingInfo> projects;

        public JiraProjectsValidator(string url, string username, string password, ICollection<MappingInfo> projects)
            : base(url) {
            this.password = password;
            this.username = username;
            this.projects = projects;
        }

        public override bool Validate() {
            var result = true;
            Logger.Log(LogMessage.SeverityType.Info, "Checking JIRA projects.");

            using (var service = GetJiraService()) {
                var token = service.Login(username, password);
                var jiraProjects = service.GetProjects(token);
                
                foreach(var project in projects) {
                    
                    //if (!jiraProjects.Any(x => x.Name.Equals(project.Name))) {
                    if (!jiraProjects.Any(x => (x.Id.Equals(project.Id) || x.Name.Equals(project.Name))   )) {
                        //Logger.Log(LogMessage.SeverityType.Error, string.Format("Cannot find JIRA projects with {0} name.", project.Name));
                        Logger.Log(LogMessage.SeverityType.Error, string.Format("Cannot find JIRA projects with {0} name or {1} id.", project.Name, project.Id));
                        result = false;
                    }
                }
                
                service.Logout(token);
            }

            Logger.Log(LogMessage.SeverityType.Info, "JIRA projects are checked.");
            return result;
        }
    }
}