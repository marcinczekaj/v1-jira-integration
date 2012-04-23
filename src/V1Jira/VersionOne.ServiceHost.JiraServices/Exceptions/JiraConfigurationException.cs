/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.ServiceHost.JiraServices.Exceptions {
    public class JiraConfigurationException : Exception {
        public JiraConfigurationException() { }

        public JiraConfigurationException(string message) : base(message) { }

        public JiraConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}