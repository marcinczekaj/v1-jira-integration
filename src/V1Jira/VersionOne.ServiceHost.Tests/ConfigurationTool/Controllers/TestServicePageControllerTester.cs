using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers 
{
    [TestFixture]
    public class TestServicePageControllerTester : BaseTester 
    {
        private readonly string[] referenceFieldList = new string[] { "Reference", "Custom_MyField" };

        private TestServiceController CreateController() 
        {
            TestServiceEntity model = new TestServiceEntity();
            model.PassedOid = "TestStatus:129";
            model.FailedOid = "TestStatus:155";
            model.ReferenceAttribute = "Reference";
            model.ChangeComment = "comment value";
            model.DescriptionSuffix = "modified by ServiceHost";
            model.CreateDefect = "CurrentIteration";
            model.BaseQueryFilter = "Reference='';Owners.Nickname='qc';Status.Name=''";
            
            model.Projects = new List<TestPublishProjectMapping>();
            TestPublishProjectMapping project = new TestPublishProjectMapping();
            project.DestinationProject = "Call Center";
            project.IncludeChildren = true;
            project.Name = "CallCenter";
            model.Projects.Add(project);

            return new TestServiceController(model, FacadeMock);
        }

        [Test]
        public void PrepareViewTest() 
        {
            ITestServicePageView viewMock = MockRepository.StrictMock<ITestServicePageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ProjectMapRowsChanged += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.CreateDefectChoiceList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestStatuses()).Return(null);
            Expect.Call(viewMock.FailedTestStatusList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestStatuses()).Return(null);
            Expect.Call(viewMock.PassedTestStatusList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestReferenceFieldList()).Return(referenceFieldList);
            Expect.Call(viewMock.ReferenceFieldList).PropertyBehavior();
            Expect.Call(FacadeMock.GetProjectList()).Return(new string[0]);
            Expect.Call(viewMock.Projects).PropertyBehavior().Return(new string[0]);
            viewMock.DataBind();
            
            MockRepository.ReplayAll();

            TestServiceController controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(controller.Model, viewMock.Model);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseBusinessExceptionTest () {
            ITestServicePageView viewMock = MockRepository.StrictMock<ITestServicePageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.ProjectMapRowsChanged += null;
            LastCall.IgnoreArguments();
            Expect.Call(viewMock.CreateDefectChoiceList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestStatuses()).Throw(new BusinessException("TestEx"));
            viewMock.DisplayError("TestEx");

            MockRepository.ReplayAll();

            TestServiceController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}