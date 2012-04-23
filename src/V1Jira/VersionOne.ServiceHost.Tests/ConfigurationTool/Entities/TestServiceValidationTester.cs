using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Entities {
    [TestFixture]
    public class TestServiceValidationTester {
        /// <summary>
        /// Create valid TestPublishProject
        /// </summary>
        /// <returns>Sample project entity</returns>
        private static TestPublishProjectMapping CreateProject() {
            var project = new TestPublishProjectMapping {
                Name = "CallCenter",
                DestinationProject = "Call Center",
                IncludeChildren = true
            };

            return project;
        }

        private static IList<ValidationResult> ConvertResultToList(ValidationResults results) {
            return new List<ValidationResult>(results);
        }

        [Test]
        public void InvalidEntityCheckTest() {
            var entity = EntityFactory.CreateTestServiceEntity();
            entity.ReferenceAttribute = null;
            entity.PassedOid = string.Empty;

            var results = Validation.Validate(entity);
            Assert.IsFalse(results.IsValid);
            Assert.AreEqual(ConvertResultToList(results).Count, 2);
        }

        [Test]
        public void ValidEntityCheckTest() {
            var entity = EntityFactory.CreateTestServiceEntity();
            var results = Validation.Validate(entity);
            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void TestServiceCheckTest() {
            var project = CreateProject();
            var results = Validation.Validate(project);
            Assert.IsTrue(results.IsValid);

            project.DestinationProject = null;
            results = Validation.Validate(project);
            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void CompositeValidationTest() {
            var project = CreateProject();
            var entity = EntityFactory.CreateTestServiceEntity();
            entity.Projects.Add(project);

            var results = Validation.Validate(entity);
            Assert.IsTrue(results.IsValid);

            project.Name = string.Empty;
            entity.FailedOid = null;
            entity.PassedOid = " ";
            results = Validation.Validate(entity);
            Assert.IsFalse(results.IsValid);
            Assert.AreEqual(ConvertResultToList(results).Count, 3);
        }
    }
}