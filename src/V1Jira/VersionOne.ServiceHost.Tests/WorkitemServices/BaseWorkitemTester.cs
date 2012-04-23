using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServerConnector;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Tests.WorkitemServices {
    public class BaseWorkitemTester {
        protected readonly MockRepository Repository = new MockRepository();
        protected ILogger LoggerMock;
        protected IVersionOneProcessor V1ProcessorMock;

        [SetUp]
        public virtual void SetUp() {
            LoggerMock = Repository.Stub<ILogger>();
            V1ProcessorMock = Repository.StrictMock<IVersionOneProcessor>();

            ComponentRepository.Instance.Register(LoggerMock);
            ComponentRepository.Instance.Register(V1ProcessorMock);
        }
    }
}