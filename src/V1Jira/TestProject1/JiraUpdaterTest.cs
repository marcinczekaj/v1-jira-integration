using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.WorkitemServices;
using VersionOne.ServiceHost.Core;
using VersionOne.Jira.SoapProxy;
using IntegrationTests.Enviornments;
using IntegrationTests.Logger;

namespace IntegrationTests
{
    [TestClass]
    public class JiraUpdaterTest
    {
        JiraServiceConfiguration jiraConfig ;
        JiraIssueReaderUpdater jiraIssueReaderUpdater;

        [TestInitialize]
        public void Initialize() {
            ComponentRepository.Instance.Register(new JiraSoapServiceFactory());
            ComponentRepository.Instance.Register(new LoggerImpl());

            jiraConfig = Enviornment.instance.GetJiraConfiguration;
            jiraIssueReaderUpdater = new JiraIssueReaderUpdater(jiraConfig);
        }


        [TestMethod]
        public void GetIssues_Defects_AssertNonZero()
        {
            var defects = jiraIssueReaderUpdater.GetIssues<Defect>(jiraConfig.OpenDefectFilter.Id);
            Console.WriteLine(string.Format("Defects count: {0}", defects.Count));

            foreach (var defect in defects) {
                Console.WriteLine(defect);
            }
            Assert.IsTrue( defects.Count > 0);
        }

        [TestMethod]
        public void Update_CreationResultSuccess_OnPilotIssue() 
        {
            WorkitemCreationResult creationResult = new WorkitemCreationResult(new Defect()){                
                Source = { ExternalId = "PILOT-416" },
            };
            creationResult.Messages.Add("Integration Test Comment - Creation Success");
            jiraIssueReaderUpdater.OnWorkitemCreated( creationResult );
        }

        [TestMethod]
        public void Update_CreationResultFailure_OnPilotIssue() 
        {
            var creationResult = new WorkitemCreationFailureResult(new Defect())
            {
                //Source = { ExternalId = "PILOT-416" },
                Source = { ExternalId = "VOID-37" },
            };
            //creationResult.Messages.Add("Integration Test Comment - Creation Failure");
            jiraIssueReaderUpdater.OnWorkitemCreationFailure(creationResult);
        }
        
        [TestMethod]
        public void Update_ChangeVersionOneStateToReady_OnPilotIssue() 
        {
            var createdResult = new WorkitemCreationResult(new Defect()) {
                Source = { ExternalId = "PILOT-416" },
            };
            
            jiraIssueReaderUpdater.OnWorkitemCreated(createdResult);
            
        }

        [TestMethod]
        public void GetAvailableStatuses() { 
            var statuses = jiraIssueReaderUpdater.getAvailableStatuses();
            Assert.IsTrue(statuses.Count > 0);

            foreach (var pair in statuses) {
                Console.WriteLine(string.Format("{0,20} - {1, 20}", pair.Key, pair.Value));
            }
        }

        [TestMethod]
        public void CreateIssueInMappedProject()
        {
            var newWorkitem = new NewVersionOneWorkitem("Scope:123456")
            {
                Title = "Title",
                Description = "Description",
                Number = "D-1",
                ProjectId = "Query scope is used",
            };

            newWorkitem.Messages.Add("Integration Test Comment - Creating JIRA Issue");
            Assert.IsTrue(jiraIssueReaderUpdater.OnNewWorkitem(newWorkitem, "JIRA"));
        }

        [TestMethod]
        public void CreateIssueInMissingProject()
        {
            var newWorkitem = new NewVersionOneWorkitem("Scope:somescope")
            {
                Title = "Title",
                Description = "Description",
                Number = "D-1",
                ProjectId = "Query scope is used",
            };

            Assert.IsFalse(jiraIssueReaderUpdater.OnNewWorkitem(newWorkitem, "JIRA"));
        }


    }
}
