using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.BZ {
    [TestFixture]
    public class DependencyValidatorTester : BaseTester {
        private DependencyValidator validator;
        private ServiceHostConfiguration settings;
        
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();
            validator = new DependencyValidator(FacadeMock);
            settings = new ServiceHostConfiguration();
        }

        [Test]
        [ExpectedException(typeof(V1ConnectionRequiredException))]
        public void VersionOneDependencyValidationFailureTest() 
        {
            Expect.Call(FacadeMock.IsConnected).Return(false);
            
            MockRepository.ReplayAll();

            BugzillaServiceEntity entity = new BugzillaServiceEntity();
            validator.CheckVersionOneDependency(entity);

            MockRepository.VerifyAll();
        }

        [Test]
        public void VersionOneDependencyValidationSuccessTest() 
        {
            Expect.Call(FacadeMock.IsConnected).Return(true);
            
            MockRepository.ReplayAll();

            BugzillaServiceEntity entity = new BugzillaServiceEntity();
            validator.CheckVersionOneDependency(entity);
            
            MockRepository.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(DependencyFailureException))]
        public void ServiceDependencyValidationFailureTest() 
        {
            P4ServiceEntity entity = new P4ServiceEntity();
            IEnumerable<BaseServiceEntity> entities = new BaseServiceEntity[] {
                entity,                                                                                            
            };

            settings = new ServiceHostConfiguration(entities);
            validator.CheckOtherServiceDependency(entity, settings);
        }

        [Test]
        public void ServiceDependencyValidationSuccessTest() 
        {
            P4ServiceEntity entity = new P4ServiceEntity();
            ChangesetWriterEntity writerEntity = new ChangesetWriterEntity();
            IList<BaseServiceEntity> entities = new BaseServiceEntity[] {
                entity, writerEntity,                                                                                            
            };

            settings = new ServiceHostConfiguration(entities);
            validator.CheckOtherServiceDependency(entity, settings);
        }

        [Test]
        [ExpectedException(typeof(DependencyFailureException))]
        public void ServiceDependenciesValidationFailureTest() 
        {
            P4ServiceEntity p4Entity = new P4ServiceEntity();
            ChangesetWriterEntity writerEntity = new ChangesetWriterEntity();
            FitnesseServiceEntity fitEntity = new FitnesseServiceEntity();
            IList<BaseServiceEntity> entities = new BaseServiceEntity[] {
                p4Entity, writerEntity, fitEntity,                                                                                           
            };

            settings = new ServiceHostConfiguration(entities);
            validator.CheckServiceDependencies(settings);
        }
    }
}