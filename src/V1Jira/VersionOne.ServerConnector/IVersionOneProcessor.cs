/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Collections.Generic;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;

namespace VersionOne.ServerConnector {
    public interface IVersionOneProcessor {
        bool ValidateConnection();
        
        IList<PrimaryWorkitem> GetWorkitemsByProjectId(string projectId);
        IList<PrimaryWorkitem> GetClosedWorkitemsByProjectId(string projectId);
        IList<FeatureGroup> GetFeatureGroups(IFilter filters, IFilter childrenFilters);
        
        void SaveWorkitems(ICollection<Workitem> workitems);
        void SaveWorkitem(Workitem workitem);
        void CloseWorkitem(PrimaryWorkitem workitem);
        void UpdateProject(string projectId, Link link);
        string GetWorkitemLink(Workitem workitem);
        ValueId CreateWorkitemStatus(string statusName);
        IList<ValueId> GetWorkitemStatuses();
        IList<ValueId> GetWorkitemPriorities();

        PropertyValues GetAvailableListValues(string typeToken, string fieldName);
        IDictionary<string, string> GetAvailableStatuses();

        bool ProjectExists(string projectId);
        bool AttributeExists(string typeName, string attributeName);
        
        void AddProperty(string attr, string prefix, bool isList);
        void AddListProperty(string fieldName, string typeToken);
        void AddOptionalProperty(string attr, string prefix);
        void AddLinkToWorkitem(Workitem workitem, Link link);

        IList<Workitem> GetPrimaryWorkitems(IFilter filter);
        IList<Workitem> GetWorkitems(string type, IFilter filter);
        Workitem GetPrimaryWorkitemByNumber(string number);

        Workitem CreateWorkitem(string assetType, string title, string description, string projectId, string projectName, string externalFieldName, string externalId, string externalSystemName, string priorityId, string owners, string urlTitle, string url, string environment, string foundBy, string versionAffected, string buildNumber, string severityLevel);
        bool UpdateWorkitem(string externalFieldName, string externalId, string externalSystemName, string statusId, UpdateResult result);

    }
}