using NUnit.Framework;
using VersionOne.ServiceHost.TestServices.Fitnesse;

namespace VersionOne.ServiceHost.Tests.TestServices.Fit {
    [TestFixture]
    public class VersionDetectorTester : XmlFileParserTester {
        private const string CurrentFileName = "CurrentVersionResults.xml";
        private const string Pre2011FileName = "Pre2011VersionResults.xml";

        [Test]
        public void GetPre2011Version() {
            var file = GetTestFitXml(Pre2011FileName);
            var detector = new VersionDetector(file);
            var result = detector.GetVersion();
            Assert.AreEqual(FitnesseVersion.Pre2011, result);
            Assert.AreNotEqual(FitnesseVersion.Current, result);
        }

        [Test]
        public void GetCurrentVersion() {
            var file = GetTestFitXml(CurrentFileName);
            var detector = new VersionDetector(file);
            var result = detector.GetVersion();
            Assert.AreEqual(FitnesseVersion.Current, result);
            Assert.AreNotEqual(FitnesseVersion.Pre2011, result);
        }
    }
}