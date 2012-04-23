using System;
using NUnit.Framework;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.TestServices.Fitnesse;

namespace VersionOne.ServiceHost.Tests.TestServices.Fit {
    [TestFixture]
    public class Pre2011XmlFileParserTester : XmlFileParserTester {
        private const string ResultsFileName = "Pre2011VersionResults.xml";

        [Test]
        public void GetSuiteRuns() {
            var testTime = DateTime.Now;
            XmlFileParser fileParser = new Pre2011XmlFileParser(GetTestFitXml(ResultsFileName), testTime);
            TestGetSuiteRuns(fileParser, testTime);
        }

        [Test]
        public void GetTestRuns() {
            var testTime = DateTime.Now;
            XmlFileParser fileParser = new Pre2011XmlFileParser(GetTestFitXml(ResultsFileName), testTime);
            var testRuns = fileParser.GetTestRuns();
            Assert.AreEqual(7, testRuns.Count);

            Verify(testRuns[0], TestRun.TestRunState.Failed, "SuiteVersionOneApiClient.SuiteAT.TestBlankAndNullKeywords", testTime);
            Verify(testRuns[1], TestRun.TestRunState.Passed, "SuiteVersionOneApiClient.SuiteAT.TestBooleanSymbols", testTime);
            Verify(testRuns[2], TestRun.TestRunState.NotRun, "SuiteVersionOneApiClient.SuiteAT.SubSuite.TestErrorKeyword", testTime);
            Verify(testRuns[3], TestRun.TestRunState.Failed, "SuiteVersionOneApiClient.SuiteAT.SubSuite.TestExceptionKeywordHandler", testTime);
            Verify(testRuns[4], TestRun.TestRunState.Passed, "SuiteVersionOneApiClient.SuiteAT.SubSuite.SubSubSuite.TestExceptionKeywordHandler", testTime);
            Verify(testRuns[5], TestRun.TestRunState.NotRun, "SuiteVersionOneApiClient.SuiteAT.SubSuite.SubSubSuite.TestErrorKeyword", testTime);
            Verify(testRuns[6], TestRun.TestRunState.NotRun, "SuiteVersionOneApiClient.AnotherSuite.TestExceptionKeywordHandler", testTime);
        }
    }
}