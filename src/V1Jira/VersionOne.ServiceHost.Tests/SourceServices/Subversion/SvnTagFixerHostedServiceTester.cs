using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.SourceServices.Subversion;

namespace VersionOne.ServiceHost.Tests.SourceServices.Subversion {
    [TestFixture]
    public class SvnTagFixerHostedServiceTester {
        internal PropertiesCollection SourceProperties() {
            var result = new PropertiesCollection();
            var properties = new Dictionary<string, string>();
            properties.Add("svn:externals", "common svn://svn/repos/Trunk/Common");
            properties.Add("svn:mime-type", "you should never see me");
            result.Add("svn://svn/repos/Tags/Build/12/Common", properties);

            properties = new Dictionary<string, string>();
            properties.Add("svn:mime-type", "you should never see me");
            result.Add("svn://svn/repos/Tags/Build/12/Tool1", properties);
            return result;
        }

        internal PropertiesCollection ExpectedProperties(int revision) {
            var result = new PropertiesCollection();
            var properties = new Dictionary<string, string>();
            properties.Add("svn:externals", "common svn://svn/repos/Trunk/Common@" + revision.ToString());
            result.Add("svn://svn/repos/Tags/Build/12/Common", properties);
            return result;
        }

        private static XmlElement _config;

        private static XmlElement Config {
            get {
                if(_config == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml("<Config><RepositoryPath>file:///svn/repo2/SVNRss/Tags/Build</RepositoryPath><RepositoryRoot>file:///svn/repo2</RepositoryRoot></Config>");
                    _config = doc.DocumentElement;
                }
                return _config;
            }
        }

        [Test]
        public void FixerUpdatesExternals() {
            var fixer = new TestableSvnTagFixerHostedService();
            IEventManager eventManager = new EventManager();
            fixer.Initialize(Config, eventManager, null);

            const int expectedRevision = 212;
            PropertiesCollection sourceProperties = SourceProperties();
            PropertiesCollection expectedProperties = ExpectedProperties(expectedRevision);
            fixer.SourceProperties = sourceProperties;

            var changedPaths = new List<string>();
            changedPaths.Add("/repos/Tags/Build/12");

            var changeInfos = new ChangeSetDictionary();
            changeInfos.Add(changedPaths[0], new HackedChangedPathInfo(SubversionAction.Add, expectedRevision, "/Trunk"));

            DateTime changedate = DateTime.Now;

            string expectedMessage = string.Format("Build 12");

            // Get the class under test to process the revision
            fixer.TestProcessRevision(expectedRevision, "ExpectedAuthor", changedate, expectedMessage, changedPaths, changeInfos);

            PropertiesCollection result = fixer.LastSavedProperties;

            foreach(string path in expectedProperties.Keys) {
                Assert.IsTrue(result.ContainsKey(path));
                Dictionary<string, string> propertyList = result[path];
                Assert.IsTrue(propertyList.ContainsKey("svn:externals"));
                Assert.AreEqual(expectedProperties[path]["svn:externals"], result[path]["svn:externals"]);
            }

            foreach(string path in sourceProperties.Keys) {
                if(!expectedProperties.ContainsKey(path)) {
                    Assert.IsFalse(result.ContainsKey(path));
                }
            }
        }
    }


    internal class TestableSvnTagFixerHostedService : SvnTagFixerHostedService {
        private PropertiesCollection _lastSavedProperties = new PropertiesCollection();
        public PropertiesCollection SourceProperties { get; set; }

        public PropertiesCollection LastSavedProperties {
            get { return _lastSavedProperties; }
        }

        protected override int LastRevision { get; set; }

        public void TestProcessRevision(int revision, string author, DateTime changeDate, string message, IList<string> filesChanged, ChangeSetDictionary changeInfos) {
            base.ProcessRevision(revision, author, changeDate, message, filesChanged, changeInfos);
        }

        protected override PropertiesCollection GetProperties(string path, int revision, bool recurse) {
            return SourceProperties;
        }

        protected override void SaveProperties(PropertiesCollection toSave) {
            _lastSavedProperties = toSave;
        }
    }

    public class HackedChangedPathInfo : IChangedPathInfo {
        public SubversionAction _action;
        public string _copyFromPath;
        public int _copyFromRevision;

        public HackedChangedPathInfo(SubversionAction action, int copyFromRevision, string copyFromPath) {
            _action = action;
            _copyFromRevision = copyFromRevision;
            _copyFromPath = copyFromPath;
        }

        #region IChangedPathInfo Members

        public SubversionAction Action {
            get { return _action; }
        }

        public int CopyFromRevision {
            get { return _copyFromRevision; }
        }

        public string CopyFromPath {
            get { return _copyFromPath; }
        }

        #endregion
    }
}