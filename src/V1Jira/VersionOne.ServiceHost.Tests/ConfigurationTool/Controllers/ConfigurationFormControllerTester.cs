using System;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;
using System.Collections.Generic;
using VersionOne.ServiceHost.ConfigurationTool.Validation;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers {
    [TestFixture]
    public class ConfigurationFormControllerTester : BaseTester {
        private const string Filename = "VersionOne.ServiceHost.exe.config";
        private const string Filename2 = "VersionOne.ServiceExecutor.exe.config";
        
        private readonly IList<string> ConfigurationFileNames = new List<string> { Filename, Filename2, };
        
        private const string GeneralPageKey = "general";

        private IUIFactory uiFactoryMock;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            uiFactoryMock = MockRepository.StrictMock<IUIFactory>();
        }

        [TearDown]
        public void TearDown() {
            MockRepository.BackToRecordAll();
        }

        private void ExpectRegisterAndPrepareView(IConfigurationView viewMock) {
            Expect.Call(FacadeMock.CreateConfiguration()).Return(new ServiceHostConfiguration());
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.GenerateSnapshotMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.NewFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.OpenFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.OptionsMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.SaveFileAsMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.SaveFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(FacadeMock.AnyFileExists(ConfigurationFileNames)).Return(false);
            Expect.Call(() => viewMock.SetServiceNodesAndRedraw(null, null));
            Expect.Call(() => viewMock.SetCoreServiceNodesEnabled(false));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterNullViewTest() {
            var facadeStub = MockRepository.Stub<IFacade>();
            var uiFactoryStub = MockRepository.Stub<IUIFactory>();
            var controller = new ConfigurationFormController(facadeStub, uiFactoryStub);
            
            controller.RegisterView(null);
        }

        [Test]
        public void RegisterAndPrepareViewTest() {
            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            ExpectRegisterAndPrepareView(viewMock);

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            Assert.AreEqual(controller.View, viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void SaveConfigurationToFileTest() {
            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            ExpectRegisterAndPrepareView(viewMock);
            Expect.Call(FacadeMock.SaveConfigurationToFile(null, Filename)).IgnoreArguments().Return(new ConfigurationValidationResult());

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.SaveToFile(Filename);

            MockRepository.VerifyAll();
        }

        [Test]
        public void SaveConfigurationToFileInvalidDataTest() {
            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            ExpectRegisterAndPrepareView(viewMock);
            var validationResult = new ConfigurationValidationResult();
            var invalidEntity = new TestServiceEntity();
            validationResult.AddEntity(invalidEntity, new List<string>());
            
            Expect.Call(FacadeMock.SaveConfigurationToFile(null, Filename)).IgnoreArguments().Return(validationResult);
            Expect.Call(uiFactoryMock.ResolvePageNameByEntity(invalidEntity)).Return("Tests");
            Expect.Call(() => viewMock.ShowErrorMessage(null)).IgnoreArguments();

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            controller.SaveToFile(Filename);

            MockRepository.VerifyAll();
        }

        [Test]
        public void SaveConfigurationToFileWithoutNameSelectionTest() {
            var config = new ServiceHostConfiguration();

            Expect.Call(FacadeMock.CreateConfiguration()).Return(config);
            Expect.Call(FacadeMock.SaveConfigurationToFile(config, Filename)).Return(new ConfigurationValidationResult());

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.SaveToFile(string.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void LoadFileInvalidFilenameTest() {
            const string wrongFilename = "VersionOne.ServiceHost.exe.config.wrong";
            var config = new ServiceHostConfiguration();
            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            Expect.Call(FacadeMock.CreateConfiguration()).Return(config);
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(FacadeMock.LoadConfigurationFromFile(wrongFilename)).Throw(new InvalidFilenameException(string.Empty));
            Expect.Call(() => viewMock.ShowErrorMessage(string.Empty)).IgnoreArguments();

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            controller.LoadFromFile(wrongFilename);

            MockRepository.VerifyAll();
        }

        [Test]
        public void LoadDefaultConfigurationOnStartupTest() {
            var config = new ServiceHostConfiguration();
            var coreServices = new[] { "Tests" };
            var customServices = new[] { "QualityCenter" };
            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            Expect.Call(FacadeMock.CreateConfiguration()).Return(config);
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.GenerateSnapshotMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.NewFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.OpenFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.OptionsMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.SaveFileAsMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.SaveFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(FacadeMock.AnyFileExists(ConfigurationFileNames)).Return(true);
            Expect.Call(FacadeMock.FileExists(Filename)).Return(true);
            Expect.Call(FacadeMock.LoadConfigurationFromFile(Filename)).Return(config);
            Expect.Call(() => FacadeMock.ResetConnection());
            Expect.Call(uiFactoryMock.GetNextPage(GeneralPageKey, config, null)).IgnoreArguments().Return(null);
            Expect.Call(viewMock.HeaderText).PropertyBehavior();
            Expect.Call(viewMock.CurrentControl).PropertyBehavior();
            Expect.Call(uiFactoryMock.GetCoreServiceNames(config)).Return(coreServices);
            Expect.Call(uiFactoryMock.GetCustomServiceNames(config)).Return(customServices);
            Expect.Call(() => viewMock.SetServiceNodesAndRedraw(coreServices, customServices));
            Expect.Call(() => viewMock.SetCoreServiceNodesEnabled(false));

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void LoadDefaultConfiguration2OnStartupTest() {
            var config = new ServiceHostConfiguration();
            var coreServices = new[] { "Tests" };
            var customServices = new[] { "QualityCenter" };
            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            Expect.Call(FacadeMock.CreateConfiguration()).Return(config);
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(viewMock.GenerateSnapshotMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.NewFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.OpenFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.OptionsMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.SaveFileAsMenuItemEnabled).PropertyBehavior();
            Expect.Call(viewMock.SaveFileMenuItemEnabled).PropertyBehavior();
            Expect.Call(FacadeMock.AnyFileExists(ConfigurationFileNames)).Return(true);
            Expect.Call(FacadeMock.FileExists(Filename)).Return(false);
            Expect.Call(FacadeMock.FileExists(Filename2)).Return(true);
            Expect.Call(FacadeMock.LoadConfigurationFromFile(Filename2)).Return(config);
            Expect.Call(() => FacadeMock.ResetConnection());
            Expect.Call(uiFactoryMock.GetNextPage(GeneralPageKey, config, null)).IgnoreArguments().Return(null);
            Expect.Call(viewMock.HeaderText).PropertyBehavior();
            Expect.Call(viewMock.CurrentControl).PropertyBehavior();
            Expect.Call(uiFactoryMock.GetCoreServiceNames(config)).Return(coreServices);
            Expect.Call(uiFactoryMock.GetCustomServiceNames(config)).Return(customServices);
            Expect.Call(() => viewMock.SetServiceNodesAndRedraw(coreServices, customServices));
            Expect.Call(() => viewMock.SetCoreServiceNodesEnabled(false));

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void LoadFileInvalidConfigurationDataTest() {
            var entity = new P4ServiceEntity();
            var config = new ServiceHostConfiguration(new BaseServiceEntity[] { entity });
            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            Expect.Call(FacadeMock.CreateConfiguration()).Return(config);
            Expect.Call(viewMock.Controller).PropertyBehavior();
            Expect.Call(FacadeMock.LoadConfigurationFromFile(Filename)).Return(config);
            Expect.Call(FacadeMock.CreateConfiguration()).Return(new ServiceHostConfiguration());
            Expect.Call(() => viewMock.SetServiceNodesAndRedraw(null, null));
            Expect.Call(() => viewMock.ShowErrorMessage(string.Empty)).IgnoreArguments();

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            controller.LoadFromFile(Filename);

            MockRepository.VerifyAll();
        }

        [Test]
        public void LoadValidConfigurationFileTest() {
            var config = new ServiceHostConfiguration();
            var coreServices = new[] { "Tests" };
            var customServices = new[] { "QualityCenter" };

            var viewMock = MockRepository.StrictMock<IConfigurationView>();

            Expect.Call(FacadeMock.CreateConfiguration()).Return(config);
            Expect.Call(viewMock.Controller).Repeat.Once().PropertyBehavior();
            Expect.Call(FacadeMock.LoadConfigurationFromFile(Filename)).Return(config);
            Expect.Call(() => FacadeMock.ResetConnection());
            Expect.Call(uiFactoryMock.GetNextPage(GeneralPageKey, config, null)).IgnoreArguments().Return(null);
            Expect.Call(viewMock.HeaderText).Repeat.Once().PropertyBehavior();
            Expect.Call(viewMock.CurrentControl).Repeat.Once().PropertyBehavior();
            Expect.Call(uiFactoryMock.GetCoreServiceNames(config)).Return(coreServices);
            Expect.Call(uiFactoryMock.GetCustomServiceNames(config)).Return(customServices);
            Expect.Call(() => viewMock.SetServiceNodesAndRedraw(coreServices, customServices));
            Expect.Call(() => viewMock.SetCoreServiceNodesEnabled(false));

            MockRepository.ReplayAll();

            var controller = new ConfigurationFormController(FacadeMock, uiFactoryMock);
            controller.RegisterView(viewMock);
            controller.LoadFromFile(Filename);

            Assert.AreEqual(Filename, controller.CurrentFileName);

            MockRepository.VerifyAll();
        }
    }
}