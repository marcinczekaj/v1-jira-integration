/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Linq;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public class ValidationStep<TValidator, TValidationResult, TResolver> : IValidationStep
            where TValidator : IValidator<TValidationResult>
            where TResolver : class, IResolver<TValidationResult> {
        private readonly TValidator validator;
        private readonly TResolver resolver;

        public ValidationStep(TValidator validator, TResolver resolver) {
            this.validator = validator;
            this.resolver = resolver;
        }

        public void Run() {
            if(Equals(validator, default(TValidator))) {
                throw new InvalidOperationException("Cannot run the step without a validator");
            }

            var results = validator.Validate();

            if(!results.IsValid && resolver == null || (resolver != null && !resolver.Resolve(results.Items.Select(x => x.Target).ToList()))) {
                throw new ValidationException("Validation error during service initialization"); //TODO 
            }
        }
    }
}