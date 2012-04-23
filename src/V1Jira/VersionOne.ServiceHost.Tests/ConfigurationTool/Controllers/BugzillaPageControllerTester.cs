using System;
using Rhino.Mocks.Interfaces;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;
using Rhino.Mocks;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers 
{
    [TestFixture]
    public class BugzillaPageControllerTester : BaseTester 
    {
        private BugzillaController CreateController() 
        {
            return new BugzillaController(CreateEntity(), FacadeMock);
        }

        private static BugzillaServiceEntity CreateEntity() 
        {
            BugzillaServiceEntity entity = new BugzillaServiceEntity();
            entity.CloseAccept.BoolValue = true;
            entity.CloseFieldId = "close_field_id";
            entity.CloseFieldValue = "close_field_value_1";
            entity.CloseReassignValue = "close_reassign_value_1";
            entity.CloseResolveValue = "close_resolve_value_1";
            entity.CreateAccept.BoolValue = false;
            entity.CreateFieldId = "create_field_id_1";
            entity.CreateFieldValue = "create_field_value_1";
            entity.CreateReassignValue = "create_reassign_value_1";
            entity.CreateResolveValue = "create_resolve_value_1";
            entity.LinkField = "link_field";
            entity.Password = "password";
            entity.SearchName = "search_name";
            entity.SourceName = "source_name";
            entity.TagName = "tag_name";
            entity.Url = "url";
            entity.UrlTemplate = "url_template";
            entity.UrlTitle = "url_title";
            entity.UserName = "user_name";
            entity.Timer = new TimerEntity();
            entity.Timer.PublishClass = "VersionOne.ServiceHost.BugzillaServices.BugzillaHostedService+IntervalSync, VersionOne.ServiceHost.BugzillaServices";

            return entity;
        }

        [Test]
        public void RegisterViewTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(viewMock, controller.View);
            Assert.AreEqual(controller.Model, viewMock.Model);

            MockRepository.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterWrongViewTest() 
        {
            BugzillaController controller = CreateController();
            controller.RegisterView(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterNullViewTest() 
        {
            BugzillaController controller = CreateController();
            controller.RegisterView(null);
        }

        [Test]
        public void RegisterFormControllerTest() 
        {
            BugzillaController controller = CreateController();
            IFormController formControllerMock = MockRepository.StrictMock<IFormController>();

            formControllerMock.BeforeSave += null;
            LastCall.IgnoreArguments();

            MockRepository.ReplayAll();

            controller.RegisterFormController(formControllerMock);
            Assert.AreEqual(controller.FormController, formControllerMock);

            MockRepository.VerifyAll();
        }

        [Test]
        public void PrepareViewTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();

            using(MockRepository.Ordered()) 
            {
                Expect.Call(viewMock.Model).PropertyBehavior();
                viewMock.ValidationRequested += null;
                LastCall.IgnoreArguments();
                viewMock.ControlValidationTriggered += null;
                LastCall.IgnoreArguments();
                Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
                Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
                Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
                Expect.Call(viewMock.VersionOneProjects).PropertyBehavior();
                Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
                Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
                viewMock.DataBind();
            }

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            viewMock.DataBind();
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Return(false);
            viewMock.SetValidationResult(false);

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void ControlValidationSuccessTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            viewMock.DataBind();
            Expect.Call(FacadeMock.ValidateEntity(viewMock.Model)).IgnoreArguments().Return(new ValidationResults());
            viewMock.SetGeneralTabValid(true);
            viewMock.SetMappingTabValid(true);

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void ControlValidationGeneralSettingsFailureTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            viewMock.DataBind();
            ValidationResults results = new ValidationResults();
            ValidationResult generalResult = new ValidationResult(string.Empty, new BugzillaServiceEntity(), null, null, null);
            results.AddResult(generalResult);
            Expect.Call(FacadeMock.ValidateEntity(viewMock.Model)).IgnoreArguments().Return(results);
            viewMock.SetGeneralTabValid(false);
            viewMock.SetMappingTabValid(true);

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void ControlValidationMappingsFailureTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.SourceList).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(null);
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            viewMock.DataBind();
            ValidationResults results = new ValidationResults();
            ValidationResult generalResult = new ValidationResult(string.Empty, new BugzillaProjectMapping(), null, null, null);
            results.AddResult(generalResult);
            Expect.Call(FacadeMock.ValidateEntity(viewMock.Model)).IgnoreArguments().Return(results);
            viewMock.SetGeneralTabValid(true);
            viewMock.SetMappingTabValid(false);

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseAssemblyLoadExceptionTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();
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
            Expect.Call(viewMock.VersionOneProjects).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(null);
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            viewMock.DataBind();
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Throw(new AssemblyLoadException("TestEx"));
            formControllerMock.FailApplication("TestEx");

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseBusinessExceptionTest() 
        {
            IBugzillaPageView viewMock = MockRepository.StrictMock<IBugzillaPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ValidationRequested += null;
            LastCall.IgnoreArguments();
            viewMock.ControlValidationTriggered += null;
            LastCall.IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Throw(new BusinessException("TestEx"));
            viewMock.DisplayError("TestEx");

            MockRepository.ReplayAll();

            BugzillaController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}