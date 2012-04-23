using NUnit.Framework;
using Rhino.Mocks;
using VersionOne.ServiceHost.ConfigurationTool.BZ;

namespace VersionOne.ServiceHost.Tests.ConfigurationTool {
    public class BaseTester {
        protected readonly MockRepository MockRepository = new MockRepository();
        protected IFacade FacadeMock;

        [SetUp]
        public virtual void SetUp() {
            FacadeMock = MockRepository.StrictMock<IFacade>();
        }
    }
}