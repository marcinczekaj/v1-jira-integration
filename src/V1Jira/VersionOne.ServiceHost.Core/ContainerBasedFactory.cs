/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using Ninject;
using Ninject.Parameters;

namespace VersionOne.ServiceHost.Core {
    public abstract class ContainerBasedFactory {
        protected readonly IKernel Container;

        protected ContainerBasedFactory(IKernel container) {
            Container = container;
        }

        protected IParameter Parameter(string name, object value) {
            return new ConstructorArgument(name, value);
        }
    }
}