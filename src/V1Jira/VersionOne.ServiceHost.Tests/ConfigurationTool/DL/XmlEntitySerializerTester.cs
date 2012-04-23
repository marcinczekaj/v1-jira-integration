using System.Linq;
using NUnit.Framework;

using VersionOne.ServiceHost.ConfigurationTool;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.DL {
    [TestFixture]
    public class XmlEntitySerializerTester {
        private XmlEntitySerializer serializer;

        [SetUp]
        public void SetUp() {
            serializer = new XmlEntitySerializer();
        }

        [Test]
        public void SerializeTest() {
            const string urlValue = "http://versionone/";
            const long timerValue = 12345;
            var bugzillaServiceConfigurationEntity = new BugzillaServiceEntity
                                                         {Timer = {TimeoutMilliseconds = timerValue}, Url = urlValue};

            serializer.Serialize(new BaseServiceEntity[] { bugzillaServiceConfigurationEntity });
            serializer.SaveToFile("test.config");

            var bugzillaUrlNodes = serializer.OutputDocument.SelectNodes(XmlEntitySerializer.RootNodeXPath +
                "/*[starts-with(@class,'" + ServicesMap.BugzillaService.FullTypeName + "')]/BugzillaUrl");
            Assert.AreEqual(1, bugzillaUrlNodes.Count);
            Assert.AreEqual(urlValue, bugzillaUrlNodes[0].InnerText);

            var intervalNodes = serializer.OutputDocument.SelectNodes(XmlEntitySerializer.RootNodeXPath +
                "/*[starts-with(@class,'" + ServicesMap.TimerService.FullTypeName + "')]/PublishClass");
            Assert.AreEqual(2, intervalNodes.Count);
            Assert.IsTrue(intervalNodes[0].InnerText.StartsWith("VersionOne.ServiceHost.CommonMode+FlushProfile"));
            Assert.IsTrue(intervalNodes[1].InnerText.StartsWith("VersionOne.ServiceHost.BugzillaServices.BugzillaHostedService+IntervalSync"));
        }

        [Test]
        public void DeserializeTest() {
            serializer.Document.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <configuration>
	                <Services>
                		<WorkitemWriterService class=""VersionOne.ServiceHost.WorkitemServices.WorkitemWriterHostedService, VersionOne.ServiceHost.WorkitemServices""/>
                		<TestWriterService class=""VersionOne.ServiceHost.TestServices.TestWriterService, VersionOne.ServiceHost.TestServices""/>
                		<ChangeSetWriterService class=""VersionOne.ServiceHost.SourceServices.ChangeSetWriterService, VersionOne.ServiceHost.SourceServices""/>
	                </Services>
                </configuration>");
            var entities = serializer.Deserialize().ToList();
            Assert.AreEqual(3, entities.Count);
            Assert.AreEqual(typeof(WorkitemWriterEntity), entities[0].GetType());
            Assert.AreEqual(typeof(TestWriterEntity), entities[1].GetType());
            Assert.AreEqual(typeof(ChangesetWriterEntity), entities[2].GetType());
        }

        [Test]
        public void DeserializeNondescriptiveNamesTest() {
            serializer.Document.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <configuration>
	                <Services>
                		<Service1 class=""VersionOne.ServiceHost.WorkitemServices.WorkitemWriterHostedService, VersionOne.ServiceHost.WorkitemServices""/>
                		<Service2 class=""VersionOne.ServiceHost.TestServices.TestWriterService, VersionOne.ServiceHost.TestServices""/>
                		<Service3 class=""VersionOne.ServiceHost.SourceServices.ChangeSetWriterService, VersionOne.ServiceHost.SourceServices""/>
	                </Services>
                </configuration>");
            var entities = serializer.Deserialize().ToList();
            Assert.AreEqual(3, entities.Count);
            Assert.AreEqual(typeof(WorkitemWriterEntity), entities[0].GetType());
            Assert.AreEqual(typeof(TestWriterEntity), entities[1].GetType());
            Assert.AreEqual(typeof(ChangesetWriterEntity), entities[2].GetType());
        }

        [Test]
        public void DeserializeTimerTest() {
            serializer.Document.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <configuration>
	                <Services>
                		<BugzillaService class=""VersionOne.ServiceHost.BugzillaServices.BugzillaHostedService, VersionOne.ServiceHost.BugzillaServices""/>
                		<BugzillaService2 class=""VersionOne.ServiceHost.BugzillaServices.BugzillaHostedService, VersionOne.ServiceHost.BugzillaServices""/>
                		<ProfileTimer class=""VersionOne.ServiceHost.Core.Services.TimePublisherService, VersionOne.ServiceHost.Core""/>
                		<BugzillaTimer class=""VersionOne.ServiceHost.Core.Services.TimePublisherService, VersionOne.ServiceHost.Core"">
                			<Interval>700000</Interval>
			                <PublishClass>VersionOne.ServiceHost.BugzillaServices.BugzillaHostedService+IntervalSync, VersionOne.ServiceHost.BugzillaServices</PublishClass>
		                </BugzillaTimer>
	                </Services>
                </configuration>");
            var entities = serializer.Deserialize().ToList();
            Assert.AreEqual(2, entities.Count);
            Assert.AreEqual(typeof(BugzillaServiceEntity), entities[0].GetType());
            Assert.AreEqual(typeof(BugzillaServiceEntity), entities[1].GetType());
            var bugzilla = (BugzillaServiceEntity)entities[0];
            Assert.AreEqual(700000, bugzilla.Timer.TimeoutMilliseconds);
            var bugzilla2 = (BugzillaServiceEntity)entities[1];
            Assert.AreEqual(700000, bugzilla2.Timer.TimeoutMilliseconds);
        }

        [Test]
        public void DeserializeEmptyTest() {
            serializer.Document.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <configuration>
                    <Services/>
                </configuration>");
            var entities = serializer.Deserialize();
            Assert.AreEqual(0, entities.Count);
        }
    }
}