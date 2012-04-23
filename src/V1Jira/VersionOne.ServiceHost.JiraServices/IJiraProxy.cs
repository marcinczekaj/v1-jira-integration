/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;

namespace VersionOne.ServiceHost.JiraServices {
    public interface IJiraProxy : IDisposable {
        string Login(string userName, string password);
        Issue[] GetIssuesFromFilter(string loginToken, string issueFilterId);
        Issue[] GetIssuesFromJqlSearch(string loginToken, string jqlQuery);
        Issue UpdateIssue(string loginToken, string issueKey, string fieldName, string fieldValue);
        Issue CreateIssue(string loginToken, Issue issue);
        IList<Item> GetPriorities(string token);
        IList<Item> GetProjects(string token);
        void AddComment(string loginToken, string issueKey, string comment);
        void ProgressWorkflow(string loginToken, string issueKey, string action, string assignee);
        bool Logout(string token);
        IEnumerable<Item> GetAvailableActions(string token, string issueId);
        IEnumerable<Item> GetCustomFields(string token);

        IDictionary<string,string> GetAvailableStatuses(string token);
    }
}