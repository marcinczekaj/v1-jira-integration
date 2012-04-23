/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using VersionOne.ServiceHost.JiraServices;

namespace VersionOne.Jira.SoapProxy {
    public class JiraSoapServiceFactory : IJiraServiceFactory {
        public IJiraProxy CreateNew(string url) {
            return new JiraSoapProxy(url);
        }
    }
}