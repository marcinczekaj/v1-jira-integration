using Microsoft.Practices.EnterpriseLibrary.Validation;
using NUnit.Framework;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Entities {
    [TestFixture]
    public class LkkServiceEntityValidationTester {
        [Test]
        public void ValidEntity() {
            var entity = EntityFactory.CreateLkkServiceEntity();
            var results = Validation.Validate(entity);
            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void InvalidAccount() {
            var entity = EntityFactory.CreateLkkServiceEntity();
            entity.Account = string.Empty;

            var results = Validation.Validate(entity);
            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void InvalidUsername() {
            var entity = EntityFactory.CreateLkkServiceEntity();
            entity.Username = string.Empty;

            var results = Validation.Validate(entity);
            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void InvalidPassword() {
            var entity = EntityFactory.CreateLkkServiceEntity();
            entity.Password = string.Empty;

            var results = Validation.Validate(entity);
            Assert.IsFalse(results.IsValid);
        }
    }
}