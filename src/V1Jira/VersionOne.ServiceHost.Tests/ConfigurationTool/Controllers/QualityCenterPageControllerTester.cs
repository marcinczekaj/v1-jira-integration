using System;
using System.Collections.Generic;

using Microsoft.Practices.EnterpriseLibrary.Validation;

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
    public class QualityCenterPageControllerTester : BaseTester 
    {
        private const string url = "http://localhost:8080/qcbin";
        private const string username = "admin";
        private const string password = "password";
        
        private QCController CreateController() 
        {
            QCServiceEntity model = new QCServiceEntity();
            model.Connection.ApplicationUrl = url;
            model.Connection.Username = username;
            model.Connection.Password = password;
            return new QCController(model, FacadeMock);
        }
        
        [Test]
        public void PrepareViewTest() 
        {
            IQualityCenterPageView viewMock = MockRepository.StrictMock<IQualityCenterPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectList()).Return(new string[0]);
            Expect.Call(viewMock.ProjectList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior().IgnoreArguments();
            viewMock.DataBind();

            MockRepository.ReplayAll();

            QCController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventTest() 
        {
            const bool validationResult = true;

            QCController controller = CreateController();
            IQualityCenterPageView viewMock = MockRepository.StrictMock<IQualityCenterPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser eventRaiser = LastCall.GetEventRaiser();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectList()).Return(new string[0]);
            Expect.Call(viewMock.ProjectList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
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
        public void ControlValidationSuccessTest() 
        {
            QCController controller = CreateController();
            IQualityCenterPageView viewMock = MockRepository.StrictMock<IQualityCenterPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            IEventRaiser eventRaiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectList()).Return(new string[0]);
            Expect.Call(viewMock.ProjectList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior().IgnoreArguments();
            viewMock.DataBind();
            Expect.Call(FacadeMock.ValidateEntity(controller.Model)).Return(new ValidationResults());
            viewMock.SetGeneralTabValid(true);
            viewMock.SetMappingTabValid(true);

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            eventRaiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseBeforeSaveTest() 
        {
            IQualityCenterPageView viewMock = MockRepository.StrictMock<IQualityCenterPageView>();
            IFormController formControllerMock = MockRepository.StrictMock<IFormController>();

            formControllerMock.BeforeSave += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectList()).Return(new string[0]);
            Expect.Call(viewMock.ProjectList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior().IgnoreArguments();
            viewMock.DataBind();
            viewMock.CommitPendingChanges();

            MockRepository.ReplayAll();

            QCController controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(null, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        // TODO generate serializers assembly, deploy it and handle AssemblyLoadException at app entry point
        [Test]
        public void RaiseAssemblyLoadExceptionTest() 
        {
            IQualityCenterPageView viewMock = MockRepository.StrictMock<IQualityCenterPageView>();
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
            Expect.Call(FacadeMock.GetProjectList()).Return(new string[0]);
            Expect.Call(viewMock.ProjectList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior().IgnoreArguments();
            viewMock.DataBind();
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Throw(new AssemblyLoadException("TestEx"));
            formControllerMock.FailApplication("TestEx");

            MockRepository.ReplayAll();

            QCController controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }
    }
}