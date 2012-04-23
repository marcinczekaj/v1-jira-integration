/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.ServiceHost.WorkitemServices.Exceptions {
    public class InvalidSourceNameException : Exception {
        public InvalidSourceNameException(string sourceName)
            : base(string.Format("Invalid Source name: {0}", sourceName)) {
        }
    }
}