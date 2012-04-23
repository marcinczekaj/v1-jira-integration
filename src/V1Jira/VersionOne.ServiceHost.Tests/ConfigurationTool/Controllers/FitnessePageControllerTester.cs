using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers {
    [TestFixture]
    public class FitnessePageControllerTester : BaseTester {
        private FitnesseController CreateController () {
            return new FitnesseController(CreateEntity(), FacadeMock);
        }

        [Test]
        public void PrepareViewTest() {
            FitnesseServiceEntity entity = new FitnesseServiceEntity();
            IFitnessePageView viewMock = MockRepository.StrictMock<IFitnessePageView>();
            FitnesseController controller = CreateController();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();

            MockRepository.ReplayAll();

            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        protected FitnesseServiceEntity CreateEntity () {
            FitnesseServiceEntity entity = new FitnesseServiceEntity();
            entity.TagName = "tag_name";
            entity.Watch = "Watch";
            entity.Filter = "Filter";

            entity.Timer = new TimerEntity();
            entity.Timer.PublishClass = "VersionOne.ServiceHost.TestServices.Fit.FitReaderService+IntervalSync, VersionOne.ServiceHost.TestServices";

            return entity;
        }
    }
}