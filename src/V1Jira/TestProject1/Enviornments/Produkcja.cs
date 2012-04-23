using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VersionOne.ServiceHost.JiraServices;

namespace IntegrationTests.Enviornments
{
    class Produkcja : Enviornment
    {
        public override string GetVersionOneUrl { get{ return "http://versionone.dev.sabre.com/VersionOne"; } }
        public override string GetVersionOneUserName { get { return "sg0913829"; } }
        public override string GetVersionOnePassword { get { return "trinity1"; } }
        public override string GetVersionOneReference { get { return "Reference";} }
        public override string GetVersionOneSource { get { return "JIRA"; } }
        public override string GetVersionOneMappingStatus { get { throw new NotImplementedException(); } }



        public override JiraServiceConfiguration GetJiraConfiguration { get { 
            JiraServiceConfiguration conf = new JiraServiceConfiguration();
            conf.Url="https://jira.sabre.com/rpc/soap/jirasoapservice-v2";
            conf.UserName="v1integration";
            conf.Password="v1password";
            conf.OpenDefectFilter = new JiraFilter("10306", true);
            conf.OpenStoryFilter = new JiraFilter("", false);
            conf.UpdateWorkitemFilter = new JiraFilter("", false);

            conf.OnCreateFieldName="customfield_10171";
            conf.OnCreateFieldValue="Ready";
            conf.OnStateChangeFieldName="customfield_10171";
            conf.OnStateChangeFieldValue="Closed";
            conf.OnFailureFieldName = "customfield_10171";
            conf.OnFailureFieldValue="Failure";
            conf.ProgressWorkflow=null;
            conf.ProgressWorkflowStateChanged=null;
            conf.AssigneeStateChanged="-1";
            conf.WorkitemLinkField=null;
            conf.UrlTemplateToIssue="https://jira.sabre.com/browse/#key#";
            conf.UrlTitleToIssue="Jira";
            
            return conf; } }

        public override string[] GetExistingProjectNames
        {
            get
            {
                return new string[] {
            "EIAPI", 
            "eHotels - Viewership Simplification", 
            "Sabre Travel Policy", 
            "Jira Demo", 
            "Integration_Testing", 
            "PSD Infrastructure", 
            "Enterprise OHSSM", 
            "Dynamic Schedules" };
            }
        }

        public override string[] GetNonExistingProjectNames
        {
            get
            {
                return new string[] {
            "Airport Check-In System", 
            "ASxFFM" };
            }
        }
    }
}