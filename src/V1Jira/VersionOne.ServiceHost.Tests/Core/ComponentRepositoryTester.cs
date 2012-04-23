using System;
using NUnit.Framework;
using VersionOne.ServiceHost.Core;

namespace VersionOne.ServiceHost.Tests.Core {
    [TestFixture]
    public class ComponentRepositoryTester {
        [Test]
        public void RegisterAndResolveComponent() {
            var component = new TestComponent();

            Assert.IsNull(ComponentRepository.Instance.Resolve<ITestComponent>());
            Assert.IsNull(ComponentRepository.Instance.Resolve<TestComponent>());

            ComponentRepository.Instance.Register(component);

            Assert.IsNotNull(ComponentRepository.Instance.Resolve<TestComponent>());
            Assert.IsNotNull(ComponentRepository.Instance.Resolve<ITestComponent>());
            Assert.IsTrue(ReferenceEquals(ComponentRepository.Instance.Resolve<TestComponent>(), component));
            Assert.IsTrue(ReferenceEquals(ComponentRepository.Instance.Resolve<ITestComponent>(), component));

            ComponentRepository.Instance.Unregister(component);

            Assert.IsNull(ComponentRepository.Instance.Resolve<ITestComponent>());
            Assert.IsNull(ComponentRepository.Instance.Resolve<TestComponent>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterNull() {
            ComponentRepository.Instance.Register<IDisposable>(null);
        }

        [Test]
        public void ReplaceRegisteredComponent() {
            var initialComponent = new TestComponent();
            var replacementComponent = new TestComponent();

            Assert.IsNull(ComponentRepository.Instance.Resolve<ITestComponent>());
            Assert.IsNull(ComponentRepository.Instance.Resolve<TestComponent>());

            ComponentRepository.Instance.Register(initialComponent);
            Assert.IsTrue(ReferenceEquals(ComponentRepository.Instance.Resolve<TestComponent>(), initialComponent));
            ComponentRepository.Instance.Register(replacementComponent);
            Assert.IsTrue(ReferenceEquals(ComponentRepository.Instance.Resolve<TestComponent>(), replacementComponent));

            ComponentRepository.Instance.Unregister(replacementComponent);

            Assert.IsNull(ComponentRepository.Instance.Resolve<ITestComponent>());
            Assert.IsNull(ComponentRepository.Instance.Resolve<TestComponent>());           
        }

        private interface ITestComponent { }

        private class TestComponent : ITestComponent { }
    }
}