using System;
using NUnit.Framework;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.TestServices.Fitnesse;

namespace VersionOne.ServiceHost.Tests.TestServices.Fit {
    [TestFixture]
    public class CurrentXmlFileParserTester : XmlFileParserTester {
        private const string ResultsFileName = "CurrentVersionResults.xml";

        private static void Verify(TestRun testRun, TestRun.TestRunState state, string testName, DateTime testTime, Double elapsed) {
            Assert.IsNotNull(testRun);
            Assert.AreEqual(elapsed, testRun.Elapsed);
            Verify(testRun, state, testName, testTime);
        }

        [Test]
        public void GetSuiteRuns() {
            var testTime = DateTime.Now;
            XmlFileParser fileParser = new CurrentXmlFileParser(GetTestFitXml(ResultsFileName), testTime);
            TestGetSuiteRuns(fileParser, testTime);
        }
        
        [Test]
        public void GetTestRuns() {
            var testTime = DateTime.Now;
            XmlFileParser fileParser = new CurrentXmlFileParser(GetTestFitXml(ResultsFileName), testTime);
            var testRuns = fileParser.GetTestRuns();
            Assert.AreEqual(7, testRuns.Count);

            Verify(testRuns[0], TestRun.TestRunState.Failed, "SuiteVersionOneApiClient.SuiteAT.TestBlankAndNullKeywords", testTime, 12);
            Verify(testRuns[1], TestRun.TestRunState.Passed, "SuiteVersionOneApiClient.SuiteAT.TestBooleanSymbols", testTime, 34);
            Verify(testRuns[2], TestRun.TestRunState.NotRun, "SuiteVersionOneApiClient.SuiteAT.SubSuite.TestErrorKeyword", testTime, 56);
            Verify(testRuns[3], TestRun.TestRunState.Failed, "SuiteVersionOneApiClient.SuiteAT.SubSuite.TestExceptionKeywordHandler", testTime, 78);
            Verify(testRuns[4], TestRun.TestRunState.Passed, "SuiteVersionOneApiClient.SuiteAT.SubSuite.SubSubSuite.TestExceptionKeywordHandler", testTime, 90);
            Verify(testRuns[5], TestRun.TestRunState.NotRun, "SuiteVersionOneApiClient.SuiteAT.SubSuite.SubSubSuite.TestErrorKeyword", testTime, 13);
            Verify(testRuns[6], TestRun.TestRunState.NotRun, "SuiteVersionOneApiClient.AnotherSuite.TestExceptionKeywordHandler", testTime, 24);
        }
    }
}