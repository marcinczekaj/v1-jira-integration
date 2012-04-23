using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using VersionOne.ServerConnector.StartupValidation;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServiceHost.Tests.ServerConnector.StartupValidation {
    [TestFixture]
    public class V1ProjectValidatorTester : BaseValidationTester {
        private ISimpleValidator validator;
        private const string ProjectToken = "Scope:-1";

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            validator = new V1ProjectValidator(ProjectToken);
        }

        [Test]
        public void ProjectExists() {
            Expect.Call(V1ProcessorMock.ProjectExists(string.Empty)).IgnoreArguments().Constraints(Is.Equal(ProjectToken)).Return(true);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result);
        }

        [Test]
        public void ProjectDoesNotExist() {
            Expect.Call(V1ProcessorMock.ProjectExists(string.Empty)).IgnoreArguments().Constraints(Is.Equal(ProjectToken)).Return(false);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result);
        }
    }
}