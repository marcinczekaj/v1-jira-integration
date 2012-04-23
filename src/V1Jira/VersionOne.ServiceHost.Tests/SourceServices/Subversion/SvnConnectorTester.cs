using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.SourceServices;
using VersionOne.ServiceHost.SourceServices.Subversion;

namespace VersionOne.ServiceHost.Tests.SourceServices {
    [TestFixture]
    public class SvnConnectorTester {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            _lasttime = DateTime.MinValue;
            _lastauthor = null;
            _lastrevision = int.MinValue;
        }

        #endregion

        private void connector_Error(object sender, ExceptionEventArgs e) {
            Console.WriteLine(e.Exception.ToString());
            throw e.Exception;
        }

        private void connector_ChangeSet(object sender, SvnConnector.RevisionArgs e) {
        }

        private void connector_Committed(object sender, SvnConnector.CommitEventArgs e) {
            _lastauthor = e.Author;
            _lasttime = e.Time;
            _lastrevision = e.Revision;
        }

        private string _lastauthor;
        private DateTime _lasttime;
        private int _lastrevision;

        private static XmlElement _config;

        private static XmlElement Config {
            get {
                if(_config == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml("<Config><RepositoryPath>svn://svn/1/Tags/Build</RepositoryPath><ReferenceExpression>[A-Z]{1,2}-+[0-9]{4}</ReferenceExpression></Config>");
                    _config = doc.DocumentElement;
                }
                return _config;
            }
        }

        [Ignore("This test should only be run manually, as it requires an SVN repository.")]
        [Test]
        public void Bar() {
            var connector = new SvnConnector();
            connector.Revision += connector_ChangeSet;
            connector.Error += connector_Error;
            connector.SetAuthentication("Autobuild", "autobuild1");
            connector.Poke("file:///svn/repo2/SVNRss/Tags/Build", 16);
        }

        [Ignore("This test should only be run manually, as it requires an SVN repository.")]
        [Test]
        public void Baz() {
            var connector = new SvnConnector();
            connector.Revision += connector_ChangeSet;
            connector.Error += connector_Error;
            connector.SetAuthentication("Autobuild", "autobuild1");
            Dictionary<string, Dictionary<string, string>> properties =
                connector.GetProperies("svn://svn/1/Tags/Build/VersionOne.APIClient/52", 914, true);
        }

        [Ignore("This test should only be run manually, as it requires an SVN repository.")]
        [Test]
        public void Foo() {
            var reader = new TestableSvnReaderService();
            IEventManager eventManager = new EventManager();
            reader.Initialize(Config, eventManager, null);

            eventManager.Publish(new SvnReaderHostedService.SvnReaderIntervalSync());
        }

        [Test]
        public void TestGetRevisionProperty() {
            var connector = new SvnConnector();
            connector.Error += connector_Error;
            connector.SetAuthentication("Donald", string.Empty);

            string path = "svn://svn/1";
            RevisionPropertyCollection props = connector.GetRevisionProperties(path, 1);

            Assert.AreEqual(props["svn:author"], "Patrick");
            Assert.AreEqual(props["svn:log"], "Created Trunk");
            Assert.AreEqual(props["svn:date"], "2007-04-13T18:19:11.840126Z");
        }

        [Ignore("This test should only be run manually, as it requires an SVN repository.")]
        [Test]
        public void TestSetRevisionProperty() {
            var connector = new SvnConnector();
            connector.Error += connector_Error;
            connector.SetAuthentication("donald", "password");
            string path = "svn://lithium/Test";
            string propname = "v1:Test";
            string propval = DateTime.Now.ToString("u");
            int revision = 1;

            int changedrev = connector.SetRevisionProperty(path, revision, propname, propval);
            Assert.AreEqual(revision, changedrev);
            RevisionPropertyCollection props = connector.GetRevisionProperties(path, revision);
            Assert.AreEqual(props[propname], propval);
        }

        [Ignore("This test should only be run manually, as it requires an SVN repository.")]
        [Test]
        public void TestUpdateProperty() {
            var connector = new SvnConnector();
            connector.Error += connector_Error;
            connector.Committed += connector_Committed;

            string path = "file:///svnrepo/testrepo/";
            string tempWorkingPath = Path.GetTempPath() + Path.GetRandomFileName();
            Directory.CreateDirectory(tempWorkingPath);
            int outrev = connector.Checkout(path, tempWorkingPath, false, true);
            string newpropvalue = DateTime.Now.ToString("u");
            connector.SaveProperty("v1:Test", newpropvalue, tempWorkingPath, false, false);
            PropertiesCollection props = connector.GetProperies(tempWorkingPath, false);
            Assert.AreEqual(newpropvalue, props[tempWorkingPath]["v1:Test"]);

            ICollection<string> targets = new[] {tempWorkingPath};

            int newrev = connector.Commit(targets, true, false, "TestUpdateProperty");
            Assert.AreEqual(outrev + 1, _lastrevision);
            Assert.AreEqual(newrev, _lastrevision);
        }
    }
}