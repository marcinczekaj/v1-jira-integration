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
    public class ClearQuestPageControllerTester : BaseTester 
    {
        private const string connectionName = "clearquest";
        private const string username = "admin";
        private const string password = "password";
        private const string database = "SAMPL";
        
        private ClearQuestController CreateController() 
        {
            ClearQuestServiceEntity model = new ClearQuestServiceEntity();
            model.UserName = username;
            model.Password = password;
            model.ConnectionName = connectionName;
            model.DataBase = database;
            return new ClearQuestController(model, FacadeMock);
        }
        
        [Test]
        public void PrepareViewTest() 
        {
            IClearQuestPageView viewMock = MockRepository.StrictMock<IClearQuestPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[] {});
            Expect.Call(viewMock.SourceList).PropertyBehavior();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior().IgnoreArguments();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior().IgnoreArguments();
            viewMock.DataBind();

            MockRepository.ReplayAll();

            ClearQuestController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventTest() 
        {
            const bool validationResult = true;

            ClearQuestController controller = CreateController();
            IClearQuestPageView viewMock = MockRepository.StrictMock<IClearQuestPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser eventRaiser = LastCall.GetEventRaiser();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[] { });
            Expect.Call(viewMock.SourceList).PropertyBehavior();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior().IgnoreArguments();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior().IgnoreArguments();
            viewMock.DataBind();
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
            IClearQuestPageView viewMock = MockRepository.StrictMock<IClearQuestPageView>();
            IFormController formControllerMock = MockRepository.StrictMock<IFormController>();

            formControllerMock.BeforeSave += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior().IgnoreArguments();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior().IgnoreArguments();
            viewMock.DataBind();
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Throw(new AssemblyLoadException("TestEx"));
            formControllerMock.FailApplication("TestEx");

            MockRepository.ReplayAll();

            ClearQuestController controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseBusinessExceptionTest() 
        {
            IClearQuestPageView viewMock = MockRepository.StrictMock<IClearQuestPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Throw(new BusinessException("TestEx"));
            viewMock.DisplayError("TestEx");

            MockRepository.ReplayAll();

            ClearQuestController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}