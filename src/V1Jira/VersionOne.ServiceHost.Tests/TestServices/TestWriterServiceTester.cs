using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using NUnit.Framework;
using VersionOne.SDK.APIClient;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.TestServices {
    [TestFixture]
    public class TestWriterServiceTester {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            _svc = new TestableTestWriterService();
            _mgr = new EventManager();
            _logmessages = new List<LogMessage>();
            _lastsaved = new List<SavedEventArgs>();
            _requests = new List<DataRequestEventArgs>();
            _svc.BeforeRequest += svc_BeforeRequest;
            _svc.StubCentral.Services.BeforeSave += Services_BeforeSave;
            _mgr.Subscribe(typeof(LogMessage), LogMessageListener);
        }

        [TearDown]
        public void TearDown() {
            WriteLogMessagesToTrace();
        }

        #endregion

        private void WriteLogMessagesToTrace() {
            foreach(var message in _logmessages) {
                if(message.Exception != null) {
                    Trace.WriteLine(message.Message);
                    Trace.WriteLine(message.Exception.ToString());
                }
            }
        }

        private void AssertSingleValueAttribute(object expected, Asset subject, string attributeToken) {
            IAttributeDefinition def = _svc.StubCentral.MetaModel.GetAttributeDefinition(attributeToken);
            object actual = subject.GetAttribute(def).Value;
            Assert.AreEqual(expected, actual, attributeToken);
        }

        private void AssertMultiValueAttribute(object expected, Asset subject, string attributeToken) {
            IAttributeDefinition def = _svc.StubCentral.MetaModel.GetAttributeDefinition(attributeToken);
            IList values = subject.GetAttribute(def).ValuesList;
            Assert.IsNotNull(values);
            Assert.IsTrue(values.Contains(expected), "Attribute: {0}\n Values: {1}\n Expected: {2}", attributeToken, EnumerateValues(values), expected);
        }

        private string EnumerateValues(IList values) {
            var valueArray = new object[values.Count];
            values.CopyTo(valueArray, 0);
            string result = "{ ";

            foreach(var value in values) {
                result += string.Format("{0},", value);
            }
            result += " }";
            return result;
        }

        private void AssertLogMessage(string expectedMessage) {
            Assert.Greater(_logmessages.Count, 0);
            bool found = false;
            foreach(var foundMessage in _logmessages) {
                if(foundMessage.Message.Contains(expectedMessage)) {
                    found = true;
                    break;
                }
            }

            if(!found) {
                Assert.Fail("Should have saved message \"{0}\" to log.", expectedMessage);
            }
        }

        private void AssertNoLogExceptions() {
            foreach(var message in _logmessages) {
                if(message.Exception != null) {
                    Assert.Fail(message.Exception.ToString());
                }

                if((message.Message.Contains("exception")) | (message.Message.Contains("Exception"))) {
                    Assert.Fail(message.Message);
                }
            }
        }

        private void LogMessageListener(object pubobj) {
            _logmessages.Add((LogMessage)pubobj);
        }

        private void svc_BeforeRequest(object sender, DataRequestEventArgs e) {
            _requests.Add(e);
        }

        private void Services_BeforeSave(object sender, SavedEventArgs e) {
            _lastsaved.Add(e);
        }

        private IList<LogMessage> _logmessages;
        private IList<SavedEventArgs> _lastsaved;
        private IList<DataRequestEventArgs> _requests;
        private IEventManager _mgr;
        private TestableTestWriterService _svc;

        private XmlElement _config;
        private XmlElement _configNoDefect;
        private XmlElement _configCurrentDefect;

        private string _defaultConfigXml =
            @"<TestWriterService><Settings><ApplicationUrl>http://localhost/VersionOne.Web/</ApplicationUrl><Username>admin</Username><Password>admin</Password><APIVersion>6.4.0.0</APIVersion><IntegratedAuth>false</IntegratedAuth></Settings><PassedOid>TestStatus:129</PassedOid><FailedOid>TestStatus:155</FailedOid><TestReferenceAttribute>Reference</TestReferenceAttribute><ChangeComment>Updated by VersionOne.ServiceHost</ChangeComment><!-- Embedded Rich Text (HTML) is valid in this suffix --><DescriptionSuffix>Check the external test system for more details.</DescriptionSuffix><CreateDefect>All</CreateDefect></TestWriterService>";

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

        private XmlElement ConfigNoDefect {
            get {
                if(_configNoDefect == null) {
                    var doc = new XmlDocument();
                    string configXml = _defaultConfigXml.Replace("<CreateDefect>All</CreateDefect>", "<CreateDefect>None</CreateDefect>");
                    doc.LoadXml(configXml);
                    _configNoDefect = doc.DocumentElement;
                }
                return _configNoDefect;
            }
        }

        private XmlElement ConfigCurrentDefect {
            get {
                if(_configCurrentDefect == null) {
                    var doc = new XmlDocument();
                    string configXml = _defaultConfigXml.Replace("<CreateDefect>All</CreateDefect>", "<CreateDefect>CurrentIteration</CreateDefect>");
                    doc.LoadXml(configXml);
                    _configCurrentDefect = doc.DocumentElement;
                }
                return _configCurrentDefect;
            }
        }

        [Test]
        public void CreateDefectClosedAT() {
            _svc.Initialize(Config, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "closedTest");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(1, _lastsaved.Count);
            Asset lastSaved = _lastsaved[0].Assets[0];
            Assert.AreEqual("Defect", lastSaved.AssetType.Token);
            AssertMultiValueAttribute(_svc.StubCentral.Services.GetOid("Story:1030"), lastSaved, "Defect.AffectedPrimaryWorkitems");
            AssertSingleValueAttribute("Story \"S-01004\" has failing Acceptance Test(s)", lastSaved, "Defect.Name");
            AssertSingleValueAttribute(
                string.Format("One or more acceptance tests failed at \"{0}\".<BR />Check the external test system for more details.", testRun.Stamp),
                lastSaved,
                "Defect.Description");
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid("Scope:0"), lastSaved, "Defect.Scope");
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid("Timebox:1009"), lastSaved, "Defect.Timebox");
            AssertNoLogExceptions();
        }

        [Test]
        public void CreateDefectClosedAT_NoTimebox() {
            _svc.Initialize(Config, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "closedTestNoTimebox");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(1, _lastsaved.Count);
            Asset lastSaved = _lastsaved[0].Assets[0];
            Assert.AreEqual("Defect", lastSaved.AssetType.Token);
            AssertMultiValueAttribute(_svc.StubCentral.Services.GetOid("Story:1030"), lastSaved, "Defect.AffectedPrimaryWorkitems");
            AssertSingleValueAttribute("Story \"S-01004\" has failing Acceptance Test(s)", lastSaved, "Defect.Name");
            AssertSingleValueAttribute(
                string.Format("One or more acceptance tests failed at \"{0}\".<BR />Check the external test system for more details.", testRun.Stamp),
                lastSaved,
                "Defect.Description");
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid("Scope:0"), lastSaved, "Defect.Scope");
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid("Timebox:1009"), lastSaved, "Defect.Timebox");
            AssertNoLogExceptions();
        }

        [Test]
        public void CreateDefectCurrentIteration_Active() {
            _svc.Initialize(ConfigCurrentDefect, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "Test4");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(1, _lastsaved.Count, "Should create a Defect when CreateDefect config is \"CurrentIteration\", and there is an active iteration in-scope.");
            Asset lastSaved = _lastsaved[0].Assets[0];
            Assert.AreEqual("Defect", lastSaved.AssetType.Token);
            AssertMultiValueAttribute(_svc.StubCentral.Services.GetOid("Story:1030"), lastSaved, "Defect.AffectedPrimaryWorkitems");
            AssertSingleValueAttribute("Story \"S-01004\" has failing Acceptance Test(s)", lastSaved, "Defect.Name");
            AssertSingleValueAttribute(
                string.Format("One or more acceptance tests failed at \"{0}\".<BR />Check the external test system for more details.", testRun.Stamp),
                lastSaved,
                "Defect.Description");
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid("Scope:0"), lastSaved, "Defect.Scope");
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid("Timebox:1010"), lastSaved, "Defect.Timebox");
            AssertNoLogExceptions();
        }

        [Test]
        public void CreateDefectCurrentIteration_Closed() {
            _svc.Initialize(ConfigCurrentDefect, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "Test5");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not create a Defect when CreateDefect config is \"CurrentIteration\", and there is NO active iteration in-scope.");
            AssertNoLogExceptions();
        }

        [Test]
        public void CreateDefectNone() {
            _svc.Initialize(ConfigNoDefect, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "closedTest");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not create a Defect when CreateDefect config is \"None\".");
            AssertNoLogExceptions();
        }

        [Test]
        public void DontCreateExistingDefect() {
            _svc.Initialize(Config, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "closedTestHasDefect");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not create a Defect when one already exists.");
            AssertNoLogExceptions();
        }

        [Test]
        public void DontSaveSuiteRunNoReference() {
            _svc.Initialize(Config, _mgr, null);
            var suiteRun = new SuiteRun("Name", "Description", DateTime.Now, null);
            _mgr.Publish(suiteRun);

            Assert.AreEqual(0, _lastsaved.Count);

            suiteRun = new SuiteRun("Name", "Description", DateTime.Now, "FooBar");
            _mgr.Publish(suiteRun);
            Assert.AreEqual(0, _lastsaved.Count);
            AssertNoLogExceptions();
        }

        [Test]
        public void NoOpEmptyReference() {
            _svc.Initialize(Config, _mgr, null);
            var testRun = new TestRun(DateTime.Now, null);
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not save any tests when the reference is not found in V1.");
            AssertLogMessage("Test Reference is null or empty. Skipping...");
            AssertNoLogExceptions();
        }

        [Test]
        public void NoOpNoStatusChange() {
            _svc.Initialize(Config, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "testRef");
            testRun.State = TestRun.TestRunState.NotRun;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not save any tests when the status does not change.");

            testRun = new TestRun(DateTime.Now, "passedTest");
            testRun.State = TestRun.TestRunState.Passed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not save any tests when the status does not change.");

            testRun = new TestRun(DateTime.Now, "failedTest");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not save any tests when the status does not change.");
            AssertNoLogExceptions();
        }

        [Test]
        public void NoOpNonExistentAT() {
            _svc.Initialize(Config, _mgr, null);
            var testRun = new TestRun(DateTime.Now, "fooBar");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(0, _lastsaved.Count, "Should not save any tests when the reference is not found in V1.");
            AssertLogMessage("No Tests found by reference");
            AssertNoLogExceptions();
        }

        [Test]
        public void SaveExistingATWithFailedStatus() {
            _svc.Initialize(Config, _mgr, null);

            var testRun = new TestRun(DateTime.Now, "failedTest");
            testRun.State = TestRun.TestRunState.Passed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(1, _lastsaved.Count);
            Asset lastSaved = _lastsaved[0].Assets[0];
            Assert.AreEqual("Test", lastSaved.AssetType.Token);
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid(Config["PassedOid"].InnerText), lastSaved, "Test.Status");

            testRun.State = TestRun.TestRunState.NotRun;
            _mgr.Publish(testRun);
            Assert.AreEqual(2, _lastsaved.Count);
            lastSaved = _lastsaved[1].Assets[0];
            Assert.AreEqual("Test", lastSaved.AssetType.Token);
            AssertSingleValueAttribute(Oid.Null, lastSaved, "Test.Status");
            AssertNoLogExceptions();
        }

        [Test]
        public void SaveExistingATWithNoStatus() {
            _svc.Initialize(Config, _mgr, null);

            var testRun = new TestRun(DateTime.Now, "testRef");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(1, _lastsaved.Count);
            Asset lastSaved = _lastsaved[0].Assets[0];
            Assert.AreEqual("Test", lastSaved.AssetType.Token);
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid(Config["FailedOid"].InnerText), lastSaved, "Test.Status");

            testRun.State = TestRun.TestRunState.Passed;
            _mgr.Publish(testRun);
            Assert.AreEqual(2, _lastsaved.Count);
            lastSaved = _lastsaved[1].Assets[0];
            Assert.AreEqual("Test", lastSaved.AssetType.Token);
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid(Config["PassedOid"].InnerText), lastSaved, "Test.Status");
            AssertNoLogExceptions();
        }

        [Test]
        public void SaveExistingATWithPassedStatus() {
            _svc.Initialize(Config, _mgr, null);

            var testRun = new TestRun(DateTime.Now, "passedTest");
            testRun.State = TestRun.TestRunState.Failed;
            testRun.Elapsed = 10;
            _mgr.Publish(testRun);

            Assert.AreEqual(1, _lastsaved.Count);
            Asset lastSaved = _lastsaved[0].Assets[0];
            Assert.AreEqual("Test", lastSaved.AssetType.Token);
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid(Config["FailedOid"].InnerText), lastSaved, "Test.Status");

            testRun.State = TestRun.TestRunState.NotRun;
            _mgr.Publish(testRun);
            Assert.AreEqual(2, _lastsaved.Count);
            lastSaved = _lastsaved[1].Assets[0];
            Assert.AreEqual("Test", lastSaved.AssetType.Token);
            AssertSingleValueAttribute(Oid.Null, lastSaved, "Test.Status");
            AssertNoLogExceptions();
        }

        [Test]
        public void SaveSuiteRun() {
            _svc.Initialize(Config, _mgr, null);
            var suiteRun = new SuiteRun("Name", "Description", DateTime.Now, "SuiteRef");
            suiteRun.Passed = 1;
            suiteRun.Failed = 2;
            suiteRun.NotRun = 3;
            suiteRun.Elapsed = 4;
            _mgr.Publish(suiteRun);

            /*	Validate
			 *		AsseType, Name, Description, Stamp, TestSuite, Passed, Failed, NotRun, Elapsed 
			 */
            Assert.AreEqual(1, _lastsaved.Count);
            Asset lastSaved = _lastsaved[0].Assets[0];
            Assert.AreEqual("TestRun", lastSaved.AssetType.Token);
            AssertSingleValueAttribute(suiteRun.Name, lastSaved, "TestRun.Name");
            AssertSingleValueAttribute(suiteRun.Description, lastSaved, "TestRun.Description");
            AssertSingleValueAttribute(suiteRun.Stamp, lastSaved, "TestRun.Date");
            AssertSingleValueAttribute(_svc.StubCentral.Services.GetOid("TestSuite:9977"), lastSaved, "TestRun.TestSuite");
            AssertSingleValueAttribute(suiteRun.Passed, lastSaved, "TestRun.Passed");
            AssertSingleValueAttribute(suiteRun.Failed, lastSaved, "TestRun.Failed");
            AssertSingleValueAttribute(suiteRun.NotRun, lastSaved, "TestRun.NotRun");
            AssertSingleValueAttribute(suiteRun.Elapsed, lastSaved, "TestRun.Elapsed");
            AssertNoLogExceptions();
        }
    }

    internal class TestableTestWriterService : TestWriterService {
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

        #region Nested type: TestStubCentral

        private class TestStubCentral : BaseStubCentral {
            protected override string MetaKeys {
                get { return "TestWriterServiceTester"; }
            }

            protected override string ServicesKeys {
                get { return "TestWriterServiceTester"; }
            }

            protected override string LocKeys {
                get { return "TestWriterServiceTester"; }
            }
        }

        #endregion
    }
}