using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServiceHost.Tests.ServerConnector.TestEntity;

namespace VersionOne.ServiceHost.Tests.Utility {
    public class TestValueId : ValueId {
        private TestValueId(Oid oid, string name) : base(oid, name) { }

        public static TestValueId Create(string name, string type, int id) {
            var oid = new TestOid(new TestAssetType(type), id, null);
            return new TestValueId(oid, name);
        }
    }
}