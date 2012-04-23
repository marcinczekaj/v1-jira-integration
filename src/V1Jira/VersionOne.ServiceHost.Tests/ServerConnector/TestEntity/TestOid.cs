using VersionOne.SDK.APIClient;

namespace VersionOne.ServiceHost.Tests.ServerConnector.TestEntity {
    public class TestOid : Oid {
        public TestOid(IAssetType assetType, int id, int? moment) : base(assetType, id, moment) {
        }

    }
}
