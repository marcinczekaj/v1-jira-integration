using System.Collections.Generic;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.UI {
    [TestFixture]
    public class UIFactoryTester {
        private static ServiceHostConfiguration CreateConfiguration() {
            var services = new BaseServiceEntity[] {
                new P4ServiceEntity(),
                new SvnServiceEntity(),
                new BugzillaServiceEntity(),
                new ChangesetWriterEntity(),
                new WorkitemWriterEntity(),                   
            };

            return new ServiceHostConfiguration(services);
        }

        [Test]
        public void GetCoreServiceNamesTest() {
            var settings = CreateConfiguration();
            var coreServiceNames = UIFactory.Instance.GetCoreServiceNames(settings);
            var serviceNamesList = new List<string>(coreServiceNames);

            Assert.AreEqual(serviceNamesList.Count, 2);
            ListAssert.Contains("Changesets", coreServiceNames);
            ListAssert.Contains("Workitems", coreServiceNames);
        }

        [Test]
        public void GetCustomServiceNamesTest() {
            var settings = CreateConfiguration();
            var customServiceNames = UIFactory.Instance.GetCustomServiceNames(settings);
            var serviceNamesList = new List<string>(customServiceNames);
            
            Assert.AreEqual(serviceNamesList.Count, 3);
            ListAssert.Contains("Perforce", customServiceNames);
            ListAssert.Contains("SVN", customServiceNames);
            ListAssert.Contains("Bugzilla", customServiceNames);
        }
    }
}