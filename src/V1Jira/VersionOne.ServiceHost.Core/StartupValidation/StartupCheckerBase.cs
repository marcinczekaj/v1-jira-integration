/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public abstract class StartupCheckerBase {
        private readonly IEventManager eventManager;

        protected StartupCheckerBase(IEventManager eventManager) {
            this.eventManager = eventManager;
        }

        public void Initialize() {
            eventManager.Subscribe(typeof(ServiceHostState), Run);
        }

        public void Run(object obj) {
            if(obj == null || !ServiceHostState.Validate.Equals(obj)) {
                return;
            }

            var steps = CreateValidators();

            foreach(var step in steps) {
                step.Run();
            }

            Complete();
        }

        protected abstract IEnumerable<IValidationStep> CreateValidators();

        protected virtual void Complete() {}
    }
}