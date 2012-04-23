using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers 
{
    [TestFixture]
    public class P4PageControllerTester : BaseTester 
    {
        private const string p4port = "p4host:1666";
        private const string username = "admin";
        private const string password = "password";
        
        private P4Controller CreateController() 
        {
            P4ServiceEntity model = new P4ServiceEntity();
            model.Port = p4port;
            model.Username = username;
            model.Password = password;
            return new P4Controller(model, FacadeMock);
        }
        
        [Test]
        public void PrepareViewTest() 
        {
            IP4PageView viewMock = MockRepository.StrictMock<IP4PageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();

            MockRepository.ReplayAll();

            P4Controller controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventTest() 
        {
            const bool validationResult = true;

            P4Controller controller = CreateController();
            IP4PageView viewMock = MockRepository.StrictMock<IP4PageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser eventRaiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.ValidateConnection(controller.Model)).Return(validationResult);
            viewMock.SetValidationResult(validationResult);

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            eventRaiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RiseAssemblyLoadExceptionTest () {
            IP4PageView viewMock = MockRepository.StrictMock<IP4PageView>();
            IFormController formControllerMock = MockRepository.StrictMock<IFormController>();

            formControllerMock.BeforeSave += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Throw(new AssemblyLoadException("TestEx"));
            formControllerMock.FailApplication("TestEx");

            MockRepository.ReplayAll();

            P4Controller controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }
    }
}