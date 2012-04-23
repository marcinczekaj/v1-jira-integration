using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using NUnit.Framework;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.Tests.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.LeanKitKanban.DL.DTO;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers {
    [TestFixture]
    public class LkkControllerTester : BaseTester {
        private LkkController controller;
        private ILkkPageView viewMock;
        
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            controller = new LkkController(EntityFactory.CreateLkkServiceEntity(), FacadeMock);
            viewMock = MockRepository.StrictMock<ILkkPageView>();
        }

        [Test]
        public void PrepareView() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetStoryStatuses()).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryStatuses).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneWorkitemTypes()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOneWorkitemTypes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void PrepareViewFailure() {
            const string exceptionMessage = "Houston, we've got a problem";

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Throw(new BusinessException(exceptionMessage));
            Expect.Call(() => viewMock.DisplayError(exceptionMessage));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateConnectionSuccess() {
            var componentMock = MockRepository.StrictMock<ILkkComponent>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();

            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();            
            Expect.Call(FacadeMock.GetStoryStatuses()).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryStatuses).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneWorkitemTypes()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOneWorkitemTypes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.ValidateConnection(controller.Model)).Return(true);
            Expect.Call(() => viewMock.SetValidationResult(true));
            Expect.Call(FacadeMock.GetLkkComponent(controller.Model.Account, controller.Model.Username, controller.Model.Password)).Return(componentMock);
            Expect.Call(componentMock.GetBoards()).Return(new List<Board>());
            Expect.Call(() => viewMock.UpdateLkkBoards(null)).IgnoreArguments();

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            validationRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateConnectionFailure() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();

            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetStoryStatuses()).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryStatuses).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneWorkitemTypes()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOneWorkitemTypes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.ValidateConnection(controller.Model)).Return(false);
            Expect.Call(() => viewMock.SetValidationResult(false));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            validationRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }
        [Test]
        public void RaiseControlValidationEventSuccessTest() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();

            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();

            Expect.Call(FacadeMock.GetStoryStatuses()).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryStatuses).PropertyBehavior().Return(new List<ListValue>());
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneWorkitemTypes()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOneWorkitemTypes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.ValidateEntity(viewMock.Model)).IgnoreArguments().Return(new ValidationResults());
            Expect.Call(() => viewMock.SetGeneralTabValidity(true));
            Expect.Call(() => viewMock.SetMappingTabValidity(true));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseControlValidationEventFailureTest() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            var raiser = LastCall.GetEventRaiser();
            
            Expect.Call(FacadeMock.GetStoryStatuses()).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryStatuses).PropertyBehavior().Return(new List<ListValue>());
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOnePriorities()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOnePriorities).PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneWorkitemTypes()).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOneWorkitemTypes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());

            var failureResults = new ValidationResults();
            failureResults.AddResult(new ValidationResult(string.Empty, new LkkServiceEntity(), "Url", string.Empty, null));
            failureResults.AddResult(new ValidationResult(string.Empty, new LkkProjectMapping(), "LkkBoard", string.Empty, null));

            Expect.Call(FacadeMock.ValidateEntity(viewMock.Model)).IgnoreArguments().Return(failureResults);
            Expect.Call(() => viewMock.SetGeneralTabValidity(false));
            Expect.Call(() => viewMock.SetMappingTabValidity(false));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            raiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

    }
}