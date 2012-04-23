using VersionOne.ServiceHost.WorkitemServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using IntegrationTests.Enviornments;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core;
using IntegrationTests.Logger;
using IntegrationTests.Factory;
using System.Collections.Generic;

namespace IntegrationTests
{
    
    
    /// <summary>
    ///This is a test class for ExternalWorkitemQuerierTest and is intended
    ///to contain all ExternalWorkitemQuerierTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExternalWorkitemQuerierTest {

        Enviornment env = Enviornment.instance;

        public ExternalWorkitemQuerierTest() {
            LoggerFactory.register();
            V1ProcessorFactory.register();
        }

        [TestInitialize]
        public void Initialize() {
            IVersionOneProcessor v1processor = ComponentRepository.Instance.Resolve<IVersionOneProcessor>();
            v1processor.ValidateConnection();
        }

        [TestCleanup]
        public void Cleanup() {
        }

        /// <summary>
        ///A test for GetWorkitemsReadyForSynchronisation
        ///</summary>
        [TestMethod()]
        public void GetWorkitemsReadyForSynchronisationTest() {
            
            //given
            ExternalWorkitemQuerier target = new ExternalWorkitemQuerier();
            DateTime modifiedSince = new DateTime(2012, 1, 1);
            string lastModifiedWorkitemId = String.Empty;
            string sourceName = env.GetVersionOneSource; 
            string externalIdFieldName = env.GetVersionOneReference;

            //when 
            WorkitemStateChangeCollection actual = target.GetWorkitemsReadyForSynchronisation(modifiedSince, sourceName, externalIdFieldName, lastModifiedWorkitemId);

            //then
            Console.WriteLine(string.Format("Found {0} items", actual.Count));
            Assert.AreNotEqual(0, actual);

            foreach (var item in actual) {
                Console.WriteLine(string.Format("Item externalId is {0}", item.ExternalId));
            }
        }


        [TestMethod()]
        public void GetNewWorkitemsTest()
        {

            //given
            ExternalWorkitemQuerier target = new ExternalWorkitemQuerier();
            DateTime lastCreatedTimestamp = new DateTime(2012, 1, 1);
            string lastCreatedWorkitemId = String.Empty;
            List<String> scopes = new List<string>() {"Scope:2257958" };

            //when
            var results = target.GetNewWorkitems(lastCreatedTimestamp, lastCreatedWorkitemId, scopes);

            //then
            Console.WriteLine(string.Format("Found {0} items", results.Count));
            Assert.AreNotEqual(0, results.Count);

            foreach (var item in results)
            {
                Console.WriteLine(string.Format("Item externalId is {0}", item.ExternalId));
            }
        }
    }
}
