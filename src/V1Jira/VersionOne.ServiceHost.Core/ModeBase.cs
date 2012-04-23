/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using Ninject;

namespace VersionOne.ServiceHost.Core {
    public class ModeBase {
        private readonly IKernel container;
        protected readonly CommonMode Starter;

        protected ModeBase() {
            container = new StandardKernel();
            Starter = CommonModeFactory.CreateStartup(container);
        }
    }
}