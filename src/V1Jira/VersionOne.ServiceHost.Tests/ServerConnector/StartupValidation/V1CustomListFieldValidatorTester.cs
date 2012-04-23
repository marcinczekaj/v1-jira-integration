using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServerConnector;
using VersionOne.ServerConnector.Entities;
using VersionOne.ServerConnector.StartupValidation;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServiceHost.Tests.ServerConnector.StartupValidation {
    [TestFixture]
    public class V1CustomListFieldValidatorTester : BaseValidationTester {
        private ISimpleValidator validator;

        private const string CustomFieldName = "Custom_BaFStatus1";

        private const string ReadyStatusToken = "Custom_BaF_status:1242";
        private const string PortedStatusToken = "Custom_BaF_status:1243";

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            validator = new V1CustomListFieldValidator(CustomFieldName, VersionOneProcessor.FeatureGroupType);
        }

        [Test]
        public void AttributeDoesNotExist() {
            Expect.Call(V1ProcessorMock.AttributeExists(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(false);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result);
        }

        [Test]
        public void FailureToEnlistAvailableValues() {
            validator = new V1CustomListFieldValidator(CustomFieldName, VersionOneProcessor.FeatureGroupType, ReadyStatusToken, PortedStatusToken);

            Expect.Call(V1ProcessorMock.AttributeExists(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(true);
            Expect.Call(V1ProcessorMock.GetAvailableListValues(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Throw(new VersionOneException(null));

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result);
        }

        [Test]
        public void CustomListValuesMissing() {
            validator = new V1CustomListFieldValidator(CustomFieldName, VersionOneProcessor.FeatureGroupType, ReadyStatusToken, PortedStatusToken);
            var propertyValues = Repository.PartialMock<PropertyValues>();

            Expect.Call(V1ProcessorMock.AttributeExists(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(true);
            Expect.Call(V1ProcessorMock.GetAvailableListValues(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(propertyValues);
            Expect.Call(propertyValues.Find(ReadyStatusToken)).Return(new ValueId());
            Expect.Call(propertyValues.Find(PortedStatusToken)).Return(null);

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsFalse(result);
        }

        [Test]
        public void ValidationSuccessfulWithoutListValues() {
            Expect.Call(V1ProcessorMock.AttributeExists(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(true);
            Expect.Call(V1ProcessorMock.GetAvailableListValues(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(new PropertyValues());

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result);
        }

        [Test]
        public void ValidationSuccessfulWithListValues() {
            validator = new V1CustomListFieldValidator(CustomFieldName, VersionOneProcessor.FeatureGroupType, ReadyStatusToken, PortedStatusToken);
            var propertyValues = Repository.PartialMock<PropertyValues>();

            Expect.Call(V1ProcessorMock.AttributeExists(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(true);
            Expect.Call(V1ProcessorMock.GetAvailableListValues(VersionOneProcessor.FeatureGroupType, CustomFieldName)).Return(propertyValues);
            Expect.Call(propertyValues.Find(ReadyStatusToken)).Return(new ValueId());
            Expect.Call(propertyValues.Find(PortedStatusToken)).Return(new ValueId());

            Repository.ReplayAll();
            var result = validator.Validate();
            Repository.VerifyAll();

            Assert.IsTrue(result);
        }
    }
}