using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;
using VersionOne.ServiceHost.Core.Logging;
using IntegrationTests.Enviornments;
using IntegrationTests.Logger;
using IntegrationTests.Factory;

namespace IntegrationTests
{
    [TestClass]
    public class VersionOneProcessorTest
    {
        private VersionOneProcessor v1processor;

        [TestInitialize]
        public void SetUp() {

            v1processor = V1ProcessorFactory.build(new LoggerImpl());
            v1processor.ValidateConnection();
       }
   
       [TestMethod]
       public void GetProjectByName_AllIntegrated_exists() {
           string[] projects = Enviornment.instance.GetExistingProjectNames;
        
            foreach( string project in projects){
                var exists = v1processor.GetProjectByName(project);
                Assert.IsNotNull(exists);
            }
        }

        [TestMethod]
        public void GetProjectByName_AllIntegrated_notExists() {

            string[] projects = Enviornment.instance.GetNonExistingProjectNames;

            foreach (string project in projects)
            {
                var dosnt_exists = v1processor.GetProjectByName(project);
                Assert.IsNull(dosnt_exists);
            }
        }

        [TestMethod]
        public void CreateDefect_in_JiraDemo_project() {

            var result = v1processor.CreateWorkitem(
                "Defect", 
                "V1 Int Test", 
                null, 
                null, 
                "Jira Demo",
                "Reference",
                "PILOT-416",
                "JIRA",
                "WorkitemPriority:138",
                "SG0209984",
                "Jira", 
                "https://jira.sabre.com/browse/PILOT-416",
                "Test Environment",
                "Reporter name",
                "1.00",
                "1234",
                "Custom_Severity_Level:7952");
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SearchClosedItems()
        {
            //given
            var closedSince = new DateTime(2012, 1, 1);
            string sourceName = Enviornment.instance.GetVersionOneReference;

            var filters = new List<IFilter> {
                    Filter.Closed(true),
                    Filter.OfTypes(VersionOneProcessor.StoryType, VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.SourceNameProperty, "JIRA"),
                    Filter.Greater(Entity.ChangeDateUtcProperty, closedSince)
            };

            var filter = GroupFilter.And(filters.ToArray());
            var workitems = v1processor.GetPrimaryWorkitems(filter);

            Assert.IsTrue(workitems.Count > 0);

            Console.WriteLine("Number of items found: " + workitems.Count.ToString());

            foreach (var item in workitems) {
                Console.WriteLine(string.Format("{0};\t{1};\t{2};\t{3};\t{4};\t{5}", item.Id, item.Number, item.CreateDateUtc, item.ChangeDateUtc, item.Name, item.Reference));
            }
        }

        [TestMethod]
        public void SearchNewDefectsInActiveProjects()
        {
            //given
            var closedSince = new DateTime(2012, 1, 1);
            string sourceName = Enviornment.instance.GetVersionOneReference;

            var filters = new List<IFilter> {
                    Filter.OfTypes(VersionOneProcessor.DefectType),
                    Filter.Equal(Entity.InactiveProperty, false),
                    Filter.Equal(Entity.ScopeParentAndUpProperty, "Scope:138022"),
                    Filter.Equal(Entity.ScopeStateProperty, 64),
                    //Filter.Equal(Entity.InactiveProperty, "JIRA"),
                    //Filter.Greater(Entity.ChangeDateUtcProperty, closedSince)
                };

            var filter = GroupFilter.And(filters.ToArray());
            var workitems = v1processor.GetPrimaryWorkitems(filter);

            Assert.IsTrue(workitems.Count > 0);

            Console.WriteLine("Number of items found: " + workitems.Count.ToString());

            foreach (var item in workitems)
            {
                Console.WriteLine(string.Format("{0};\t{1};\t{2};\t{3};\t{4};\t{5}", item.Id, item.Number, item.CreateDateUtc, item.ChangeDateUtc, item.Name, item.Reference));
            }
        }

        [TestMethod]
        public void GetAvailableStatuses() {
            var dictionary = v1processor.GetAvailableStatuses();
            Assert.IsTrue(dictionary.Count > 0);

            foreach (var entry in dictionary) {
                Console.WriteLine(string.Format("{0, -30} - {1, -30}", entry.Key, entry.Value));
            }
        }


        [TestMethod]
        public void UpdateStatusFromExternalId() { 
        
            string externalSystemName = Enviornment.instance.GetVersionOneSource;
            string externalFieldName = Enviornment.instance.GetVersionOneReference;
            string externalId = "VOID-41";
            string statusId = "StoryStatus:133";
            UpdateResult result = new UpdateResult();

            bool success = v1processor.UpdateWorkitem(externalFieldName, externalId, externalSystemName, statusId, result);

            Assert.IsTrue(success);
            Assert.IsFalse(result.isDefault());
            Assert.IsTrue(result.modificationTime != DateTime.MinValue);
            Assert.IsFalse(string.IsNullOrEmpty(result.number));
        }

        [TestMethod]
        public void UpdateStatusFromExternalId_IssueNotExisting() {

            string externalSystemName = Enviornment.instance.GetVersionOneSource;
            string externalFieldName = Enviornment.instance.GetVersionOneReference;
            string externalId = "NOT-EXISTING-ISSUE";
            string statusId = "StoryStatus:133";
            UpdateResult result = new UpdateResult();


            bool success = v1processor.UpdateWorkitem(externalFieldName, externalId, externalSystemName, statusId, result);
            Assert.IsFalse(success);
            Assert.IsTrue(result.isDefault());
            Assert.IsTrue(result.modificationTime == DateTime.MinValue);
            Assert.IsTrue(string.IsNullOrEmpty(result.number));

        }


    }
}
