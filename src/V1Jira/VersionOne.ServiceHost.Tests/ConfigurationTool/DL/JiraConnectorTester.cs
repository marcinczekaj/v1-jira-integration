using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.DL;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.DL {
    [TestFixture]
    public class JiraConnectorTester {
        private const string JiraUrl = "http://integsrv01:8083/rpc/soap/jirasoapservice-v2";
        private const string JiraUser = "admin";
        private const string JiraPassword = "admin";

        [Test]
        [Ignore("This test requieres running JIRA server.")]
        public void GetPrioritiesTest() {
            var connector = new JiraConnector(JiraUrl, JiraUser, JiraPassword);
            connector.Login();
            var priorities = connector.GetPriorities();
            Assert.IsNotNull(priorities);
            Assert.IsTrue(priorities.Count > 0);
        }
    }
}