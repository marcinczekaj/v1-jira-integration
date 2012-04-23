using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Entities {
    [TestFixture]
    public class QualityCenterEntityValidationTester {
        [Test]
        public void InvalidEntityTest() {
            var entity = EntityFactory.CreateQCServiceEntity();
            entity.SourceField = string.Empty;

            Assert.IsFalse(IsEntityValid(entity));

            entity.SourceField = "   ";
            Assert.IsFalse(IsEntityValid(entity));
        }

        [Test]
        public void ValidEntityTest() {
            var entity = EntityFactory.CreateQCServiceEntity();
            Assert.IsTrue(IsEntityValid(entity));
        }

        [Test]
        public void InvalidConnectionTest() {
            var entity = new QCServiceEntity();
            EntityFactory.SetQCConnectionParameters(entity, null, string.Empty, "password");
            Assert.IsFalse(IsEntityValid(entity.Connection));
        }

        [Test]
        public void ValidConnectionTest() {
            var entity = new QCServiceEntity();
            EntityFactory.SetQCConnectionParameters(entity, "http://localhost:8080/qcbin", "alex_qc", string.Empty);
            Assert.IsTrue(IsEntityValid(entity.Connection));
        }

        [Test]
        public void QCProjectValidationTest() {
            var project = EntityFactory.CreateQCProject("ID1");

            project.Id = string.Empty;
            project.Domain = null;

            var results = Validation.Validate(project);
            var resultList = new List<ValidationResult>(results);
            Assert.IsFalse(results.IsValid);
            Assert.AreEqual(resultList.Count, 2);

            project.Domain = "DEFAULT";
            project.Id = "ID1";

            results = Validation.Validate(project);
            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void QCFilterValidationTest() {
            var filter = EntityFactory.CreateDefectFilter(string.Empty, "FieldValue");

            var results = Validation.Validate(filter);
            Assert.IsFalse(results.IsValid);

            filter.FieldName = "ValidNonEmptyFieldName";
            results = Validation.Validate(filter);
            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void CompositeValidationTest() {
            var entity = new QCServiceEntity();
            EntityFactory.SetQCConnectionParameters(entity, "http://localhost:8080/qcbin/", "alex_qc", "passw0rd_");
            var firstProject = EntityFactory.CreateQCProject("ID1");
            var secondProject = EntityFactory.CreateQCProject("ID1");
            var filter = EntityFactory.CreateDefectFilter(string.Empty, "test");
            entity.Projects.AddRange(new[] { firstProject, secondProject, });
            entity.DefectFilters.Add(filter);

            var results = Validation.Validate(entity);
            var resultList = new List<ValidationResult>(results);

            Assert.IsFalse(results.IsValid);
            Assert.AreEqual(resultList.Count, 5);
            var uniqueCheckResult = resultList.Find(result => result.Key.Equals("Unique"));
            Assert.IsNotNull(uniqueCheckResult);

            filter.FieldName = "ValidName";
            secondProject.Id = "ID2";
            entity.CreateStatus = "Created";
            entity.CloseStatus = "Closed";
            entity.SourceField = "Quality Center";

            results = Validation.Validate(filter);
            Assert.IsTrue(results.IsValid);
        }

        private static bool IsEntityValid<TEntity>(TEntity entity) where TEntity : class {
            var validator = ValidationFactory.CreateValidator<TEntity>();
            return validator.Validate(entity).IsValid;
        }
    }
}