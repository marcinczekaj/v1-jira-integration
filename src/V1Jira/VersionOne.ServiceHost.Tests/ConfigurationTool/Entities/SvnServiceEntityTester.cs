using Microsoft.Practices.EnterpriseLibrary.Validation;
using NUnit.Framework;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Entities {
    [TestFixture]
    public class SvnServiceEntityValidationTester {
        [Test]
        public void ValidEntity() {
            var entity = EntityFactory.CreateSvnServiceEntity();
            var results = Validation.Validate(entity);
            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void InvalidRepositoryPath() {
            var entity = EntityFactory.CreateSvnServiceEntity();
            entity.Path = string.Empty;

            var results = Validation.Validate(entity);
            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void InvalidReferenceExpression() {
            var entity = EntityFactory.CreateSvnServiceEntity();
            entity.ReferenceExpression = string.Empty;

            var results = Validation.Validate(entity);
            Assert.IsFalse(results.IsValid);
        }
    }
}