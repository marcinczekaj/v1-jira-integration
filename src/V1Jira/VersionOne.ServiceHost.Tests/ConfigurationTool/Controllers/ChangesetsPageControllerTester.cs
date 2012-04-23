using System;
using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.Entities;
using VersionOne.ServiceHost.ConfigurationTool.UI.Controllers;
using VersionOne.ServiceHost.ConfigurationTool.UI.Interfaces;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool.Controllers 
{
    [TestFixture]
    public class ChangesetsPageControllerTester : BaseTester 
    {
        private ChangesetsController CreateController() 
        {
            ChangesetWriterEntity model = new ChangesetWriterEntity();
            model.AlwaysCreate = true;
            model.ChangeComment = "comment";
            model.Link.Name = "link name";
            model.Link.OnMenu.BoolValue = true;
            model.Link.Url = "http://example.com/link";

            return new ChangesetsController(model, FacadeMock);
        }
        
        [Test]
        public void RegisterViewTest() 
        {
            IChangesetsPageView viewMock = MockRepository.StrictMock<IChangesetsPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            
            MockRepository.ReplayAll();

            ChangesetsController controller = CreateController();
            controller.RegisterView(viewMock);
            Assert.AreEqual(viewMock, controller.View);
            Assert.AreEqual(controller.Model, viewMock.Model);

            MockRepository.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterWrongViewTest() 
        {
            ChangesetsController controller = CreateController();
            controller.RegisterView(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterNullViewTest() 
        {
            ChangesetsController controller = CreateController();
            controller.RegisterView(null);
        }

        [Test]
        public void RegisterFormControllerTest() 
        {
            ChangesetsController controller = CreateController();
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
            IChangesetsPageView viewMock = MockRepository.StrictMock<IChangesetsPageView>();

            Expect.Call(viewMock.Model).PropertyBehavior();
            viewMock.DataBind();

            MockRepository.ReplayAll();

            ChangesetsController controller = CreateController();
            controller.RegisterView(viewMock);
            controller.PrepareView();

            MockRepository.VerifyAll();
        }
    }
}
