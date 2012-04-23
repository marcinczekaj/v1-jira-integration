/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.ServiceHost.JiraServices.Exceptions {
    /// <summary>
    /// Represent remote(JIRA) validation exception.
    /// </summary>
    public class JiraValidationException : JiraException {
        public JiraValidationException(string message, Exception innerException) 
            : base(message, innerException) {            
        }         
    }
}