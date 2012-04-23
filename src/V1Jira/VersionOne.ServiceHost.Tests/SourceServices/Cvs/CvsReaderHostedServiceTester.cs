using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Ninject;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.SourceServices.Cvs;

namespace VersionOne.ServiceHost.Tests.SourceServices.Cvs {
    [TestFixture]
    public class CvsReaderHostedServiceTester {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            repository = new MockRepository();

            profileMock = repository.StrictMock<IProfile>();
            eventManagerMock = repository.StrictMock<IEventManager>();
            connectorMock = repository.StrictMock<ICvsConnector>();
            logParserMock = repository.StrictMock<ICvsLogParser>();
            processorMock = repository.StrictMock<IChangesetProcessor>();
            storageMock = repository.Stub<IChangesetStorage>();
            containerMock = repository.StrictMock<IKernel>();

            CvsComponentFactory.Instance.RegisterOverride(connectorMock);
            CvsComponentFactory.Instance.RegisterOverride(logParserMock);
            CvsComponentFactory.Instance.RegisterOverride(processorMock);
            CvsComponentFactory.Instance.RegisterOverride(storageMock);
        }

        [TearDown]
        public void TearDown() {
            CvsComponentFactory.Instance.ResetOverrides();
        }

        #endregion

        private MockRepository repository;
        private IProfile profileMock;
        private IEventManager eventManagerMock;
        private ICvsConnector connectorMock;
        private ICvsLogParser logParserMock;
        private IChangesetProcessor processorMock;
        private IChangesetStorage storageMock;
        private IKernel containerMock;

        private static XmlElement CreateConfigNode() {
            var doc = new XmlDocument();
            doc.LoadXml(Resources.CvsServiceConfigXmlStub);
            return doc.DocumentElement;
        }

        private static List<CvsChange> CreateSingleEntryListOfChanges() {
            var changes = new List<CvsChange>();
            var change = new CvsChange("/path/to/file.txt", "user", "1.1", "branchName", "symNames", DateTime.Now, "test");
            changes.Add(change);

            return changes;
        }

        private static List<CvsChangeSet> CreateSingleEntryListOfChangesets(IEnumerable<CvsChange> changes) {
            var result = new List<CvsChangeSet>(1);
            var changeSet = new CvsChangeSet("user", "1", DateTime.Now, "test");
            changeSet.Changes.AddRange(changes);
            result.Add(changeSet);

            return result;
        }

        private static List<ChangeSetInfo> CreateChangeSetInfos(ICollection<CvsChangeSet> changesets) {
            var result = new List<ChangeSetInfo>(changesets.Count);

            foreach(var item in changesets) {
                var info = new ChangeSetInfo(item.Author, item.Message, null, item.ChangesetId, item.ChangeDate, null);
                result.Add(info);
            }

            return result;
        }

        public class TestEventManager : IEventManager {
            public readonly IList<ChangeSetInfo> Changes = new List<ChangeSetInfo>();
            private Type pubType;
            private EventDelegate subscriber;

            #region IEventManager Members

            public void Subscribe(Type pubtype, EventDelegate listener) {
                subscriber = listener;
                pubType = pubtype;
            }

            public void Unsubscribe(Type pubtype, EventDelegate listener) {
            }

            public void Publish(object pubobj) {
                if(pubobj.GetType() == pubType && subscriber != null) {
                    subscriber.Invoke(pubobj);
                }

                if(pubobj is ChangeSetInfo) {
                    Changes.Add((ChangeSetInfo)pubobj);
                }
            }

            #endregion
        }

        [Test]
        public void HandleConnectorErrorTest() {
            var connectorException = new Exception();
            var publishObject = new object();

            eventManagerMock.Subscribe(typeof(CvsReaderHostedService.IntervalSync), null);
            LastCall.IgnoreArguments();
            connectorMock.Error += null;
            LastCall.IgnoreArguments();
            IEventRaiser eventRaiser = LastCall.GetEventRaiser();
            eventManagerMock.Publish(publishObject);
            LastCall.IgnoreArguments();

            repository.ReplayAll();

            var service = new CvsReaderHostedService();
            service.Initialize(CreateConfigNode(), eventManagerMock, profileMock);
            eventRaiser.Raise(connectorMock, new ExceptionEventArgs(connectorException));

            repository.VerifyAll();
        }

        [Test]
        public void InitializeServiceTest() {
            eventManagerMock.Subscribe(typeof(CvsReaderHostedService.IntervalSync), null);
            LastCall.IgnoreArguments();
            connectorMock.Error += null;
            LastCall.IgnoreArguments();

            repository.ReplayAll();

            var service = new CvsReaderHostedService();
            service.Initialize(CreateConfigNode(), eventManagerMock, profileMock);
            Assert.AreEqual(service.Connector, connectorMock);

            repository.VerifyAll();
        }

        [Test]
        public void OnIntervalTest() {
            var publishObject = new CvsReaderHostedService.IntervalSync();
            var profileInnerMock = repository.StrictMock<IProfile>();

            var eventManager = new TestEventManager();
            List<CvsChange> changes = CreateSingleEntryListOfChanges();
            List<CvsChangeSet> changeSets = CreateSingleEntryListOfChangesets(changes);
            List<ChangeSetInfo> changeSetInfos = CreateChangeSetInfos(changeSets);

            using(repository.Ordered()) {
                connectorMock.Error += null;
                LastCall.IgnoreArguments();

                Expect.Call(profileMock[CvsReaderHostedService.LastQueryDateKey]).Return(profileInnerMock);
                Expect.Call(profileInnerMock.Value).PropertyBehavior().IgnoreArguments().Return(null).Repeat.Once();

                connectorMock.RunLogCommand(DateTime.Now, DateTime.Now);
                LastCall.IgnoreArguments();

                Expect.Call(logParserMock.Parse()).Return(changes);

                Expect.Call(processorMock.Group(changes)).Return(changeSets);
                Expect.Call(processorMock.Transform(changeSets)).Return(changeSetInfos);

                Expect.Call(profileMock[CvsReaderHostedService.LastQueryDateKey]).Return(profileInnerMock);
            }

            repository.ReplayAll();

            var service = new CvsReaderHostedService();
            service.Initialize(CreateConfigNode(), eventManager, profileMock);
            eventManager.Publish(publishObject);
            Assert.AreEqual(eventManager.Changes.Count, 1);

            repository.VerifyAll();
        }
    }
}