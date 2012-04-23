using System.Linq;
using NUnit.Framework;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.Filters;

namespace VersionOne.ServiceHost.Tests.ServerConnector.Filters {
    [TestFixture]
    [Ignore("Integration tests")]
    public class GroupFilterIntegrationTester : BaseIntegrationTester {
        [Test]
        public void And() {
            const string storyName = "Story 1";
            const string secondStoryName = "Story 2";

            var assetScope = AssetDisposer.CreateAndRegisterForDisposal(() => CreateProject("_INTEG_TEST", null, RootProjectToken));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(storyName, null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory(secondStoryName, null, assetScope.Oid, null, null, null));

            var filter = GroupFilter.And(Filter.Equal(Entity.NameProperty, storyName),
                                         Filter.OfTypes(VersionOneProcessor.StoryType),
                                         Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            var workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.IsNotNull(workitems);
            Assert.AreEqual(1, workitems.Count);
            Assert.IsTrue(workitems.Any(item => string.Equals(item.Name, storyName)));
            Assert.IsFalse(workitems.Any(item => string.Equals(item.Name, secondStoryName)));
        }

        [Test]
        public void MutuallyExclusiveAnd() {
            var assetScope = AssetDisposer.CreateAndRegisterForDisposal(() => CreateProject("_INTEG_TEST", null, RootProjectToken));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory("1", null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateDefect("2", null, assetScope.Oid, null, null, null));

            var filter = GroupFilter.And(Filter.OfTypes(VersionOneProcessor.StoryType),
                                         Filter.OfTypes(VersionOneProcessor.DefectType),
                                         Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            var workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(0, workitems.Count);
        }

        [Test]
        public void Or() {
            var assetScope = AssetDisposer.CreateAndRegisterForDisposal(() => CreateProject("_INTEG_TEST", null, RootProjectToken));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateStory("1", null, assetScope.Oid, null, null, null));
            AssetDisposer.CreateAndRegisterForDisposal(() => CreateDefect("2", null, assetScope.Oid, null, null, null));

            var filter = GroupFilter.And(
                GroupFilter.Or(Filter.OfTypes(VersionOneProcessor.StoryType), Filter.OfTypes(VersionOneProcessor.DefectType)),
                Filter.Equal(Entity.ScopeProperty, assetScope.Oid.Momentless));
            var workitems = V1Processor.GetPrimaryWorkitems(filter);
            
            Assert.AreEqual(2, workitems.Count);
            Assert.IsTrue(workitems.Any(item => item is Story && item.Name == "1"));
            Assert.IsTrue(workitems.Any(item => item is Defect && item.Name == "2"));
        }
    }
}