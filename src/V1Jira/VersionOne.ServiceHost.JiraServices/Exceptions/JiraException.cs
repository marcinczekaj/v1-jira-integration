/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
namespace VersionOne.ServiceHost.JiraServices.Exceptions {
    public class JiraException : Exception {
        public JiraException(string message, Exception innerException) 
            : base(message, innerException) {            
        }            
    }
}