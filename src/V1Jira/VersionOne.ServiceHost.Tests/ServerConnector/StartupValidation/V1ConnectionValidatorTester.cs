using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServerConnector.StartupValidation;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServiceHost.Tests.ServerConnector.StartupValidation {
    [TestFixture]
    public class V1ConnectionValidatorTester : BaseValidationTester {
        private ISimpleValidator validator;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            validator = new V1ConnectionValidator();
        }

        [Test]
        public void ConnectionIsValid() {
            Expect.Call(V1ProcessorMock.ValidateConnection()).Return(true);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result);
        }

        [Test]
        public void ConnectionIsNotValid() {
            Expect.Call(V1ProcessorMock.ValidateConnection()).Return(false);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result);
        }
    }
}