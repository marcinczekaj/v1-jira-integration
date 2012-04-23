using System;
using System.IO;
using System.Xml;
using VersionOne.ServiceHost.SourceServices.Cvs;
using NUnit.Framework;

namespace VersionOne.ServiceHost.Tests.SourceServices.Cvs {
    [TestFixture]
    public class ChangesetStorageTester 
    {
        private readonly string Filename = CvsComponentFactory.GetStorageFilename(Environment.CurrentDirectory, ChangesetStorage.XmlFileName, null);

        [TearDown]
        public void TearDown() 
        {
            File.Delete(Filename);
        }

        [Test]
        public void FileCreationTest() 
        {
            LoadChangeSetXml(true);
            Assert.IsTrue(File.Exists(Filename));
        }

        [Test]
        public void ChangeSetSerializationTest() 
        {
            ChangesetStorage storage = new ChangesetStorage(Filename);
            CvsChange change = CreateSingleChange("test.ext", 0, "Update to TK-01001");
            storage.PersistChangeset(new CvsChangeSet(change.Author, "TestId", change.ChangeDate, change.Message));
            storage.Flush();

            CvsChangeSet changeSet = storage.GetChangeset("TestId");
            Assert.IsNotNull(changeSet);
            Assert.IsTrue(changeSet.IsPersistent);

            storage.PersistChangeset(new CvsChangeSet(change.Author, "TestId2", change.ChangeDate, change.Message));

            changeSet = storage.GetChangeset("TestId2");
            Assert.IsNotNull(changeSet);
            Assert.IsFalse(changeSet.IsPersistent);
        }

        [Test]
        public void LookupChangeTest() 
        {
            LoadChangeSetXml(true);
            ChangesetStorage storage = new ChangesetStorage(Filename);

            CvsChange change = CreateSingleChange("test.ext", 0, "Update to TK-01001");
            storage = new ChangesetStorage(Filename);

            CvsChange foundChange = storage.LookupChange(change);
            Assert.IsNotNull(foundChange);
            Assert.AreEqual(foundChange, change);

            foundChange = storage.LookupChange(CreateSingleChange("test.ext", 0, "ttt"));
            Assert.IsNull(foundChange);
        }

        [Test]
        public void XmlFileCreationTest() 
        {
            ChangesetStorage storage = new ChangesetStorage(Filename);
            CvsChange change = CreateSingleChange("test.ext", 0, "Update to TK-01001");
            CvsChangeSet changeSet = new CvsChangeSet(change.Author, "TestId", change.ChangeDate, change.Message);
            changeSet.Changes.Add(change);
            storage.PersistChangeset(changeSet);
            storage.PersistChangeset(new CvsChangeSet(change.Author, "TestId2", change.ChangeDate, change.Message));
            storage.Flush();

            XmlDocument document = new XmlDocument();
            document.Load(Filename);

            XmlNodeList changesetNodes = document.SelectNodes("//changesets/changeset");
            Assert.AreEqual(changesetNodes.Count, 2);

            XmlNodeList changeNodes = changesetNodes[0].SelectNodes("change");
            Assert.AreEqual(changeNodes.Count, 1);
        }

        private void LoadChangeSetXml(bool flushToFile) 
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Resources.ChangeSetsXml);
            ChangesetStorage storage = new ChangesetStorage(doc, Filename);
            
            if (flushToFile) 
            {
                storage.Flush();
            }
        }

        private static CvsChange CreateSingleChange(string fileName, int minutes, string message) 
        {
            return new CvsChange(fileName, "Author", "1.1", "BranchName", "symNames", new DateTime(2010, 1, 1, 1, minutes, 0), message);
        }
    }
}