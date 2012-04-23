using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.TestServices.QTP;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.TestServices.QTP {
    [TestFixture]
    public class QTPReaderServiceTester {
        private static void Verify(TestRun actual, double expectedElapsed, string expectedTestRef, DateTime expectedStamp, TestRun.TestRunState expectedState) {
            Assert.AreEqual(expectedTestRef, actual.TestRef, "Test Reference");
            Assert.AreEqual(expectedStamp, actual.Stamp, "Test Stamp");
            Assert.AreEqual(expectedElapsed, actual.Elapsed, "Test Elapsed");
            Assert.AreEqual(expectedState, actual.State, "Test State");
        }

        private XmlDocument GetTestReportXml(string file) {
            var doc = new XmlDocument();
            doc.Load(ResourceLoader.LoadClassStream(file, GetType()));
            return doc;
        }

        private static void Verify(SuiteRun suiteRun, string suiteRef, int passed, int failed, int notRun, string description, DateTime testtime) {
            Assert.AreEqual(testtime, suiteRun.Stamp);
            Assert.AreEqual(suiteRef, suiteRun.Name);
            Assert.AreEqual(description, suiteRun.Description);
            Assert.AreEqual(suiteRef, suiteRun.SuiteRef);
            Assert.AreEqual(passed, suiteRun.Passed);
            Assert.AreEqual(failed, suiteRun.Failed);
            Assert.AreEqual(notRun, suiteRun.NotRun);
        }

        private static XmlDocument GetPassingTest() {
            string reportXml =
                @"
<Report ver=""2.0"" tmZone=""Eastern Standard Time"">
	<Doc rID=""T1"" productName=""QuickTest Professional"">
		<NodeArgs eType=""StartTest"" icon=""1"" nRep=""4"">
		</NodeArgs>
	</Doc>
</Report>";
            var doc = new XmlDocument();
            doc.LoadXml(reportXml);
            return doc;
        }

        [Test]
        public void GetSuiteRuns() {
            // This tests expectations assume a single test suite, whos Reference value is specified in config
            var reports = new List<XmlDocument>();
            reports.Add(GetTestReportXml("Passed1.xml"));
            reports.Add(GetTestReportXml("Failed1.xml"));
            reports.Add(GetTestReportXml("Passed2.xml"));

            DateTime testTime = DateTime.Now;
            string expectedSuiteRef = "SuiteRef";
            var testSubject = new QTPReportReader(expectedSuiteRef);

            SuiteRun actualResult = testSubject.GetSuiteRun(reports, testTime);

            string expectedDescription = string.Format("{0} on {1}", expectedSuiteRef, testTime);

            Verify(actualResult, expectedSuiteRef, 2, 1, 0, expectedDescription, testTime);
        }

        [Test]
        public void GetTestRuns() {
            var reports = new List<XmlDocument>();
            reports.Add(GetTestReportXml("Passed1.xml"));
            reports.Add(GetTestReportXml("Failed1.xml"));
            reports.Add(GetTestReportXml("Passed2.xml"));

            string expectedSuiteRef = "SuiteRef";
            var testSubject = new QTPReportReader(expectedSuiteRef);

            IList<TestRun> results = testSubject.GetTestRuns(reports);
            Assert.AreEqual(3, results.Count, "Number of Results does not match");
            Verify(results[0], 11000, "Passed1", new DateTime(2007, 10, 26, 9, 51, 46), TestRun.TestRunState.Passed);
            Verify(results[1], 13000, "Failed1", new DateTime(2007, 10, 26, 10, 20, 57), TestRun.TestRunState.Failed);
            Verify(results[2], 11000, "Passed2", new DateTime(2007, 10, 26, 9, 51, 46), TestRun.TestRunState.Passed);
        }
    }
}