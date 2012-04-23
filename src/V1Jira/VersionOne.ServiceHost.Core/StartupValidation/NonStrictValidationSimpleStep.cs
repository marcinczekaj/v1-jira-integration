/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public class NonStrictValidationSimpleStep: IValidationStep {
        private readonly ISimpleResolver resolver;
        private readonly ISimpleValidator validator;

        public NonStrictValidationSimpleStep(ISimpleValidator validator, ISimpleResolver resolver) {
            this.validator = validator;
            this.resolver = resolver;
        }

        public void Run() {
            if(validator == null) {
                throw new InvalidOperationException("Cannot run the step without a validator.");
            }
            var isValid = validator.Validate();

            if(!isValid && resolver != null){
                resolver.Resolve();
            }
        }
    }
}