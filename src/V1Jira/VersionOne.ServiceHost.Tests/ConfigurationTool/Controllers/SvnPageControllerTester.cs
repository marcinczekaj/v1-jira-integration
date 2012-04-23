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
    public class SvnPageControllerTester : BaseTester 
    {
        private const string path = "svn://svnhost";
        private const string username = "admin";
        private const string password = "password";
        
        private SvnController CreateController() 
        {
            return new SvnController(CreateEntity(), FacadeMock);
        }

        protected SvnServiceEntity CreateEntity() 
        {
            SvnServiceEntity entity = new SvnServiceEntity();
            entity.Password = password;
            entity.UserName = username;
            entity.Path = path;
            entity.ReferenceExpression = "^[A-Z]{1,2}-\\d+$";
            entity.Timer = new TimerEntity();
            entity.Timer.PublishClass = "VersionOne.ServiceHost.SourceServices.Subversion.SvnReaderHostedService+SvnReaderIntervalSync, VersionOne.ServiceHost.SourceServices";

            return entity;
        }

        
        [Test]
        public void PrepareViewTest() 
        {
            ISvnPageView viewMock = MockRepository.StrictMock<ISvnPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();

            MockRepository.ReplayAll();

            SvnController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventTest() 
        {
            const bool validationResult = true;

            SvnController controller = CreateController();
            ISvnPageView viewMock = MockRepository.StrictMock<ISvnPageView>();

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
        public void RaiseAssemblyLoadExceptionTest() 
        {
            ISvnPageView viewMock = MockRepository.StrictMock<ISvnPageView>();
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

            SvnController controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }
    }
}