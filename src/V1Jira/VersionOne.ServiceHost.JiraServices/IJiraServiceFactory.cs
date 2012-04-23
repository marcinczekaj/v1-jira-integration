/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.JiraServices {
    public interface IJiraServiceFactory {
        IJiraProxy CreateNew(string url);
    }
}