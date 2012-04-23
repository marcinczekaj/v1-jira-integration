/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using Ninject;
using Ninject.Parameters;

namespace VersionOne.ServiceHost.Core {
    public static class CommonModeFactory {
        public static CommonMode CreateStartup(IKernel container) {
            var starter = container.Get<CommonMode>(new ConstructorArgument("container", container));
            starter.Initialize();
            return starter;
        }
    }
}