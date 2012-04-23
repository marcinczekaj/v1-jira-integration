using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers 
{
    [TestFixture]
    public class TestsPageControllerTester : BaseTester 
    {
        private readonly string[] referenceFieldList = new string[] { "Reference", "Custom_MyField" };

        private TestsController CreateController() 
        {
            TestWriterEntity model = new TestWriterEntity();
            model.PassedOid = "TestStatus:129";
            model.FailedOid = "TestStatus:155";
            model.ReferenceAttribute = "Reference";
            model.ChangeComment = "comment value";
            model.DescriptionSuffix = "modified by ServiceHost";
            model.CreateDefect = "CurrentIteration";
            return new TestsController(model, FacadeMock);
        }

        [Test]
        public void PrepareViewTest() 
        {
            ITestsPageView viewMock = MockRepository.StrictMock<ITestsPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            Expect.Call(viewMock.CreateDefectChoiceList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestStatuses()).Return(null);
            Expect.Call(viewMock.FailedTestStatusList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestStatuses()).Return(null);
            Expect.Call(viewMock.PassedTestStatusList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestReferenceFieldList()).Return(referenceFieldList);
            Expect.Call(viewMock.ReferenceFieldList).PropertyBehavior();

            MockRepository.ReplayAll();

            TestsController controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(controller.Model, viewMock.Model);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseBusinessExceptionTest () {
            ITestsPageView viewMock = MockRepository.StrictMock<ITestsPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            Expect.Call(viewMock.CreateDefectChoiceList).PropertyBehavior();
            Expect.Call(FacadeMock.GetTestStatuses()).Throw(new BusinessException("TestEx"));
            viewMock.DisplayError("TestEx");

            MockRepository.ReplayAll();

            TestsController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}