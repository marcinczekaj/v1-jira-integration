using System.Linq;
using NUnit.Framework;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;

namespace VersionOne.ServiceHost.Tests.ServerConnector.Filters {
    [TestFixture]
    [Ignore("Integration tests")]
    public class FilterIntegrationTester : BaseIntegrationTester {
        private const string StoryName = "Story 1";
        private const string SecondStoryName = "Story 2";
        private const string ThirdStoryName = "Story 3";

        [Test]
        public void And() {
            var assetScope = AssetDisposer.CreateAndRegisterForDisposal(() => CreateProject("_INTEG_TEST", null, RootProjectToken));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(StoryName, null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(SecondStoryName, null, assetScope.Oid, null, null, null));

            var filter = GroupFilter.And(
                Filter.And(Entity.NameProperty).Equal(StoryName).NotEqual(SecondStoryName),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            var workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(1, workitems.Count);
            Assert.IsTrue(workitems.Any(item => string.Equals(item.Name, StoryName)));
            Assert.IsFalse(workitems.Any(item => string.Equals(item.Name, SecondStoryName)));

            filter = GroupFilter.And(
                Filter.And(Entity.NameProperty).Equal(StoryName).NotEqual(StoryName),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(0, workitems.Count);
        }

        [Test]
        public void Or() {
            var assetScope = AssetDisposer.CreateAndRegisterForDisposal(() => CreateProject("_INTEG_TEST", null, RootProjectToken));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(StoryName, null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(SecondStoryName, null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(ThirdStoryName, null, assetScope.Oid, null, null, null));

            var filter = GroupFilter.And(
                Filter.Or(Entity.NameProperty).Equal(StoryName).Equal(SecondStoryName),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            var workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(2, workitems.Count);
            Assert.IsTrue(workitems.Any(item => string.Equals(item.Name, StoryName)));
            Assert.IsTrue(workitems.Any(item => string.Equals(item.Name, SecondStoryName)));
            Assert.IsFalse(workitems.Any(item => string.Equals(item.Name, ThirdStoryName)));

            filter = GroupFilter.And(
                Filter.Or(Entity.NameProperty).Equal(StoryName).NotEqual(StoryName),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            workitems = V1Processor.GetPrimaryWorkitems(filter);

            Assert.AreEqual(3, workitems.Count);
        }

        [Test]
        public void Empty() {
            var assetScope = AssetDisposer.CreateAndRegisterForDisposal(() => CreateProject("_INTEG_TEST", null, RootProjectToken));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(StoryName, null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(SecondStoryName, null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateDefect("A defect", null, assetScope.Oid, null, null, null));

            var filter = GroupFilter.And(
                Filter.Empty(),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            var workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(3, workitems.Count);
            Assert.AreEqual(2, workitems.Where(item => item is Story).Count());
            Assert.AreEqual(1, workitems.Where(item => item is Defect).Count());
        }

        [Test]
        public void Closed() {
            var assetScope = AssetDisposer.CreateAndRegisterForDisposal(() => CreateProject("_INTEG_TEST", null, RootProjectToken));
            var firstStoryAsset = AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(StoryName, null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(SecondStoryName, null, assetScope.Oid, null, null, null));

            var filter = GroupFilter.And(
                Filter.Closed(false),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            var workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(2, workitems.Count);

            ExecuteOperation(firstStoryAsset, VersionOneProcessor.InactivateOperation);

            filter = GroupFilter.And(
                Filter.Closed(false),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(1, workitems.Count);
            Assert.IsFalse(workitems.Any(item => item.Name == StoryName));
            Assert.IsTrue(workitems.Any(item => item.Name == SecondStoryName));

            filter = GroupFilter.And(
                Filter.Closed(true),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(1, workitems.Count);
            Assert.IsTrue(workitems.Any(item => item.Name == StoryName));
            Assert.IsFalse(workitems.Any(item => item.Name == SecondStoryName));

            ExecuteOperation(firstStoryAsset, VersionOneProcessor.ReactivateOperation);
        }
    }
}