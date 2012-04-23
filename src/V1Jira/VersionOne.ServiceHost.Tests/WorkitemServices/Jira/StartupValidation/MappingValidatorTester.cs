using System.Collections.Generic;
using NUnit.Framework;
using VersionOne.ServiceHost.JiraServices.StartupValidation;
using VersionOne.ServiceHost.Core.Configuration;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira.StartupValidation {
    [TestFixture]
    public class MappingValidatorTester : BaseJiraTester {
        [Test]
        public void Validate() {
            var mapping = new Dictionary<MappingInfo, MappingInfo> {
                {new MappingInfo("Scope:0", "Name"), new MappingInfo("0", "Name")} ,
                {new MappingInfo("Scope:1", "Name 1"), new MappingInfo("1", "Name 1")} ,
            };
            var validator = new MappingValidator(mapping, "Tester");

            Assert.IsTrue(validator.Validate(), "Incorrect validator processing.");
        }

        [Test]
        public void ValidateWithEmptyName() {
            var mapping = new Dictionary<MappingInfo, MappingInfo> {
                {new MappingInfo("Scope:0", ""), new MappingInfo("0", "")} ,
                {new MappingInfo("Scope:1", ""), new MappingInfo("1", "")} ,
            };
            var validator = new MappingValidator(mapping, "Tester");

            Assert.IsTrue(validator.Validate(), "Incorrect validator processing.");
        }

        [Test]
        public void ValidateWithEmptyId() {
            var mapping = new Dictionary<MappingInfo, MappingInfo> {
                {new MappingInfo("", "Name"), new MappingInfo("", "Name")} ,
                {new MappingInfo("", "Name 1"), new MappingInfo("", "Name 1")} ,
            };
            var validator = new MappingValidator(mapping, "Tester");

            Assert.IsTrue(validator.Validate(), "Incorrect validator processing.");
        }

        [Test]
        public void ValidateWithEmptyIdAndName1() {
            var mapping = new Dictionary<MappingInfo, MappingInfo> {
                {new MappingInfo("", ""), new MappingInfo("", "Name")} ,
            };
            var validator = new MappingValidator(mapping, "Tester");

            Assert.IsFalse(validator.Validate(), "Incorrect validator processing.");
        }

        [Test]
        public void ValidateWithEmptyIdAndName2() {
            var mapping = new Dictionary<MappingInfo, MappingInfo> {
                {new MappingInfo("Scope:0", "Name"), new MappingInfo("", "")} ,
                {new MappingInfo("Scope:1", "Name 1"), new MappingInfo("", "")} ,
            };
            var validator = new MappingValidator(mapping, "Tester");

            Assert.IsFalse(validator.Validate(), "Incorrect validator processing.");
        }
    }
}