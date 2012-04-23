using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.JiraServices;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.Jira {
    [TestFixture]
    public class BaseJiraTester {
        protected const string Url = "http://localhost";
        protected const string Username = "user";
        protected const string Password = "password";
        protected const string Token = "random_token";

        protected readonly MockRepository Repository = new MockRepository();
        private ILogger loggerMock;
        protected IJiraProxy SoapService;
        protected IJiraServiceFactory ServiceFactory;

        [SetUp]
        public virtual void SetUp() {
            SoapService = Repository.StrictMock<IJiraProxy>();
            ServiceFactory = Repository.StrictMock<IJiraServiceFactory>();
            loggerMock = Repository.Stub<ILogger>();

            ComponentRepository.Instance.Register(loggerMock);
            ComponentRepository.Instance.Register(SoapService);
            ComponentRepository.Instance.Register(ServiceFactory);
        }

        [TearDown]
        public void TearDown() {
            Repository.BackToRecordAll();
        }
    }
}