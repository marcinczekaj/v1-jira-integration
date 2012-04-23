using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Ninject;
using VersionOne.Profile;
using VersionOne.SDK.APIClient;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.EvalServices;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.EvalServices {
    [TestFixture]
    public class EvalManagerServicesTester {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            _lastSendData = new List<DataRequestEventArgs>();
            _lastsaved = new List<SavedEventArgs>();
            _logmessages = new List<LogMessage>();
            _eventmanager = new EventManager();
            _eventmanager.Subscribe(typeof(LogMessage), LogMessageListener);
        }

        #endregion

        private static readonly string TempLogFile = Path.Combine(Path.GetTempPath(), "temp.log");

        private IEventManager _eventmanager;
        private IList<SavedEventArgs> _lastsaved;
        private IList<DataRequestEventArgs> _lastSendData;

        private string _defaultConfigXml =
            @"<EvalManagerService><Settings><ApplicationUrl>http://localhost/VersionOne.Web/</ApplicationUrl><Username>admin</Username><Password>admin</Password><APIVersion>7.2.0.0</APIVersion><IntegratedAuth>false</IntegratedAuth></Settings></EvalManagerService>";

        private XmlElement _config;

        private XmlElement Config {
            get {
                if(_config == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml(_defaultConfigXml);
                    _config = doc.DocumentElement;
                }
                return _config;
            }
        }

        private IList<LogMessage> _logmessages;

        private void LogMessageListener(object pubobj) {
            _logmessages.Add((LogMessage)pubobj);
        }

        private void ServicesConnector_BeforeSendData(object sender, DataRequestEventArgs e) {
            _lastSendData.Add(e);
        }

        private void Services_BeforeSave(object sender, SavedEventArgs e) {
            _lastsaved.Add(e);
        }

        internal class V1InstallerStubService : IHostedService {
            private readonly bool _pass;

            public V1InstallerStubService(bool pass) {
                _pass = pass;
            }

            public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile) {
                eventManager.Subscribe(typeof(V1Install), InstallHandler);
            }

            public void Start() { }

            private void InstallHandler(object pubobj) {
                var install = (V1Install)pubobj;

                if(_pass) {
                    install.State.Status = V1InstallState.StatusType.Success;
                    install.State.ActualInstanceName = "Instance X";
                    install.State.ReleaseVersion = "Release Y";
                    install.State.Url = "Url Z";
                } else {
                    install.State.Status = V1InstallState.StatusType.Failure;
                    install.State.StatusMessage = "Stub Service Failure";
                    install.State.LogFile = CreateTempLog();
                }
            }

            private static string CreateTempLog() {
                File.WriteAllText(TempLogFile, "Log File Content");
                return TempLogFile;
            }
        }

        internal class TestEvalManagerService : EvalManagerService {
            private BaseStubCentral _stubcentral;

            protected override ICentral Central {
                get { return StubCentral; }
            }

            internal BaseStubCentral StubCentral {
                get {
                    if(_stubcentral == null) {
                        _stubcentral = new TestStubCentral();
                    }
                    return _stubcentral;
                }
            }

            #region Nested type: TestStubCentral

            private class TestStubCentral : BaseStubCentral {
                protected override string MetaKeys {
                    get { return "EvalManagerServicesTester"; }
                }

                protected override string ServicesKeys {
                    get { return "EvalManagerServicesTester"; }
                }

                protected override string LocKeys {
                    get { return "EvalManagerServicesTester"; }
                }
            }

            #endregion
        }

        [Test]
        public void RequestFailure() {
            var svc = new TestEvalManagerService();
            svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            svc.StubCentral.ServicesConnector.BeforeSendData += ServicesConnector_BeforeSendData;

            var installstub = new V1InstallerStubService(false);
            installstub.Initialize(Config, _eventmanager, null);
            svc.Initialize(Config, _eventmanager, null);
            _eventmanager.Publish(new EvalManagerService.IntervalSync());
            Assert.AreEqual(3, _logmessages.Count);
            Assert.AreEqual("Unable to form a valid instance name for Request: R-01000", _logmessages[0].Message);
            Assert.AreEqual("Install request R-01001 failed to process\nStatus Message: Stub Service Failure", _logmessages[1].Message);
            Assert.AreEqual(string.Format("Log File Attached: {0} Note:4000", TempLogFile), _logmessages[2].Message);

            Assert.AreEqual(1, _lastsaved.Count);

            SavedEventArgs savedNoteEvent = _lastsaved[0];
            Assert.AreEqual(1, savedNoteEvent.Assets.Count);
            Asset savedNote = savedNoteEvent.Assets[0];

            IAttributeDefinition NoteName = svc.StubCentral.MetaModel.GetAttributeDefinition("Note.Name");
            IAttributeDefinition NoteAsset = svc.StubCentral.MetaModel.GetAttributeDefinition("Note.Asset");
            IAttributeDefinition NotePersonal = svc.StubCentral.MetaModel.GetAttributeDefinition("Note.Personal");
            IAttributeDefinition NoteContent = svc.StubCentral.MetaModel.GetAttributeDefinition("Note.Content");

            Oid requestOid = Oid.FromToken("Request:1027", svc.StubCentral.MetaModel);

            Assert.AreEqual("Setup Log", savedNote.GetAttribute(NoteName).Value);
            Assert.AreEqual(requestOid, savedNote.GetAttribute(NoteAsset).Value);
            Assert.AreEqual(false, savedNote.GetAttribute(NotePersonal).Value);
            Assert.AreEqual("Log File Content", savedNote.GetAttribute(NoteContent).Value);

            //Verify Closed Request
            DataRequestEventArgs lastSend = _lastSendData[_lastSendData.Count - 1];
            string url = lastSend.Prefix + lastSend.Path;
            Assert.AreEqual("rest-1.v1/Data/Request/1027?op=Inactivate", url);
        }

        [Test]
        public void RequestNotProcessed() {
            EvalManagerService svc = new TestEvalManagerService();
            svc.Initialize(Config, _eventmanager, null);
            _eventmanager.Publish(new EvalManagerService.IntervalSync());
            Assert.AreEqual(2, _logmessages.Count);
            Assert.AreEqual("Unable to form a valid instance name for Request: R-01000", _logmessages[0].Message);
            Assert.AreEqual("Install request R-01001 was never processed", _logmessages[1].Message);
        }

        [Test]
        public void RequestSuccess() {
            var svc = new TestEvalManagerService();

            svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            svc.StubCentral.ServicesConnector.BeforeSendData += ServicesConnector_BeforeSendData;

            var installstub = new V1InstallerStubService(true);
            installstub.Initialize(Config, _eventmanager, null);
            svc.Initialize(Config, _eventmanager, null);
            _eventmanager.Publish(new EvalManagerService.IntervalSync());

            Assert.AreEqual(1, _logmessages.Count);
            Assert.AreEqual("Unable to form a valid instance name for Request: R-01000", _logmessages[0].Message);

            Assert.AreEqual(3, _lastsaved.Count);

            Oid storyOid = Oid.FromToken("Story:2000", svc.StubCentral.MetaModel);

            //Verify Saved Story
            SavedEventArgs storySaveEvent = _lastsaved[0];
            Assert.AreEqual(1, storySaveEvent.Assets.Count);
            Asset savedStory = storySaveEvent.Assets[0];
            Assert.AreEqual(storyOid, savedStory.Oid);

            IAttributeDefinition StoryName = svc.StubCentral.MetaModel.GetAttributeDefinition("Story.Name");
            IAttributeDefinition StoryReference = svc.StubCentral.MetaModel.GetAttributeDefinition("Story.Reference");
            IAttributeDefinition StorySource = svc.StubCentral.MetaModel.GetAttributeDefinition("Story.Source");
            IAttributeDefinition StoryScope = svc.StubCentral.MetaModel.GetAttributeDefinition("Story.Scope");
            IAttributeDefinition StoryRequests = svc.StubCentral.MetaModel.GetAttributeDefinition("Story.Requests");
            IAttributeDefinition StoryOwners = svc.StubCentral.MetaModel.GetAttributeDefinition("Story.Owners");
            IAttributeDefinition StoryCategory = svc.StubCentral.MetaModel.GetAttributeDefinition("Story.Category");

            IAttributeDefinition LinkName = svc.StubCentral.MetaModel.GetAttributeDefinition("Link.Name");
            IAttributeDefinition LinkURL = svc.StubCentral.MetaModel.GetAttributeDefinition("Link.URL");
            IAttributeDefinition LinkAsset = svc.StubCentral.MetaModel.GetAttributeDefinition("Link.Asset");
            IAttributeDefinition LinkOnMenu = svc.StubCentral.MetaModel.GetAttributeDefinition("Link.OnMenu");

            Oid storySourceOid = Oid.FromToken("StorySource:2001", svc.StubCentral.MetaModel);
            Oid storyCategoryOid = Oid.FromToken("StoryCategory:1001", svc.StubCentral.MetaModel);
            Oid scopeOid = Oid.FromToken("Scope:0", svc.StubCentral.MetaModel);
            Oid requestOid = Oid.FromToken("Request:1027", svc.StubCentral.MetaModel);
            Oid memberOid = Oid.FromToken("Member:20", svc.StubCentral.MetaModel);

            Assert.AreEqual("Company A", savedStory.GetAttribute(StoryName).Value);
            Assert.AreEqual("Instance X", savedStory.GetAttribute(StoryReference).Value);
            Assert.AreEqual(storySourceOid, savedStory.GetAttribute(StorySource).Value);
            Assert.AreEqual(storyCategoryOid, savedStory.GetAttribute(StoryCategory).Value);
            Assert.AreEqual(scopeOid, savedStory.GetAttribute(StoryScope).Value);
            Assert.AreEqual(requestOid, savedStory.GetAttribute(StoryRequests).ValuesList[0]);
            Assert.AreEqual(memberOid, savedStory.GetAttribute(StoryOwners).ValuesList[0]);

            //Verify Saved Story Link
            SavedEventArgs savedStoryLinkEvent = _lastsaved[1];
            Assert.AreEqual(1, savedStoryLinkEvent.Assets.Count);
            Asset savedStoryLink = savedStoryLinkEvent.Assets[0];
            Assert.AreEqual("Url Z", savedStoryLink.GetAttribute(LinkName).Value);
            Assert.AreEqual("Url Z", savedStoryLink.GetAttribute(LinkURL).Value);
            Assert.AreEqual(savedStory.Oid, savedStoryLink.GetAttribute(LinkAsset).Value);
            Assert.IsTrue((bool)savedStoryLink.GetAttribute(LinkOnMenu).Value);

            //Verify Saved Request Link
            SavedEventArgs savedRequestLinkEvent = _lastsaved[2];
            Assert.AreEqual(1, savedRequestLinkEvent.Assets.Count);
            Asset savedRequestLink = savedRequestLinkEvent.Assets[0];
            Assert.AreEqual("Url Z", savedRequestLink.GetAttribute(LinkName).Value);
            Assert.AreEqual("Url Z", savedRequestLink.GetAttribute(LinkURL).Value);
            Assert.AreEqual(requestOid, savedRequestLink.GetAttribute(LinkAsset).Value);
            Assert.IsTrue((bool)savedRequestLink.GetAttribute(LinkOnMenu).Value);

            //Verify Closed Request
            DataRequestEventArgs lastSend = _lastSendData[_lastSendData.Count - 1];
            string url = lastSend.Prefix + lastSend.Path;
            Assert.AreEqual("rest-1.v1/Data/Request/1027?op=Inactivate", url);
        }
    }
}