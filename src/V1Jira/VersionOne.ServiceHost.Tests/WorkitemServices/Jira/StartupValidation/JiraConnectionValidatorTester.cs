using System;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.JiraServices.StartupValidation;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira.StartupValidation {
    [TestFixture]
    public class JiraConnectionValidatorTester : BaseJiraTester {
        [Test]
        public void ValidConnection() {
            var validator = new JiraConnectionValidator(Url, Username, Password);

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();
            Assert.IsTrue(result, "Connection is not valid.");
        }

        [Test]
        public void InvalidConnection() {
            var validator = new JiraConnectionValidator(Url, Username, Password);

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Throw(new Exception());
            SoapService.Dispose();

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();
            Assert.IsFalse(result, "Connection is not valid.");
        }

        [Test]
        public void EmptyToken() {
            var validator = new JiraConnectionValidator(Url, Username, Password);

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(null);
            SoapService.Dispose();

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();
            Assert.IsFalse(result, "Connection is not valid.");
        }
    }
}