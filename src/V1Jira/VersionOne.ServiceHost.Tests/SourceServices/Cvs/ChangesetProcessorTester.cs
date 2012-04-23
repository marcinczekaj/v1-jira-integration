using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.SourceServices.Cvs;

namespace VersionOne.ServiceHost.Tests.SourceServices.Cvs 
{
    [TestFixture]
    public class ChangesetProcessorTester 
    {
        private const string ReferenceExpression = "[A-Z]{1,2}-[0-9]+";
        private IChangesetStorage storage;
        private ChangesetProcessor processor;
        private List<CvsChange> changes;

        private MockRepository repository;

        [SetUp]
        public void SetUp()
        {
            repository  = new MockRepository();
            changes = new List<CvsChange>();
        }

        [Test]
        public void GroupTest() 
        {
            storage = repository.Stub<IChangesetStorage>();            
            processor = new ChangesetProcessor(storage, ReferenceExpression);

            CvsChange change01 = CreateSingleChange("file", 0, "MessageTest");
            CvsChange change02 = CreateSingleChange("file1", 1, "MessageTest");
            CvsChange change03 = CreateSingleChange("file1", 2, "MessageTest");

            CvsChange change1 = CreateSingleChange("file", 2, "MessageTest1");

            CvsChange change2 = CreateSingleChange("file2", 3, "MessageTest2");

            CvsChange change30 = CreateSingleChange("file31", 14, "MessageTest3");
            CvsChange change31 = CreateSingleChange("file32", 10, "MessageTest3");
            CvsChange change32 = CreateSingleChange("file33", 11, "MessageTest3");

            changes.AddRange(new CvsChange[] { change01, change02, change03, change1, change2, change30, change31, change32 });
            IList<CvsChangeSet> changeSets = processor.Group(changes);
            Assert.AreEqual(changeSets.Count, 5);
            Assert.AreEqual(changeSets[0].Changes.Count, 2);
            Assert.AreEqual(changeSets[4].Changes.Count, 3);
        }

        [Test]
        public void TransformTest() 
        {
            storage = repository.Stub<IChangesetStorage>();            
            processor = new ChangesetProcessor(storage, ReferenceExpression);

            CvsChange change01 = CreateSingleChange("file", 0, "MessageTest");
            changes.Add(change01);
            IList<CvsChangeSet> changeSets = processor.Group(changes);
            Assert.AreEqual(1, changeSets.Count);

            IList<ChangeSetInfo> setInfos = (IList<ChangeSetInfo>)processor.Transform(changeSets);
            Assert.AreEqual(1, setInfos.Count);
            Assert.AreEqual(1, setInfos[0].ChangedFiles.Count);
            Assert.AreEqual(change01.File + " REVISION: 1.1 BRANCH: BranchName", setInfos[0].ChangedFiles[0]);
        }

        [Test]
        public void TimeSpanTest() 
        {
            storage = repository.Stub<IChangesetStorage>();
            processor = new ChangesetProcessor(storage, ReferenceExpression);

            CvsChange change01 = CreateSingleChange("file", 0, "Add B-01001");
            CvsChange change02 = CreateSingleChange("file2", 9, "Add B-01001");
            changes.Add(change01);
            changes.Add(change02);
            
            IList<CvsChangeSet> changeSets = processor.Group(changes);
            Assert.AreEqual(changeSets.Count, 1);
            Assert.AreEqual(changeSets[0].Changes.Count, 2);

            processor = new ChangesetProcessor(storage, ReferenceExpression);
            processor.ChangesetTimeSpanSeconds = 300;
            changeSets = processor.Group(changes);
            Assert.AreEqual(changeSets.Count, 2);
        }

        [Test]
        public void ProcessPartiallyPersistedChangesTest() 
        {
            storage = repository.StrictMock<IChangesetStorage>();
            processor = new ChangesetProcessor(storage, ReferenceExpression);
            
            CvsChange change01 = CreateSingleChange("file", 0, "Add B-01001");
            CvsChange change02 = CreateSingleChange("file2", 9, "Add B-01001");
            changes.Add(change01);
            changes.Add(change02);

            Expect.Call(storage.LookupChange(change01)).Return(change01);
            Expect.Call(storage.LookupChange(change02)).Return(null);
            storage.PersistChangeset(null);
            LastCall.IgnoreArguments();
            
            repository.ReplayAll();
            
            IList<CvsChangeSet> changeSets = processor.Group(changes);
            Assert.AreEqual(changeSets.Count, 1);
            Assert.AreEqual(changeSets[0].Changes.Count, 1);

            repository.VerifyAll();
        }

        [Test]
        public void ProcessPersistedChangesTest() 
        {
            storage = repository.StrictMock<IChangesetStorage>();
            processor = new ChangesetProcessor(storage, ReferenceExpression);

            CvsChange change01 = CreateSingleChange("file", 0, "Add B-01001");
            CvsChange change02 = CreateSingleChange("file2", 9, "Add B-01001");
            changes.Add(change01);
            changes.Add(change02);

            Expect.Call(storage.LookupChange(change01)).Return(change01);
            Expect.Call(storage.LookupChange(change02)).Return(change02);
            
            repository.ReplayAll();
            
            IList<CvsChangeSet> changeSets = processor.Group(changes);
            Assert.AreEqual(changeSets.Count, 0);

            repository.VerifyAll();
        }

        private static CvsChange CreateSingleChange(string fileName, int min, string message) 
        {
            return new CvsChange(fileName, "Author", "1.1", "BranchName", "SymNames", new DateTime(2010, 1, 1, 1, min, 0), message);
        }
    }
}