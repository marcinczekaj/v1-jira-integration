using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServerConnector.Filters;
using VersionOne.SDK.APIClient;

namespace VersionOne.ServiceHost.Tests.ServerConnector.Filters {
    [TestFixture]
    public class FilterTester {
        private readonly MockRepository repository = new MockRepository();
        
        private IAssetType assetType;
        private IAttributeDefinition definition;

        [SetUp]
        public void SetUp() {
            assetType = repository.StrictMock<IAssetType>();
            definition = repository.StrictMock<IAttributeDefinition>();
        }

        [Test]
        public void CreateEmptyFilter() {
            var filter = Filter.Empty();
            var result = filter.GetFilter(assetType);

            Assert.AreEqual(false, result.HasTerms);
        }

        [Test]
        public void CreateFilter() {
            const string filterToken = "(Type='Custom_BaF_Status%3a1047'|Type!='Custom_BaF_Status%3a1048')";
            var filter = Filter.Or("Custom_BaFstatus2").Equal("Custom_BaF_Status:1047").NotEqual("Custom_BaF_Status:1048");

            Expect.Call(definition.Token).Repeat.Twice().Return("Type");
            Expect.Call(assetType.GetAttributeDefinition(null)).IgnoreArguments().Repeat.Twice().Return(definition);

            repository.ReplayAll();
            var result = filter.GetFilter(assetType);
            var token = result.Token;
            repository.VerifyAll();

            Assert.AreEqual(true, result.HasTerms);
            Assert.AreEqual(filterToken, token);
        }
    }
}