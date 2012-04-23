/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Linq;
using System.Collections.Generic;
using VersionOne.ServiceHost.Core.Utility;
using VersionOne.ServiceHost.Core.Configuration;

namespace VersionOne.ServiceHost.JiraServices {
    public class JiraServiceConfiguration {
        private readonly IDictionary<MappingInfo, MappingInfo> projectMappings = new Dictionary<MappingInfo, MappingInfo>();
        private readonly IDictionary<MappingInfo, MappingInfo> priorityMappings = new Dictionary<MappingInfo, MappingInfo>();
        private readonly IDictionary<MappingInfo, MappingInfo> severityMappings = new Dictionary<MappingInfo, MappingInfo>();
        private readonly IDictionary<MappingInfo, MappingInfo> fieldMappings = new Dictionary<MappingInfo, MappingInfo>();
        private readonly IDictionary<MappingInfo, IList<MappingInfo>> transitionMappings = new Dictionary<MappingInfo, IList<MappingInfo>>();
        private readonly IDictionary<MappingInfo, IList<MappingInfo>> statusMappings = new Dictionary<MappingInfo, IList<MappingInfo>>();
        private IDictionary<string, HashSet<string>> reverseStatusMapping = new Dictionary<string, HashSet<string>>();

        private IDictionary<MappingInfo, MappingInfo> reversePriorityMapping = new Dictionary<MappingInfo, MappingInfo>();
        private IDictionary<MappingInfo, MappingInfo> reverseSeverityMapping = new Dictionary<MappingInfo, MappingInfo>();

        [ConfigFileValue("JIRAUserName", null)]
        public string UserName;

        [ConfigFileValue("JIRAPassword", null)]
        public string Password;

        [ConfigFileValue("JIRAUrl", null)]
        public string Url;

        [ConfigFileValue("CreateFieldId")]
        public string OnCreateFieldName;

        [ConfigFileValue("BuildNumberFieldId")]
        public string BuildNumberFieldName;

        [ConfigFileValue("SeverityFieldId")]
        public string SeverityFieldName;

        [ConfigFileValue("CreateFieldValue")]
        public string OnCreateFieldValue;

        [ConfigFileValue("CloseFieldId")]
        public string OnStateChangeFieldName;

        [ConfigFileValue("CloseFieldValue")]
        public string OnStateChangeFieldValue;


        [ConfigFileValue("FailureFieldId")]
        public string OnFailureFieldName;

        [ConfigFileValue("FailureFieldValue")]
        public string OnFailureFieldValue;


        [IgnoreConfigFieldAttribute]
        public JiraFilter OpenDefectFilter;

        [IgnoreConfigFieldAttribute]
        public JiraFilter OpenStoryFilter;

        [IgnoreConfigFieldAttribute]
        public JiraFilter UpdateWorkitemFilter;

        [ConfigFileValue("ProgressWorkflow")]
        public string ProgressWorkflow;

        [ConfigFileValue("ProgressWorkflowClosed")]
        public string ProgressWorkflowStateChanged;

        [ConfigFileValue("AssigneeStateChanged")]
        public string AssigneeStateChanged;

        [ConfigFileValue("WorkitemLinkFieldId")]
        public string WorkitemLinkField;

        [ConfigFileValue("WorkitemStateFieldId")]
        public string WorkitemStateField;

        [ConfigFileValue("JIRAIssueUrlTemplate")]
        public string UrlTemplateToIssue;

        [ConfigFileValue("JIRAIssueUrlTitle")]
        public string UrlTitleToIssue;

        [ConfigFileValue("JiraUserTimeZone")]
        public string TimeZone;


        public IDictionary<MappingInfo, MappingInfo> ProjectMappings {
            get { return projectMappings; }
        }

        public IDictionary<MappingInfo, MappingInfo> PriorityMappings {
            get { return priorityMappings; }
        }

        public IDictionary<MappingInfo, MappingInfo> SeverityMappings
        {
            get { return severityMappings; }
        }

        public IDictionary<MappingInfo, MappingInfo> FieldMappings {
            get { return fieldMappings; }
        }

        public IDictionary<MappingInfo, IList<MappingInfo>> TransitionMappings {
            get { return transitionMappings; }
        }

        public IDictionary<MappingInfo, IList<MappingInfo>> StatusMappings {
            get { return statusMappings; }
        }

        public IDictionary<string, HashSet<string>> ReverseStatusMapping {
            get { return reverseStatusMapping; }
            set { reverseStatusMapping = value; }
        }

        public IDictionary<MappingInfo, MappingInfo> ReversePriorityMapping {
            get { return reversePriorityMapping; }
            set { reversePriorityMapping = value; }
        }

        public IDictionary<MappingInfo, MappingInfo> ReverseSeverityMapping {
            get { return reverseSeverityMapping; }
            set { reverseSeverityMapping = value; }
        }
        

        public static TimeZoneInfo GetJiraTimeZone(string timezoneName) {
            return TimeZoneInfo.GetSystemTimeZones().Where(x => x.Id.Equals(timezoneName)).FirstOrDefault();
        }

    }
}