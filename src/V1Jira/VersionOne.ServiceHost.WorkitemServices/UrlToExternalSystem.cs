/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
namespace VersionOne.ServiceHost.WorkitemServices {
    public class UrlToExternalSystem {
        public readonly string Url;
        public readonly string Title;

        public UrlToExternalSystem(string url, string title) {
            Url = url;
            Title = title;
        }
    }
}