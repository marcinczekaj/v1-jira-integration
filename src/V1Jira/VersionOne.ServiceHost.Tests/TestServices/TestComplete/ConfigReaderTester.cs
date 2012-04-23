using System;
using NUnit.Framework;
using VersionOne.ServiceHost.TestServices.TestComplete;

namespace VersionOne.ServiceHost.Tests.TestServices.TestComplete
{
    [TestFixture]
    public class ConfigReaderTester
    {
        [Test]
        public void TestProjectConfig()
        {
            ProjectConfig project = new ProjectConfig(".\\Project1.mds.tcLS");
            Assert.AreEqual(".\\Log", project.LogDir);
            Assert.AreEqual("Project1", project.Name);
        }

        [Test]
        public void TestSuiteConfig()
        {
            SuiteConfig suite = new SuiteConfig(".\\ProjectSuite1.pjs.tcLS");
            Assert.AreEqual(".\\Log", suite.LogDir);
            Assert.AreEqual("ProjectSuite1", suite.Name);
            
            Guid projectGuid = new Guid("{0761baf0-d3e3-4b83-8e0d-f29c7b82ef42}");
            ProjectConfig project = suite.GetProject(projectGuid);
            Assert.AreEqual(".\\Log", project.LogDir);
            Assert.AreEqual("Project1", project.Name);
        }
    }
}
