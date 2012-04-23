/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.Core.StartupValidation {
    public interface IValidator<T> {
        ValidationResults<T> Validate();
    }
}