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
    public class WorkitemWriterTest {

        Enviornment env = Enviornment.instance;
        IVersionOneProcessor v1processor;

        public WorkitemWriterTest()
        {
            LoggerFactory.register();
            V1ProcessorFactory.register();
        }

        [TestInitialize]
        public void Initialize() {
            v1processor = ComponentRepository.Instance.Resolve<IVersionOneProcessor>();
            v1processor.ValidateConnection();
        }

        [TestCleanup]
        public void Cleanup() {

        }


        [TestMethod()]
        public void UpdateWorkitemTest() {
            //given
            WorkitemWriter writer = new WorkitemWriter("Source");
            var defect = new VersionOne.ServiceHost.WorkitemServices.Defect();
            defect.Number = "D-78694";
            defect.ExternalId = "JRA-1000";
            defect.ExternalSystemName = "JIRA";
            UrlToExternalSystem url = new UrlToExternalSystem("http://jira.sabre.com/", "JIRA LINK");
            defect.ExternalUrl = url;

            //when 
            writer.UpdateExternalWorkitem(defect);

            //then
            //TODO check if it was executed

        }
    }
}
