/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using System.Linq;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.JiraServices.Exceptions;
using VersionOne.ServiceHost.WorkitemServices;
using System.Text.RegularExpressions;

namespace VersionOne.ServiceHost.JiraServices {
    public class JiraIssueReaderUpdater : IJiraIssueProcessor {
        private readonly JiraServiceConfiguration configuration;
        private readonly IJiraServiceFactory jiraServiceFactory;
        private readonly ILogger logger;

         private IDictionary<string, string> mappingJiraIdToStatus = null;
        private TimeZoneInfo JiraTimezone;

        public JiraIssueReaderUpdater(JiraServiceConfiguration config) {
            configuration = config;
            JiraTimezone = JiraServiceConfiguration.GetJiraTimeZone(config.TimeZone);

            jiraServiceFactory = ComponentRepository.Instance.Resolve<IJiraServiceFactory>();
            logger = ComponentRepository.Instance.Resolve<ILogger>();
        }


        public IList<Workitem> GetIssues<T>(string filterId) where T : Workitem, new() {
            string buildNumberFieldName = configuration.BuildNumberFieldName;
            string severityFieldName = configuration.SeverityFieldName;

            using(var service = GetJiraService()) {
                var token = service.Login(configuration.UserName, configuration.Password);
                var remoteIssues = service.GetIssuesFromFilter(token, filterId);
                var items = new List<Workitem>();

                foreach(var issue in remoteIssues) {
                    var projectMapping = ResolveVersionOneProjectMapping(issue.ProjectKey, issue.Project);

                    var priorityMapping = ResolveVersionOnePriorityMapping(issue.Priority);
                    var description = string.IsNullOrEmpty(issue.Description) ? issue.Description : issue.Description.Replace("\n", "<br/>");
                    var versionOneStatus = ResolveVersionOneStatus(issue.Status);

                    var item = new T {
                        Title = issue.Summary,
                        Project = projectMapping.Name,
                        ExternalId = issue.Key,
                        ProjectId = projectMapping.Id,
                        Description = description,
                        Owners = issue.Assignee,
                        Environment = issue.Environment,
                        Updated = issue.Updated,
                        Created = issue.Created,
                        //Status = issue.Status,
                        Status = versionOneStatus,
                    };

                    if(!string.IsNullOrEmpty(configuration.UrlTemplateToIssue) && !string.IsNullOrEmpty(configuration.UrlTitleToIssue)) {
                        item.ExternalUrl = new UrlToExternalSystem(configuration.UrlTemplateToIssue.Replace("#key#", issue.Key), configuration.UrlTitleToIssue);
                    }

                    /*if (issue.Updated.HasValue == true) {
                        DateTime dateTimeToConvert = new DateTime(issue.Updated.Value.Ticks, DateTimeKind.Unspecified);
                        var updatedUtc = TimeZoneInfo.ConvertTimeToUtc(dateTimeToConvert, JiraTimezone);
                        item.Updated = updatedUtc;
                    }*/


                    if(priorityMapping != null) {
                        item.Priority = priorityMapping.Id;
                    }

                    if (issue.Reporter != null) {
                        item.FoundBy = issue.Reporter;
                    }

                    if (issue.AffectsVersion != null) {
                        item.VersionAffected = issue.AffectsVersion;
                    }

                    if (issue.CustomFields.Count > 0) {
                        item.BuildNumber = issue.GetCustomField(buildNumberFieldName);
                        item.SeverityLevel = GetSeverityLevel(issue.GetCustomField(severityFieldName));
                    }

                    items.Add(item);
                }

                service.Logout(token);
                return items;
            }
        }

         ///<summary>
        ///  A Workitem has been created in VersionOne in response to an Issue in JIRA.
        ///  We must update the Issue in JIRA to reflect the Workitem creation in VersionOne.
        ///  That reflection may be manifest by...
        ///  1) Updating a field (probably a custom field) to some value.
        ///  2) Progressing the workflow to another status.
        ///  One of these updates should keep the Issue from appearing in the new Issues filter again.
        ///</summary>
        ///<param name = "createdResult">Everything we need to know about the Workitem created in VersionOne.</param>
        public void OnWorkitemCreated(WorkitemCreationResult createdResult) {
            var issueId = createdResult.Source.ExternalId;
            var fieldName = configuration.OnCreateFieldName;
            var fieldValue = configuration.OnCreateFieldValue;
            var workflowId = configuration.ProgressWorkflow;
            var messages = createdResult.Messages;

            UpdateJiraIssue(issueId, fieldName, fieldValue, messages, workflowId, null);

            if(!string.IsNullOrEmpty(configuration.WorkitemLinkField)) {
                using(var service = GetJiraService()) {
                    var token = service.Login(configuration.UserName, configuration.Password);

                    logger.Log(LogMessage.SeverityType.Info, string.Format("Updating field {0} in JIRA issue {1}", configuration.WorkitemLinkField, issueId));
                    service.UpdateIssue(token, issueId, configuration.WorkitemLinkField, createdResult.Permalink);

                    service.Logout(token);
                }
            }
        }
        
        public void OnWorkitemCreationFailure(WorkitemCreationFailureResult createdResult) {
            var issueId = createdResult.Source.ExternalId;
            var messages = createdResult.Messages;
            
            var fieldName = configuration.OnFailureFieldName;
            var fieldValue = configuration.OnFailureFieldValue;
            
            //dont progress workflow
            //var workflowId = configuration.ProgressWorkflow;

            UpdateJiraIssue(issueId, fieldName, fieldValue, messages, null, null);
        }

        ///<summary>
        ///  A Workitem in VersionOne that was created as a result of an Issue in JIRA has changed
        ///  state. We must now reflect that change of state in the Issue in JIRA. We can relfect
        ///  that change in one of two ways:
        ///  1) Updating a field (probably a custom field) to some value.
        ///  2) Progressing the workflow to another status.
        ///</summary>
        ///<param name = "stateChangeResult">Tells us what Workitem state changed and what Issue to update.</param>
        public bool OnWorkitemStateChanged(WorkitemStateChangeResult stateChangeResult) {
            var issueId = stateChangeResult.ExternalId;
            var fieldName = configuration.OnStateChangeFieldName;
            var fieldValue = configuration.OnStateChangeFieldValue;
            var workflowId = configuration.ProgressWorkflowStateChanged;
            var messages = stateChangeResult.Messages;
            var assignee = configuration.AssigneeStateChanged;

            return UpdateJiraIssue(issueId, fieldName, fieldValue, messages, workflowId, assignee);
        }

        public bool OnNewWorkitem(NewVersionOneWorkitem workitem, string systemName) {
            Issue result = CreateJiraIssue(workitem);

            if (result == null) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Could not create JIRA issue for VersionOne workitem {0}", workitem.Number));
                return false;
            }

            workitem.ExternalId = result.Key;
            workitem.ExternalSystemName = systemName;
            workitem.ExternalUrl = new UrlToExternalSystem(configuration.UrlTemplateToIssue.Replace("#key#", result.Key), configuration.UrlTitleToIssue);

            return true;
        }

        public bool OnWorkitemUpdate(WorkitemStateChangeResult stateChangeResult) {
            var issueId = stateChangeResult.ExternalId;
            var workflowId = String.Empty;
            var messages = stateChangeResult.Messages;
            var fieldUpdates = stateChangeResult.FieldUpdates;
            var assignee = configuration.AssigneeStateChanged;

            return UpdateJiraIssue(issueId, fieldUpdates, messages, workflowId, assignee);
        }

        private string ResolveWorkflowTransition(IJiraProxy service, string token, string issueId, IDictionary<string, string> fieldUpdates) {

            if(string.IsNullOrEmpty(fieldUpdates[VersionOne.ServerConnector.Entities.Entity.StatusNameProperty]))
                return string.Empty;
            
            string status = fieldUpdates[VersionOne.ServerConnector.Entities.Entity.StatusNameProperty];

            var potentialStatuses = ResolveStatusMapping(status);
            if (potentialStatuses  == null)
                return string.Empty;

            var availableActions = service.GetAvailableActions(token, issueId);

            foreach (var potentialStatus in potentialStatuses) {

                var validActions = ResolveTransitionMapping(potentialStatus.Name);
                MappingInfo transition = FindNeededTransitionFromAvalableTransitions(availableActions, validActions);

                if (transition != null) {
                    logger.Log(LogMessage.SeverityType.Info, string.Format("We will use '{0}({1})' transition to get to {2}", transition.Name, transition.Id, potentialStatus.Name));
                    return transition.Id;
                }
            }

            return string.Empty;
        }

        private MappingInfo FindNeededTransitionFromAvalableTransitions(IEnumerable<Item> availableActions, IList<MappingInfo> validActions) {

            foreach (var validAction in validActions)
                foreach (var availableAction in availableActions)
                    if (validAction.Name.Equals(availableAction.Name))
                        return new MappingInfo(availableAction.Id, availableAction.Name);

            return null;
        }

        private bool UpdateJiraIssue(string issueId, IDictionary<string, string> fieldUpdates, List<string> messages, string workflowId, string assignee) {
            using (var service = GetJiraService()) {
                var token = service.Login(configuration.UserName, configuration.Password);


                foreach (var fieldPair in fieldUpdates) { 
                    string fieldName = ResolveJiraFieldMapping(fieldPair.Key);
                    string fieldValue = fieldPair.Value;

                    if (String.IsNullOrEmpty(fieldName))
                        continue;

                    try {
                        logger.Log(LogMessage.SeverityType.Info, string.Format("Updating JIRA field {0} in issue {1}", fieldName, issueId));
                        service.UpdateIssue(token, issueId, fieldName, fieldValue);
                    }
                    catch (JiraException ex) {
                        logger.Log(LogMessage.SeverityType.Error, string.Format("Error updating {0} '{1}' to '{2}':\r\n{3}", issueId, fieldName, fieldValue, ex.Message));
                        service.Logout(token);
                        return false;
                    }
                    catch (Exception ex) {
                        logger.Log(LogMessage.SeverityType.Error, string.Format("Error updating {0} '{1}' to '{2}':\r\n{3}", issueId, fieldName, fieldValue, ex));
                        service.Logout(token);
                        return false;
                    }
                }

                if (messages.Count > 0) {
                    logger.Log(LogMessage.SeverityType.Info, string.Format("Adding comments to JIRA issue {0}", issueId));
                }

                foreach (var message in messages) {
                    try {
                        service.AddComment(token, issueId, message);
                    }
                    catch (Exception ex) {
                        logger.Log(LogMessage.SeverityType.Error, string.Format("Error during adding comment {0} to JIRA issue {1}:\r\n{2}", message, issueId, ex));
                    }
                }

                workflowId = ResolveWorkflowTransition(service, token, issueId, fieldUpdates);

                if (!string.IsNullOrEmpty(workflowId)) {
                    if (IsActionAvaliable(workflowId, issueId)) {
                        logger.Log(LogMessage.SeverityType.Info, string.Format("Processing workflow {0} for JIRA issue {1}", workflowId, issueId));

                        try {
                            service.ProgressWorkflow(token, issueId, workflowId, assignee);
                        }
                        catch (Exception ex) {
                            logger.Log(LogMessage.SeverityType.Error,
                                string.Format("Error during processing workflow {0} for JIRA issue {1}:\r\n{2}", workflowId, issueId, ex));
                            service.Logout(token);
                            return false;
                        }
                    }
                }

                service.Logout(token);
            }

            return true;
        }

        private Issue CreateJiraIssue(NewVersionOneWorkitem workitem) {
            Issue issue = BuildIssue(workitem, workitem.QueryScopeId);

            if (issue == null) {
                logger.Log(LogMessage.SeverityType.Error, string.Format("Could not map VersionOne workitem {0} to JIRA issue", workitem.Number));
                return null; // TODO throw exception
            }

            if (!string.IsNullOrEmpty(configuration.OnCreateFieldName)) {
                issue.CustomFields.Add(configuration.OnCreateFieldName, configuration.OnCreateFieldValue);
            }

            if (!string.IsNullOrEmpty(configuration.WorkitemLinkField) && !string.IsNullOrEmpty(workitem.Url)) {
                issue.CustomFields.Add(configuration.WorkitemLinkField, workitem.Url);
            }
            
            logger.Log(LogMessage.SeverityType.Info, string.Format("Creating JIRA issue for Workitem {0}", workitem.Number));

            Issue result = null;

            using (var service = GetJiraService()) {
                var token = service.Login(configuration.UserName, configuration.Password);

                string reporter = issue.Reporter;
                string assignee = issue.Assignee;

                try {
                    issue.Reporter = null;
                    issue.Assignee = null;

                    result = service.CreateIssue(token, issue);
					logger.Log(LogMessage.SeverityType.Info, string.Format("Issue {0} created", result.Key));

                    if (!string.IsNullOrEmpty(reporter))
                        UpdateIssueField(service, token, result.Key, "reporter", reporter);

                    if (!string.IsNullOrEmpty(assignee))
                        UpdateIssueField(service, token, result.Key, "assignee", assignee);

                    if (workitem.Messages.Count > 0) {
                        logger.Log(LogMessage.SeverityType.Info, string.Format("Adding comments to JIRA issue {0}", result.Key));
                        AddCommentsToIssue(service, token, result.Key, workitem.Messages);
                    }
                } catch (Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, string.Format("Error while creating issue in JIRA for Workitem {0}:\r\n{1}", workitem.Number, ex.Message));
                }
                service.Logout(token); // unnecessary?
            }

            return result;
        }

        private void UpdateIssueField(IJiraProxy service, string token, string key, string field, string value) {
            try {
                logger.Log(LogMessage.SeverityType.Debug, string.Format("Setting field '{0}' of issue '{1}' to '{2}'", field, key, value));
                service.UpdateIssue(token, key, field, value);
            } catch (JiraValidationException ex) {
                logger.Log(LogMessage.SeverityType.Info, string.Format("Could not update field '{0}' of issue '{1}': {2}", field, key, ex.Message));
            }
        }

        private Issue BuildIssue(Workitem item, string scopeId) {
            var jiraProject = ResolveJiraProjectMapping(scopeId, null);

            if (jiraProject == null || jiraProject.Id == null)
                return null; // TODO Throw exception

            Issue issue = new Issue {
                Summary = item.Title,
                Project = jiraProject.Name,
                ProjectKey = jiraProject.Id,
                IssueType = "1", // TODO mapping
                Assignee = "",
                Priority = ResolveJiraPriorityId(item.Priority),
                Environment = item.Environment,
                Reporter = item.CreatedBy,
            };

            if (!string.IsNullOrEmpty(item.Description))
                issue.Description = Regex.Replace(item.Description, @"<[^>]*>", String.Empty);

            if (!string.IsNullOrEmpty(item.Owners))
                issue.Assignee = item.Owners.Split(',').FirstOrDefault();

            if (!string.IsNullOrEmpty(configuration.BuildNumberFieldName))
                issue.CustomFields.Add(configuration.BuildNumberFieldName, item.BuildNumber);

            if (!string.IsNullOrEmpty(configuration.SeverityFieldName) && !string.IsNullOrEmpty(item.SeverityLevel))
                issue.CustomFields.Add(configuration.SeverityFieldName, ResolveJiraSeverity(item.SeverityLevel));

            return issue;
        }

        private string ResolveJiraSeverity(string versionOneSeverity) {
            if (string.IsNullOrEmpty(versionOneSeverity))
                return null;

            foreach (var pair in configuration.ReverseSeverityMapping) {
                if (versionOneSeverity.Equals(pair.Key.Name))
                    return pair.Value.Name;
            }

            logger.Log(LogMessage.SeverityType.Debug, string.Format("Could not map VersionOne severity {0} to JIRA severity.", versionOneSeverity));

            return null;
        }

        private void AddCommentsToIssue(IJiraProxy service, string token, string issueKey, List<string> comments) {
            foreach (var message in comments) {
                try {
                    service.AddComment(token, issueKey, message);
                } catch (Exception ex) {
                    logger.Log(LogMessage.SeverityType.Error, string.Format("Error during adding comment {0} to JIRA issue {1}:\r\n{2}", message, issueKey, ex));
                }
            }
        }

        private bool UpdateJiraIssue(string issueId, string fieldName, string fieldValue, List<string> messages, string workflowId, string assignee) {
            using (var service = GetJiraService()) {
                var token = service.Login(configuration.UserName, configuration.Password);

                if (!string.IsNullOrEmpty(fieldName)) {
                    try {
                        logger.Log(LogMessage.SeverityType.Info, string.Format("Updating JIRA field {0} in issue {1}", fieldName, issueId));
                        service.UpdateIssue(token, issueId, fieldName, fieldValue);
                    } catch (JiraException ex) {
                        logger.Log(LogMessage.SeverityType.Error, string.Format("Error updating {0} '{1}' to '{2}':\r\n{3}", issueId, fieldName, fieldValue, ex.Message));
                        service.Logout(token);
                        return false;
                    } catch (Exception ex) {
                        logger.Log(LogMessage.SeverityType.Error, string.Format("Error updating {0} '{1}' to '{2}':\r\n{3}", issueId, fieldName, fieldValue, ex));
                        service.Logout(token);
                        return false;
                    }
                }

                if (messages.Count > 0) {
                    logger.Log(LogMessage.SeverityType.Info, string.Format("Adding comments to JIRA issue {0}", issueId));
                }

                foreach (var message in messages) {
                    try {
                        service.AddComment(token, issueId, message);
                    } catch (Exception ex) {
                        logger.Log(LogMessage.SeverityType.Error, string.Format("Error during adding comment {0} to JIRA issue {1}:\r\n{2}", message, issueId, ex));
                    }
                }

                if (!string.IsNullOrEmpty(workflowId)) {
                    if (IsActionAvaliable(workflowId, issueId)) {
                        logger.Log(LogMessage.SeverityType.Info, string.Format("Processing workflow {0} for JIRA issue {1}", workflowId, issueId));

                        try {
                            service.ProgressWorkflow(token, issueId, workflowId, assignee);
                        } catch (Exception ex) {
                            logger.Log(LogMessage.SeverityType.Error,
                                string.Format("Error during processing workflow {0} for JIRA issue {1}:\r\n{2}", workflowId, issueId, ex));
                            service.Logout(token);
                            return false;
                        }
                    }
                }

                service.Logout(token);
            }

            return true;
        }
        [Obsolete("Replaced by MappintInfo version", true)]
        private MappingInfo ResolveVersionOneProjectMapping(string jiraProject) {
            foreach(var mapping in configuration.ProjectMappings) {
                if(mapping.Key.Name.Equals(jiraProject)) {
                    return mapping.Value;
                }
            }
            return new MappingInfo(null, jiraProject);
        }

        private MappingInfo ResolveVersionOneProjectMapping(string mappingInfoId, string mappingInfoName) {
            foreach (var mapping in configuration.ProjectMappings) {
                if (mapping.Key.Id.Equals(mappingInfoId))
                    return mapping.Value;

                if (mapping.Key.Name.Equals(mappingInfoName))
                    return mapping.Value;
            }
            return new MappingInfo(null, mappingInfoName);
        }

        private MappingInfo ResolveJiraProjectMapping(string mappingInfoId, string mappingInfoName) {
            foreach (var mapping in configuration.ProjectMappings) {
                if (mapping.Value.Id.Equals(mappingInfoId))
                    return mapping.Key;

                if (mapping.Value.Name.Equals(mappingInfoName))
                    return mapping.Key;
            }
            return new MappingInfo(null, null);
        }

        private MappingInfo ResolveVersionOnePriorityMapping(string jiraPriorityId) {
            foreach(var mapping in configuration.PriorityMappings) {
                if(mapping.Key.Id.Equals(jiraPriorityId)) {
                    return mapping.Value;
                }
            }
            return null;
        }

        private string ResolveJiraPriorityId(string versionOnePriority) {
            if (string.IsNullOrEmpty(versionOnePriority))
                return null;

            foreach (var pair in configuration.ReversePriorityMapping) {
                if (versionOnePriority.Equals(pair.Key.Name))
                    return pair.Value.Id;
            }

            logger.Log(LogMessage.SeverityType.Debug, string.Format("Could not map VersionOne priority {0} to JIRA priority.", versionOnePriority));

            return null;
        }

        private string GetSeverityLevel(string severityLevelValue) {

            if (!string.IsNullOrEmpty(severityLevelValue)) {
                var severityMapping = ResolveVersionOneSeverityMapping(severityLevelValue);
                if (severityMapping != null) {
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Mapping severity level '{0}' to '{1}'", severityLevelValue, severityMapping.Name));
                    return severityMapping.Id;
                }
                logger.Log(LogMessage.SeverityType.Debug, string.Format("Could not map severity level '{0}'", severityLevelValue));
            }

            return null;
        }

        private MappingInfo ResolveVersionOneSeverityMapping(string jiraSeverityId) {
            if (!string.IsNullOrEmpty(jiraSeverityId)) { 
                foreach (var mapping in configuration.SeverityMappings) {
                    if (jiraSeverityId.Contains(mapping.Key.Name)) {
                        return mapping.Value;
                    }
                }
            }
            return null;
        }

        private string ResolveJiraFieldMapping(string versionOneFieldName) {
            foreach (var mapping in configuration.FieldMappings) {
                if (mapping.Key.Name.Equals(versionOneFieldName)) {
                    return mapping.Value.Name;
                }
            }
            logger.Log(LogMessage.SeverityType.Error, string.Format("Skpping field {0} since I can't find mapping to Jira Field - please update configuration mapping", versionOneFieldName));
            return null;
        }

        private IList<MappingInfo> ResolveStatusMapping(string versionOneStatus) { 
            foreach(var mapping in configuration.StatusMappings){
                if (mapping.Key.Name.Equals(versionOneStatus)) {
                    logger.Log(string.Format("VersionOne status '{0}' is mapped to following Jira statuses: {1}", versionOneStatus, string.Join(", ", mapping.Value.Select(x => x.Name).ToArray()) ));
                    return mapping.Value;
                }
            }
            logger.Log(LogMessage.SeverityType.Info, string.Format("Skipping status {0} since I can't find mapping to Jira Status", versionOneStatus));
            return null;
        }

        private IList<MappingInfo> ResolveTransitionMapping(string requestedStatus) {
            foreach (var mapping in configuration.TransitionMappings) {
                if (mapping.Key.Name.Equals(requestedStatus)) {
                    return mapping.Value;
                }
            }
            return null;
        }


        private string ResolveVersionOneStatus(string jiraStatusId) {

            if (mappingJiraIdToStatus == null)
                mappingJiraIdToStatus = getAvailableStatuses();

            if (mappingJiraIdToStatus.ContainsKey(jiraStatusId) == false)
                return string.Empty;

            string jiraStatus = mappingJiraIdToStatus[jiraStatusId];

            if (configuration.ReverseStatusMapping.ContainsKey(jiraStatus) == false)
                return string.Empty;

            if (configuration.ReverseStatusMapping[jiraStatus].Count <= 0)
                return string.Empty;

            return configuration.ReverseStatusMapping[jiraStatus].First();
        }

        //TODO why are we login to JIRA one more time?
        private bool IsActionAvaliable(string workflowId, string issueId) {
            using(var service = GetJiraService()) {
                var token = service.Login(configuration.UserName, configuration.Password);
                logger.Log(LogMessage.SeverityType.Debug, string.Format("Checking if workflow {0} is available for issue {1}", workflowId, issueId));

                try {
                    var actions = service.GetAvailableActions(token, issueId).ToList();
                    var actionIds = actions.Select(x => x.Id).ToList();

                    if(actionIds.Contains(workflowId)) {
                        logger.Log(LogMessage.SeverityType.Debug, string.Format("Issue {0} can be processed to workflow {1}", issueId, workflowId));
                        return true;
                    }

                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Cannot process issue {0} to state {1}. Available states are: {2}",
                            issueId, workflowId, string.Join(", ", actions.Select(x => x.ToString()).ToArray())));
                } catch(Exception ex) {
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Error getting available actions for issue {0}:\r\n{1}", issueId, ex));
                } finally {
                    if (!string.IsNullOrEmpty(token)) {
                        service.Logout(token);
                    }
                }

                
            }

            return false;
        }

        private IList<Item> getAvaliableActions(string issueId) {
            List<Item> actions = new List<Item>();

            using (var service = GetJiraService()) {
                var token = service.Login(configuration.UserName, configuration.Password);

                try {
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Checking available actions for issue {0}", issueId));
                    actions = service.GetAvailableActions(token, issueId).ToList();
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Available actions for issue {0} are: {1}", issueId, string.Join(", ", actions.Select(x => x.ToString()).ToArray())));
                }
                catch (Exception ex) {
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Error getting available actions for issue {0}:\r\n{1}", issueId, ex));
                } finally {
                    if (!string.IsNullOrEmpty(token)) {
                        service.Logout(token);
                    }
                }
            }

            return actions;
        }


        public IDictionary<string, string> getAvailableStatuses() {
            IDictionary<string, string> statuses = new Dictionary<string, string>();
            
            using (var service = GetJiraService()) {
                var token = service.Login(configuration.UserName, configuration.Password);

                try {
                
                    statuses = service.GetAvailableStatuses(token);
                    logger.Log(string.Format("Got {0} statuses", statuses.Count));

                } catch (Exception ex) {
                    logger.Log(LogMessage.SeverityType.Debug, string.Format("Error getting available statuses, reason: {0}:\r\n{1}",ex));
                } finally {
                    if (!string.IsNullOrEmpty(token))
                        service.Logout(token);
                }
            }

            return statuses;
        }

        private IJiraProxy GetJiraService() {
            return jiraServiceFactory.CreateNew(configuration.Url);
        }
    }
}