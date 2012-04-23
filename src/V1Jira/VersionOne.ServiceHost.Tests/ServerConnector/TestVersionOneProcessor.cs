using System.Xml;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Tests.ServerConnector {
    public class TestVersionOneProcessor : VersionOneProcessor {
        public TestVersionOneProcessor(XmlElement config, ILogger logger) : base(config, logger) {
        }

        internal void ConnectTest(IServices testServices, IMetaModel testMetaData, IQueryBuilder testQueryBuilder) {
            base.Connect(testServices, testMetaData, testQueryBuilder);
        }
    }
}