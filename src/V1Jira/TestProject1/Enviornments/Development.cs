using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.Core.Configuration;

namespace IntegrationTests.Enviornments
{
    class Development : Enviornment
    {
        public override string GetVersionOneUrl { get { return "http://ctovm1065.dev.sabre.com/VersionOne"; } }
        public override string GetVersionOneUserName { get { return "sg0913829"; } }
        public override string GetVersionOnePassword { get { return "trinity1"; } }
        public override string GetVersionOneReference { get { return "Reference"; } }


        public override string[] GetExistingProjectNames { get { throw new NotImplementedException(); } }
        public override string[] GetNonExistingProjectNames { get { throw new NotImplementedException(); } }

        public override string GetVersionOneSource { get { return "JIRA_INTEGRATION"; } }
        public override string GetVersionOneMappingStatus { get { return "customfield_10200"; } }

        public override JiraServiceConfiguration GetJiraConfiguration
        {
            get
            {
                JiraServiceConfiguration conf = new JiraServiceConfiguration();
                conf.Url = "http://ctovm1226.dev.sabre.com:8080/rpc/soap/jirasoapservice-v2";
                conf.UserName = "v1integration";
                conf.Password = "v1password";

                conf.OpenDefectFilter = new JiraFilter("10100", true);
                conf.OpenStoryFilter = new JiraFilter("", false);
                conf.UpdateWorkitemFilter = new JiraFilter("10101", false);


                conf.OnCreateFieldName = "customfield_10102";
                conf.OnCreateFieldValue = "Created";
                conf.OnStateChangeFieldName = "customfield_10102";
                conf.OnStateChangeFieldValue = "Closed";
                conf.OnFailureFieldName = "customfield_10102";
                conf.OnFailureFieldValue = "Failure";
                conf.BuildNumberFieldName = "customfield_10300";
                conf.SeverityFieldName = "customfield_10301";

                conf.ProgressWorkflow = "4";
                conf.ProgressWorkflowStateChanged = "5";
                conf.AssigneeStateChanged = "-1";
                conf.WorkitemLinkField = "customfield_10101";
                conf.WorkitemStateField = "customfield_10200";

                conf.UrlTemplateToIssue = "https://jira.sabre.com/browse/#key#";
                conf.UrlTitleToIssue = "Jira";
                conf.PriorityMappings.Add(new MappingInfo("1","Blocker"), new MappingInfo("WorkitemPriority:140","High"));
                conf.PriorityMappings.Add(new MappingInfo("2", "Critical"), new MappingInfo("WorkitemPriority:140", "High"));
                conf.PriorityMappings.Add(new MappingInfo("3", "Major"), new MappingInfo("WorkitemPriority:139", "Medium"));
                conf.PriorityMappings.Add(new MappingInfo("4", "Minor"), new MappingInfo("WorkitemPriority:138", "Low"));
                conf.PriorityMappings.Add(new MappingInfo("5", "Trivial"), new MappingInfo("WorkitemPriority:138", "Low"));

                conf.SeverityMappings.Add(new MappingInfo("1", "1"), new MappingInfo("Custom_Severity_Level:7952","Level 1"));
                conf.SeverityMappings.Add(new MappingInfo("2", "2"), new MappingInfo("Custom_Severity_Level:7953", "Level 2"));
                conf.SeverityMappings.Add(new MappingInfo("3", "3"), new MappingInfo("Custom_Severity_Level:7954", "Level 3"));
                conf.SeverityMappings.Add(new MappingInfo("4", "4"), new MappingInfo("Custom_Severity_Level:7955", "Level 4"));
                
                conf.ProjectMappings.Add(new KeyValuePair<MappingInfo, MappingInfo>(new MappingInfo("VOIDM", "VersionOne Integration Demo [Monika]"), new MappingInfo("Scope:123456", "Fake Scope")));
                conf.ProjectMappings.Add(new KeyValuePair<MappingInfo, MappingInfo>(new MappingInfo("VOID", "VersionOne Integration Demo"), new MappingInfo("Scope:2257958", "")));
                
                return conf;
            }
        } 
    }
}