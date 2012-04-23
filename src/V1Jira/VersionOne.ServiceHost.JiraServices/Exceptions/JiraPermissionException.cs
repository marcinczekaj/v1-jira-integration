/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.ServiceHost.JiraServices.Exceptions {
    /// <summary>
    /// Represent remote(JIRA) permission execption.
    /// </summary>
    public class JiraPermissionException : JiraException {
        public JiraPermissionException(string message, Exception innerException) 
            : base(message, innerException) {            
        }
    }
}
