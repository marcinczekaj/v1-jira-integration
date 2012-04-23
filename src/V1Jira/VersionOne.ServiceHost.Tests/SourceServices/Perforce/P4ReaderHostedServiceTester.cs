using System;
using System.Collections.Generic;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.SourceServices.Perforce;
using System.Text;
using VersionOne.ServiceHost.SourceServices;
using NUnit.Framework;
using System.Xml;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Tests.SourceServices.Perforce
{
    [TestFixture]
    public class P4ReaderHostedServiceTester
    {

        [Test]
        public void ReaderGetsBasicInfo()
        {
            string referenceExpression = "[A-Z]{1,2}-+[0-9]{4}";

            List<String> references = new List<string>(),
                         changedFiles = new List<string>();
            references.Add("TK-1001");
            references.Add("S-1001");

            changedFiles.Add("/Path/Path/Foo.txt");

            AssertOnChangeSet(referenceExpression, 1, references, changedFiles, 0);
        }

        [Test]
        public void ProcessRevisionOnlyOnce()
        {
            string referenceExpression = "[A-Z]{1,2}-+[0-9]{4}";

            TestableP4ReaderService reader = new TestableP4ReaderService();
            IEventManager eventManager = new EventManager();
            reader.Initialize(Config, eventManager, null, null);

            reader.TestOnRevision(new P4ChangeSetEventArgs(1, "Author", "message", DateTime.Now, new List<string>()), referenceExpression);
            ChangeSetInfo firstResult = reader.LastChangeSet;

            reader.TestOnRevision(new P4ChangeSetEventArgs(1, "Author", "message", DateTime.Now, new List<string>()), referenceExpression);
            ChangeSetInfo secondResult = reader.LastChangeSet;

            Assert.AreSame(firstResult, secondResult);

            reader.Dispose();

        }

        private static XmlElement _config;
        private static XmlElement Config
        {
            get
            {
                if (_config == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml("<Config><Port>JSDKsrv01:1666</Port><View>//Depot/VersionOne/...</View><Password></Password><User></User><ReferenceExpression>[BD]{1}-[0-9]+</ReferenceExpression></Config>");
                    _config = doc.DocumentElement;
                }
                return _config;
            }
        }


		private static void AssertOnChangeSet(string referenceExpression, int expectedRevision, List<string> references, List<string> changedFiles, int expectedLastRevision)
		{
            TestableP4ReaderService reader = new TestableP4ReaderService();
			IEventManager eventManager = new EventManager();
            reader.Initialize(Config, eventManager, new EmptyProfile(), null);
			string expectedMessage = string.Format("{0} Exposed the Fronat API", string.Join(", ", references.ToArray()));
			ChangeSetInfo sourceOfExpected = new ChangeSetInfo("ExpectedAuthor", expectedMessage, new List<string>(), expectedRevision.ToString(), DateTime.Now, new List<string>());
			foreach(string reference in references)
				sourceOfExpected.References.Add(reference);
			foreach (string file in changedFiles)
				sourceOfExpected.ChangedFiles.Add(file);

			reader.LastRevisionNumber = expectedLastRevision;
            reader.TestProcessChange(sourceOfExpected.RevisionAsInteger, sourceOfExpected.Author, sourceOfExpected.ChangeDate,
			                       sourceOfExpected.Message, sourceOfExpected.ChangedFiles, referenceExpression );

            ChangeSetInfo result = reader.LastChangeSet;
			

			Assert.AreEqual(sourceOfExpected.Author, result.Author);
			Assert.AreEqual(sourceOfExpected.ChangeDate, result.ChangeDate);
			Assert.AreEqual(sourceOfExpected.ChangedFiles.Count, result.ChangedFiles.Count, "ChangedFile cound does not match.");
			foreach (string file in sourceOfExpected.ChangedFiles)
                Assert.IsTrue(result.ChangedFiles.Contains(file), "Result should contain changed file \"{0}\"", file);
			
			Assert.AreEqual(sourceOfExpected.Message, result.Message);
			Assert.AreEqual(sourceOfExpected.References.Count, result.References.Count, "Reference Count does not match.");
			foreach (string reference in sourceOfExpected.References)
				Assert.IsTrue(result.References.Contains(reference), "Result should contain reference \"{0}\".", reference);

			Assert.AreEqual(sourceOfExpected.Revision, result.Revision);
			Assert.AreEqual(sourceOfExpected.Revision, reader.LastRevisionNumber.ToString());

            reader.Dispose();
		}
	}
}
