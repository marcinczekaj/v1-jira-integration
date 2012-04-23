using System;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.JiraServices.StartupValidation;
namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira.StartupValidation {
    [TestFixture]
    public class JiraFilterValidationTester : BaseJiraTester {
        [Test]
        public void FilterExists() {
            const string filterId = "1";
            var filter = new JiraFilter(filterId, true);
            var validator = new JiraFilterValidation(Url, Username, Password, filter);

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetIssuesFromFilter(Token, filterId)).Return(null);            
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result, "Incorrect filter processing");
        }

        [Test]
        public void FilterDoesNotExist() {
            const string filterId = "1";
            var filter = new JiraFilter(filterId, true);
            var validator = new JiraFilterValidation(Url, Username, Password, filter);

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetIssuesFromFilter(Token, filterId)).Throw(new Exception());
            SoapService.Dispose();

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result, "Incorrect filter processing");
        }

        [Test]
        public void FilterDisabled() {
            const string filterId = "1";
            var filter = new JiraFilter(filterId, false);
            var validator = new JiraFilterValidation(Url, Username, Password, filter);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result, "Incorrect filter processing");
        }
    }
}