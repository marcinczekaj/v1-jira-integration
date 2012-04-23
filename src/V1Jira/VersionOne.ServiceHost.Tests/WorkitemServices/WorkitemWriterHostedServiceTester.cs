using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Core.Services;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.ServiceHost.Tests.WorkitemServices {
    [TestFixture]
    public class WorkitemWriterHostedServiceTester : BaseWorkitemTester {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            workitemWriterMock = Repository.StrictMock<WorkitemWriter>();
            eventManagerMock = Repository.StrictMock<IEventManager>();

            eventManager = new EventManager();
            service = new WorkitemWriterHostedService();

            service.Initialize(null, eventManager, null);
        }

        #endregion

        private IEventManager eventManager;
        private IEventManager eventManagerMock;
        private IHostedService service;
        private WorkitemWriter workitemWriterMock;

        [Test]
        [Ignore("Test is incompleted by now")]
        public void ProcessWorkitemSuccess() {
            var defect = new Defect();
            var result = new WorkitemCreationResult(defect);

            Expect.Call(workitemWriterMock.CheckForDuplicate(defect)).Return(false);
            Expect.Call(workitemWriterMock.CreateWorkitem(defect)).Return(result);
            Expect.Call(() => eventManagerMock.Publish(result));

            Repository.ReplayAll();
            eventManager.Publish(defect);
            Repository.VerifyAll();
        }
    }
}