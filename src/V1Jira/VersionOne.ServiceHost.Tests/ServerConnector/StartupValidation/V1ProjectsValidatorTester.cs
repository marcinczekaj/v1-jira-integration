using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServerConnector.StartupValidation;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Tests.LeanKitKanban;

namespace VersionOne.ServiceHost.Tests.ServerConnector.StartupValidation {
    [TestFixture]
    public class VersionOneProjectValidatorTester : BaseLkkTester {
        private readonly IDictionary<MappingInfo, MappingInfo> mappings = new Dictionary<MappingInfo, MappingInfo>
                                                                          {
                                                                              {new MappingInfo("123", "Board 1"), new MappingInfo("S:123", "Project 1")},
                                                                              {new MappingInfo("456", "Board 2"), new MappingInfo("S:456", "Project 2")}
                                                                          };

        [Test]
        public void AllProjectsExistValidatorTest() {
            var validator = new V1ProjectsValidator(mappings.Values);

            Expect.Call(V1ProcessorMock.ProjectExists("S:123")).Return(true);
            Expect.Call(V1ProcessorMock.ProjectExists("S:456")).Return(true);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result, "Not all board exist.");
        }

        [Test]
        public void AllProjectsNotExistValidatorTest() {
            var validator = new V1ProjectsValidator(mappings.Values);

            Expect.Call(V1ProcessorMock.ProjectExists("S:123")).Return(false);
            Expect.Call(V1ProcessorMock.ProjectExists("S:456")).Return(false);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result, "All project exist.");
        }

        [Test]
        public void OneProjectsNotExistValidatorTest() {
            var validator = new V1ProjectsValidator(mappings.Values);

            Expect.Call(V1ProcessorMock.ProjectExists("S:123")).Return(true);
            Expect.Call(V1ProcessorMock.ProjectExists("S:456")).Return(false);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result, "Incorrect projects status.");
        }
    }
}