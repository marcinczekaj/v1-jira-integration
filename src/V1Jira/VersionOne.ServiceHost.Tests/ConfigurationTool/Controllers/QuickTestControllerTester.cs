using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers {
    [TestFixture]
    public class QuickTestControllerTester : BaseTester {

        private QTPController CreateController () {
            return new QTPController(CreateEntity(), FacadeMock);
        }

        private QTPServiceEntity CreateEntity () {
            QTPServiceEntity entity = new QTPServiceEntity();
            entity.Watch = "QuickTestPro";
            entity.Filter = "Filter.xml";
            entity.SuiteName = "TestSuite";

            return entity;
        }

        [Test]
        public void RegisterViewTest () {
            IQTPPageView viewMock = MockRepository.StrictMock<IQTPPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();

            MockRepository.ReplayAll();

            QTPController controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(viewMock, controller.View);
            Assert.AreEqual(controller.Model, viewMock.Model);

            MockRepository.VerifyAll();
        }

        [Test]
        public void PrepareViewTest () {
            IQTPPageView viewMock = MockRepository.StrictMock<IQTPPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();

            MockRepository.ReplayAll();

            QTPController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

    }
}
