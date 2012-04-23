using System;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServiceHost.Tests.ServerConnector.StartupValidation {
    [TestFixture]
    public class ValidationStepTester : BaseValidationTester {
        private ISimpleValidator validatorMock;
        private ISimpleResolver resolverMock;
        
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            validatorMock = Repository.StrictMock<ISimpleValidator>();
            resolverMock = Repository.StrictMock<ISimpleResolver>();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EmptyValidatorFailure() {
            Expect.Call(validatorMock.Validate()).Repeat.Never();
            Expect.Call(resolverMock.Resolve()).Repeat.Never();

            Repository.ReplayAll();
            var step = new ValidationSimpleStep(null, resolverMock);
            step.Run();
            Repository.VerifyAll();
        }
        
        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidationFailureEmptyResolver() {
            Expect.Call(validatorMock.Validate()).Return(false);
            Expect.Call(resolverMock.Resolve()).Repeat.Never();

            Repository.ReplayAll();
            var step = new ValidationSimpleStep(validatorMock, null);
            step.Run();
            Repository.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ResolveFailure() {
            Expect.Call(validatorMock.Validate()).Return(false);
            Expect.Call(resolverMock.Resolve()).Return(false);

            Repository.ReplayAll();
            var step = new ValidationSimpleStep(validatorMock, resolverMock);
            step.Run();
            Repository.VerifyAll();
        }

        [Test]
        public void SuccessfulValidate() {
            Expect.Call(validatorMock.Validate()).Return(true);
            Expect.Call(resolverMock.Resolve()).Repeat.Never();

            Repository.ReplayAll();
            var step = new ValidationSimpleStep(validatorMock, resolverMock);
            step.Run();
            Repository.VerifyAll();
        }

        [Test]
        public void SuccessfulResolve() {
            Expect.Call(validatorMock.Validate()).Return(false);
            Expect.Call(resolverMock.Resolve()).Return(true);

            Repository.ReplayAll();
            var step = new ValidationSimpleStep(validatorMock, resolverMock);
            step.Run();
            Repository.VerifyAll();
        }
    }
}