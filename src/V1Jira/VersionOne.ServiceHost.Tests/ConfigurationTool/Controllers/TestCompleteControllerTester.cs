using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers {
    [TestFixture]
    public class TestCompleteControllerTester : BaseTester{
        private TestCompleteController CreateController () {
            return new TestCompleteController(CreateEntity(), FacadeMock);
        }

        private TestCompleteEntity CreateEntity() {
            TestCompleteEntity entity = new TestCompleteEntity();
            entity.TagName = "tag_name";
            entity.ProjectSuiteConfig = "TestComplete7";
            entity.RetryAttempts = 3;
            entity.RetryTimeoutMilliSeconds = 1000;

            entity.Timer = new TimerEntity();
            entity.Timer.PublishClass = "VersionOne.ServiceHost.TestServices.TestComplete.TCReaderService+IntervalSync, VersionOne.ServiceHost.TestServices";

            return entity;
        }

        [Test]
        public void RegisterViewTest() {
            ITestCompletePageView viewMock = MockRepository.StrictMock<ITestCompletePageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();

            MockRepository.ReplayAll();

            TestCompleteController controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(viewMock, controller.View);
            Assert.AreEqual(controller.Model, viewMock.Model);

            MockRepository.VerifyAll();
        }

        [Test]
        public void PrepareViewTest () {
            ITestCompletePageView viewMock = MockRepository.StrictMock<ITestCompletePageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();

            MockRepository.ReplayAll();

            TestCompleteController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}
