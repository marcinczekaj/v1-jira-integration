using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.SDK.APIClient;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.SourceServices;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.SourceServices {
    [TestFixture]
    public class ChangeSetWriterTester {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            _logmessages = new List<LogMessage>();
            _lastsaved = new List<SavedEventArgs>();
        }

        [TearDown]
        public void TearDown() {
            foreach(LogMessage message in _logmessages) {
                Console.WriteLine("Log Message: [{0}] {1}", message.Severity, message.Message);
            }
        }

        #endregion

        private IList<LogMessage> _logmessages;
        private IList<SavedEventArgs> _lastsaved;

        private static readonly DateTime referenceDate = new DateTime(2000, 1, 1, 12, 34, 56, DateTimeKind.Utc);

        private string GetReferenceChangeSetNameDef() {
            DateTime localTime = TimeZone.CurrentTimeZone.ToLocalTime(referenceDate);
            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(localTime);
            string formattedTime = string.Format("{0} UTC{1}{2}:{3:00}", localTime, offset.TotalMinutes >= 0 ? "+" : string.Empty, offset.Hours, offset.Minutes);
            return "'Author' on '" + formattedTime + "'";
        }

        private static XmlElement _configwithlink;

        private static XmlElement ConfigWithLink {
            get {
                if(_configwithlink == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml(
                        "<Config><ReferenceAttribute>Reference</ReferenceAttribute><ChangeComment>Updated by VersionOne.Fitter</ChangeComment><Link><Name>ChangeSet: {0}</Name><URL>http://server/{0}</URL><OnMenu>True</OnMenu></Link></Config>");
                    _configwithlink = doc.DocumentElement;
                }
                return _configwithlink;
            }
        }

        private static XmlElement _confignolink;

        private static XmlElement ConfigNoLink {
            get {
                if(_confignolink == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml("<Config><ReferenceAttribute>Reference</ReferenceAttribute><ChangeComment>Updated by VersionOne.Fitter</ChangeComment></Config>");
                    _confignolink = doc.DocumentElement;
                }
                return _confignolink;
            }
        }

        private static XmlElement _configAlwaysCreate;

        private static XmlElement ConfigAlwaysCreate {
            get {
                if(_configAlwaysCreate == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml(
                        "<Config><ReferenceAttribute>Reference</ReferenceAttribute><ChangeComment>Updated by VersionOne.Fitter</ChangeComment><AlwaysCreate>true</AlwaysCreate></Config>");
                    _configAlwaysCreate = doc.DocumentElement;
                }
                return _configAlwaysCreate;
            }
        }

        private void Services_BeforeSave(object sender, SavedEventArgs e) {
            _lastsaved.Add(e);
        }


        private void LogMessageListener(object pubobj) {
            var msg = (LogMessage)pubobj;
            _logmessages.Add(msg);
        }

        private class TestChangeSetWriterService : ChangeSetWriterService {
            private BaseStubCentral _stubcentral;

            protected override ICentral Central {
                get { return StubCentral; }
            }

            internal BaseStubCentral StubCentral {
                get {
                    if(_stubcentral == null) {
                        _stubcentral = new ChangeSetStubCentral();
                    }
                    return _stubcentral;
                }
            }

            internal event ResponseConnector.OnDataHandler BeforeRequest {
                add {
                    StubCentral.LocConnector.BeforeSendData += value;
                    StubCentral.MetaModelConnector.BeforeSendData += value;
                    StubCentral.ServicesConnector.BeforeSendData += value;

                    StubCentral.LocConnector.BeforeGetData += value;
                    StubCentral.MetaModelConnector.BeforeGetData += value;
                    StubCentral.ServicesConnector.BeforeGetData += value;
                }
                remove {
                    StubCentral.LocConnector.BeforeSendData -= value;
                    StubCentral.MetaModelConnector.BeforeSendData -= value;
                    StubCentral.ServicesConnector.BeforeSendData -= value;

                    StubCentral.LocConnector.BeforeGetData -= value;
                    StubCentral.MetaModelConnector.BeforeGetData -= value;
                    StubCentral.ServicesConnector.BeforeGetData -= value;
                }
            }

            #region Nested type: ChangeSetStubCentral

            private class ChangeSetStubCentral : BaseStubCentral {
                protected override string MetaKeys {
                    get { return "ChangeSetWriterTester"; }
                }

                protected override string ServicesKeys {
                    get { return "ChangeSetWriterTester"; }
                }

                protected override string LocKeys {
                    get { return "ChangeSetWriterTester"; }
                }
            }

            #endregion
        }

        [Test]
        public void ChangeSetAlreadyExists() {
            IEventManager mgr = new EventManager();
            var svc = new TestChangeSetWriterService();
            svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            svc.Initialize(ConfigNoLink, mgr, null);
            mgr.Subscribe(typeof(LogMessage), LogMessageListener);
            IList<string> references = new List<string>();
            references.Add("S-1000");
            mgr.Publish(new ChangeSetInfo("Author", "Message", new List<string>(), "5", referenceDate, references));

            IAttributeDefinition changesetreferencedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Reference");
            IAttributeDefinition changesetprimaryworkitemsdef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.PrimaryWorkitems");
            IAttributeDefinition changesetnamedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Name");
            IAttributeDefinition changesetdescriptionref = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Description");

            Assert.AreEqual(1, _lastsaved.Count);

            Asset beforesave = _lastsaved[0].Assets[0];

            Assert.AreEqual(GetReferenceChangeSetNameDef(), beforesave.GetAttribute(changesetnamedef).Value);
            Assert.AreEqual("Message", beforesave.GetAttribute(changesetdescriptionref).Value);
            Assert.AreEqual("5", beforesave.GetAttribute(changesetreferencedef).Value);
            Assert.AreEqual(svc.StubCentral.Services.GetOid("Story:1004"), beforesave.GetAttribute(changesetprimaryworkitemsdef).ValuesList[0]);

            //Assert.AreEqual(1,_logmessages.Count);
            Assert.AreEqual("Using existing Change Set: 5 (ChangeSet:1003:105)", _logmessages[0].Message);
        }

        [Test]
        public void ChangeSetNoReferences() {
            IEventManager mgr = new EventManager();
            var svc = new TestChangeSetWriterService();
            svc.Initialize(ConfigNoLink, mgr, null);
            mgr.Subscribe(typeof(LogMessage), LogMessageListener);
            mgr.Publish(new ChangeSetInfo("Author", "Message", new List<string>(), "6", DateTime.Now, new List<string>()));
            Assert.AreEqual(1, _logmessages.Count);
            Assert.AreEqual("No Change Set References. Ignoring Change Set: 6", _logmessages[0].Message);
        }

        [Test]
        public void ChangeSetNoReferences_AlwasyCreate() {
            IEventManager mgr = new EventManager();
            var svc = new TestChangeSetWriterService();
            svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            svc.Initialize(ConfigAlwaysCreate, mgr, null);
            mgr.Subscribe(typeof(LogMessage), LogMessageListener);
            mgr.Publish(new ChangeSetInfo("Author", "Message", new List<string>(), "6", referenceDate, new List<string>()));

            IAttributeDefinition changesetreferencedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Reference");
            IAttributeDefinition changesetprimaryworkitemsdef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.PrimaryWorkitems");
            IAttributeDefinition changesetnamedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Name");
            IAttributeDefinition changesetdescriptionref = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Description");

            Assert.AreEqual(1, _lastsaved.Count);

            Asset beforesave = _lastsaved[0].Assets[0];

            Assert.AreEqual(GetReferenceChangeSetNameDef(), beforesave.GetAttribute(changesetnamedef).Value);
            Assert.AreEqual("Message", beforesave.GetAttribute(changesetdescriptionref).Value);
            Assert.AreEqual("6", beforesave.GetAttribute(changesetreferencedef).Value);
            Assert.IsNull(beforesave.GetAttribute(changesetprimaryworkitemsdef));
        }

        [Test]
        public void ChangeSetNoWorkitems() {
            IEventManager mgr = new EventManager();
            var svc = new TestChangeSetWriterService();
            svc.Initialize(ConfigNoLink, mgr, null);
            mgr.Subscribe(typeof(LogMessage), LogMessageListener);
            IList<string> references = new List<string>();
            references.Add("TK-1111");
            mgr.Publish(new ChangeSetInfo("Author", "Message", new List<string>(), "7", DateTime.Now, references));
            Assert.AreEqual(2, _logmessages.Count);
            Assert.AreEqual("No Stories or Defects related to reference: TK-1111", _logmessages[0].Message);
            Assert.AreEqual("No Change Set References. Ignoring Change Set: 7", _logmessages[1].Message);
        }

        [Test]
        public void SaveChangeSet() {
            IEventManager mgr = new EventManager();
            var svc = new TestChangeSetWriterService();
            svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            svc.Initialize(ConfigNoLink, mgr, null);
            mgr.Subscribe(typeof(LogMessage), LogMessageListener);
            IList<string> references = new List<string>();
            references.Add("S-1000");
            mgr.Publish(new ChangeSetInfo("Author", "Message", new List<string>(), "8", referenceDate, references));

            IAttributeDefinition changesetreferencedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Reference");
            IAttributeDefinition changesetprimaryworkitemsdef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.PrimaryWorkitems");
            IAttributeDefinition changesetnamedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Name");
            IAttributeDefinition changesetdescriptionref = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Description");

            Assert.AreEqual(1, _lastsaved.Count);

            Asset beforesave = _lastsaved[0].Assets[0];

            Assert.AreEqual(GetReferenceChangeSetNameDef(), beforesave.GetAttribute(changesetnamedef).Value);
            Assert.AreEqual("Message", beforesave.GetAttribute(changesetdescriptionref).Value);
            Assert.AreEqual("8", beforesave.GetAttribute(changesetreferencedef).Value);
            Assert.AreEqual(svc.StubCentral.Services.GetOid("Story:1004"), beforesave.GetAttribute(changesetprimaryworkitemsdef).ValuesList[0]);
        }

        [Test]
        public void SaveChangeSetWithLink() {
            IEventManager mgr = new EventManager();
            var svc = new TestChangeSetWriterService();
            svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            svc.Initialize(ConfigWithLink, mgr, null);
            mgr.Subscribe(typeof(LogMessage), LogMessageListener);
            IList<string> references = new List<string>();
            references.Add("S-1000");
            mgr.Publish(new ChangeSetInfo("Author", "Message", new List<string>(), "8", referenceDate, references));

            Assert.AreEqual(2, _lastsaved.Count);

            Asset savedchangeset = _lastsaved[0].Assets[0];

            IAttributeDefinition changesetreferencedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Reference");
            IAttributeDefinition changesetprimaryworkitemsdef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.PrimaryWorkitems");
            IAttributeDefinition changesetnamedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Name");
            IAttributeDefinition changesetdescriptionref = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Description");

            Assert.AreEqual(GetReferenceChangeSetNameDef(), savedchangeset.GetAttribute(changesetnamedef).Value);
            Assert.AreEqual("Message", savedchangeset.GetAttribute(changesetdescriptionref).Value);
            Assert.AreEqual("8", savedchangeset.GetAttribute(changesetreferencedef).Value);
            Assert.AreEqual(svc.StubCentral.Services.GetOid("Story:1004"), savedchangeset.GetAttribute(changesetprimaryworkitemsdef).ValuesList[0]);

            Asset savedlink = _lastsaved[1].Assets[0];

            IAttributeDefinition linknamedef = svc.StubCentral.MetaModel.GetAttributeDefinition("Link.Name");
            IAttributeDefinition linkurldef = svc.StubCentral.MetaModel.GetAttributeDefinition("Link.URL");
            IAttributeDefinition linkonmenudef = svc.StubCentral.MetaModel.GetAttributeDefinition("Link.OnMenu");

            Assert.AreEqual("ChangeSet: 8", savedlink.GetAttribute(linknamedef).Value);
            Assert.AreEqual("http://server/8", savedlink.GetAttribute(linkurldef).Value);
            Assert.AreEqual(true, savedlink.GetAttribute(linkonmenudef).Value);
        }

        [Test]
        public void SaveExistingChangeSetAlreadyRelated() {
            IEventManager mgr = new EventManager();
            var svc = new TestChangeSetWriterService();
            svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            svc.Initialize(ConfigNoLink, mgr, null);
            mgr.Subscribe(typeof(LogMessage), LogMessageListener);
            IList<string> references = new List<string>();
            references.Add("S-1000");
            mgr.Publish(new ChangeSetInfo("Author", "Message", new List<string>(), "12", referenceDate, references));

            IAttributeDefinition changesetreferencedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Reference");
            IAttributeDefinition changesetprimaryworkitemsdef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.PrimaryWorkitems");
            IAttributeDefinition changesetnamedef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Name");
            IAttributeDefinition changesetdescriptionref = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Description");
            IAttributeDefinition changeSetLinksUrlDef = svc.StubCentral.MetaModel.GetAttributeDefinition("ChangeSet.Links.URL");

            Assert.AreEqual(1, _lastsaved.Count);

            Asset beforesave = _lastsaved[0].Assets[0];

            Assert.AreEqual(GetReferenceChangeSetNameDef(), beforesave.GetAttribute(changesetnamedef).Value);
            Assert.AreEqual("Message", beforesave.GetAttribute(changesetdescriptionref).Value);
            Assert.AreEqual("12", beforesave.GetAttribute(changesetreferencedef).Value);
            Assert.AreEqual(svc.StubCentral.Services.GetOid("Story:1004"), beforesave.GetAttribute(changesetprimaryworkitemsdef).ValuesList[0]);
            Assert.AreEqual(1, beforesave.GetAttribute(changeSetLinksUrlDef).ValuesList.Count);

            //Assert.AreEqual(1,_logmessages.Count);
            Assert.AreEqual("Using existing Change Set: 12 (ChangeSet:1052)", _logmessages[0].Message);
        }
    }
}