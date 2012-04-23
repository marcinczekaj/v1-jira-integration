using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.SourceServices.Cvs;

namespace VersionOne.ServiceHost.Tests.SourceServices.Cvs {
    [TestFixture]
    public class CvsLogParserTester 
    {
        private static CvsLogParser CreateLogParser(string sourceXml) 
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(sourceXml);
            return new CvsLogParser(document);
        }

        [Test]
        public void ParseEmptyCvsResponseTest() 
        {
            CvsLogParser parser = CreateLogParser(Resources.EmptyCvsServerLog);
            
            IList<CvsChange> result = parser.Parse();
            Assert.IsNull(result);
        }

        [Test]
        public void ParseSingleEntryCvsResponseTest() 
        {
            CvsLogParser parser = CreateLogParser(Resources.NonEmptyCvsServerLog);
            
            IList<CvsChange> result = parser.Parse();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            
            CvsChange changeset = result[0];
            Assert.AreEqual(changeset.Author, "cvsuser");
            Assert.AreEqual(changeset.Message, "addition to TK-00001");
            Assert.AreEqual(changeset.File, "new2/2.txt");
            Assert.AreEqual(changeset.Branch, "BranchName");
            Assert.AreEqual(changeset.SymNames, "SymNames");
        }
    }
}