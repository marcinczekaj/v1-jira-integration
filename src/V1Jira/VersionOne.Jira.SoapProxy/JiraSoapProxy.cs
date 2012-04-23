/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Linq;
using VersionOne.Jira.SoapProxy.Jira;
using VersionOne.ServiceHost.JiraServices;
using System.Web.Services.Protocols;
using VersionOne.ServiceHost.JiraServices.Exceptions;
using System.Collections;

namespace VersionOne.Jira.SoapProxy {
    public class JiraSoapProxy : JiraSoapService, IJiraProxy {
        private readonly int MAX_RESULTS=10000;

        public JiraSoapProxy(string url) {
            Url = url;
        }

        public string Login(string userName, string password) {
            return login(userName, password);
        }

        public Issue[] GetIssuesFromFilter(string loginToken, string issueFilterId) {
            var remoteIssues = getIssuesFromFilter(loginToken, issueFilterId);
            return remoteIssues.Select(remoteIssue => BuildIssue(loginToken, remoteIssue)).ToArray();
        }

        public Issue[] GetIssuesFromJqlSearch(string loginToken, string jqlQuery) {
            var remoteIssues = getIssuesFromJqlSearch(loginToken, jqlQuery, MAX_RESULTS);
            return remoteIssues.Select(remoteIssue => BuildIssue(loginToken, remoteIssue)).ToArray();
        }

        public Issue UpdateIssue(string loginToken, string issueKey, string fieldName, string fieldValue) {
            try {
                var remoteFieldValue = new RemoteFieldValue {id = fieldName, values = new[] {fieldValue}};
                return BuildIssue(loginToken, updateIssue(loginToken, issueKey, new[] {remoteFieldValue}));
            } catch(SoapException ex) {
                ProcessException(ex);
                throw;
            }
        }

        public Issue CreateIssue(string loginToken, Issue issue) {
            try {
                RemoteIssue request = BuildRemoteIssue(issue);
                RemoteIssue response = createIssue(loginToken, request);

                return BuildIssue(loginToken, response);
            } catch (SoapException ex) {
                ProcessException(ex);
                throw;
            }
        }

        public void AddComment(string loginToken, string issueKey, string comment) {
            var remoteComment = new RemoteComment {body = comment};
            addComment(loginToken, issueKey, remoteComment);
        }

        public void ProgressWorkflow(string loginToken, string issueKey, string action, string assignee) {
            if(null != assignee) {
                var assigneeField = new RemoteFieldValue {id = "assignee", values = new[] {assignee}};
                progressWorkflowAction(loginToken, issueKey, action, new[] {assigneeField});
            } else {
                progressWorkflowAction(loginToken, issueKey, action, new RemoteFieldValue[] {});
            }
        }

        private Issue BuildIssue(string loginToken, RemoteIssue remote) {
            var result = new Issue {
                Summary = remote.summary,
                Description = remote.description,
                Project = GetProjectNameFromKey(loginToken, remote.project),
                IssueType = remote.type,
                Assignee = remote.assignee,
                Id = remote.id,
                Key = remote.key,
                Priority = remote.priority,
                Environment = remote.environment,
                Reporter = remote.reporter,
                AffectsVersion = GetAffectsVersion(remote.affectsVersions),
                Updated = remote.updated,
                Created = remote.created,
                ProjectKey = remote.project,
                CustomFields = GetCustomFields(remote.customFieldValues),
                Status = remote.status,
            };

            return result;
        }

            private RemoteIssue BuildRemoteIssue(Issue issue) {
            RemoteIssue remoteIssue = new RemoteIssue();

            remoteIssue.summary = issue.Summary;
            remoteIssue.description = issue.Description;
            remoteIssue.project = issue.ProjectKey;
            remoteIssue.type = issue.IssueType;
            remoteIssue.assignee = issue.Assignee;
            remoteIssue.id = issue.Id;
            remoteIssue.key = issue.Key;
            remoteIssue.priority = issue.Priority;
            remoteIssue.environment = issue.Environment;

            List<RemoteCustomFieldValue> customFields = new List<RemoteCustomFieldValue>();

            foreach(var issueField in issue.CustomFields) {
                customFields.Add(new RemoteCustomFieldValue() { customfieldId = issueField.Key, values = new[] { issueField.Value } });
            }

            remoteIssue.customFieldValues = customFields.ToArray();
            
            remoteIssue.created = issue.Created;
            remoteIssue.updated = issue.Updated;

            return remoteIssue;
        }

        private IDictionary<string, string> GetCustomFields(RemoteCustomFieldValue[] customFieldValuesField) {
            IDictionary<string, string> CustomFields = new Dictionary<string, string>();

            if (customFieldValuesField != null && customFieldValuesField.Length > 0) {
                foreach(RemoteCustomFieldValue customValue in customFieldValuesField) {
                    string value = string.Join(",", customValue.values);
                    string id = customValue.customfieldId;
                    CustomFields.Add(new KeyValuePair<string, string>(id, value));
                }
            }
            return CustomFields;
        }

        private string GetAffectsVersion(RemoteVersion[] affectsVersions) {
            RemoteVersion affectsVersion = affectsVersions!= null && affectsVersions.Length > 0
                ? affectsVersions.First()
                : null;
            return affectsVersion != null ? affectsVersion.name : null;
        }

        private string GetProjectNameFromKey(string loginToken, string projectKey) {
            var remoteProject = getProjectByKey(loginToken, projectKey);

            return remoteProject.name;
        }

        public IList<Item> GetPriorities(string token) {
            var remotePriorities = getPriorities(token);
            return remotePriorities.Select(remotePriority => new Item(remotePriority.id, remotePriority.name)).ToList();
        }

        public IList<Item> GetProjects(string token) {
            var remoteProjects = getProjects(token);
            //return remoteProjects.Select(remoteProject => new Item(remoteProject.id, remoteProject.name)).ToList();
            return remoteProjects.Select(remoteProject => new Item(remoteProject.key, remoteProject.name)).ToList();
        }

        public bool Logout(string token) {
            return logout(token);
        }

        public IEnumerable<Item> GetAvailableActions(string token, string issueId) {
            var remoteActions = getAvailableActions(token, issueId);
            return remoteActions.Select(rempoteAction => new Item(rempoteAction.id, rempoteAction.name)).ToList();
        }

        public IDictionary<string, string> GetAvailableStatuses(string token) {
            var remoteStatuses = getStatuses(token);
            return remoteStatuses.ToDictionary(x => x.id, x => x.name);
        }


       public IEnumerable<Item> GetCustomFields(string token) {
            try {
                var removeFields = getCustomFields(token);
                return removeFields.Select(removeField => new Item(removeField.id, removeField.name)).ToList();
            } catch(SoapException ex) {
                ProcessException(ex);
                throw;
            }
        }

        //TODO: if it's possible - find better way to process remote exception from JIRA
        private static void ProcessException(SoapException exception) {
            var remoteMessages = exception.Message.Split(':');
            var message = remoteMessages.Count() > 1 ? String.Join(" ", remoteMessages, 1, remoteMessages.Count() - 1) : exception.Message;
            if (exception.Message.Contains("RemotePermissionException")) {
                throw new JiraPermissionException(message.Trim(), exception);
            }
            if( exception.Message.Contains("RemoteValidationException") ) {
                throw new JiraValidationException(message.Trim(), exception);
            }
        }

        

    }
}