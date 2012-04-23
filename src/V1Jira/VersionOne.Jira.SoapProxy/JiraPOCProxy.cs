/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using VersionOne.Jira.SoapProxy.Jira;
using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.Jira.SoapProxy {
    public class JiraPOCProxy {
        private string Url { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        
        public JiraPOCProxy(string url) {
            Url = url;
        }

        public JiraPOCProxy(string url, string userName, string password) {
            Url = url;
            UserName = userName;
            Password = password;
        }

        public string CreateIssue(Issue issue) {
            using(IJiraSoapService service = new JiraSoapService()) {
                var token = service.login(UserName, Password);
                var types = service.getIssueTypes(token);
                var remoteIssue = CreateRemoteIssue(issue);

                remoteIssue.project = "MP";
                remoteIssue.type = types[0].id;
                remoteIssue.assignee = "remote";

                var createdIssue = service.createIssue(token, remoteIssue);

                service.logout(token);

                return createdIssue.key;
            }
        }

        public RemoteIssue CreateRemoteIssue(Issue issue) {
            var remoteIssue = new RemoteIssue {
                summary = issue.Summary, 
                description = issue.Description, 
                project = issue.Project, 
                type = issue.IssueType, 
                assignee = issue.Assignee
            };

            return remoteIssue;
        }

        /// <summary>
        /// Get all issues in JIRA that match the filter that says that a defect should be created in V1.
        /// </summary>
        /// <returns>Isses that need to have defects created for them.</returns>
        public List<Defect> GetIssues() {
            using(IJiraSoapService service = new JiraSoapService()) {
                var token = service.login(UserName, Password);
                var remoteIssues = service.getIssuesFromFilter(token, "10000");
                var defects = new List<Defect>();

                foreach(var issue in remoteIssues) {
                    defects.Add(new Defect(issue.summary, issue.description, issue.project, issue.assignee, issue.priority, issue.environment));
                }

                service.logout(token);

                return defects;
            }
        }
    }

}