using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using VersionOne.ServiceHost.TestServices.Mark;

namespace VersionOne.ServiceHost.TestServices.Tests.Mark
{
	[TestFixture]
	public class MarkXmlReaderTester
	{
		private MarkXmlReader _reader;
		private readonly DateTime DateStamp = new DateTime(2007, 2, 14, 13, 21, 40);

		[SetUp]
		public void SetUp()
		{
			_reader = new MarkXmlReader(GetTestMarkXml());
		}

		[Test]
		public void TestGetTestRuns()
		{
			IList<TestRun> testRuns = _reader.GetTestRuns();
			Assert.AreEqual(4, testRuns.Count);
			Verify(testRuns[0], TestRun.TestRunState.Passed, "S1.S1-1.AT1", DateStamp);
			Verify(testRuns[1], TestRun.TestRunState.Passed, "S1.S1-1.AT2",DateStamp);
			Verify(testRuns[2], TestRun.TestRunState.Passed, "S1.AT3",DateStamp);
			Verify(testRuns[3], TestRun.TestRunState.Failed, "S1.AT4",DateStamp);
		}

		[Test]
		public void TestGetSuiteRuns()
		{
			IList<SuiteRun> suiteRuns = _reader.GetSuiteRuns();
			Assert.AreEqual(2, suiteRuns.Count);
			Verify(suiteRuns[0], "Test Results", "Mark Test Results", "S1-1", 2, 0, 0, DateStamp);
			Verify(suiteRuns[1], "Test Results", "Mark Test Results", "S1", 3, 1, 0, DateStamp);			
		}

		private void Verify(SuiteRun suiteRun, string suiteName, string suiteDesc, string suiteRef, int passed, int failed, int notRun, DateTime stamp)
		{
			Assert.AreEqual(stamp, suiteRun.Stamp);
			Assert.AreEqual(suiteName, suiteRun.Name);
			Assert.AreEqual(suiteDesc, suiteRun.Description);
			Assert.AreEqual(suiteRef, suiteRun.SuiteRef);
			Assert.AreEqual(passed, suiteRun.Passed);
			Assert.AreEqual(failed, suiteRun.Failed);
			Assert.AreEqual(notRun, suiteRun.NotRun);
		}

		private void Verify(TestRun testRun, TestRun.TestRunState state, string testName, DateTime stamp)
		{
			Assert.AreEqual(stamp, testRun.Stamp);
			Assert.AreEqual(state, testRun.State);
			Assert.AreEqual(testRun.TestRef, testName);
		}

		private static XmlDocument GetTestMarkXml()
		{
			string fitXml = @"<Run Date='2/14/2007 1:21:40 PM' Title='Test Results' Description='Mark Test Results'>
								  <Suite RefId='S1'>
									<Suite RefId='S1-1'>
										<AcceptanceTest RefId='S1.S1-1.AT1' Elapsed='5000' Passed='True' />
										<AcceptanceTest RefId='S1.S1-1.AT2' Elapsed='4000' Passed='True' />
									</Suite>
									<AcceptanceTest RefId='S1.AT3' Elapsed='5000' Passed='True' />
									<AcceptanceTest RefId='S1.AT4' Elapsed='4000' Passed='False' />
								  </Suite>
								</Run>";
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(fitXml);
			return doc;
		}
	}
}