using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.BZ;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers 
{
    [TestFixture]
    public class DefectsPageControllerTester : BaseTester 
    {
        private readonly string[] referenceFieldList = new string[] { "Reference", "Custom_MyField" };

        private DefectsController CreateController() 
        {
            WorkitemWriterEntity model = new WorkitemWriterEntity();
            model.Disabled = false;
            model.ExternalIdFieldName = "Reference";
            return new DefectsController(model, FacadeMock);
        }

        [Test]
        public void PrepareViewTest() 
        {
            IWorkitemsPageView viewMock = MockRepository.StrictMock<IWorkitemsPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            Expect.Call(FacadeMock.GetDefectReferenceFieldList()).Return(referenceFieldList);
            Expect.Call(viewMock.ReferenceFieldList).PropertyBehavior();

            MockRepository.ReplayAll();

            DefectsController controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(controller.Model, viewMock.Model);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }

        [Test]
        public void RaiseBusinessExceptionTest () {
            IWorkitemsPageView viewMock = MockRepository.StrictMock<IWorkitemsPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();
            Expect.Call(viewMock.ReferenceFieldList).PropertyBehavior();
            Expect.Call(FacadeMock.GetDefectReferenceFieldList()).Throw(new BusinessException("TestEx"));
            viewMock.DisplayError("TestEx");

            MockRepository.ReplayAll();

            DefectsController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}
