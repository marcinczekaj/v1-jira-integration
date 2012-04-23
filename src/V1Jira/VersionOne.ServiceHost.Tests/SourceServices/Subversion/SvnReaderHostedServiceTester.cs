using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.SourceServices;
using VersionOne.ServiceHost.SourceServices.Subversion;

namespace VersionOne.ServiceHost.Tests.SourceServices {
    [TestFixture]
    public class SvnReaderHostedServiceTester {
        /*
			 * We expect the Reader to 
			 *	Poke the connector with a revision
			 *	Consume revision info
			 *	Create ChangeSetInfos with the proper information
			 *		- Auther, Message, FilesChanged, ChangedDate
			 *		- List of reference strings, according to config'd REGEX (not trying to unit test the REGEX)
			 *	Maintain the proper revision number
			 * 
			 * The code we are really interested in is the "OnChangeSet" method.
			 * 	
			 */
        //[Ignore]

        private static XmlElement _config;

        private static XmlElement Config {
            get {
                if(_config == null) {
                    var doc = new XmlDocument();
                    doc.LoadXml(
                        "<Config><RepositoryPath>file:///svn/repo2/SVNRss</RepositoryPath><ReferenceExpression>[A-Z]{1,2}-+[0-9]{4}</ReferenceExpression></Config>");
                    _config = doc.DocumentElement;
                }
                return _config;
            }
        }

        private static void AssertOnChangeSet(string referenceExpression, int expectedRevision, List<string> references, List<string> changedFiles,
            int expectedLastRevision) {
            var reader = new TestableSvnReaderService();
            IEventManager eventManager = new EventManager();
            reader.Initialize(Config, eventManager, null);
            string expectedMessage = string.Format("{0} Exposed the Fronat API", string.Join(", ", references.ToArray()));
            var sourceOfExpected = new ChangeSetInfo("ExpectedAuthor", expectedMessage, new List<string>(), expectedRevision.ToString(), DateTime.Now, new List<string>());
            foreach(string reference in references) {
                sourceOfExpected.References.Add(reference);
            }
            foreach(string file in changedFiles) {
                sourceOfExpected.ChangedFiles.Add(file);
            }

            reader.LastRevisionNumber = expectedLastRevision;
            reader.TestProcessRevision(sourceOfExpected.RevisionAsInteger, sourceOfExpected.Author, sourceOfExpected.ChangeDate,
                sourceOfExpected.Message, sourceOfExpected.ChangedFiles, referenceExpression);

            ChangeSetInfo result = reader.LastChangeSet;

            Assert.AreEqual(sourceOfExpected.Author, result.Author);
            Assert.AreEqual(sourceOfExpected.ChangeDate, result.ChangeDate);
            Assert.AreEqual(sourceOfExpected.ChangedFiles.Count, result.ChangedFiles.Count, "ChangedFile cound does not match.");
            foreach(string file in sourceOfExpected.ChangedFiles) {
                Assert.IsTrue(result.ChangedFiles.Contains(file), "Result should contain changed file \"{0}\"", file);
            }

            Assert.AreEqual(sourceOfExpected.Message, result.Message);
            Assert.AreEqual(sourceOfExpected.References.Count, result.References.Count, "Reference Count does not match.");
            foreach(string reference in sourceOfExpected.References) {
                Assert.IsTrue(result.References.Contains(reference), "Result should contain reference \"{0}\".", reference);
            }

            Assert.AreEqual(sourceOfExpected.Revision, result.Revision);
            Assert.AreEqual(sourceOfExpected.Revision, reader.LastRevisionNumber.ToString());
        }

        [Test]
        public void ProcessRevisionOnlyOnce() {
            string referenceExpression = "[A-Z]{1,2}-+[0-9]{4}";

            var reader = new TestableSvnReaderService();
            IEventManager eventManager = new EventManager();
            reader.Initialize(Config, eventManager, null);

            reader.TestOnRevision(new SvnConnector.RevisionArgs(1, "Author", DateTime.Now, "message", new List<string>(), new ChangeSetDictionary()), referenceExpression);
            ChangeSetInfo firstResult = reader.LastChangeSet;

            reader.TestOnRevision(new SvnConnector.RevisionArgs(1, "Author", DateTime.Now, "message", new List<string>(), new ChangeSetDictionary()), referenceExpression);
            ChangeSetInfo secondResult = reader.LastChangeSet;

            Assert.AreSame(firstResult, secondResult);
        }

        [Test]
        public void ReaderGetsBasicInfo() {
            string referenceExpression = "[A-Z]{1,2}-+[0-9]{4}";

            List<String> references = new List<string>(),
                         changedFiles = new List<string>();
            references.Add("TK-1001");
            references.Add("S-1001");

            changedFiles.Add("/Path/Path/Foo.txt");

            AssertOnChangeSet(referenceExpression, 1, references, changedFiles, 0);
        }
    }

    internal class TestableSvnReaderService : SvnReaderHostedService {
        internal ChangeSetInfo LastChangeSet { get; private set; }

        protected override int LastRevision { get; set; }

        public int LastRevisionNumber {
            get { return LastRevision; }
            set { LastRevision = value; }
        }

        public void TestOnRevision(SvnConnector.RevisionArgs e, string referenceExpression) {
            base.ReferenceExpression = referenceExpression;
            _connector_Revision(this, e);
        }

        public void TestProcessRevision(int revision, string author, DateTime changeDate, string message, IList<string> filesChanged, string referenceExpression) {
            base.ReferenceExpression = referenceExpression;
            base.ProcessRevision(revision, author, changeDate, message, filesChanged, new ChangeSetDictionary());
        }

        protected override void PublishChangeSet(ChangeSetInfo changeSet) {
            LastChangeSet = changeSet;
        }
    }
}