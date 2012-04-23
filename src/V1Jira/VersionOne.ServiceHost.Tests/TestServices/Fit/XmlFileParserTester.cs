using System;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.TestServices;
using VersionOne.ServiceHost.TestServices.Fitnesse;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.TestServices.Fit {
    public abstract class XmlFileParserTester {
        private static void Verify(SuiteRun suiteRun, string suiteName, int passed, int failed, int notRun, string description, DateTime testtime) {
            Assert.IsNotNull(suiteRun);
            Assert.AreEqual(testtime, suiteRun.Stamp);
            Assert.AreEqual(suiteName, suiteRun.Name);
            Assert.AreEqual(description, suiteRun.Description);
            Assert.AreEqual(suiteName, suiteRun.SuiteRef);
            Assert.AreEqual(passed, suiteRun.Passed);
            Assert.AreEqual(failed, suiteRun.Failed);
            Assert.AreEqual(notRun, suiteRun.NotRun);
        }

        protected static void Verify(TestRun testRun, TestRun.TestRunState state, string testName, DateTime testtime) {
            Assert.IsNotNull(testRun);
            Assert.AreEqual(testtime, testRun.Stamp);
            Assert.AreEqual(state, testRun.State);
            Assert.AreEqual(testName, testRun.TestRef);
        }

        protected XmlDocument GetTestFitXml(string fileName) {
            var doc = new XmlDocument();
            doc.Load(ResourceLoader.LoadClassStream(fileName, GetType()));
            return doc;
        }

        protected static void TestGetSuiteRuns(XmlFileParser fileParser, DateTime testTime) {
            Assert.IsNotNull(fileParser);
            var suiteRuns = fileParser.GetSuiteRuns();
            Assert.AreEqual(5, suiteRuns.Count);
            var numer = suiteRuns.GetEnumerator();

            while(numer.MoveNext()) {
                var suiteRun = numer.Current;
                Assert.IsNotNull(suiteRun);

                switch(suiteRun.Name) {
                    case "SuiteVersionOneApiClient":
                        Verify(suiteRun, "SuiteVersionOneApiClient", 2, 2, 3, "Fit Test Suite", testTime);
                        break;
                    case "SuiteVersionOneApiClient.SuiteAT":
                        Verify(suiteRun, "SuiteVersionOneApiClient.SuiteAT", 2, 2, 2, "Fit Test Suite", testTime);
                        break;
                    case "SuiteVersionOneApiClient.SuiteAT.SubSuite":
                        Verify(suiteRun, "SuiteVersionOneApiClient.SuiteAT.SubSuite", 1, 1, 2, "Fit Test Suite", testTime);
                        break;
                    case "SuiteVersionOneApiClient.SuiteAT.SubSuite.SubSubSuite":
                        Verify(suiteRun, "SuiteVersionOneApiClient.SuiteAT.SubSuite.SubSubSuite", 1, 0, 1, "Fit Test Suite", testTime);
                        break;
                    case "SuiteVersionOneApiClient.AnotherSuite":
                        Verify(suiteRun, "SuiteVersionOneApiClient.AnotherSuite", 0, 0, 1, "Fit Test Suite", testTime);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }
}