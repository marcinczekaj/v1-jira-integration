using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.TestServices.TestComplete;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.TestServices.TestComplete
{
    [TestFixture]
    public class TCLogReaderTester
    {
        [Test]
        public void TestEmptyTestRuns()
        {
            DateTime testTime = DateTime.Now;
            TCLogReader reader = new TCLogReader(new XmlDocument(), testTime, null);
            ICollection<TestRun> testRuns = reader.TestRuns;
            Assert.AreEqual(0, testRuns.Count, "EmptyRun:testRuns.Count");

            ICollection<SuiteRun> suiteRuns = reader.SuiteRuns;
            Assert.AreEqual(0, suiteRuns.Count, "EmptyRun:suiteRuns.Count");
        }

        [Test]
        public void TestGetTestRunsOnTestitemRun()
        {
            DateTime testTime = DateTime.Now;
            TCLogReader reader = new TCLogReader(GetTestXml(3), testTime, null);
            ICollection<TestRun> testRuns = reader.TestRuns;
            Assert.AreEqual(1, testRuns.Count, "ItemRun:testRuns.Count");
            IEnumerator<TestRun> numer = testRuns.GetEnumerator();
            numer.MoveNext();
            Verify(numer.Current, TestRun.TestRunState.Failed, ".ProjectTestItem2", null, 7);
        }

        [Test]
        public void TestGetSuiteRunsOnTestitemRun()
        {
            DateTime testTime = DateTime.Now;
            TCLogReader reader = new TCLogReader(GetTestXml(3), testTime, null);
            ICollection<SuiteRun> suiteRuns = reader.SuiteRuns;
            Assert.AreEqual(1, suiteRuns.Count, "ItemRun:suiteRuns.Count");
            IEnumerator<SuiteRun> numer = suiteRuns.GetEnumerator();
            numer.MoveNext();
            Verify(numer.Current, ".", 0, 1, 0, null);
        }

        [Test]
        public void TestGetTestRunsOnProjectRun()
        {
            DateTime testTime = DateTime.Now;
            TCLogReader reader = new TCLogReader(GetTestXml(2), testTime, null);
            ICollection<TestRun> testRuns = reader.TestRuns;
            Assert.AreEqual(2, testRuns.Count, "ProjectRun:testRuns.Count");
            IEnumerator<TestRun> numer = testRuns.GetEnumerator();
            numer.MoveNext();
            Verify(numer.Current, TestRun.TestRunState.Failed, ".ProjectTestItem2", null, 2);
            numer.MoveNext();
            Verify(numer.Current, TestRun.TestRunState.Passed, ".ProjectTestItem1", null, 9);
        }

        [Test]
        public void TestGetSuiteRunsOnProjectRun()
        {
            DateTime testTime = DateTime.Now;
            TCLogReader reader = new TCLogReader(GetTestXml(2), testTime, null);
            ICollection<SuiteRun> suiteRuns = reader.SuiteRuns;
            Assert.AreEqual(1, suiteRuns.Count, "ProjectRun:suiteRuns.Count");
            IEnumerator<SuiteRun> numer = suiteRuns.GetEnumerator();
            numer.MoveNext();
            Verify(numer.Current, ".", 1, 1, 0, null);
        }

        private static void Verify(SuiteRun suiteRun, string suiteName, int passed, int failed, int notRun,
                                   DateTime? testtime)
        {
            Assert.AreEqual(suiteName, suiteRun.Name, "suiteRun.Name");
            Assert.AreEqual(suiteName, suiteRun.SuiteRef, "suiteRun.SuiteRef");
            if (testtime != null)
                Assert.AreEqual(testtime, suiteRun.Stamp, "suiteRun.Stamp");
            Assert.AreEqual(passed, suiteRun.Passed, "suiteRun.Passed");
            Assert.AreEqual(failed, suiteRun.Failed, "suiteRun.Failed");
            Assert.AreEqual(notRun, suiteRun.NotRun, "suiteRun.NotRun");
        }

        private static void Verify(TestRun testRun, TestRun.TestRunState state, string testName, DateTime? testtime, double elasped)
        {
            Assert.AreEqual(testName, testRun.TestRef, "testRun.TestRef");
            Assert.AreEqual(state, testRun.State, "testRun.State");
            if (testtime != null)
                Assert.AreEqual(testtime, testRun.Stamp, "testRun.Stamp");
            Assert.AreEqual(elasped, testRun.Elapsed, 0.5, "testRun.Elapsed");
        }

        private XmlDocument GetTestXml(int file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ResourceLoader.LoadClassStream(string.Format("RootLogData.{0}.dat", file), GetType()));
            return doc;
        }
    }
}