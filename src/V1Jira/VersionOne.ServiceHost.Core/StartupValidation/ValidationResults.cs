/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Collections.Generic;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public class ValidationResults<T> {
        public IList<ValidationResult<T>> Items { get; private set; }

        public ValidationResults() {
            Items = new List<ValidationResult<T>>();
        }

        public void Add(ValidationResult<T> result) {
            Items.Add(result);
        }

        public int Count {
            get { return Items.Count; }
        }

        public bool IsValid {
            get { return Items.Count == 0; }
        }
    }
}