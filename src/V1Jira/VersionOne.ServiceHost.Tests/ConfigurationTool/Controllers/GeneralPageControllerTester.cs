using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers 
{
    [TestFixture]
    public class GeneralPageControllerTester : BaseTester 
    {
        private const string url = "http://localhost/VersionOne";
        private const string username = "admin";
        private const string password = "password";
        private const bool integrated = false;
        private const string proxyPath = "http://proxy:3128";
        private const string proxyUsername = "user";
        private const string proxyPassword = "pass";
        private const string proxyDomain = "";
        
        private GeneralController CreateController() 
        {
            ServiceHostConfiguration model = new ServiceHostConfiguration();
            return new GeneralController(model, FacadeMock);
        }
        
        [Test]
        public void PrepareViewTest() 
        {
            IGeneralPageView viewMock = MockRepository.StrictMock<IGeneralPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();

            MockRepository.ReplayAll();

            GeneralController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventTest() 
        {
            const bool validationResult = true;

            IGeneralPageView viewMock = MockRepository.StrictMock<IGeneralPageView>();
            IFormController formControllerMock = MockRepository.StrictMock<IFormController>();
            VersionOneSettings settings = GetVersionOneSettings();

            formControllerMock.BeforeSave += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser eventRaiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.IsVersionOneConnectionValid(settings)).Return(validationResult);
            viewMock.SetValidationResult(validationResult);
            viewMock.SetProxyUrlValidationFault(validationResult);
            formControllerMock.SetCoreServiceNodesEnabled(validationResult);

            MockRepository.ReplayAll();

            GeneralController controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            eventRaiser.Raise(viewMock, new ConnectionValidationEventArgs(settings));

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventWithIncorrectProxyHostTest() {
            const bool validationResult = false;

            IGeneralPageView viewMock = MockRepository.StrictMock<IGeneralPageView>();
            IFormController formControllerMock = MockRepository.StrictMock<IFormController>();
            VersionOneSettings settings = GetVersionOneSettings();
            settings.ProxySettings.Uri = "incorrect_uri";

            formControllerMock.BeforeSave += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser eventRaiser = LastCall.GetEventRaiser();
            FacadeMock.ResetConnection();
            
            viewMock.SetProxyUrlValidationFault(validationResult);
            formControllerMock.SetCoreServiceNodesEnabled(validationResult);

            MockRepository.ReplayAll();

            GeneralController controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            eventRaiser.Raise(viewMock, new ConnectionValidationEventArgs(settings));

            MockRepository.VerifyAll();
        }

        private VersionOneSettings GetVersionOneSettings() {
            VersionOneSettings settings = new VersionOneSettings();
            settings.ApplicationUrl = url;
            settings.IntegratedAuth = integrated;
            settings.Password = password;
            settings.Username = username;
            settings.ProxySettings = new ProxyConnectionSettings();
            settings.ProxySettings.Uri = proxyPath;
            settings.ProxySettings.UserName = proxyUsername;
            settings.ProxySettings.Password = proxyPassword;
            settings.ProxySettings.Domain = proxyDomain;
            settings.ProxySettings.Enabled = true;
            return settings;
        }
    }
}
