using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServerConnector;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Tests.ServerConnector.TestEntity;
using VersionOne.ServerConnector.Entities;
using VersionOne.SDK.APIClient;
using VersionOne.ServerConnector.Filters;
namespace VersionOne.ServiceHost.Tests.ServerConnector {
    [TestFixture]
    public class VersionOneProcessorTester {
        private TestVersionOneProcessor processor;
        private IServices mockServices;
        private IMetaModel mockMetaModel;
        private IQueryBuilder mockQueryBuilder;

        private readonly MockRepository repository = new MockRepository();

        [SetUp]
        public void SetUp() {
            var logger = repository.Stub<ILogger>();
            mockServices = repository.StrictMock<IServices>();
            mockMetaModel = repository.StrictMock<IMetaModel>();
            mockQueryBuilder = repository.StrictMock<IQueryBuilder>();

            processor = new TestVersionOneProcessor(null, logger);
            processor.ConnectTest(mockServices, mockMetaModel, mockQueryBuilder);
        }
        
        [Test]
        public void AddLinkToWorkitem() {
            const string url = "http://qqq.com";
            const string title = "Url title";
            var workitemAsset = new Asset(new TestOid(new TestAssetType("Workitem"), 100, null));
            var workitem = new TestWorkitem(workitemAsset, null);
            var link = new Link(url, title);
            var linkAsset = new TestAssetType("Link");
            var asset = new Asset(new TestOid(new TestAssetType("Link"), 10, null));

            Expect.Call(mockMetaModel.GetAssetType(VersionOneProcessor.LinkType)).Return(linkAsset);
            Expect.Call(mockQueryBuilder.Query(string.Empty, Filter.Empty())).IgnoreArguments().Return(new AssetList());
            Expect.Call(mockServices.New(null, null)).IgnoreArguments().Return(asset);
            Expect.Call(() => mockServices.Save(asset));

            repository.ReplayAll();
            processor.AddLinkToWorkitem(workitem, link);
            repository.VerifyAll();
        }

        [Test]
        public void AddLinkToWorkitemWithExistingLink() {
            const string type = "Link";
            const string url = "http://qqq.com";
            const string title = "Url title";
            var workitemAsset = new Asset(new TestOid(new TestAssetType("Workitem"), 100, null));
            var workitem = new TestWorkitem(workitemAsset, null);
            var link = new Link(url, title);
            var linkAsset = new TestAssetType(type);
            var definitions = new Dictionary<string, IAttributeDefinition> {
                {Entity.NameProperty, new TestAttributeDefinition(linkAsset)},
                {Link.OnMenuProperty, new TestAttributeDefinition(linkAsset)},
                {Link.UrlProperty, new TestAttributeDefinition(linkAsset)},
            };
            var linkOid = new TestOid(new TestAssetType(type, definitions), 10, null);
            var existedLink = new Asset(linkOid);

            Expect.Call(mockMetaModel.GetAssetType(VersionOneProcessor.LinkType)).Return(linkAsset);
            Expect.Call(mockQueryBuilder.Query(string.Empty, Filter.Empty())).IgnoreArguments().Return(new AssetList { existedLink });

            repository.ReplayAll();
            processor.AddLinkToWorkitem(workitem, link);
            repository.VerifyAll();
        }
    }
}