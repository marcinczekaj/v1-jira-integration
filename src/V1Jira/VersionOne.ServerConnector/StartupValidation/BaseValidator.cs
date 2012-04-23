/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core.StartupValidation;

namespace VersionOne.ServerConnector.StartupValidation {
    public abstract class BaseValidator : ISimpleValidator {
        protected readonly ILogger Logger;
        protected readonly IVersionOneProcessor V1Processor;

        protected  BaseValidator() {
            Logger = ComponentRepository.Instance.Resolve<ILogger>();
            V1Processor = ComponentRepository.Instance.Resolve<IVersionOneProcessor>();
        }

        public abstract bool Validate();
    }
}