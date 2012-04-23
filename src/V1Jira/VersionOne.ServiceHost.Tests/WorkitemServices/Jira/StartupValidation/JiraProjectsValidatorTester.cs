using System.Collections.Generic;
using NUnit.Framework;
using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.JiraServices.StartupValidation;
using VersionOne.ServiceHost.Core.Configuration;
using Rhino.Mocks;
namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira.StartupValidation {
    [TestFixture]
    public class JiraProjectsValidatorTester : BaseJiraTester {
        [Test]
        public void ProjectsExist() {
            var projects = new List<MappingInfo> {
                new MappingInfo("1", "Name 1"),
                new MappingInfo("2", "Name 2"),
            };
            var existProjects = new List<Item> {
                new Item("1", "Name 1"),
                new Item("2", "Name 2"),
            };
            var validator = new JiraProjectsValidator(Url, Username, Password, projects);

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetProjects(Token)).Return(existProjects);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result, "Incorrect processing projects.");
        }

        [Test]
        public void ProjectsDoesntExist() {
            var projects = new List<MappingInfo> {
                new MappingInfo("1", "Name 1"),
                new MappingInfo("2", "Name 2"),
            };
            var existProjects = new List<Item> {
                new Item("2", "Name 2"),
                new Item("3", "Name 3"),
            };
            var validator = new JiraProjectsValidator(Url, Username, Password, projects);

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetProjects(Token)).Return(existProjects);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result, "Incorrect processing projects.");
        }
    }
}