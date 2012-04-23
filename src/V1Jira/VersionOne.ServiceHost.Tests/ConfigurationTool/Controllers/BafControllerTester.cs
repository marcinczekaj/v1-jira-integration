using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.DL;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;
using VersionOne.ServiceHost.Tests.ConfigurationTool.Entities;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers {
    [TestFixture]
    public class BafControllerTester : BaseTester {
        private BafController controller;
        private IBafPageView viewMock;

        private const string ExceptionMessage = "Houston, we've got a problem";

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            controller = new BafController(EntityFactory.CreateBafEntity(), FacadeMock);
            viewMock = MockRepository.StrictMock<IBafPageView>();
        }

        [Test]
        public void PrepareView() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();            
        }

        [Test]
        public void PrepareViewFailedRetrievingCustomListFields() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(string.Empty)).IgnoreArguments().Throw(new BusinessException(ExceptionMessage));
            Expect.Call(() => viewMock.DisplayError(ExceptionMessage));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void PrepareViewFailedRetrievingCustomNumericFields() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(string.Empty)).IgnoreArguments().Throw(new BusinessException(ExceptionMessage));
            Expect.Call(() => viewMock.DisplayError(ExceptionMessage));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void PrepareViewFailedRetrievingProjects() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Throw(new BusinessException(ExceptionMessage));
            Expect.Call(() => viewMock.DisplayError(ExceptionMessage));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void ValidateConnectionSuccess() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.ValidateConnection(controller.Model)).Return(true);
            Expect.Call(() => viewMock.SetValidationResult(true));

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
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
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
        public void ValidateConnectionFailedAssemblyLoad() {
            var exception = new AssemblyLoadException(ExceptionMessage);
            var formControllerMock = MockRepository.StrictMock<IFormController>();

            Expect.Call(() => formControllerMock.BeforeSave += null).IgnoreArguments();
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.ValidateConnection(controller.Model)).Throw(exception);
            Expect.Call(() => formControllerMock.FailApplication(ExceptionMessage));

            MockRepository.ReplayAll();

            controller.RegisterFormController(formControllerMock);
            controller.RegisterView(viewMock);
            controller.PrepareView();
            validationRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void FeatureGroupFieldNameChangedSuccess() {
            const string type = "CustomTypeToken";
            var values = new List<ListValue>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.GetVersionOneCustomListTypeName(string.Empty, V1Connector.FeatureGroupTypeToken)).IgnoreArguments().Return(type);
            Expect.Call(FacadeMock.GetVersionOneCustomTypeValues(type)).Return(values);
            Expect.Call(() => viewMock.BindFgStatusCombos(values));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            validationRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void FeatureGroupFieldNameChangedFailure() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.GetVersionOneCustomListTypeName(string.Empty, V1Connector.FeatureGroupTypeToken)).IgnoreArguments().Throw(new BusinessException(string.Empty));
            Expect.Call(FacadeMock.GetVersionOneCustomTypeValues(string.Empty)).IgnoreArguments().Repeat.Never();
            Expect.Call(() => viewMock.BindFgStatusCombos(null)).IgnoreArguments().Constraints(List.Count(Is.Equal(0)));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            validationRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void StoryFieldNameChangedSuccess() {
            const string type = "CustomTypeToken";
            var values = new List<ListValue>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.GetVersionOneCustomListTypeName(string.Empty, V1Connector.StoryTypeToken)).IgnoreArguments().Return(type);
            Expect.Call(FacadeMock.GetVersionOneCustomTypeValues(type)).Return(values);
            Expect.Call(() => viewMock.BindStoryStatusCombos(values));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            validationRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void StoryFieldNameChangedFailure() {
            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            var validationRaiser = LastCall.GetEventRaiser();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.GetVersionOneCustomListTypeName(string.Empty, V1Connector.StoryTypeToken)).IgnoreArguments().Throw(new BusinessException(string.Empty));
            Expect.Call(FacadeMock.GetVersionOneCustomTypeValues(string.Empty)).IgnoreArguments().Repeat.Never();
            Expect.Call(() => viewMock.BindStoryStatusCombos(null)).IgnoreArguments().Constraints(List.Count(Is.Equal(0)));

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            validationRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }

        [Test]
        public void ShirtSizeFieldNameChanged() {
            const string shirtSizeType = "Custom_TShirtSize";

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(() => viewMock.ValidationRequested += null).IgnoreArguments();
            Expect.Call(() => viewMock.ControlValidationTriggered += null).IgnoreArguments();
            Expect.Call(() => viewMock.FgFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.StoryFieldNameChanged += null).IgnoreArguments();
            Expect.Call(() => viewMock.ShirtSizeFieldNameChanged += null).IgnoreArguments();
            var eventRaiser = LastCall.GetEventRaiser();
            Expect.Call(FacadeMock.GetProjectWrapperList()).Return(new List<ProjectWrapper>());
            Expect.Call(viewMock.Projects).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomListFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomListFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.FeatureGroupTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.FgCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(FacadeMock.GetVersionOneCustomNumericFields(V1Connector.StoryTypeToken)).Return(new List<ListValue>());
            Expect.Call(viewMock.StoryCustomNumericFields).IgnoreArguments().PropertyBehavior();
            Expect.Call(viewMock.BafShirtSizes).IgnoreArguments().PropertyBehavior();
            Expect.Call(() => viewMock.DataBind());
            Expect.Call(FacadeMock.GetVersionOneCustomListTypeName(controller.Model.ShirtSizeFieldName, V1Connector.StoryTypeToken)).Return(shirtSizeType);
            Expect.Call(FacadeMock.GetVersionOneCustomTypeValues(shirtSizeType)).Return(new List<ListValue>());
            Expect.Call(viewMock.VersionOneShirtSizes).PropertyBehavior();
            Expect.Call(() => viewMock.BindShirtSizeMappingGrid());

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();
            eventRaiser.Raise(controller, EventArgs.Empty);

            MockRepository.VerifyAll();
        }
    }
}