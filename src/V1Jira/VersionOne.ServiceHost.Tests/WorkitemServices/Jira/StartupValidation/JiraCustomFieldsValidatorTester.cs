using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.JiraServices.StartupValidation;
namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira.StartupValidation {
    [TestFixture]
    public class JiraCustomFieldsValidatorTester : BaseJiraTester{
        [Test]
        public void ValidateExistField() {
            var validator = new JiraCustomFieldsValidator(Url, Username, Password, "ID_001", "ID_002");
            var existedFields = new List<Item> { new Item("ID_001", "field1"), new Item("ID_002", "field2") };

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetCustomFields(Token)).Return(existedFields);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            Assert.IsTrue(validator.Validate());
            Repository.VerifyAll();
        }

        [Test]
        public void ValidateNonExistField() {
            var validator = new JiraCustomFieldsValidator(Url, Username, Password, "ID_001", "ID_002");
            var existedFields = new List<Item> { new Item("ID_001", "field1") };

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetCustomFields(Token)).Return(existedFields);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            Assert.IsFalse(validator.Validate());
            Repository.VerifyAll();            
        }

        [Test]
        public void ValidateNoCustomFields() {
            var validator = new JiraCustomFieldsValidator(Url, Username, Password, "ID_001", "ID_002");
            var existedFields = new List<Item>();

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetCustomFields(Token)).Return(existedFields);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            Assert.IsFalse(validator.Validate());
            Repository.VerifyAll();
        }

        [Test]
        public void ValidateDoublicateFieldNames() {
            var validator = Repository.PartialMock<JiraCustomFieldsValidator>(Url, Username, Password, new []{"ID_001", "ID_001"});
            var existedFields = new List<Item> { new Item("ID_001", "field1") };

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetCustomFields(Token)).Return(existedFields);
            Expect.Call(validator.ValidateField(null, null)).IgnoreArguments().Repeat.Once().Return(true);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            Assert.IsTrue(validator.Validate());
            Repository.VerifyAll();             
        }

        [Test]
        public void ValidateEmptyField() {
            var validator = new JiraCustomFieldsValidator(Url, Username, Password, "ID_001", "");
            var existedFields = new List<Item> { new Item("ID_001", "field1") };

            Expect.Call(ServiceFactory.CreateNew(Url)).Return(SoapService);
            Expect.Call(SoapService.Login(Username, Password)).Return(Token);
            Expect.Call(SoapService.GetCustomFields(Token)).Return(existedFields);
            Expect.Call(SoapService.Logout(Token)).Return(true);
            SoapService.Dispose();

            Repository.ReplayAll();
            Assert.IsTrue(validator.Validate());
            Repository.VerifyAll();
        }
    }
}