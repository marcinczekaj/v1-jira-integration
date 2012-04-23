using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.StartupValidation;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.Tests.LeanKitKanban;
using VersionOne.ServiceHost.Tests.ServerConnector.TestEntity;
using VersionOne.ServiceHost.Tests.Utility;

namespace VersionOne.ServiceHost.Tests.ServerConnector.StartupValidation {
    [TestFixture]
    public class VersionOnePriorityValidatorTester : BaseLkkTester {
        [Test]
        public void AllPrioritiesExistValidatorTester() {
            var priorities = new Dictionary<MappingInfo, MappingInfo> {
                                 { new MappingInfo("123", "Lkk priority 1"), new MappingInfo("P:123", "V1 prioprity 1") },
                                 { new MappingInfo("456", "Lkk priority 2"), new MappingInfo("P:456", "V1 prioprity 2") }
                             };
            var validator = new V1PrioritiesValidator(priorities.Values);

            var workitemPriorities = new List<ValueId> {
                                        TestValueId.Create("V1 priority 1", "P", 123),
                                        TestValueId.Create("V1 priority 2", "P", 456),
                                     };

            Expect.Call(V1ProcessorMock.GetWorkitemPriorities()).Return(workitemPriorities);
            
            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result, "Not all priorities exist");
        }
    }
}