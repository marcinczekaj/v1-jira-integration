using System;
using System.Collections.Generic;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;
using Rhino.Mocks;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers {
    [TestFixture]
    public class JiraPageControllerTester : BaseTester {
        private const string TestExceptionMessage = "Test";

        private JiraController CreateController() {
            return new JiraController(CreateEntity(), FacadeMock);
        }

        private static JiraServiceEntity CreateEntity() {
            var entity = new JiraServiceEntity {
                Timer = new TimerEntity { PublishClass = "VersionOne.ServiceHost.JiraServices.JiraHostedService+IntervalSync, VersionOne.ServiceHost.JiraServices" },
                Url = "http://jirahost/",
                UserName = "user",
                Password = "password",
                CloseFieldId = "cf_closefield",
                CloseFieldValue = "closed",
                CreateFieldId = "cf_createfield",
                CreateFieldValue = "created",
                AssigneeStateChanged = "assignee_State_Changed",
                CreateDefectFilter = new JiraFilter {Disabled = false, Id = "1"},
                LinkField = "link_field",
                ProgressWorkflow = {NumberValue = 2},
                ProgressWorkflowClosed = {NumberValue = 3},
                SourceName = "source_name",
                TagName = "tag_name",
                UrlTemplate = "url_template",
                UrlTitle = "url_title"
            };

            return entity;
        }

        [Test]
        public void RegisterViewTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(viewMock, controller.View);
            Assert.AreEqual(controller.Model, viewMock.Model);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RegisterFormControllerTest() {
            var controller = CreateController();
            var formControllerMock = MockRepository.StrictMock<IFormController>();
            
            Expect.Call(() => formControllerMock.BeforeSave += null).IgnoreArguments();

            MockRepository.ReplayAll();

            controller.RegisterFormController(formControllerMock);
            Assert.AreEqual(controller.FormController, formControllerMock);

            MockRepository.VerifyAll();
        }

        [Test]
        public void PrepareViewTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.AvailableSources).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.ProjectWrapperList).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(viewMock.DataBind);

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventFailureTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.AvailableSources).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.ProjectWrapperList).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(viewMock.DataBind);
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Return(false);
            viewMock.SetValidationResult(false);
            Expect.Call(FacadeMock.GetJiraPriorities(null, null, null)).IgnoreArguments().Repeat.Never();

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseValidateEventSuccessTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.AvailableSources).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.ProjectWrapperList).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(viewMock.DataBind);
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Return(true);
            viewMock.SetValidationResult(true);
            Expect.Call(FacadeMock.GetJiraPriorities(null, null, null)).IgnoreArguments().Return(new List<ListValue>());
            viewMock.UpdateJiraPriorities(new List<ListValue>());

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseControlValidationEventSuccessTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.AvailableSources).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.ProjectWrapperList).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(viewMock.DataBind);
            Expect.Call(FacadeMock.ValidateEntity(viewMock.Model)).IgnoreArguments().Return(new ValidationResults());
            Expect.Call(() => viewMock.SetGeneralTabValidity(true));
            Expect.Call(() => viewMock.SetMappingTabValidity(true));

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseControlValidationEventFailureTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.AvailableSources).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.ProjectWrapperList).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(viewMock.DataBind);

            var failureResults = new ValidationResults();
            failureResults.AddResult(new ValidationResult(string.Empty, new JiraServiceEntity(), "Url", string.Empty, null));
            failureResults.AddResult(new ValidationResult(string.Empty, new JiraPriorityMapping(), "Name", string.Empty, null));

            Expect.Call(FacadeMock.ValidateEntity(viewMock.Model)).IgnoreArguments().Return(failureResults);
            viewMock.SetGeneralTabValidity(false);
            viewMock.SetMappingTabValidity(false);

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseAssemblyLoadExceptionTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();
            var formControllerMock = MockRepository.StrictMock<IFormController>();

            Expect.Call(() => formControllerMock.BeforeSave += null).IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Return(new string[0]);
            Expect.Call(viewMock.AvailableSources).PropertyBehavior().Return(new string[0]);
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.ProjectWrapperList).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(viewMock.DataBind);
            Expect.Call(FacadeMock.ValidateConnection(null)).IgnoreArguments().Throw(new AssemblyLoadException(TestExceptionMessage));
            Expect.Call(() => formControllerMock.FailApplication(TestExceptionMessage));

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(viewMock, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseBusinessExceptionTest() {
            var viewMock = MockRepository.StrictMock<IJiraPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetSourceList()).Throw(new BusinessException(TestExceptionMessage));
            viewMock.DisplayError(TestExceptionMessage);

            MockRepository.ReplayAll();

            var controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}