using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.Validation;
using VersionOne.ServiceHost.Tests.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.BZ 
{
    [TestFixture]
    public class FacadeTester 
    {
        private ServiceHostConfiguration configuration;

        [SetUp]
        public void SetUp() 
        {
            configuration = Facade.Instance.CreateConfiguration();
        }

        [Test]
        public void ValidateConfigurationFailureTest() 
        {
            QCServiceEntity entity = EntityFactory.CreateQCServiceEntity();
            entity.CreateStatus = string.Empty;
            configuration.AddService(entity);

            ConfigurationValidationResult validationResult = Facade.Instance.ValidateConfiguration(configuration);
            Assert.AreEqual(validationResult.InvalidEntitiesCount, 1);
        }

        [Test]
        public void ValidateConfigurationSuccessTest() 
        {
            QCServiceEntity qcEntity = EntityFactory.CreateQCServiceEntity();
            TestServiceEntity testServiceEntity = EntityFactory.CreateTestServiceEntity();
            configuration.AddService(qcEntity);
            configuration.AddService(testServiceEntity);

            ConfigurationValidationResult result = Facade.Instance.ValidateConfiguration(configuration);
            Assert.AreEqual(result.InvalidEntitiesCount, 0);
        }

        [Test]
        public void SaveConfigurationValidationFailureTest() 
        {
            QCServiceEntity entity = EntityFactory.CreateQCServiceEntity();
            entity.CreateStatus = string.Empty;
            configuration.AddService(entity);

            ConfigurationValidationResult result = Facade.Instance.SaveConfigurationToFile(configuration, "file.config");
            Assert.AreEqual(result.InvalidEntitiesCount, 1);
        }
    }
}